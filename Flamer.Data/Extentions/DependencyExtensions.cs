using Flamer.Data.Repositories.Account;
using Flamer.Data.Repositories.Db;
using Flamer.Data.Repositories.IPA;
using Flamer.Data.Repositories.Blob;
using Flamer.Data.Repositories.Projects;
using Flammer.Model.Backend.Databases.Main;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flammer.Data
{
    public static class DependencyExtensions 
    {

        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMainDb(configuration);
            services.AddRepositories();

            return services;
        }

        private static IServiceCollection AddMainDb(this IServiceCollection services, IConfiguration configuration)
        {
            var mainDb = new MainDatabase(configuration);

            services.AddSingleton(mainDb.GetSQLiteConnection());

            mainDb.Init();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmailActivationRepository, EmailActivationRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();

            services.AddScoped<IProjectRepository, ProjectRepository>();

            services.AddScoped<IDbSchemeRepository, DbSchemeRepository>();
            services.AddScoped<IDbUserRepository, DbUserRepository>();
            services.AddScoped<IDbSchemeUserRelativeRepository, DbSchemeUserRelativeRepository>();

            services.AddScoped<IOssFileRepository, OssFileRepository>();
            services.AddScoped<IIpaBundleRepository, IpaBundleRepository>();

            return services;
        }
    }
}
