using Flamer.Service.OSS.Extensions;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Service.OSS.Services
{
    public class MinioService : IMinioService
    {
        private readonly MinioSettingCollection settingCollection;

        public MinioService(MinioSettingCollection settingCollection)
        {
            this.settingCollection = settingCollection;
        }

        /// <summary>
        /// 获取节点配置
        /// </summary>
        /// <param name="channel">MinIO通道</param>
        /// <returns></returns>
        public MinioSettings GetSettings(MinioChannels channel = MinioChannels.Inner)
        {
            MinioSettings settings;
            switch (channel)
            {
                case MinioChannels.Web:
                    {
                        settings = settingCollection.Web;
                        break;
                    }
                default:
                    {
                        settings = settingCollection.Inner;
                        break;
                    }
            }

            return settings;
        }

        /// <summary>
        /// 获取节点客户端
        /// </summary>
        /// <param name="channel">MinIO通道</param>
        /// <returns></returns>
        public MinioClient GetClient(MinioChannels channel = MinioChannels.Inner)
        {
            var settings = GetSettings(channel);
            if (settings == null)
            {
                return null;
            }

            var minio = new MinioClient(settings.Endpint, settings.AccessKey, settings.SecretKey);
            return settings.UseSsl ? minio.WithSSL() : minio;
        }

        /// <summary>
        /// 构建存储桶
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="channel">MinIO通道</param>
        /// <returns></returns>
        public string BuildBucketName(string bucket, MinioChannels channel = MinioChannels.Inner)
        {
            var setting = GetSettings(channel);

            return string.IsNullOrEmpty(setting.BucketPrefix) ? bucket : $"{setting.BucketPrefix}-{bucket}";
        }

        /// <summary>
        /// 创建存储桶
        /// </summary>
        /// <param name="bucket">存储桶名</param>
        /// <param name="isPublic">是否是公开的</param>
        /// <param name="channel">MinIO通道</param>
        /// <returns></returns>
        public async Task CreateBucket(string bucket, bool isPublic, MinioChannels channel = MinioChannels.Inner)
        {
            var minio = GetClient(channel);
            if (minio == null)
            {
                return;
            }

            var bucketName = BuildBucketName(bucket, channel);
            var exists = await minio.BucketExistsAsync(bucketName);
            if (!exists)
            {
                await minio.MakeBucketAsync(bucketName);
            }

            if (isPublic)
            {
                await minio.SetPolicyAsync(bucketName, $"{{\"Version\":\"2012-10-17\",\"Statement\":[{{\"Action\":[\"s3:GetObject\"],\"Effect\":\"Allow\",\"Principal\":{{\"AWS\":[\"*\"]}},\"Resource\":[\"arn:aws:s3:::{bucketName}/*\"],\"Sid\":\"\"}}]}}");
            }

        }

    }

    public interface IMinioService
    {
        /// <summary>
        /// 获取节点配置
        /// </summary>
        /// <param name="channel">MinIO通道</param>
        /// <returns></returns>
        MinioSettings GetSettings(MinioChannels channel = MinioChannels.Inner);

        /// <summary>
        /// 获取节点客户端
        /// </summary>
        /// <param name="channel">MinIO通道</param>
        /// <returns></returns>
        MinioClient GetClient(MinioChannels channel = MinioChannels.Inner);

        /// <summary>
        /// 构建存储桶
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="channel">MinIO通道</param>
        /// <returns></returns>
        string BuildBucketName(string bucket, MinioChannels channel = MinioChannels.Inner);

        /// <summary>
        /// 创建存储桶
        /// </summary>
        /// <param name="bucket">存储桶名</param>
        /// <param name="isPublic">是否是公开的</param>
        /// <param name="channel">节点</param>
        /// <returns></returns>
        Task CreateBucket(string bucket, bool isPublic, MinioChannels channel = MinioChannels.Inner);
    }
}
