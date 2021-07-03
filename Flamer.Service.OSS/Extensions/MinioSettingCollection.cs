using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Service.OSS.Extensions
{
    /// <summary>
    /// minio服务配置集合
    /// </summary>
    public class MinioSettingCollection
    {
        /// <summary>
        /// 内网
        /// </summary>
        public MinioSettings Inner { get; internal set; }

        /// <summary>
        /// 外网（当需要获取预签名URL时）
        /// </summary>
        public MinioSettings Web { get; internal set; }

    }

    /// <summary>
    /// MinIO通道
    /// </summary>
    public enum MinioChannels
    {
        /// <summary>
        /// 内网
        /// </summary>
        Inner = 0,

        /// <summary>
        /// 外网
        /// </summary>
        Web = 1,
    }

}
