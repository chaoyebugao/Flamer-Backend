using Flamer.Portal.LocalWeb.Extentions;
using Flamer.Service.ImageProxy.Extensions;
using Flamer.Service.OSS.Extensions;
using Flamer.Data;
using Flamer.Service.Domain;
using Flamer.Service.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flamer.Portal.LocalWeb.Extentions
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
