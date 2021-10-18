using Flamer.Utility.Wroker;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return Retry.DoAscending(userService.Login, int.MaxValue, onSuccess: () =>
            {
                logger.Info("登录完毕");
            }, onTryFailed: (ex, delaySeconds) =>
            {
                logger.Warn($"登录不成功，{delaySeconds}秒后重试:{Environment.NewLine}{ex}");
            });

        }

    }
}
