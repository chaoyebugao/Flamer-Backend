using SQLite;
using System;

namespace Flammer.Model.Backend.Databases.Main.Account
{
    /// <summary>
    /// 鉴权票据
    /// </summary>
    public class Ticket
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

    }
}
