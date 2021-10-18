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
        public static string Token(string key)
        {
            return $"Token_{key}";
        }

        /// <summary>
        /// 令牌系统用户id对应
        /// </summary>
        public static string TokenSysUserName(string key)
        {
            return $"TokenSysUserName_{key}";
        }
    }
}
