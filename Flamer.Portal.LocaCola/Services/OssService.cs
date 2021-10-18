using Flamer.Model.ViewModel.Blob;
using Flamer.Portal.LocaCola.Models.Blob.Minio;
using Flamer.Portal.LocalWeb.Areas.Blob.Models.Oss;
using Flamer.Portal.Web.Areas.Blob.Models.Oss;
using System.IO;
using System.Threading.Tasks;
using Flamer.Utility.Security;
using System.Text;
using Newtonsoft.Json;
using Flamer.Service.OSS.Extensions;

namespace Flamer.Portal.LocaCola.Services
{
    public class OssService : IOssService
    {
        private readonly IWebProcessor webProcessor;

        public OssService(IWebProcessor webProcessor)
        {
            this.webProcessor = webProcessor;
        }

        /// <summary>
        /// 获取访问/上传信息
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        public Task<AccessVm> GetAccess(GetAccessSub sub)
        {
            return webProcessor.Post4Data<AccessVm>("/api/blob/oss/getaccess", sub);
        }

        /// <summary>
        /// 获取Minio访问/上传信息
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        public async Task<UploadMetaInfoVm> GetUploadMeta(GetAccessSub sub)
        {
            var vm = await webProcessor.Post4Data<UploadMetaInfoVm>("/api/blob/oss/getuploadmeta", sub);

            if(vm.MinIOSettings?.Length > 0)
            {
                var minioSettingCollectionBytes = vm.MinIOSettings.Decrypt("Yg25ggqhiIUWUq4iLsYqOZScna0b7gDW", "xJuu0M1BJx95vzfH");
                var minioSettingCollectionStr = Encoding.UTF8.GetString(minioSettingCollectionBytes);
                var minioSettingCollection = JsonConvert.DeserializeObject<MinioSettingCollection>(minioSettingCollectionStr);
                vm.MinIOSettings_Decrypt = minioSettingCollection;
            }

            return vm;
        }

        /// <summary>
        /// 获取本地服务地址
        /// </summary>
        /// <returns></returns>
        public Task<string> GetLocalWebAddr()
        {
            return webProcessor.Get4Data<string>("/api/blob/oss/getlocalwebAddr");
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public Task Upload(string url, FileInfo fileInfo)
        {
            return webProcessor.UploadAsync(url, fileInfo);
        }

        /// <summary>
        /// 保存上传
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        public Task<OssInfo> SaveUpload(SaveUploadSub sub)
        {
            return webProcessor.Post4Data<OssInfo>("/api/blob/oss/saveupload", sub);
        }

        #region 本地服务
        /// <summary>
        /// 获取本地服务文件上传URL
        /// </summary>
        /// <param name="localWebAddr">本地服务地址</param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public Task<string> GetLocalUploadUrl(string localWebAddr, PresignedUrlSub sub)
        {
            return webProcessor.Post4Data<string>($"{localWebAddr}/api/blob/oss/getuploadurl", sub);
        }

        /// <summary>
        /// 获取本地服务文件访问URL
        /// </summary>
        /// <param name="localWebAddr">本地服务地址</param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public Task<string> GetLocalUrl(string localWebAddr, PresignedUrlSub sub)
        {
            return webProcessor.Post4Data<string>($"{localWebAddr}/api/blob/oss/geturl", sub);
        }

        #endregion

    }

    public interface IOssService
    {
        /// <summary>
        /// 获取访问/上传信息
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        Task<AccessVm> GetAccess(GetAccessSub sub);

        /// <summary>
        /// 获取Minio访问/上传信息
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        Task<UploadMetaInfoVm> GetUploadMeta(GetAccessSub sub);

        /// <summary>
        /// 获取本地服务请求地址
        /// </summary>
        /// <returns></returns>
        Task<string> GetLocalWebAddr();

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        Task Upload(string url, FileInfo fileInfo);

        /// <summary>
        /// 保存上传
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        Task<OssInfo> SaveUpload(SaveUploadSub sub);

        #region 本地服务
        /// <summary>
        /// 获取本地服务文件上传URL
        /// </summary>
        /// <param name="localWebAddr">本地服务地址</param>
        /// <param name="sub"></param>
        /// <returns></returns>
        Task<string> GetLocalUploadUrl(string localWebAddr, PresignedUrlSub sub);

        /// <summary>
        /// 获取本地服务文件访问URL
        /// </summary>
        /// <param name="localWebAddr">本地服务地址</param>
        /// <param name="sub"></param>
        /// <returns></returns>
        Task<string> GetLocalUrl(string localWebAddr, PresignedUrlSub sub);
        #endregion
    }
}
