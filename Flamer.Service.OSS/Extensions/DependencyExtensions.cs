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
                Endpint = configuration["Minio:Endpoint"],
                AccessKey = configuration["Minio:AccessKey"],
                SecretKey = configuration["Minio:SecretKey"],
                UseSsl = bool.Parse(configuration["Minio:UseSsl"]),
                Address = configuration["Minio:Address"],
                BucketPrefix = configuration["Minio:BucketPrefix"],
            };
            settingCollection.Inner = innerSettings;

            var webSettings = new MinioSettings()
            {
                Endpint = configuration["Minio:Address"].Replace("https://", null).Replace("http://", null),
                AccessKey = configuration["Minio:AccessKey"],
                SecretKey = configuration["Minio:SecretKey"],
                UseSsl = configuration["Minio:Address"].StartsWith("https"),
                Address = configuration["Minio:Address"],
                BucketPrefix = configuration["Minio:BucketPrefix"],
            };
            settingCollection.Web = webSettings;

            services.AddSingleton(sp => settingCollection);

            return services;
        }
    }
}
