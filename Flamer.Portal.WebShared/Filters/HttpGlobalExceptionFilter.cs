using Flamer.Model.Result.WebResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace Flammer.Portal.Filters
{
    /// <summary>
    /// 全局异常捕捉
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var msg = filterContext.Exception.Message;

            Console.WriteLine(filterContext.Exception);


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
