using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Portal.LocalWeb.Areas.OSS.Models.Home
{
    public class PresignedUrlSub
    {
        /// <summary>
        /// 所属用户名
        /// </summary>
        public string SysUserName { get; set; }

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
    }
}
