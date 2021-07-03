using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Service.Domain.Blob.Models
{
    /// <summary>
    /// imageproxy图像处理模型
    /// </summary>
    public class ImageProxyModel
    {
        /// <summary>
        /// 宽度。(0, 1)单位为原值比例，>=1单位为像素
        /// </summary>
        public double? Width { get; set; }

        /// <summary>
        /// 高度。(0, 1)单位为原值比例，>=1单位为像素
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// 是否自动匹配，不剪裁
        /// </summary>
        public bool Fit { get; set; }

        /// <summary>
        /// 旋转度数，90/180/270
        /// </summary>
        public int? Rotate { get; set; }

        /// <summary>
        /// 水平翻转
        /// </summary>
        public bool FlipHorizontal { get; set; }

        /// <summary>
        /// 垂直翻转
        /// </summary>
        public bool FlipVertical { get; set; }

        /// <summary>
        /// 质量(仅JPEG，(1,99))
        /// </summary>
        public int? Quality { get; set; }

        /// <summary>
        /// 格式
        /// </summary>
        /// <remarks>指定转换为 "jpeg", "png", 或"tiff"格式图像</remarks>
        public string Format { get; set; }

        /// <summary>
        /// 矩形裁剪 - 坐标X
        /// </summary>
        public double? CropX { get; set; }

        /// <summary>
        /// 矩形裁剪 - 坐标y
        /// </summary>
        public double? CropY { get; set; }

        /// <summary>
        /// 矩形裁剪 - 宽度
        /// </summary>
        public double? CropWidth { get; set; }

        /// <summary>
        /// 矩形裁剪 - 高度
        /// </summary>
        public double? CropHeight { get; set; }

        /// <summary>
        /// 内容识别智能剪裁
        /// </summary>
        /// <remarks>此项将覆盖其他所有剪裁相关参数</remarks>
        public bool SmartCrop { get; set; }
    }
}
