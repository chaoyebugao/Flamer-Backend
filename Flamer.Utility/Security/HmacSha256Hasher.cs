using Flammer.Utility.Base62;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Flammer.Utility.Security
{
    /// <summary>
    /// HMAC SHA256哈希
    /// </summary>
    public static class HmacSha256Hasher
    {
        /// <summary>
        /// 创建HMAC SHA256哈希字符串（Base64）
        /// </summary>
        /// <param name="message">原始信息</param>
        /// <param name="secret">干扰项，盐</param>
        /// <returns>哈希字符串（Base64）</returns>
        public static string Create(byte[] message, string secret)
        {
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException("secret");
            }
            var encoding = new System.Text.ASCIIEncoding();
            var keyBytes = encoding.GetBytes(secret);
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(message);
                return Convert.ToBase64String(hashmessage);
            }
        }

        /// <summary>
        /// 创建HMAC SHA256哈希字符串（Base64）
        /// </summary>
        /// <param name="message">原始信息</param>
        /// <param name="secret">干扰项，盐</param>
        /// <returns>哈希字符串（Base64）</returns>
        public static string Create(string message, string secret)
        {
            var encoding = new System.Text.ASCIIEncoding();
            var messageBytes = encoding.GetBytes(message);

            return Create(messageBytes, secret);
        }

        /// <summary>
        /// 创建HMAC SHA256哈希字符串（Base62）
        /// </summary>
        /// <param name="message">原始信息</param>
        /// <param name="secret">干扰项，盐</param>
        /// <returns>Base62字符串</returns>
        public static string CreateAsBase62(string message, string secret)
        {
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException("secret");
            }
            var encoding = new System.Text.ASCIIEncoding();
            var keyBytes = encoding.GetBytes(secret);
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                var messageBytes = encoding.GetBytes(message);
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                var hash = hashmessage.ToBase62();
                return hash;
            }
        }

        /// <summary>
        /// 创建HMAC SHA256哈希字符串（字节数组）
        /// </summary>
        /// <param name="message">原始信息</param>
        /// <param name="secret">干扰项，盐</param>
        /// <returns>哈希</returns>
        public static byte[] CreateToBytes(string message, string secret)
        {
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException("secret");
            }
            var encoding = new System.Text.ASCIIEncoding();
            var keyBytes = encoding.GetBytes(secret);
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                var messageBytes = encoding.GetBytes(message);
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return hashmessage;
            }
        }

        /// <summary>
        /// 创建HMAC SHA256哈希字符串（字节数组）
        /// </summary>
        /// <param name="message">原始信息</param>
        /// <param name="secret">干扰项，盐</param>
        /// <returns>哈希</returns>
        public static byte[] CreateToBytes(byte[] messageBytes, string secret)
        {
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException("secret");
            }
            var encoding = new System.Text.ASCIIEncoding();
            var keyBytes = encoding.GetBytes(secret);
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return hashmessage;
            }
        }

        /// <summary>
        /// 创建HMAC SHA256哈希字符串（字节数组）
        /// </summary>
        /// <param name="message">原始信息</param>
        /// <param name="secret">干扰项，盐</param>
        /// <returns>哈希</returns>
        public static byte[] CreateToBytes(byte[] messageBytes, byte[] secretBytes)
        {
            if (secretBytes == null || secretBytes.Length == 0)
            {
                throw new ArgumentNullException("secret");
            }

            var keyBytes = secretBytes;
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return hashmessage;
            }
        }

    }
}
