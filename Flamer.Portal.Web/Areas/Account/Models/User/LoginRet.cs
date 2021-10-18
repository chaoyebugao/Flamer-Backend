using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Portal.Web.Areas.Account.Models.User
{
    /// <summary>
    /// 登录结果
    /// </summary>
    public class LoginRet
    {
        public string Token { get; set; }

        public long ExpireAt { get; set; }

    }
}
