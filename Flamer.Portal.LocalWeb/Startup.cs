using Flamer.Portal.MidWare;
using Flamer.Portal.LocalWeb.Extentions;
using Flamer.Portal.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Portal.LocalWeb
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

            services.AddCors(option => option.AddPolicy("cors", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.FullName);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Flammer.Portal.LocalWeb", Version = "v1" });
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
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UsePerformanceLog();

            app.UseRouting();

            app.UseCors("cors");

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
