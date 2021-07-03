using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flammer.Service.Email
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
