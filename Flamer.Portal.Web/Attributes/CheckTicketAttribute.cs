using Flammer.Service.Domain.User;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Flammer.Portal.Web.Attributes
{
    public class CheckTicketAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = context.HttpContext.Request.Cookies["tid"];
            var userService = context.HttpContext.RequestServices.GetService<IUserService>();
            userService.VerifyTokenAsync(token);

            return base.OnActionExecutionAsync(context, next);
        }

    }
}
