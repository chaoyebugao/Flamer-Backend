using Flamer.Service.Domain.Blob.CONST;
using Flamer.Service.OSS.Extensions;
using Flamer.Service.OSS.Services;
using Microsoft.Extensions.Hosting;
using Minio;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flamer.Portal.Web.Services
{
    public class InitHostedService : IHostedService
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IMinioService minioService;

        public InitHostedService(IMinioService minioService)
        {
            this.minioService = minioService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Action<Task> createContinue = t =>
            {
                logger.Error(t.Exception);
                //TODO:日志
            };

            Task.Run(async () =>
            {
                await minioService.CreateBucket(Buckets.SysUser, false, MinioChannels.Web);

                await minioService.CreateBucket(Buckets.Public, true, MinioChannels.Web);
            }).ContinueWith(createContinue, TaskContinuationOptions.OnlyOnFaulted);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
