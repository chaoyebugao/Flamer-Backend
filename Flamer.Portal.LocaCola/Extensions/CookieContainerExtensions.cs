using Flamer.Portal.Web.Areas.Account.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Portal.LocaCola.Extensions
{
    public static class CookieContainerExtensions
    {
        public static CookieContainer AttachTicket(this CookieContainer cookieContainer, HttpClient httpClient, LoginRet loginRet, string requestUrl)
        {
            if (loginRet == null)
            {
                return cookieContainer;
            }

            if (requestUrl.StartsWith("http://") || requestUrl.StartsWith("https://"))
            {
                if (new Uri(requestUrl).Host != httpClient.BaseAddress.Host)
                {
                    return cookieContainer;
                }
            }

            cookieContainer.Add(httpClient.BaseAddress, new Cookie("tid", loginRet.Token));
            return cookieContainer;
        }
    }
}
