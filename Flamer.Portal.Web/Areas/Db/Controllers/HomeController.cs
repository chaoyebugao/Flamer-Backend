using Flammer.Data.ViewModels.Db;
using Flammer.Pagination;
using Flammer.Portal.Web.Areas.Db.Models.Home;
using Flammer.Portal.Web.Attributes;
using Flammer.Service.Domain.Db;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Flammer.Portal.Web.Areas.Db.Controllers
{
    [Area("db")]
    [CheckTicket]
    [Route("api/[area]/[action]")]
    public class HomeController : BaseController
    {
        private readonly IDatabaseService databaseService;

        public HomeController(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        [ProducesResponseType(typeof(PagedList<SchemeVm>), 200)]
        [HttpPost]
        public async Task<IActionResult> GetList([FromBody] GetListQry qry)
        {
            var list = await databaseService.GetList(SysUserName, qry, qry.Keyword);

            return Paged(list);
        }

        [HttpPost]
        public async Task<IActionResult> AddScheme([FromBody] AddSchemeSub sub)
        {
            var schemeUsers = sub.Users?.Select(m => new NetworkCredential(m.Key, m.Value));
            await databaseService.AddSchemeAsync(SysUserName, sub.ProjectId, sub.VolPath, sub.Host, sub.Name, schemeUsers);

            return Success();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] AddUserSub sub)
        {
            await databaseService.AddUserAsync(sub.SchemeId, new NetworkCredential(sub.Username, sub.Password));

            return Success();
        }

        //[HttpPost]
        //public async Task<IActionResult> RemoveScheme([FromBody] RemoveSchemeSub sub)
        //{
        //    await databaseService.RemoveSchemeAsync(sub.DbSchemeId);

        //    return Success();
        //}

        [HttpPost]
        public async Task<IActionResult> RemoveUser([FromBody] RemoveUserSub sub)
        {
            await databaseService.RemoveUserAsync(sub.DbUserId);

            return Success();
        }
    }
}
