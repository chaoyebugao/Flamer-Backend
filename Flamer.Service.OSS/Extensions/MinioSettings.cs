using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Service.OSS.Extensions
{
    /// <summary>
    /// minio服务配置
    /// </summary>
    public class MinioSettings
    {
        /// <summary>
        /// 端点
        /// </summary>
        public string Endpint { get; set; }

        /// <summary>
        /// 鉴权Key
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// 鉴权秘钥
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 使用SSL
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        /// 外部访问地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 存储桶前缀
        /// </summary>
        public string BucketPrefix { get; set; }

    }

}
