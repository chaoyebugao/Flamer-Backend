using Flamer.Service.OSS.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Service.OSS.Extensions
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddOssServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMinioServices(configuration);

            services.AddSingleton<IMinioService, MinioService>();

            return services;
        }

        private static IServiceCollection AddMinioServices(this IServiceCollection services, IConfiguration configuration)
        {
            var settingCollection = new MinioSettingCollection();
            var innerSettings = new MinioSettings()
            {
                Endpint = configuration["MinioInner:Endpoint"],
                AccessKey = configuration["MinioInner:AccessKey"],
                SecretKey = configuration["MinioInner:SecretKey"],
                UseSsl = bool.Parse(configuration["MinioInner:UseSsl"]),
                Address = configuration["MinioInner:Address"],
                BucketPrefix = configuration["MinioInner:BucketPrefix"],
            };
            settingCollection.Inner = innerSettings;

            MinioSettings webSettings = null;

            if(configuration["MinioWeb:Address"] == null)
            {
                webSettings = innerSettings;
            }
            else
            {
                webSettings = new MinioSettings()
                {
                    Endpint = configuration["MinioWeb:Address"].Replace("https://", null).Replace("http://", null),
                    AccessKey = configuration["MinioWeb:AccessKey"],
                    SecretKey = configuration["MinioWeb:SecretKey"],
                    UseSsl = configuration["MinioWeb:Address"].StartsWith("https"),
                    Address = configuration["MinioWeb:Address"],
                    BucketPrefix = configuration["MinioWeb:BucketPrefix"],
                };
            }

            settingCollection.Web = webSettings;

            services.AddSingleton(sp => settingCollection);

            return services;
        }
    }
}
