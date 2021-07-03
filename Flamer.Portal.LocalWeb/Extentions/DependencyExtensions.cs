using Flamer.Portal.LocalWeb.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Portal.LocalWeb.Extentions
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            services.AddHostedService<InitHostedService>();

            return services;
        }
    }
}
