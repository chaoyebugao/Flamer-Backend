using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;

namespace Flamer.Portal.LocalWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "本地服务";

            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging(logging =>
                {
                    //先清空所有的日志记录提供程序
                    logging.ClearProviders();
                    //使用微软提供的控制台输出
                    logging.AddConsole();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                }).UseNLog();
    }
}
