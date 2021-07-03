using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamer.Service.ImageProxy.OptionBuilder
{
    /// <summary>
    /// ImageProxy URL参数修改扩展实现
    /// </summary>
    /// <remarks>
    /// 因服务端和App端对图片均做有缓存，所以对于URL的参数，相同图片应尽量取相同的参数，有不同，尽量固定几个值
    /// 参数修改扩展参考：https://godoc.org/willnorris.com/go/imageproxy#ParseOptions
    /// </remarks>
    public static class Extensions
    {
        /// <summary>
        /// 原图标记参数
        /// </summary>
        public const string ORIGIN_OPTION = "/0x0/";

        /// <summary>
        /// 从字符串转为参数
        /// </summary>
        /// <param name="imageProxyUrl">原始URL</param>
        /// <returns></returns>
        public static ImageProxyOptions AsImageProxyOptions(this string imageProxyUrl)
        {
            return new ImageProxyOptions(imageProxyUrl);
        }

        /// <summary>
        /// 设置宽度
        /// </summary>
        /// <param name="options">参数</param>
        /// <param name="width">宽度。(0, 1)单位为原值比例，>=1单位为像素</param>
        /// <returns></returns>
        public static ImageProxyOptions Width(this ImageProxyOptions options, double width)
        {
            options.SetWidth(width);

            return options;
        }

        /// <summary>
        /// 设置高度
        /// </summary>
        /// <param name="options">参数</param>
        /// <param name="height">高度。(0, 1)单位为原值比例，>=1单位为像素</param>
        /// <returns></returns>
        public static ImageProxyOptions Height(this ImageProxyOptions options, double height)
        {
            options.SetHeight(height);

            return options;
        }

        /// <summary>
        /// 正方形
        /// </summary>
        /// <param name="options">参数</param>
        /// <param name="width">宽度。(0, 1)单位为原值比例，>=1单位为像素</param>
        /// <param name="fit">是否自动缩小到指定正方形，不裁剪</param>
        /// <returns></returns>
        public static ImageProxyOptions Square(this ImageProxyOptions options, double width, bool fit = false)
        {
            options.Square(width);
            if (fit)
            {
                options.SetFit();
            }

            return options;
        }

        /// <summary>
        /// 设置为自动缩小，不裁剪
        /// </summary>
        /// <param name="options">参数</param>
        /// <returns></returns>
        public static ImageProxyOptions Fit(this ImageProxyOptions options)
        {
            options.SetFit();

            return options;
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="options">参数</param>
        /// <param name="rotate"><param name="rotate">逆时针旋转度数</param></param>
        /// <returns></returns>
        public static ImageProxyOptions Rotate(this ImageProxyOptions options, Rotates rotate)
        {
            options.SetRotate(rotate);

            return options;
        }

        /// <summary>
        /// 翻转
        /// </summary>
        /// <param name="options">参数</param>
        /// <param name="horizontal">垂直翻转</param>
        /// <param name="vertical">水平翻转</param>
        /// <returns></returns>
        public static ImageProxyOptions Flip(this ImageProxyOptions options, bool horizontal, bool vertical)
        {
            options.Flip(horizontal, vertical);

            return options;
        }

        /// <summary>
        /// 水平翻转
        /// </summary>
        /// <param name="options">参数</param>
        /// <param name="horizontal">垂直翻转</param>
        /// <param name="vertical">水平翻转</param>
        /// <returns></returns>
        public static ImageProxyOptions FlipHorizontal(this ImageProxyOptions options)
        {
            options.Flip(true, false);

            return options;
        }

        /// <summary>
        /// 垂直翻转
        /// </summary>
        /// <param name="options">参数</param>
        /// <param name="horizontal">垂直翻转</param>
        /// <param name="vertical">水平翻转</param>
        /// <returns></returns>
        public static ImageProxyOptions FlipVertical(this ImageProxyOptions options)
        {
            options.Flip(false, true);

            return options;
        }

        /// <summary>
        /// 设定质量(仅JPEG)
        /// </summary>
        /// <remarks>如果未指定，则默认95</remarks>
        /// <param name="options">参数</param>
        /// <param name="quality">原值比例(1,99)， 如50表示为原图质量50%</param>
        /// <returns></returns>
        public static ImageProxyOptions Quality(this ImageProxyOptions options, int quality)
        {
            options.SetQuality(quality);

            return options;
        }

        /// <summary>
        /// 格式转换
        /// </summary>
        /// <remarks>部分格式之间转换不支持</remarks>
        /// <param name="options">参数</param>
        /// <param name="format">格式</param>
        /// <returns></returns>
        public static ImageProxyOptions Convert(this ImageProxyOptions options, ConvertableFormats format)
        {
            options.Convert(format);

            return options;
        }

        /// <summary>
        /// 矩形剪裁
        /// </summary>
        /// <param name="options">参数</param>
        /// <param name="x">坐标x，左上角为坐标原点，默认0</param>
        /// <param name="y">坐标y，左上角为坐标原点，默认0</param>
        /// <param name="width">宽度，默认原图宽度</param>
        /// <param name="height">高度，默认原图高度</param>
        /// <returns></returns>
        public static ImageProxyOptions Crop(this ImageProxyOptions options, double? x = null, double? y = null, double? width = null, double? height = null)
        {
            options.Crop(x, y, width, height);

            return options;
        }

        /// <summary>
        /// 智能剪裁
        /// </summary>
        /// <remarks>基于内容感知，如设置将覆盖其他剪裁参数</remarks>
        /// <param name="options">参数</param>
        /// <returns></returns>
        public static ImageProxyOptions SmartCrop(this ImageProxyOptions options)
        {
            options.SetSmartCrop();

            return options;
        }


    }

   


}
