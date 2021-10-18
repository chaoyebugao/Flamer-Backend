using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flamer.Service.Email
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddServiceEmail(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
