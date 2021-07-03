using Flamer.Data.ViewModels.IPA;
using Flamer.Service.Domain.IPA;
using Flammer.Pagination;
using Flammer.Portal.Web.Areas.IPA.Models.Home;
using Flammer.Portal.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Flammer.Portal.Web.Areas.IPA.Controllers
{
    [Area("ipa")]
    [Route("api/[area]/[action]")]
    public class HomeController : BaseController
    {
        private readonly IIpaService ipaService;

        public HomeController(IIpaService ipaService)
        {
            this.ipaService = ipaService;
        }

        #region 外部开放

        /// <summary>
        /// 安装页面展示信息展示
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="projectCode"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{sysUserName}/{projectCode}/{id?}")]
        public async Task<IActionResult> Get(string sysUserName, string projectCode, string id)
        {
            var vm = await ipaService.GetForInstallPage(sysUserName, projectCode, id);
            return Data(vm);
        }

        /// <summary>
        /// 远程安装
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="projectCode">项目代码</param>
        /// <param name="id">指定的ipa记录</param>
        /// <returns></returns>
        [HttpGet("{sysUserName}/{projectCode}/{id?}")]
        public async Task<IActionResult> Plist(string sysUserName, string projectCode, string id)
        {
            var vm = await ipaService.GetForInstallTemplate(sysUserName, projectCode, id);
            var plistTemplate = await System.IO.File.ReadAllTextAsync("Templates/info.plist.template");

            var plist = string.Format(plistTemplate, vm.SoftwarePackage, vm.FullSizeImage, vm.DisplayImage, vm.Title, vm.Version, vm.Identifier);
            return File(Encoding.UTF8.GetBytes(plist), "binary/octet-stream");
        }

        /// <summary>
        /// 指定ipa文件URL安装（本地安装）
        /// </summary>
        /// <param name="ipaUrl">指定的ipa文件URL</param>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="projectCode">项目代码</param>
        /// <param name="id">指定的ipa记录</param>
        /// <returns></returns>
        [HttpGet("{ipaUrl}/{sysUserName}/{projectCode}/{id?}")]
        public async Task<IActionResult> SpecificPlist(string ipaUrl, string sysUserName, string projectCode, string id)
        {
            var vm = await ipaService.GetForInstallTemplate(sysUserName, projectCode, id, true);
            vm.SoftwarePackage = HttpUtility.UrlDecode(ipaUrl);
            var plistTemplate = await System.IO.File.ReadAllTextAsync("Templates/info.plist.template");

            var plist = string.Format(plistTemplate, vm.SoftwarePackage, vm.FullSizeImage, vm.DisplayImage, vm.Title, vm.Version, vm.Identifier);
            return File(Encoding.UTF8.GetBytes(plist), "binary/octet-stream");
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
            var list = await ipaService.GetHistoryList(qry.SysUserName, qry, qry.ProjectCode);
            return Paged(list);
        }
        #endregion

        #region 管理端
        [CheckTicket]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddSub sub)
        {
            await ipaService.Add(sub.ProjectId, sub.Identifier, sub.Version, sub.FullSizeImage, sub.SoftwarePackage);

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
