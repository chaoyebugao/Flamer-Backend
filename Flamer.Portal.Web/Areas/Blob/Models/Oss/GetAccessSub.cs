using Flamer.Model.ViewModel.Blob;

namespace Flamer.Portal.Web.Areas.Blob.Models.Oss
{
    public class GetAccessSub
    {
        /// <summary>
        /// 文件sha1哈希
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// 归类，参考<see cref="Flamer.Service.Domain.Blob.CONST.Categories"/>
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 原始文件名
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// imageproxy图像处理
        /// </summary>
        public ImageProxyModel ImageProxy { get; set; }

    }
}
