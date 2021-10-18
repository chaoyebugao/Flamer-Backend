using Claunia.PropertyList;
using Flamer.Portal.LocaCola.Settings;
using Flamer.Portal.LocalWeb.Areas.Blob.Models.Oss;
using Flamer.Portal.Web.Areas.Blob.Models.Oss;
using Flamer.Service.OSS.Extensions;
using Flamer.Utility.Security;
using Flamer.Utility.Wroker;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Hosting;
using Minio;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flamer.Portal.LocaCola.Services.Background
{
    public class MonitorAutoService : BackgroundService
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IpaSettingCollection ipaSettingCollection;
        private readonly IOssService ossService;
        private readonly IIpaService ipaService;

        private FileSystemWatcher[] ipaWatchers;

        public MonitorAutoService(IpaSettingCollection ipaSettingCollection,
            IOssService ossService,
            IIpaService ipaService)
        {
            this.ipaSettingCollection = ipaSettingCollection;
            this.ossService = ossService;
            this.ipaService = ipaService;

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                logger.Info("执行被请求停止");
                return Task.CompletedTask;
            }

            return MonitorWork();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (ipaWatchers != null && ipaWatchers.Count() > 0)
            {
                foreach (var item in ipaWatchers)
                {
                    item?.Dispose();
                }
            }

            return base.StopAsync(cancellationToken);
        }

        private const string BLOBCATEGORY = "IpaBundle";
        private string LocalWebAddr;
        private Task MonitorWork()
        {
            return Retry.DoAscending(async () =>
            {
                if (WebProcessor.LoginRet == null || string.IsNullOrEmpty(WebProcessor.SysUserName))
                {
                    throw new Exception("未登录的操作");
                }

                LocalWebAddr = await ossService.GetLocalWebAddr();

                //立即检查上传
                await CheckAllUpload();

                //监听文件变化
                Watch();
            }, int.MaxValue, onSuccess: () =>
            {
                logger.Info("检查监控完毕");
            }, onTryFailed: (ex, delaySeconds) =>
            {
                logger.Warn($"监控不成功，{delaySeconds}秒后重试:{Environment.NewLine}{ex}");
            });

        }

        private Task CheckAllUpload()
        {
            var tasks = ipaSettingCollection.Select(m => CheckUpload(m));
            return Task.WhenAll(tasks);
        }

        private async Task CheckUpload(IpaSettings settings)
        {
            var path = Path.Combine(settings.Directory, settings.File);
            logger.Info($"{settings}-正在检查");
            if (!File.Exists(path))
            {
                logger.Info($"{settings}-ipa不存在，略过");
                return;
            }
            var fileInfo = new FileInfo(path);
            if (fileInfo == null)
            {
                return;
            }
            if (fileInfo.Length == 0)
            {
                logger.Info($"{settings}-长度为0，略过");
                return;
            }

            using var fs = fileInfo.OpenRead();
            var hash = Sha1Hasher.GetSha1(fs);

            var uploadMetaVm = await ossService.GetUploadMeta(new GetAccessSub()
            {
                Category = BLOBCATEGORY,
                Hash = hash,
                OriginalFileName = fileInfo.Name,
            });
            if (uploadMetaVm.OssInfo == null)
            {
                if (uploadMetaVm.MinIOSettings_Decrypt == null)
                {
                    logger.Error($"{settings}-上传配置为空");
                    return;
                }

                var tmpPath = $"tmp/{settings.ProjectCode}/";
                if (Directory.Exists(tmpPath))
                {
                    Directory.Delete(tmpPath, true);
                }
                Directory.CreateDirectory(tmpPath);
                var infoPlistZipPath = $"Payload/{settings.File.Replace(".ipa", ".app")}/Info.plist";
                try
                {
                    var zip = new FastZip();
                    zip.ExtractZip(path, tmpPath, infoPlistZipPath);
                }
                catch
                {
                    logger.Error($"{settings}-解压失败");
                    throw;
                }
                var infoPlistPath = Path.Combine(tmpPath, infoPlistZipPath);
                using var plistFs = File.Open(infoPlistPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var plist = (NSDictionary)PropertyListParser.Parse(plistFs);
                var identifier = plist.ObjectForKey("CFBundleIdentifier").ToString();
                var version = plist.ObjectForKey("CFBundleVersion").ToString();

                logger.Info($"{settings}-ipa解析:{identifier},{version}");
                logger.Info($"{settings}-准备上传");
                var localUpdUrl = await ossService.GetLocalUploadUrl(LocalWebAddr, new PresignedUrlSub()
                {
                    Category = BLOBCATEGORY,
                    Hash = hash,
                    OriginalFileName = fileInfo.Name,
                    SysUserName = WebProcessor.SysUserName,
                });

                var minioInner = GetClient(uploadMetaVm.MinIOSettings_Decrypt.Inner);
                var innerUpldTask = minioInner.PutObjectAsync(uploadMetaVm.BucketName, uploadMetaVm.ObjectName, fileInfo.FullName);
                _ = innerUpldTask.ContinueWith(_ =>
                {
                    logger.Info($"{settings}-内网已上传");
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
                

                var minioWeb = GetClient(uploadMetaVm.MinIOSettings_Decrypt.Web);
                var webUpldTask =  minioWeb.PutObjectAsync(uploadMetaVm.BucketName, uploadMetaVm.ObjectName, fileInfo.FullName);
                _ = webUpldTask.ContinueWith(_ =>
                {
                    logger.Info($"{settings}-外网已上传");
                }, TaskContinuationOptions.OnlyOnRanToCompletion);


                await Task.WhenAll(innerUpldTask, webUpldTask);

                await ossService.SaveUpload(new SaveUploadSub()
                {
                    Category = "IpaBundle",
                    Hash = hash,
                    OriginalFileName = fileInfo.Name,
                    Size = fileInfo.Length,
                });

                await ipaService.Add(new Web.Areas.IPA.Models.Home.AddSub()
                {
                    Identifier = identifier,
                    ProjectCode = settings.ProjectCode,
                    Version = version,
                    SoftwarePackage = hash,
                });

                logger.Info($"{settings}-记录添加完毕");

                _ = Task.Delay(2000).ContinueWith(_ =>
                {
                    try
                    {
                        if (Directory.Exists(tmpPath))
                        {
                            Directory.Delete(tmpPath, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                });
            }
        }

        /// <summary>
        /// 获取节点客户端
        /// </summary>
        /// <param name="settings">MinIO设置</param>
        /// <returns></returns>
        private MinioClient GetClient(MinioSettings settings)
        {
            var minio = new MinioClient(settings.Endpint, settings.AccessKey, settings.SecretKey);
            return settings.UseSsl ? minio.WithSSL() : minio;
        }

        private void Watch()
        {
            ipaWatchers = new FileSystemWatcher[ipaSettingCollection.Count];

            foreach (var settings in ipaSettingCollection)
            {
                var index = ipaSettingCollection.IndexOf(settings);

                ipaWatchers[index] = new FileSystemWatcher()
                {
                    Filter = "*.ipa",
                    Path = settings.Directory,
                    EnableRaisingEvents = true,
                };

                ipaWatchers[index].Changed += async (sender, e) =>
                {
                    try
                    {
                        logger.Info($"{settings}-ipa已被修改, 类型{e.ChangeType}");

                        var fileSettings = ipaSettingCollection.FirstOrDefault(m => m.FullPath == e.FullPath);
                        if (fileSettings == null)
                        {
                            logger.Info($"{settings}-未找到指定文件配置:{e.FullPath}");
                            return;
                        }

                        await CheckUpload(fileSettings);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                };

                logger.Info($"{settings}-已监听");

            }
        }

    }
}
