using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flamer.Portal.LocaCola.Services
{
    public class MonitorAutoService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("执行被请求停止");
                return Task.CompletedTask;
            }

            return Monitor();
        }

        private Task Monitor()
        {
            while (true)
            {

            }
        }

    }
}
