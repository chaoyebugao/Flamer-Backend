using Flammer.Portal.Web.Areas.Account.Models.User;
using Flammer.Portal.Web.Attributes;
using Flammer.Service.Domain.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Flammer.Portal.Web.Areas.Account.Controllers
{
    [Area("account")]
    [Route("api/[area]/[controller]/[action]")]
    public class UserController : BaseController
    {
        private readonly IUserService userService;
        private readonly IConfiguration configuration;

        public UserController(IUserService userService,
            IConfiguration configuration)
        {
            this.userService = userService;
            this.configuration = configuration;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpSub sub)
        {
            await userService.SignUpAsync(sub.Name, sub.Email, sub.Password);

            return Success();
        }

        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="t">令牌</param>
        /// <param name="ts">时间戳</param>
        /// <param name="s">签名</param>
        /// <param name="n">随机字符串</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Activate(string t, ulong ts, string s, string n)
        {
            await userService.SignUpActivateAsync(t, ts, s, n);

            var redirectUrl = $"{configuration["FrontAddr"]}/#/account/activated";

            return Redirect(redirectUrl);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginSub sub)
        {
            var ticket = await userService.LoginAsync(sub.LoginName, sub.Password);

            return Data(new
            {
                ticket.Token,
                ExpireAt = ticket.ExpireAt.ToUnixTimeSeconds(),
            });
        }


        /// <summary>
        /// 注销登录
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Cookies["tid"];
            if (token == null)
            {
                return Success();
            }

            await userService.LogoutAsync(token);

            return Success();
        }


        /// <summary>
        /// 注销登录
        /// </summary>
        /// <returns></returns>
        [CheckTicket]
        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            var info = await userService.GetInfoAsync(SysUserName);

            return Data(info);
        }
    }
}
