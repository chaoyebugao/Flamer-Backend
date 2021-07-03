using Flamer.Service.Domain.Blob.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Portal.Web.Areas.OSS.Models.Home
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
    }
}
