using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Model.ViewModel.IPA
{
    public class IpaBundleVm
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属项目id
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 所属项目代码
        /// </summary>
        public string ProjectCode { get; set; }

        /// <summary>
        /// 所属用户名（上传人）
        /// </summary>
        public string SysUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

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
        /// 缩略图
        /// </summary>
        public string DisplayImage { get; set; }

        /// <summary>
        /// ipa文件Hash
        /// </summary>
        public string SoftwarePackageHash { get; set; }

        /// <summary>
        /// ipa文件
        /// </summary>
        public string SoftwarePackage { get; set; }

        /// <summary>
        /// 工程名
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 环境
        /// </summary>
        public string Env { get; set; }
    }
}
