using Flamer.Portal.MidWare;
using Flammer.Portal.Filters;
using Flammer.Portal.Web.Extentions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Flammer.Portal.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add<HttpGlobalExceptionFilter>();
            });

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.FullName);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Flammer.Portal.Web", Version = "v1" });
            });

            services.Collect(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flammer.Portal.Web v1"));
            }

            app.UsePerformanceLog();

            app.UseRouting();

            app.UseAuthorization();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "/api/{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                routes.MapRoute(
                   name: "default",
                   template: "/api/{controller}/{action}/{id?}",
                   defaults: new { controller = "Home", action = "Index" }
                );
            });

        }
    }
}
