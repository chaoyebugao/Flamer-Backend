using SQLite;
using System;

namespace Flamer.Model.Web.Databases.Main.Account
{
    /// <summary>
    /// 激活邮件
    /// </summary>
    public class EmailActivation
    {
        /// <summary>
        /// 令牌，主键
        /// </summary>
        [PrimaryKey]
        public string Token { get; set; }

        /// <summary>
        /// 所属用户名
        /// </summary>
        public string SysUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 有效期至
        /// </summary>
        public DateTimeOffset ExpireAt { get; set; }

        /// <summary>
        /// 是否已激活过
        /// </summary>
        public bool Activated { get; set; }

    }
}
