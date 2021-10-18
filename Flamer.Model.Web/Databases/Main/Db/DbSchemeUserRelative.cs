using SQLite;

namespace Flamer.Model.Web.Databases.Main.Db
{
    /// <summary>
    /// 数据库实例和用户关联
    /// </summary>
    public class DbSchemeUserRelative
    {
        /// <summary>
        /// 实例Id
        /// </summary>
        [Indexed(Name = "DbSchemeUserRelativeId", Order = 1, Unique = true)]
        public string DbSchemeId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [Indexed(Name = "DbSchemeUserRelativeId", Order = 2, Unique = true)]
        public string DbUserId { get; set; }
    }
}
