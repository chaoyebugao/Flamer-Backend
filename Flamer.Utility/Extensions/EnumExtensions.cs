using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Flammer.Utility.Extensions
{
    /// <summary>
    /// 枚举
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取描述
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">枚举值</param>
        /// <returns></returns>
        public static string GetDescription<T>(this T value) where T : Enum
        {
            var memberInfoes = typeof(T).GetMember(value.ToString());
            if ((memberInfoes != null && memberInfoes.Length > 0))
            {
                var attrs = memberInfoes[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if ((attrs != null && attrs.Count() > 0))
                {
                    return ((DescriptionAttribute)attrs.ElementAt(0)).Description;
                }
            }
            return value.ToString();
        }
    }
}
