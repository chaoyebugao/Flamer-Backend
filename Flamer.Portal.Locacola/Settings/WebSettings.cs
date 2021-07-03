using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Portal.LocaCola.Settings
{
    /// <summary>
    /// 主服务配置
    /// </summary>
    public class WebSettings
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 主服务请求地址
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
