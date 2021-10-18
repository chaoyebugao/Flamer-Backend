using Flamer.Service.Domain.User;
using Microsoft.Extensions.DependencyInjection;

namespace Flamer.Portal.Web
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
