using System;
using System.Collections.Generic;
using System.Text;

namespace Flammer.Utility.Text
{
    /// <summary>
    /// 字符串
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// 将传入的字符串中间部分字符替换成特殊字符
        /// </summary>
        /// <param name="value">需要替换的字符串</param>
        /// <param name="startLen">前保留长度</param>
        /// <param name="endLen">尾保留长度</param>
        /// <param name="specialChar">特殊字符.</param>
        /// <returns>
        /// 被特殊字符替换的字符串
        /// </returns>
        public static string ReplaceWithSpecialChar(this string value, int startLen, int endLen = 4, char specialChar = '*')
        {
            if (string.IsNullOrEmpty(value) || value.Length <= startLen + endLen)
            {
                return value;
            }

            var pre = value.Substring(0, startLen);

            int midLen = value.Length - startLen - endLen;
            var middle = GenerateRepeatedStr(midLen, specialChar);

            var sub = value.Substring(value.Length - endLen, endLen);

            return pre + middle + sub;
        }

        /// <summary>
        /// 根据指定长度比来隐藏尾部
        /// </summary>
        /// <param name="value">需要替换的字符串</param>
        /// <param name="preRemainLength">前部保留</param>
        /// <param name="specialCharLength">特殊字符长度</param>
        /// <returns></returns>
        public static string ReplaceSub(this string value, int preRemainLength, int specialCharLength)
        {
            var pre = value.Substring(0, preRemainLength);

            var sub = GenerateRepeatedStr(specialCharLength);

            return pre + sub;
        }

        /// <summary>
        /// 根据百分比来隐藏尾部
        /// </summary>
        /// <param name="value">需要替换的字符串</param>
        /// <param name="percent">百分比，0-100</param>
        /// <returns></returns>
        public static string ReplaceSubByPercentage(this string value, int percent)
        {
            var subLen = value.Length * percent / 100;
            subLen = subLen > value.Length ? value.Length : subLen;

            var startLen = value.Length - subLen;

            var pre = value.Substring(0, startLen);

            var sub = GenerateRepeatedStr(subLen);

            return pre + sub;
        }

        /// <summary>
        /// 生成字符重复的字符串
        /// </summary>
        /// <param name="len">长度</param>
        /// <param name="character">字符</param>
        /// <returns></returns>
        public static string GenerateRepeatedStr(int len, char character = '*')
        {
            var chars = new List<char>();
            for (var i = 0; i < len; i++)
            {
                chars.Add(character);
            }
            return new string(chars.ToArray());
        }

        /// <summary>
        /// 统一手机号码部分隐藏风格
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <returns></returns>
        public static string PhonePartiallyHidden(this string phone)
        {
            return phone?.Trim().ReplaceWithSpecialChar(3, 3);
        }

        /// <summary>
        /// 统一二代身份证部分隐藏风格
        /// </summary>
        /// <param name="idnum">身份证号码</param>
        /// <returns></returns>
        public static string IdNumPartiallyHidden(this string idnum)
        {
            return idnum?.Trim().ReplaceWithSpecialChar(4, 4);
        }

        /// <summary>
        /// 手机敏感处理隐藏
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns></returns>
        public static string PhoneSensitivelyHidden(this string phone)
        {
            phone = (phone ?? string.Empty).Trim();
            if (phone.Length <= 11)
            {
                return PhonePartiallyHidden(phone);
            }

            var pre = phone.Substring(0, 3);
            var sub = phone.Substring(phone.Length - 3, 3);

            var mid = GenerateRepeatedStr(5);
            return pre + mid + sub;
        }

        /// <summary>
        /// 身份证号敏感处理隐藏
        /// </summary>
        /// <param name="idNum">身份证号</param>
        /// <returns></returns>
        public static string IdNumSensitivelyHidden(this string idNum)
        {
            idNum = (idNum ?? string.Empty).Trim();
            if (idNum.Length <= 18)
            {
                return IdNumPartiallyHidden(idNum);
            }

            var pre = idNum.Substring(0, 3);
            var sub = idNum.Substring(idNum.Length - 3, 3);

            var mid = GenerateRepeatedStr(12);
            return pre + mid + sub;
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length">目标字符串的长度</param>
        /// <param name="useNum">是否包含数字，1=包含，默认为包含</param>
        /// <param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        /// <param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        /// <param name="useSpecial">是否包含特殊字符，1=包含，默认为不包含</param>
        /// <param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        /// <returns>
        /// 指定长度的随机字符串
        /// </returns>
        public static string GetRandomString(int length, bool useNum = true, bool useLow = true, bool useUpp = true, bool useSpecial = false, string custom = null)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true)
            {
                str += "0123456789";
            }
            if (useLow == true)
            {
                str += "abcdefghijklmnopqrstuvwxyz";
            }
            if (useUpp == true)
            {
                str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            if (useSpecial == true)
            {
                str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }
    }
}
