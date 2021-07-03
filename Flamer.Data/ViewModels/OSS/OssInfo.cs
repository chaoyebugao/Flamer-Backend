using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Data.ViewModels.OSS
{
    public class OssInfo
    {
        /// <summary>
        /// 访问URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 后缀名
        /// </summary>
        public string Ext { get; set; }
    }
}
