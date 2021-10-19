using Microsoft.Extensions.Hosting;
using NLog;
using Polly;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flamer.Portal.LocaCola.Services.Background
{
    /// <summary>
    /// 账户自动登录
    /// </summary>
    public class AccountAutoService : BackgroundService
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IUserService userService;

        public AccountAutoService(IUserService userService)
        {
            this.userService = userService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                logger.Info("执行被请求停止");
                return Task.CompletedTask;
            }

            return LoginWork();
        }

        private Task LoginWork()
        {
            //进行登录，重试机制加持
            var plc = Policy.Handle<Exception>().WaitAndRetryForeverAsync(interval => TimeSpan.FromSeconds(5), (ex, ts) =>
            {
                logger.Warn($"登录失败，{ts.TotalSeconds}秒后重试:{Environment.NewLine}{ex}");
            });

            return plc.ExecuteAsync(userService.Login);
        }

    }
}
