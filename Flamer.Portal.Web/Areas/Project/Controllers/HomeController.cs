using Flamer.Model.ViewModel.Projects;
using Flamer.Pagination;
using Flamer.Portal.Web.Areas.Project.Models.Home;
using Flamer.Portal.Web.Attributes;
using Flamer.Service.Domain.Projects;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flamer.Portal.Web.Areas.Project.Controllers
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
            var list = await projectService.GetList(qry, qry.Creator, qry.Keyword);
            return Paged(list);
        }

        [HttpPost]
        public async Task<IActionResult> GetSelectList([FromBody] GetListQry qry)
        {
            var list = await projectService.GetListForSelect(qry.Keyword);
            return Data(list);
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
            await projectService.Edit(sub.Id, sub.Code, sub.Name, sub.Logo);

            return Success();
        }


        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] RemoveSub sub)
        {
            await projectService.Remove(sub.Ids);

            return Success();
        }
    }
}
