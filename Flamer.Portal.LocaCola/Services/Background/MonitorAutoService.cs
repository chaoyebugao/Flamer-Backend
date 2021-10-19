using Claunia.PropertyList;
using Flamer.Portal.LocaCola.Settings;
using Flamer.Portal.LocalWeb.Areas.Blob.Models.Oss;
using Flamer.Portal.Web.Areas.Blob.Models.Oss;
using Flamer.Service.OSS.Extensions;
using Flamer.Utility.Security;
using Flamer.Utility.Wroker;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Build.Construction;
using Microsoft.Extensions.Hosting;
using Minio;
using NLog;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        private async Task MonitorWork()
        {
            if (ipaSettingCollection == null || ipaSettingCollection.Count == 0)
            {
                logger.Info($"无任何监听配置项");
                return;
            }

            //检查登录，重试机制加持
            Policy.HandleResult(true).WaitAndRetryForever(time => TimeSpan.FromSeconds(2), (ret, ts) =>
            {
#if DEBUG
                logger.Debug("无登录凭证");
#endif
            })
                .Execute(() =>
            {
                return WebProcessor.LoginRet == null || string.IsNullOrEmpty(WebProcessor.SysUserName);
            });

            //获取局域网地址，重试机制加持
            await Policy.Handle<Exception>().WaitAndRetryForeverAsync(time => TimeSpan.FromSeconds(5), (ex, ts) =>
            {
                logger.Warn($"获取局域网地址失败，{ts.TotalSeconds}秒后重试:{Environment.NewLine}{ex}");
            })
                .ExecuteAsync(async () =>
            {
                LocalWebAddr = await ossService.GetLocalWebAddr();
            });

            ipaWatchers = new FileSystemWatcher[ipaSettingCollection.Count];

            //检查目录是否存在，重试机制加持
            var checkDirectoryPlc = Policy.HandleResult(false).WaitAndRetryForever((time, ctx) =>
            {
                return TimeSpan.FromSeconds(12);
            }, (ret, ts, ctx) =>
            {
#if DEBUG
                var directory = ctx.ContainsKey("Directory") ? ctx["Directory"] : null;
                logger.Debug($"目录不存在:{directory}");
#endif
            });

            var tasks = ipaSettingCollection.Select(settings =>
            {
                return Task.Factory.StartNew(async () =>
                {
                    checkDirectoryPlc.Execute(ctx => Directory.Exists(settings.Directory), new Context()
                    {
                        { "Directory",  settings.Directory}
                    });

                    Watch(settings);

                    await CheckUpload(settings);
                }, TaskCreationOptions.LongRunning);
            });

            await Task.WhenAll(tasks);
        }

        private async Task CheckUpload(IpaSettings settings)
        {
            var path = Path.Combine(settings.Directory, settings.File);

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
            if (uploadMetaVm.OssInfo != null)
            {
                //已存在记录
                return;
            }

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
            var env = GetEnvironment(settings);

            logger.Info($"{settings}-准备上传ipa: {identifier}, {version}, {env}");
            var localUpdUrl = await ossService.GetLocalUploadUrl(LocalWebAddr, new PresignedUrlSub()
            {
                Category = BLOBCATEGORY,
                Hash = hash,
                OriginalFileName = fileInfo.Name,
                SysUserName = WebProcessor.SysUserName,
            });

            var minioInner = GetClient(uploadMetaVm.MinIOSettings_Decrypt.Inner);

            var upldPlc = Policy.Handle<Exception>().WaitAndRetryAsync(5, time => TimeSpan.FromSeconds(6), (ex, ts) =>
            {
                logger.Info($"{settings}-内网上传失败，准备重试: {ex.Message}");
            });

            await upldPlc.ExecuteAsync(() =>
            {
                return minioInner.PutObjectAsync(uploadMetaVm.BucketName, uploadMetaVm.ObjectName, fileInfo.FullName);
            });
            logger.Info($"{settings}-内网已上传");

            var minioWeb = GetClient(uploadMetaVm.MinIOSettings_Decrypt.Web);
            await upldPlc.ExecuteAsync(() =>
            {
                return minioWeb.PutObjectAsync(uploadMetaVm.BucketName, uploadMetaVm.ObjectName, fileInfo.FullName);
            });
            logger.Info($"{settings}-公网已上传");

            var saveRecordPlc = Policy.Handle<Exception>().WaitAndRetryAsync(5, time => TimeSpan.FromSeconds(5), (ex, ts) =>
            {
                logger.Info($"{settings}-记录添加失败，准备重试: {ex.Message}");
            });

            await saveRecordPlc.ExecuteAsync(async () =>
            {
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
                    Env = env,
                });
            });

            logger.Info($"{settings}-记录添加完毕");
            Console.WriteLine();
            Console.WriteLine();

            _ = Task.Delay(2000).ContinueWith(_ =>
            {
                Policy.Handle<Exception>().WaitAndRetry(3, time => TimeSpan.FromSeconds(2), (ex, ts) =>
                {
                    logger.Warn($"清理临时文件异常:{ex.Message}");
                }).Execute(() =>
                {
                    if (Directory.Exists(tmpPath))
                    {
                        Directory.Delete(tmpPath, true);
                    }
                });
            });
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

        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="settings"></param>
        private void Watch(IpaSettings settings)
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

        /// <summary>
        /// 获取环境
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private string GetEnvironment(IpaSettings settings)
        {
            try
            {
                return Policy.Handle<Exception>().WaitAndRetry(3, time => TimeSpan.FromSeconds(2), (ex, ts) =>
                {
                    logger.Error($"获取环境失败:{ex.Message}");
                }).Execute(() =>
                {
                    var slash = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"\" : "/";

                    var directory = settings.Directory.Replace(@$"{slash}TradingApp.iOS{slash}bin{slash}iPhone{slash}Debug", null);
                    directory = directory.Replace(@$"{slash}TradingApp.iOS{slash}bin{slash}iPhone{slash}Debug{slash}", null);
                    var csprojPath = Path.Combine(directory, @$"TradingApp{slash}TradingApp.csproj");

                    var rootEl = ProjectRootElement.Open(csprojPath);

                    var removedItems = rootEl.Items.Where(m => m.ElementName == "Compile" && m.Remove.StartsWith(@"Config\ServiceConfig."))
                        .Select(m => m.Remove);

                    var rmHasProd = removedItems.Any(m => m.Contains("Production", StringComparison.OrdinalIgnoreCase));
                    return rmHasProd ? "测试" : "生产";
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                logger.Error(ex);
#endif
                return null;
            }

        }
    }
}
