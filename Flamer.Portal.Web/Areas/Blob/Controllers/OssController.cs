using Flamer.Portal.Web.Areas.Blob.Models.Oss;
using Flamer.Service.Domain.Blob;
using Flamer.Service.Domain.Blob.CONST;
using Flamer.Portal.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Flamer.Service.OSS.Extensions;

namespace Flamer.Portal.Web.Areas.Blob.Controllers
{
    [Area("blob")]
    [Route("api/[area]/[controller]/[action]")]
    public class OssController : BaseController
    {
        private readonly IOssService ossService;
        private readonly IConfiguration configuration;

        public OssController(IOssService ossService,
            IConfiguration configuration)
        {
            this.ossService = ossService;
            this.configuration = configuration;
        }

        /// <summary>
        /// 获取访问信息
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        [CheckTicket]
        [HttpPost]
        public async Task<IActionResult> GetUrl([FromBody] GetUrlSub sub)
        {
            var minioChannel = sub.IsLocal ? MinioChannels.Inner : MinioChannels.Web;
            var ossInfo = await ossService.GetUrl(sub.Hash, true, sub.ImageProxy, minioChannel);
            return Data(ossInfo);
        }

        /// <summary>
        /// 获取访问/上传信息
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        [CheckTicket]
        [HttpPost]
        public async Task<IActionResult> GetAccess([FromBody] GetAccessSub sub)
        {
            var category = Enum.Parse<Categories>(sub.Category);
            var vm = await ossService.GetAccess(SysUserName, sub.Hash, category, sub.OriginalFileName, sub.ImageProxy);
            return Data(vm);
        }

        /// <summary>
        /// 获取Minio访问/上传信息
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        [CheckTicket]
        [HttpPost]
        public async Task<IActionResult> GetUploadMeta([FromBody] GetAccessSub sub)
        {
            var category = Enum.Parse<Categories>(sub.Category);
            var vm = await ossService.GetUploadMeta(SysUserName, sub.Hash, category, sub.OriginalFileName, sub.ImageProxy);
            return Data(vm);
        }

        /// <summary>
        /// 保存上传结果（上传完成）
        /// </summary>
        /// <returns></returns>
        [CheckTicket]
        [HttpPost]
        public async Task<IActionResult> SaveUpload([FromBody] SaveUploadSub sub)
        {
            var category = Enum.Parse<Categories>(sub.Category);
            var ossInfo = await ossService.SaveUpload(SysUserName, sub.Hash, category, sub.OriginalFileName,
                sub.Size, imageProxy: sub.ImageProxy);

            return Data(ossInfo);
        }

        /// <summary>
        /// 为本地服务地址
        /// </summary>
        /// <returns></returns>
        [CheckTicket]
        [HttpGet]
        public IActionResult GetLocalWebAddr()
        {
            return Data(configuration["LocalWebAddr"]);
        }

        /// <summary>
        /// 为本地访问获取文件信息
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        [CheckTicket]
        [HttpPost]
        public async Task<IActionResult> GetFileInfoForLocal([FromBody] GetFileInfoSub sub)
        {
            var ossInfo = await ossService.GetFileInfo(sub.Hash);
            return Data(new
            {
                FileInfo = ossInfo,
                LocalWebAddr = configuration["LocalWebAddr"],
            });
        }

    }
}
