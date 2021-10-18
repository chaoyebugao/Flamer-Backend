using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Model.Web.Databases.Main.Blob
{
    /// <summary>
    /// Oss文件记录
    /// </summary>
    public class OssFile
    {
        /// <summary>
        /// 文件哈希
        /// </summary>
        [PrimaryKey]
        public string Hash { get; set; }

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
        /// 分类
        /// </summary>
        [NotNull]
        public string Category { get; set; }

        /// <summary>
        /// 上传文件名
        /// </summary>
        [NotNull]
        public string OriginalFileName { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string Ext { get; set; }

        /// <summary>
        /// 内容类型
        [NotNull]
        public string ContentType { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Minio存储桶
        /// </summary>
        [NotNull]
        public string BucketName { get; set; }

        /// <summary>
        /// Minio对象名
        /// </summary>
        [NotNull]
        public string ObjectName { get; set; }

    }
}
