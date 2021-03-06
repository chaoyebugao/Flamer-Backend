using SQLite;
using System;

namespace Flamer.Model.Web.Databases.Main.Db
{
    /// <summary>
    /// 数据库实例
    /// </summary>
    public class DbScheme
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// 所属项目id
        /// </summary>
        [NotNull]
        public string ProjectId { get; set; }

        /// <summary>
        /// 所属用户名
        /// </summary>
        [NotNull]
        public string SysUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 实例名
        /// </summary>
        [NotNull]
        public string Name { get; set; }

        /// <summary>
        /// 数据卷映射路径
        /// </summary>
        [NotNull]
        public string VolPath { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [NotNull]
        public string Host { get; set; }

        

    }
}
