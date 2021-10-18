using Flamer.Service.Domain.ImageProxy;
using Flamer.Service.Domain.IPA;
using Flamer.Service.Domain.Blob;
using Flamer.Service.Domain.Projects;
using Flamer.Service.Domain.Db;
using Flamer.Service.Domain.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yitter.IdGenerator;

namespace Flamer.Service.Domain
{
    public static class DependencyExtensions
    {

        public static IServiceCollection AddServiceDomain(this IServiceCollection services, IConfiguration configuration)
        {
            InitIdGenerator();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IProjectService, ProjectService>();

            services.AddScoped<IDatabaseService, DatabaseService>();

            services.AddScoped<IOssService, OssService>();
            services.AddScoped<IImageProxyService, ImageProxyService>();

            services.AddScoped<IIpaService, IpaService>();

            return services;
        }

        private static void InitIdGenerator()
        {
            // 创建 IdGeneratorOptions 对象，请在构造函数中输入 WorkerId：
            var options = new IdGeneratorOptions(1);
            // options.WorkerIdBitLength = 10; // WorkerIdBitLength 默认值6，支持的 WorkerId 最大值为2^6-1，若 WorkerId 超过64，可设置更大的 WorkerIdBitLength
            // ...... 其它参数设置参考 IdGeneratorOptions 定义，一般来说，只要再设置 WorkerIdBitLength （决定 WorkerId 的最大值）。

            // 保存参数（必须的操作，否则以上设置都不能生效）：
            YitIdHelper.SetIdGenerator(options);
            // 以上初始化过程只需全局一次，且必须在第2步之前设置。
        }
    }
}
