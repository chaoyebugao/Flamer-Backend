using SQLite;
using System;

namespace Flamer.Model.Web.Databases.Main.Projects
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
        /// 创建人用户名
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 项目代码
        /// </summary>
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
