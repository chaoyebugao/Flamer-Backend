using SQLite;
using System;

namespace Flamer.Model.Web.Databases.Main.Account
{
    /// <summary>
    /// 用户
    /// </summary>
    public class SysUser
    {
        /// <summary>
        /// 用户名，主键
        /// </summary>
        [PrimaryKey]
        public string Name { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 电子邮件地址
        /// </summary>
        [NotNull]
        public string Email { get; set; }

        /// <summary>
        /// 是否已激活
        /// </summary>
        public bool Activated { get; set; }

        /// <summary>
        /// 密码哈希
        /// </summary>
        [NotNull]
        public string PasswordHash { get; set; }

    }
}
