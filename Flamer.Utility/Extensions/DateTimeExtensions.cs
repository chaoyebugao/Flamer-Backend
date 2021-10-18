using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Flamer.Utility.Extensions
{
    /// <summary>
    /// 时间
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 获取Unix时间戳，即从格林威治时间1970年01月01日00时00分00秒起至现在的总秒数
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static ulong ToUnixSeconds(this DateTime time)
        {
            var ut = (ulong)(time.ToUniversalTime().Ticks - 621355968000000000);
            return ut / 10000000;
        }

        /// <summary>
        /// Unix时间戳转换为DateTime
        /// </summary>
        /// <param name="unixTotalSeconds">获取Unix时间戳</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this ulong unixTotalSeconds)
        {
            var startUnixTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
            return startUnixTime.AddSeconds(unixTotalSeconds);
        }

        /// <summary>
        /// Unix时间戳转换为DateTime
        /// </summary>
        /// <param name="unixTotalSeconds">获取Unix时间戳</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long unixTotalSeconds)
        {
            var startUnixTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
            return startUnixTime.AddSeconds(unixTotalSeconds);
        }

        /// <summary>
        /// Unix时间戳转换为DateTime，0时返回null值
        /// </summary>
        /// <param name="unixTotalSeconds">获取Unix时间戳</param>
        /// <returns></returns>
        public static DateTime? ToNullableDateTime(this long unixTotalSeconds)
        {
            if(unixTotalSeconds == 0)
            {
                return null;
            }

            return unixTotalSeconds.ToDateTime();
        }

        /// <summary>
        /// 本地时间转为Offset时间
        /// </summary>
        /// <param name="localTime">本地时间</param>
        /// <returns></returns>
        public static DateTimeOffset ToOffset(this DateTime localTime)
        {
            localTime = DateTime.SpecifyKind(localTime, DateTimeKind.Local);

            DateTimeOffset offsetTime = localTime;
            return offsetTime;
        }

        /// <summary>
        /// 转为标准格式日志
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStdString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 从特殊字符串解析
        /// </summary>
        /// <param name="str">字符串日期</param>
        /// <param name="format">格式，如yyyyMMdd</param>
        /// <returns></returns>
        public static DateTime ParseCnExact(this string str, string format)
        {
            var ifp = new CultureInfo("zh-CN", true);

            var dt = DateTime.ParseExact(str, format, ifp);
            return dt;
        }

    }
}
