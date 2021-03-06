using Flamer.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Portal.Web.Areas.IPA.Models.Home
{
    public class AddSub
    {
        /// <summary>
        /// 所属项目代码
        /// </summary>
        public string ProjectCode { get; set; }

        /// <summary>
        /// 包名
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 全尺寸
        /// </summary>
        public string FullSizeImage { get; set; }

        /// <summary>
        /// ipa文件
        /// </summary>
        public string SoftwarePackage { get; set; }

        /// <summary>
        /// 环境
        /// </summary>
        public string Env { get; set; }
    }
}
