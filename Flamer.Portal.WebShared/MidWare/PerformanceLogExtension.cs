﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Flamer.Portal.MidWare
{
    public static class PerformanceLogExtension
    {
        public static IApplicationBuilder UsePerformanceLog(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Use(async (context, next) =>
            {
                var profiler = new Stopwatch();
                profiler.Start();
                await next();
                profiler.Stop();

                var logger = context.RequestServices.GetService<ILoggerFactory>()
                    .CreateLogger("PerformanceLog");
                logger.LogInformation("TraceId:{TraceId}, RequestMethod:{RequestMethod}, RequestPath:{RequestPath}, ElapsedMilliseconds:{ElapsedMilliseconds}, Response StatusCode: {StatusCode}",
                                        context.TraceIdentifier, context.Request.Method, context.Request.Path, profiler.ElapsedMilliseconds, context.Response.StatusCode);
            });
            return applicationBuilder;
        }
    }
}
