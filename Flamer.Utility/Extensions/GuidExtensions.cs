using System;
using System.Collections.Generic;
using System.Text;

namespace Flamer.Utility.Extensions
{
    /// <summary>
    /// Guid扩展
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// 转长整型
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns></returns>
        public static long ToLong(this Guid guid)
        {
            var buffer = guid.ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// 转长整型字符串
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns></returns>
        public static string ToLongStr(this Guid guid)
        {
            return ToLong(guid).ToString();
        }

    }
}
