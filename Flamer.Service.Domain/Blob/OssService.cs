using Flamer.Data.Repositories.Blob;
using Flamer.Model.Web.Databases.Main.Blob;
using Flamer.Model.ViewModel.Blob;
using Flamer.Service.Domain.Blob.CONST;
using Flamer.Service.Domain.Blob.ViewModels;
using Flamer.Service.Domain.ImageProxy;
using Flamer.Service.OSS.Extensions;
using Flamer.Service.OSS.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Flamer.Utility.Security;
using Newtonsoft.Json;
using System.Text;

namespace Flamer.Service.Domain.Blob
{
    public class OssService : IOssService
    {
        private readonly IMinioService minioService;
        private readonly IOssFileRepository ossFileRepository;
        private readonly IImageProxyService imageProxyService;
        private readonly MinioSettingCollection minioSettingCollection;
        private const int EXPIRES_SECONDS = 172800;

        public OssService(IMinioService minioService,
            IOssFileRepository ossFileRepository,
            IImageProxyService imageProxyService,
            MinioSettingCollection minioSettingCollection)
        {
            this.minioService = minioService;
            this.ossFileRepository = ossFileRepository;
            this.imageProxyService = imageProxyService;
            this.minioSettingCollection = minioSettingCollection;
        }

        /// <summary>
        /// 保存上传结果（已完成上传）
        /// </summary>
        /// <param name="sysUserName">所属用户</param>
        /// <param name="hash">文件sha1哈希</param>
        /// <param name="category">归档类型</param>
        /// <param name="originalFileName">上传文件名</param>
        /// <param name="size">文件大小</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="imageProxy">imageproxy图像处理模型</param>
        /// <returns>访问URL</returns>
        public async Task<OssInfo> SaveUpload(string sysUserName, string hash, Categories category, string originalFileName,
            long size, string contentType = null, ImageProxyModel imageProxy = null)
        {
            var (bucketName, objectName, ext) = BuildMeta(sysUserName, hash, category, originalFileName);
            if (string.IsNullOrEmpty(contentType))
            {
                contentType = ContentTyper.Get(originalFileName);
            }

            var file = new OssFile()
            {
                BucketName = bucketName,
                Category = category.ToString(),
                ContentType = contentType,
                CreateTime = DateTimeOffset.UtcNow,
                Ext = ext,
                Hash = hash,
                ObjectName = objectName,
                OriginalFileName = originalFileName,
                Size = size,
                SysUserName = sysUserName,
            };
            await ossFileRepository.Add(file);

            var urlBrowsing = IsUrlBrowsing(category);
            var ossInfo = await GetUrl(file, urlBrowsing, imageProxy);
            return ossInfo;
        }

        /// <summary>
        /// 构建文件记录相关信息
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="hash">文件sha1哈希</param>
        /// <param name="category">归类</param>
        /// <param name="originalFileName">原始文件名</param>
        /// <returns></returns>
        private (string bucketName, string objectName, string ext) BuildMeta(string sysUserName, string hash, Categories category, string originalFileName)
        {
            var ext = originalFileName.Contains(".") ? originalFileName.Split('.').Last() : null;
            string bucket;
            string objPrePath;

            switch (category)
            {
                case Categories.ProjectLogo:
                    {
                        //可公开访问
                        bucket = Buckets.Public;
                        objPrePath = category.ToString();
                        break;
                    }
                case Categories.IpaBundle:
                    {
                        //可公开访问
                        bucket = Buckets.Public;
                        objPrePath = category.ToString();

                        break;
                    }
                default:
                    {
                        //默认是私有的，归集在各个用户路径下面
                        bucket = Buckets.SysUser;
                        objPrePath = $"{sysUserName}/{category}";

                        break;
                    }

            }

            var bucketName = minioService.BuildBucketName(bucket);
            var objectName = $"{objPrePath}/{hash}.{ext}";

            return (bucketName, objectName, ext);
        }

        /// <summary>
        /// 获取访问URL
        /// </summary>
        /// <param name="file">OSS文件</param>
        /// <param name="urlBrowsing">是否需要访问URL</param>
        /// <param name="imageProxy">imageproxy图像处理模型</param>
        /// <param name="minioChannel">Minio实例通道</param>
        /// <returns></returns>
        private async Task<OssInfo> GetUrl(OssFile file, bool urlBrowsing, ImageProxyModel imageProxy = null, MinioChannels minioChannel = MinioChannels.Web)
        {
            var category = System.Enum.Parse<Categories>(file.Category);

            var isPublic = false;
            string browseUrl = null;
            var isOssUrl = false;   //是否使用OSS浏览地址

            switch (category)
            {
                case Categories.ProjectLogo:
                    {
                        isPublic = true;
                        break;
                    }
                case Categories.IpaBundle:
                    {
                        isPublic = true;
                        isOssUrl = true;
                        break;
                    }
            }

            if (urlBrowsing)
            {
                if (isPublic && isOssUrl)
                {
                    var (bucketName, objectName, _) = BuildMeta(file.SysUserName, file.Hash, category, file.OriginalFileName);
                    var minioSettings = minioService.GetSettings(minioChannel);

                    browseUrl = $"{minioSettings.Address}/{bucketName}/{objectName}";
                }
                else
                {
                    browseUrl = await imageProxyService.BuildUrl(isPublic, file.BucketName, file.ObjectName, imageProxy);
                }

            }

            return new OssInfo()
            {
                Url = browseUrl,
                ContentType = file.ContentType,
                Ext = file.Ext,
            };
        }

        /// <summary>
        /// 【无所属用户限定，慎用】获取访问URL
        /// </summary>
        /// <param name="hash">文件sha1哈希</param>
        /// <param name="urlBrowsing">是否需要访问URL</param>
        /// <param name="imageProxy">imageproxy图像处理模型</param>
        /// <param name="minioChannel"></param>
        /// <returns></returns>
        public async Task<OssInfo> GetUrl(string hash, bool urlBrowsing, ImageProxyModel imageProxy = null, MinioChannels minioChannel = MinioChannels.Web)
        {
            var file = await ossFileRepository.Get(hash);
            if (file == null)
            {
                return null;
            }
            var ossInfo = await GetUrl(file, urlBrowsing, imageProxy, minioChannel);
            return ossInfo;
        }

        /// <summary>
        /// 是否要访问文件URL
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private bool IsUrlBrowsing(Categories category)
        {
            var urlBrowsing = true;
            switch (category)
            {
                case Categories.IpaBundle:
                    {
                        urlBrowsing = false;
                        break;
                    }
            }
            return urlBrowsing;
        }

        /// <summary>
        /// 获取文件访问/上传信息
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="hash">文件哈希</param>
        /// <param name="category">分类</param>
        /// <param name="originalFileName">原始文件名</param>
        /// <param name="imageProxy">imageproxy图像处理模型</param>
        /// <returns></returns>
        public async Task<AccessVm> GetAccess(string sysUserName, string hash, Categories category,
            string originalFileName, ImageProxyModel imageProxy = null)
        {
            var urlBrowsing = IsUrlBrowsing(category);
            var ossInfo = await GetUrl(hash, urlBrowsing, imageProxy);
            string presignedPutUrl = null;
            if (ossInfo == null)
            {
                presignedPutUrl = await PresignedPutUrl(sysUserName, hash, category, originalFileName);
            }

            return new AccessVm()
            {
                SysUserName = sysUserName,
                OssInfo = ossInfo,
                UploadUrls = new string[] { presignedPutUrl },
            };
        }

        /// <summary>
        /// 获取Minio文件访问/上传信息
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="hash">文件哈希</param>
        /// <param name="category">分类</param>
        /// <param name="originalFileName">原始文件名</param>
        /// <param name="imageProxy">imageproxy图像处理模型</param>
        /// <returns></returns>
        public async Task<UploadMetaVm> GetUploadMeta(string sysUserName, string hash, Categories category,
            string originalFileName, ImageProxyModel imageProxy = null)
        {
            var urlBrowsing = IsUrlBrowsing(category);
            var ossInfo = await GetUrl(hash, urlBrowsing, imageProxy);

            var vm = new UploadMetaVm()
            {
                OssInfo = ossInfo,
            };

            if (ossInfo == null)
            {
                var (bucketName, objectName, _) = BuildMeta(sysUserName, hash, category, originalFileName);
                vm.BucketName = bucketName;
                vm.ObjectName = objectName;

                var minioSettingCollectionStr = JsonConvert.SerializeObject(minioSettingCollection);
                var minioSettingCollectionBytes = Encoding.UTF8.GetBytes(minioSettingCollectionStr);
                vm.MinIOSettings = minioSettingCollectionBytes.Encrypt("Yg25ggqhiIUWUq4iLsYqOZScna0b7gDW", "xJuu0M1BJx95vzfH");
            }

            return vm;
        }

        /// <summary>
        /// 获取预签名上传URL
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="hash">文件哈希</param>
        /// <param name="category">分类</param>
        /// <param name="originalFileName">原始文件名</param>
        /// <returns></returns>
        public async Task<string> PresignedPutUrl(string sysUserName, string hash, Categories category,
            string originalFileName)
        {
            var (bucketName, objectName, _) = BuildMeta(sysUserName, hash, category, originalFileName);
            var minioChannel = MinioChannels.Web;
            var defaultMinio = minioService.GetClient(minioChannel);
            var minioSettings = minioService.GetSettings(minioChannel);
            var url = await defaultMinio.PresignedPutObjectAsync(bucketName, objectName, EXPIRES_SECONDS);
            url = url.Replace($"https://{minioSettings.Endpint}:443", minioSettings.Address);
            url = url.Replace($"http://{minioSettings.Endpint}", minioSettings.Address);
            return url;
        }

        /// <summary>
        /// 获取预签名访问URL
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="hash">文件哈希</param>
        /// <param name="category">分类</param>
        /// <param name="originalFileName">原始文件名</param>
        /// <returns></returns>
        public async Task<string> PresignedGetUrl(string sysUserName, string hash, Categories category,
            string originalFileName)
        {
            var (bucketName, objectName, _) = BuildMeta(sysUserName, hash, category, originalFileName);
            var minioChannel = MinioChannels.Web;
            var defaultMinio = minioService.GetClient(minioChannel);
            var minioSettings = minioService.GetSettings(minioChannel);
            var url = await defaultMinio.PresignedGetObjectAsync(bucketName, objectName, EXPIRES_SECONDS);
            //前端浏览
            url = url.Replace($"https://{minioSettings.Endpint}:443", minioSettings.Address);
            url = url.Replace($"http://{minioSettings.Endpint}", minioSettings.Address);
            return url;
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public async Task<OssFileInfo> GetFileInfo(string hash)
        {
            var ossFile = await ossFileRepository.Get(hash);
            if (ossFile == null)
            {
                return null;
            }

            return new OssFileInfo()
            {
                Category = System.Enum.Parse<Categories>(ossFile.Category),
                OriginalFileName = ossFile.OriginalFileName,
            };
        }

    }

    public interface IOssService
    {
        /// <summary>
        /// 保存上传结果（已完成上传）
        /// </summary>
        /// <param name="sysUserName">所属用户</param>
        /// <param name="hash">文件sha1哈希</param>
        /// <param name="category">归档类型</param>
        /// <param name="originalFileName">上传文件名</param>
        /// <param name="size">文件大小</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="imageProxy">imageproxy图像处理模型</param>
        /// <returns>访问URL</returns>
        Task<OssInfo> SaveUpload(string sysUserName, string hash, Categories category, string originalFileName,
            long size, string contentType = null, ImageProxyModel imageProxy = null);

        /// <summary>
        /// 【无所属用户限定，慎用】获取访问URL
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="hash">文件sha1哈希</param>
        /// <param name="urlBrowsing">是否需要访问URL</param>
        /// <param name="imageProxy">imageproxy图像处理模型</param>
        /// <param name="minioChannel">Minio实例通道</param>
        /// <returns></returns>
        Task<OssInfo> GetUrl(string hash, bool urlBrowsing, ImageProxyModel imageProxy = null, MinioChannels minioChannel = MinioChannels.Web);

        /// <summary>
        /// 获取文件访问/上传信息
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="hash">文件哈希</param>
        /// <param name="category">分类</param>
        /// <param name="originalFileName">原始文件名</param>
        /// <param name="imageProxy">imageproxy图像处理模型</param>
        /// <returns></returns>
        Task<AccessVm> GetAccess(string sysUserName, string hash, Categories category,
            string originalFileName, ImageProxyModel imageProxy = null);

        /// <summary>
        /// 获取Minio文件访问/上传信息
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="hash">文件哈希</param>
        /// <param name="category">分类</param>
        /// <param name="originalFileName">原始文件名</param>
        /// <param name="imageProxy">imageproxy图像处理模型</param>
        /// <returns></returns>
        Task<UploadMetaVm> GetUploadMeta(string sysUserName, string hash, Categories category,
            string originalFileName, ImageProxyModel imageProxy = null);

        /// <summary>
        /// 获取预签名上传URL
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="hash">文件哈希</param>
        /// <param name="category">分类</param>
        /// <param name="originalFileName">原始文件名</param>
        /// <returns></returns>
        Task<string> PresignedPutUrl(string sysUserName, string hash, Categories category,
            string originalFileName);

        /// <summary>
        /// 获取预签名访问URL
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="hash">文件哈希</param>
        /// <param name="category">分类</param>
        /// <param name="originalFileName">原始文件名</param>
        /// <returns></returns>
        Task<string> PresignedGetUrl(string sysUserName, string hash, Categories category,
            string originalFileName);

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task<OssFileInfo> GetFileInfo(string hash);
    }
}
