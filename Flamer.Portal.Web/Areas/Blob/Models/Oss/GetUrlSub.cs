using Flamer.Model.ViewModel.Blob;

namespace Flamer.Portal.Web.Areas.Blob.Models.Oss
{
    public class GetUrlSub
    {
        /// <summary>
        /// 文件sha1哈希
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// imageproxy图像处理
        /// </summary>
        public ImageProxyModel ImageProxy { get; set; }

        /// <summary>
        /// 是否要本地的
        /// </summary>
        public bool IsLocal { get; set; }
    }
}
