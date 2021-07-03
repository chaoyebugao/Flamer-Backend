using Flamer.Portal.LocaCola.Services;
using Flamer.Portal.LocaCola.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Flamer.Portal.LocaCola
{
    class Program
    {
        static void Main(string[] args)
        {
            var build = new HostBuilder()
                .ConfigureHostConfiguration(cfg =>
                {
                    cfg.AddInMemoryCollection()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                })
                .ConfigureServices(service =>
                {
                    service.AddOptions<WebSettings>("Web");

                    service.AddHostedService<MonitorAutoService>();
                });

            build.RunConsoleAsync();

        }
    }
}
