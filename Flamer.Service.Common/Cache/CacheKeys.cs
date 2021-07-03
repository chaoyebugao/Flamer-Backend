using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Service.Common.Cache
{
    /// <summary>
    /// 缓存Key
    /// </summary>
    public static class CacheKeys
    {
        /// <summary>
        /// 令牌
        /// </summary>
        public static string Token { get { return "_Token"; } }

        /// <summary>
        /// 令牌系统用户id对应
        /// </summary>
        public static string TokenSysUserName { get { return "_TokenSysUserName"; } }
    }
}
