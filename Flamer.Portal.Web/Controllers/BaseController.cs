using Flamer.Model.Result.WebResults;
using Flammer.Pagination;
using Flammer.Service.Domain.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Flammer.Portal.Web
{
    /// <summary>
    /// 通用基类
    /// </summary>
    public class BaseController : BareController
    {
        /// <summary>
        /// 返回用户
        /// </summary>
        /// <returns></returns>
        public string SysUserName
        {
            get
            {
                var userService = Request.HttpContext.RequestServices.GetService<IUserService>();
                var cookieToken = Request.Cookies["tid"];
                var task = userService.GetNameByTokenAsync(cookieToken);
                return task.Result;
            }
        }

    }
}
