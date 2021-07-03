using System;
using System.Collections.Generic;
using System.Text;

namespace Flamer.Service.ImageProxy.OptionBuilder
{
    /// <summary>
    /// 转换格式
    /// </summary>
    public enum ConvertableFormats
    {
        /// <summary>
        /// 转换
        /// </summary>
        NoConvert = 0,

        /// <summary>
        /// jpeg
        /// </summary>
        Jpeg = 1,

        /// <summary>
        /// png
        /// </summary>
        Png = 2,

        /// <summary>
        /// tiff
        /// </summary>
        Tiff = 3,
    }
}
