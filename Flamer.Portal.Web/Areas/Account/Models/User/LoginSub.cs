using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flammer.Portal.Web.Areas.Account.Models.User
{
    public class LoginSub
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
