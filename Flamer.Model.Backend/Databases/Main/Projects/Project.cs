using SQLite;
using System;

namespace Flammer.Model.Backend.Databases.Main.Projects
{
    /// <summary>
    /// 项目
    /// </summary>
    public class Project
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// 所属用户名
        /// </summary>
        [Indexed(Name = "ProjectId", Order = 1, Unique = true)]
        public string SysUserName { get; set; }

        /// <summary>
        /// 项目代码
        /// </summary>
        [Indexed(Name = "ProjectId", Order = 2, Unique = true)]
        public string Code { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 项目名
        /// </summary>
        [NotNull]
        public string Name { get; set; }

        /// <summary>
        /// Logo
        /// </summary>
        public string Logo { get; set; }



    }
}
