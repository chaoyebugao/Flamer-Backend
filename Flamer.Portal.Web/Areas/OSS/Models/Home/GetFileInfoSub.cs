using Flamer.Service.Domain.Blob.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Portal.Web.Areas.OSS.Models.Home
{
    public class GetFileInfoSub
    {
        /// <summary>
        /// 文件sha1哈希
        /// </summary>
        public string Hash { get; set; }

    }
}
