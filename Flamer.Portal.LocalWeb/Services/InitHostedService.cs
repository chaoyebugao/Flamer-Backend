using Flamer.Service.Domain.Blob.CONST;
using Flamer.Service.OSS.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flamer.Portal.LocalWeb.Services
{
    public class InitHostedService : IHostedService
    {
        private readonly IMinioService minioService;

        public InitHostedService(IMinioService minioService)
        {
            this.minioService = minioService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Action<Task> createContinue = t =>
            {
                Console.WriteLine(t.Exception);
                //TODO:日志
            };

            Task.Run(async () =>
            {
                await minioService.CreateBucket(Buckets.SysUser, false);

                await minioService.CreateBucket(Buckets.Public, true);
            }).ContinueWith(createContinue, TaskContinuationOptions.OnlyOnFaulted);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
