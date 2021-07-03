using Flamer.Portal.Web.Areas.OSS.Models.Home;
using Flamer.Service.Domain.Blob;
using Flamer.Service.Domain.Blob.CONST;
using Flammer.Portal.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Flammer.Portal.Web.Areas.OSS.Controllers
{
    [Area("oss")]
    [Route("api/[area]/[action]")]
    public class HomeController : BaseController
    {
        private readonly IOssService ossService;
        private readonly IConfiguration configuration;

        public HomeController(IOssService ossService,
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
            var ossInfo = await ossService.GetUrl(sub.Hash, true, sub.ImageProxy);
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
        /// 为本地访问获取文件信息
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
