using Flamer.Service.Domain.Blob.CONST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Service.Domain.Blob.ViewModels
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class OssFileInfo
    {
        /// <summary>
        /// 归类
        /// </summary>
        public Categories Category { get; set; }

        /// <summary>
        /// 原始文件名
        /// </summary>
        public string OriginalFileName { get; set; }
    }
}
