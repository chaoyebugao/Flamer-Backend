using Flammer.Utility.Base62;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Flammer.Utility.Security
{
    /// <summary>
    /// SHA1哈希
    /// </summary>
    public static class Sha1Hasher
    {
        /// <summary>
        /// 从流获取SHA1哈希值
        /// </summary>
        /// <param name="inputStream">流</param>
        /// <returns></returns>
        public static string GetSha1(this Stream inputStream)
        {
            using var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(inputStream);
            var formatted = new StringBuilder(2 * hash.Length);
            foreach (byte b in hash)
            {
                formatted.AppendFormat("{0:X2}", b);
            }

            return formatted.ToString();
        }

        /// <summary>
        /// 从流获取SHA1哈希值（Base62）
        /// </summary>
        /// <param name="inputText">文本</param>
        /// <returns></returns>
        public static string GetBase62Sha1(this string inputText)
        {
            using var sha1 = new SHA1Managed();
            var buffer = Encoding.UTF8.GetBytes(inputText);
            var hash = sha1.ComputeHash(buffer);

            return hash.ToBase62();
        }

        /// <summary>
        /// 从流获取SHA1哈希值（Base64）
        /// </summary>
        /// <param name="inputText">文本</param>
        /// <returns></returns>
        public static string GetBase64Sha1(this string inputText)
        {
            using var sha1 = new SHA1Managed();
            var buffer = Encoding.UTF8.GetBytes(inputText);
            var hash = sha1.ComputeHash(buffer);

            return Convert.ToBase64String(hash);
        }

    }
}
