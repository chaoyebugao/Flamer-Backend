using Flamer.Portal.LocaCola.Services;
using Flamer.Portal.LocaCola.Services.Background;
using Flamer.Portal.LocaCola.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Portal.LocaCola
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddLocaCola(this IServiceCollection services, HostBuilderContext context)
        {
            services.AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(context.Configuration);

            });

            var webSettings = new WebSettings();
            context.Configuration.GetSection("Web").Bind(webSettings);
            services.AddSingleton(webSettings);

            var ipaSettingCollection = new IpaSettingCollection();
            context.Configuration.GetSection("Ipa").Bind(ipaSettingCollection);
            services.AddSingleton(ipaSettingCollection);

            var cookie = new CookieContainer();
            services.AddSingleton(cookie);

            //命名注入
            services.AddHttpClient<IWebProcessor, WebProcessor>((sp, c) =>
            {
                var settings = sp.GetService<WebSettings>();
                c.Timeout = TimeSpan.FromMinutes(15);
                c.BaseAddress = new Uri(settings.BaseAddress);
                
            })
            .ConfigurePrimaryHttpMessageHandler(sp => new HttpClientHandler()
            {
                CookieContainer = sp.GetService<CookieContainer>(),
                UseCookies = true
            });

            services.AddHostedService<AccountAutoService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IOssService, OssService>();
            services.AddSingleton<IIpaService, IpaService>();

            services.AddHostedService<MonitorAutoService>();


            return services;
        }
    }
}
