using Flamer.Model.Result.WebResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using System;
using System.Net;

namespace Flamer.Portal.Filters
{
    /// <summary>
    /// 全局异常捕捉
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public void OnException(ExceptionContext filterContext)
        {
            var msg = filterContext.Exception.Message;

            logger.Error(filterContext.Exception);

            filterContext.Result = new JsonResult(new MsgRet()
            {
                Code = RetCodes.InternalError,
                Msg = msg,
            });
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            filterContext.ExceptionHandled = true;
        }        
    }
}
