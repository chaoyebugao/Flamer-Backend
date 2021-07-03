using SQLite;
using System;

namespace Flammer.Model.Backend.Databases.Main.IPA
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
        public string ProjectId { get; set; }

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

        ///// <summary>
        ///// 缩略图
        ///// </summary>
        //public string DisplayImage { get; set; }

        /// <summary>
        /// ipa文件
        /// </summary>
        public string SoftwarePackage { get; set; }
    }
}
