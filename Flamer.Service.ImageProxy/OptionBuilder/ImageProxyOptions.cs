using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Flamer.Service.ImageProxy.OptionBuilder
{
    /// <summary>
    /// 参数修改
    /// </summary>
    public class ImageProxyOptions
    {
        /// <summary>
        /// 图像组装URL
        /// </summary>
        public string Url { get; private set; }

        #region Option values

        /// <summary>
        /// 宽度
        /// </summary>
        public string Width { get; private set; }

        /// <summary>
        /// 高度
        /// </summary>
        public string Height { get; private set; }

        /// <summary>
        /// 是否自动匹配，不剪裁
        /// </summary>
        public string Fit { get; private set; }

        /// <summary>
        /// 旋转度数
        /// </summary>
        public string Rotate { get; private set; }

        /// <summary>
        /// 水平翻转
        /// </summary>
        public string FlipHorizontal { get; private set; }

        /// <summary>
        /// 垂直翻转
        /// </summary>
        public string FlipVertical { get; private set; }

        /// <summary>
        /// 质量(仅JPEG)
        /// </summary>
        public string Quality { get; private set; }

        /// <summary>
        /// 格式
        /// </summary>
        /// <remarks>指定转换为 "jpeg", "png", 或"tiff"格式图像</remarks>
        public string Format { get; private set; }

        /// <summary>
        /// 矩形裁剪 - 坐标X
        /// </summary>
        public string CropX { get; private set; }

        /// <summary>
        /// 矩形裁剪 - 坐标y
        /// </summary>
        public string CropY { get; private set; }

        /// <summary>
        /// 矩形裁剪 - 宽度
        /// </summary>
        public string CropWidth { get; private set; }

        /// <summary>
        /// 矩形裁剪 - 高度
        /// </summary>
        public string CropHeight { get; private set; }

        /// <summary>
        /// 内容识别智能剪裁
        /// </summary>
        /// <remarks>此项将覆盖其他所有剪裁相关参数</remarks>
        public string SmartCrop { get; private set; }

        #endregion

        public ImageProxyOptions(string imageProxyUrl)
        {
            Url = imageProxyUrl;
        }

        /// <summary>
        /// 设置宽度，不指定高度时高度自动
        /// </summary>
        /// <param name="width">宽度。(0, 1)单位为原值比例，>=1单位为像素</param>
        internal void SetWidth(double width) => this.Width = width.ToString();

        /// <summary>
        /// 设置高度，不指定宽度时跨宽度自动
        /// </summary>
        /// <param name="height">高度。(0, 1)单位为原值比例，>=1单位为像素</param>
        internal void SetHeight(double height) => this.Height = height.ToString();

        /// <summary>
        /// 正方形
        /// </summary>
        /// <param name="width">宽度。(0, 1)单位为原值比例，>=1单位为像素</param>
        internal void Square(double width)
        {
            this.Width = width.ToString();
            Height = null;
        }

        /// <summary>
        /// 设置为自动缩小，不裁剪
        /// </summary>
        internal void SetFit()
        {
            Fit = "ft";
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="rotate">逆时针旋转度数</param>
        internal void SetRotate(Rotates rotate)
        {
            switch (rotate)
            {
                case Rotates.Deg90:
                    {
                        this.Rotate = "r90";
                        break;
                    }
                case Rotates.Deg180:
                    {
                        this.Rotate = "r180";
                        break;
                    }
                case Rotates.Deg270:
                    {
                        this.Rotate = "r270";
                        break;
                    }
                case Rotates.NoRotate:
                    {
                        this.Rotate = null;
                        break;
                    }
            }
        }

        /// <summary>
        /// 翻转
        /// </summary>
        /// <param name="horizontal">垂直翻转</param>
        /// <param name="vertical">水平翻转</param>
        internal void Flip(bool horizontal, bool vertical)
        {
            if (horizontal)
            {
                this.FlipHorizontal = "fh";
            }

            if (vertical)
            {
                this.FlipVertical = "fv";
            }
        }

        /// <summary>
        /// 设定质量(仅JPEG)
        /// </summary>
        /// <remarks>如果未指定，则默认95</remarks>
        /// <param name="quality">原值比例(1,99)， 如50表示为原图质量50%</param>
        internal void SetQuality(int quality)
        {
            this.Quality = $"q{quality}";
        }

        /// <summary>
        /// 格式转换
        /// </summary>
        /// <param name="format">格式</param>
        internal void Convert(ConvertableFormats format)
        {
            switch (format)
            {
                case ConvertableFormats.Jpeg:
                    {
                        Format = "jpeg";
                        break;
                    }
                case ConvertableFormats.Png:
                    {
                        Format = "png";
                        break;
                    }
                case ConvertableFormats.Tiff:
                    {
                        Format = "tiff";
                        break;
                    }
                case ConvertableFormats.NoConvert:
                    {
                        Format = null;
                        break;
                    }
            }
        }

        /// <summary>
        /// 矩形剪裁
        /// </summary>
        /// <param name="x">坐标x，左上角为坐标原点，默认0</param>
        /// <param name="y">坐标y，左上角为坐标原点，默认0</param>
        /// <param name="width">宽度，默认原图宽度</param>
        /// <param name="height">高度，默认原图高度</param>
        internal void Crop(double? x = null, double? y = null, double? width = null, double? height = null)
        {
            CropX = x.HasValue ? $"cx" + x.Value : null;
            CropY = y.HasValue ? $"cy" + y.Value : null;
            CropWidth = width.HasValue ? $"cw" + width.Value : null;
            CropHeight = height.HasValue ? $"ch" + height.Value : null;
        }

        /// <summary>
        /// 使用智能剪裁
        /// </summary>
        /// <remarks>基于内容感知，如设置将覆盖其他剪裁参数</remarks>
        internal void SetSmartCrop()
        {
            SmartCrop = "sc";
        }

        /// <summary>
        /// 格式化为URL
        /// </summary>
        /// <remarks>如果原始URL包含原图标记/0x0/，则替换之；否则添加为参数imgopt</remarks>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Url))
            {
                return Url;
            }

            var rectangle = Width == null && Height == null ? null : $"{Width}x{Height}";

            var options = new string[]
            {
                rectangle,
                Fit,
                Rotate,
                FlipHorizontal,
                FlipVertical,
                Quality,
                Format,
                CropX,
                CropY,
                CropWidth,
                CropHeight,
                SmartCrop,
            };
            var notNullOptions = options.Where(m => m != null);

            var hasOriginOption = Url.Contains(Extensions.ORIGIN_OPTION);
            var imgOpt = string.Join(",", notNullOptions);

            if (hasOriginOption)
            {
                // 包含原图标记
                Url = Url.Replace(Extensions.ORIGIN_OPTION, $"/{imgOpt}/");
            }
            else
            {
                //无原图标记
                Url = Url.Contains("?") ? $"{Url}&imgopt={imgOpt}" : $"{Url}?imgopt={imgOpt}";
            }

            return Url;
        }

        /// <summary>
        /// 隐式转换为string
        /// </summary>
        /// <param name="options">参数实例</param>
        public static implicit operator string(ImageProxyOptions options)
        {
            return options.ToString();
        }

    }
}
