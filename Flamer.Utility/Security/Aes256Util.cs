using System.Security.Cryptography;
using System.Text;


namespace Flamer.Utility.Security
{
    /// <summary>
    /// AES CBC加密
    /// </summary>
    public static class Aes256Util
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="key">秘钥，32字节/256位</param>
        /// <param name="iv">向量，16字节/128位</param>
        /// <returns>加密数据</returns>
        public static byte[] Encrypt(this byte[] data, string key, string iv)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var ivBytes = Encoding.UTF8.GetBytes(iv);
            return Encrypt(data, keyBytes, ivBytes);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="keyBytes">秘钥，32字节/256位</param>
        /// <param name="ivBytes">向量，16字节/128位</param>
        /// <returns>加密数据</returns>
        public static byte[] Encrypt(this byte[] data, byte[] keyBytes, byte[] ivBytes)
        {
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var transform = aes.CreateEncryptor(keyBytes, ivBytes))
                {
                    var outputData = transform.TransformFinalBlock(data, 0, data.Length);//加密
                    return outputData;
                }
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <param name="key">秘钥，32字节/256位</param>
        /// <param name="iv">向量，16字节/128位</param>
        /// <returns>原始数据</returns>
        public static byte[] Decrypt(this byte[] data, string key, string iv)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var ivBytes = Encoding.UTF8.GetBytes(iv);
            return Decrypt(data, keyBytes, ivBytes);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <param name="keyBytes">秘钥，32字节/256位</param>
        /// <param name="ivBytes">向量，16字节/128位</param>
        /// <returns>原始数据</returns>
        public static byte[] Decrypt(this byte[] data, byte[] keyBytes, byte[] ivBytes)
        {
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var transform = aes.CreateDecryptor(keyBytes, ivBytes))
                {
                    var outputData = transform.TransformFinalBlock(data, 0, data.Length);
                    return outputData;
                }
            }
        }

    }

}
