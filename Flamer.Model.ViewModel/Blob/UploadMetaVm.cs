using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Model.ViewModel.Blob
{
    public class UploadMetaVm
    {
        /// <summary>
        /// 文件已存在时的访问URL
        /// </summary>
        public OssInfo OssInfo { get; set; }

        /// <summary>
        /// 存储桶名
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// 对象名
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Minio上传集合（已加密）
        /// </summary>
        public byte[] MinIOSettings { get; set; }
    }
}
