using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Flamer.Portal.LocaCola
{
    class Program
    {
        static Task Main(string[] args)
        {
            Console.Title = "LocaCola";

            var build = new HostBuilder()
                .ConfigureHostConfiguration(cfg =>
                {
                    cfg.AddInMemoryCollection()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddLocaCola(context);
                });

            return build.RunConsoleAsync();
        }
    }
}
