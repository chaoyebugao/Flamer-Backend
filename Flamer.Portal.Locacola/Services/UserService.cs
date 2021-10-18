using Flamer.Model.ViewModel.Account;
using Flamer.Portal.LocaCola.Settings;
using Flamer.Portal.Web.Areas.Account.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Portal.LocaCola.Services
{
    public class UserService : IUserService
    {
        private readonly WebSettings webSettings;
        private readonly IWebProcessor webProcessor;

        public UserService(WebSettings webSettings,
            IWebProcessor webProcessor)
        {
            this.webSettings = webSettings;
            this.webProcessor = webProcessor;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public async Task Login()
        {
            var loginRet = await webProcessor.Post4Data<LoginRet>("/api/account/user/login", new LoginSub()
            {
                LoginName = webSettings.LoginName,
                Password = webSettings.Password,
            });

            WebProcessor.LoginRet = loginRet;

            var userInfo = await GetUserInfo();
            WebProcessor.SysUserName = userInfo.Name;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public Task<UserInfo> GetUserInfo()
        {
            return webProcessor.Get4Data<UserInfo>("/api/account/user/getinfo");
        }
    }

    public interface IUserService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        Task Login();

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        Task<UserInfo> GetUserInfo();
    }
}
