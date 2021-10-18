using Flamer.Model.ViewModel.IPA;
using Flamer.Pagination;
using Flamer.Portal.Web.Areas.IPA.Models.Home;
using Flamer.Portal.Web.Attributes;
using Flamer.Service.Domain.Blob;
using Flamer.Service.Domain.IPA;
using Flamer.Service.OSS.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Flamer.Portal.Web.Areas.IPA.Controllers
{
    [Area("ipa")]
    [Route("api/[area]/[action]")]
    public class HomeController : BaseController
    {
        private readonly IIpaService ipaService;
        private readonly IOssService ossService;

        public HomeController(IIpaService ipaService,
            IOssService ossService)
        {
            this.ipaService = ipaService;
            this.ossService = ossService;
        }

        #region 外部开放

        /// <summary>
        /// 安装页面展示信息展示
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetQry qry)
        {
            var vm = await ipaService.GetForInstallPage(qry.projectCode, qry.sysUserName, qry.id);
            return Data(vm);
        }

        /// <summary>
        /// 远程安装
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="projectCode">项目代码</param>
        /// <param name="id">指定的ipa记录</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Plist(string id)
        {
            var vm = await ipaService.GetForInstallTemplate(id);
            var plistTemplate = await System.IO.File.ReadAllTextAsync("Templates/info.plist.template");

            var plist = string.Format(plistTemplate, vm.SoftwarePackage, vm.FullSizeImage, vm.DisplayImage, vm.Title, vm.Version, vm.Identifier);
            return File(Encoding.UTF8.GetBytes(plist), "application/octet-stream");
        }

        /// <summary>
        /// 指定ipa文件URL安装（本地安装）
        /// </summary>
        /// <param name="id">指定的ipa记录</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> LocalPlist(string id)
        {
            var vm = await ipaService.GetForInstallTemplate(id);
            var localIpaInfo = await ossService.GetUrl(vm.SoftwarePackageHash, true, minioChannel: MinioChannels.Inner);
            vm.SoftwarePackage = HttpUtility.UrlDecode(localIpaInfo.Url);
            var plistTemplate = await System.IO.File.ReadAllTextAsync("Templates/info.plist.template");

            var plist = string.Format(plistTemplate, vm.SoftwarePackage, vm.FullSizeImage, vm.DisplayImage, vm.Title, vm.Version, vm.Identifier);
            return File(Encoding.UTF8.GetBytes(plist), "application/octet-stream");
        }

        /// <summary>
        /// 历史记录
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(PagedList<IpaBundleVm>), 200)]
        [CheckTicket]
        [HttpPost]
        public async Task<IActionResult> GetHistoryList([FromBody] GetHistoryListQry qry)
        {
            var list = await ipaService.GetHistoryList(qry, qry.ProjectCode, qry.SysUserName);
            return Paged(list);
        }
        #endregion

        #region 管理端
        [CheckTicket]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddSub sub)
        {
            await ipaService.Add(SysUserName, sub.ProjectCode, sub.Identifier, sub.Version, sub.FullSizeImage, sub.SoftwarePackage, sub.Env);

            return Success();
        }

        [CheckTicket]
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] EditSub sub)
        {
            await ipaService.Edit(SysUserName, sub.Id, sub.ProjectId, sub.Identifier, sub.Version, sub.FullSizeImage, sub.SoftwarePackage);

            return Success();
        }

        [CheckTicket]
        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] RemoveSub sub)
        {
            await ipaService.Remove(SysUserName, sub.Ids);
            return Success();
        }

        [ProducesResponseType(typeof(PagedList<IpaBundleVm>), 200)]
        [CheckTicket]
        [HttpPost]
        public async Task<IActionResult> GetList([FromBody] GetListQry qry)
        {
            var list = await ipaService.GetList(SysUserName, qry, qry.Keyword);
            return Paged(list);
        }
        #endregion
    }
}
