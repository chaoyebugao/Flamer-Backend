using Flamer.Portal.LocalWeb.Extentions;
using Flamer.Service.ImageProxy.Extensions;
using Flamer.Service.OSS.Extensions;
using Flammer.Data;
using Flammer.Service.Domain;
using Flammer.Service.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flammer.Portal.LocalWeb.Extentions
{
    public static class ServiceCollector
    {
        public static void Collect(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddData(configuration);
            services.AddServiceEmail(configuration);
            services.AddOssServices(configuration);
            services.AddImageProxyServices(configuration);
            services.AddServiceDomain(configuration);
            services.AddWebServices();
        }
    }
}
