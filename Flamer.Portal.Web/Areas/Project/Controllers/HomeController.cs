using Flamer.Data.ViewModels.Projects;
using Flamer.Service.Domain.Projects;
using Flammer.Pagination;
using Flammer.Portal.Web.Areas.Project.Models.Home;
using Flammer.Portal.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flammer.Portal.Web.Areas.Project.Controllers
{
    [Area("project")]
    [CheckTicket]
    [Route("api/[area]/[action]")]
    public class HomeController : BaseController
    {
        private readonly IProjectService projectService;

        public HomeController(IProjectService projectService)
        {
            this.projectService = projectService;
        }

        [ProducesResponseType(typeof(PagedList<ProjectVm>), 200)]
        [HttpPost]
        public async Task<IActionResult> GetList([FromBody] GetListQry qry)
        {
            var list = await projectService.GetList(SysUserName, qry, qry.Keyword);
            return Paged(list);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddSub sub)
        {
            await projectService.Add(SysUserName, sub.Code, sub.Name, sub.Logo);

            return Success();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] EditSub sub)
        {
            await projectService.Edit(SysUserName, sub.Id, sub.Code, sub.Name, sub.Logo);

            return Success();
        }


        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] RemoveSub sub)
        {
             await projectService.Remove(SysUserName, sub.Ids);

            return Success();
        }
    }
}
