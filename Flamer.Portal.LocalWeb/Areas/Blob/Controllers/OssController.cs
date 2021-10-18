using Flamer.Portal.LocalWeb.Areas.Blob.Models.Oss;
using Flamer.Service.Domain.Blob;
using Flamer.Service.Domain.Blob.CONST;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Flamer.Portal.LocalWeb.Areas.Blob.Controllers
{
    [Area("blob")]
    [Route("api/[area]/[controller]/[action]")]
    public class OssController : BareController
    {
        private readonly IOssService ossService;
        public OssController(IOssService ossService)
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
            var url = await ossService.PresignedPutUrl(sub.SysUserName, sub.Hash, category, sub.OriginalFileName);
            return Data(url);
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
            var ossInfo = await ossService.GetUrl(sub.Hash, true);
            var url = HttpUtility.UrlEncode(ossInfo.Url);
            return Data(url);
        }

    }
}
