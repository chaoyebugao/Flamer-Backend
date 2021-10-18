using SQLite;
using System;

namespace Flamer.Model.Web.Databases.Main.IPA
{
    /// <summary>
    /// ipa包
    /// </summary>
    public class IpaBundle
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// 所属项目Id
        /// </summary>
        [NotNull]
        public string ProjectId { get; set; }

        /// <summary>
        /// 所属用户名（上传人）
        /// </summary>
        [NotNull]
        public string SysUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 包名
        /// </summary>
        [NotNull]
        public string Identifier { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [NotNull]
        public string Version { get; set; }

        /// <summary>
        /// 全尺寸
        /// </summary>
        public string FullSizeImage { get; set; }

        /// <summary>
        /// ipa文件
        /// </summary>
        [NotNull]
        public string SoftwarePackage { get; set; }

        /// <summary>
        /// 环境
        /// </summary>
        public string Env { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
