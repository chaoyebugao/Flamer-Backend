using SQLite;
using System;

namespace Flamer.Model.Web.Databases.Main.Db
{
    /// <summary>
    /// 数据库用户
    /// </summary>
    public class DbUser
    {
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [NotNull]
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [NotNull]
        public string Password { get; set; }

    }
}
