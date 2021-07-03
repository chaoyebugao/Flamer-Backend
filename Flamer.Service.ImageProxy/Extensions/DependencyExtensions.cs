using Flamer.Service.ImageProxy.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flamer.Service.ImageProxy.Extensions
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddImageProxyServices(this IServiceCollection services, IConfiguration configuration)
        {
            var address = configuration["ImageProxy:Address"];
            var defaultOptions = configuration["ImageProxy:DefaultOptions"];

            var settings = new ImageProxySettings()
            {
                Address = address,
                DefaultOptions = defaultOptions,
            };

            services.AddSingleton(sp => settings);

            return services;
        }

    }
}
