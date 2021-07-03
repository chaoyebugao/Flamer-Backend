using Flamer.Portal.LocalWeb.Areas.OSS.Models.Home;
using Flamer.Service.Domain.Blob;
using Flamer.Service.Domain.Blob.CONST;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Flammer.Portal.LocalWeb.Areas.OSS.Controllers
{
    [Area("oss")]
    [Route("api/[area]/[action]")]
    public class HomeController : BareController
    {
        private readonly IOssService ossService;
        public HomeController(IOssService ossService)
        {
            this.ossService = ossService;
        }

        /// <summary>
        /// 获取上传URL
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        //[CheckTicket]
        [HttpPost]
        public async Task<IActionResult> GetUploadUrl([FromBody] PresignedUrlSub sub)
        {
            var category = Enum.Parse<Categories>(sub.Category);
            var vm = await ossService.PresignedPutUrl(sub.SysUserName, sub.Hash, category, sub.OriginalFileName);
            return Data(vm);
        }

        /// <summary>
        /// 获取访问URL
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        //[CheckTicket]
        [HttpPost]
        public async Task<IActionResult> GetUrl([FromBody] PresignedUrlSub sub)
        {
            var category = Enum.Parse<Categories>(sub.Category);
            var url = await ossService.PresignedGetUrl(sub.SysUserName, sub.Hash, category, sub.OriginalFileName);
            url = HttpUtility.UrlEncode(url);
            return Data(url);
        }

    }
}
