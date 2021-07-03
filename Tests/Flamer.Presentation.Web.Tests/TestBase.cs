using DryIoc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Flammer.Presentation.Web.Tests
{
    public class TestBase : Ease.XUnit.DryIoc.XUnitDryIocContainerTestBase
    {
        protected override void RegisterTypes()
        {
            var client = new HttpClient
            {
                BaseAddress = baseAddress
            };

            var container = ResolveType<IContainer>();
            container.RegisterInstance(client);
        }

        protected Uri baseAddress = new Uri("http://127.0.0.1:5000");

        protected async Task<T> PostAsync<T>(string url, object data) where T : class, new()
        {
            try
            {
                var client = ResolveType<HttpClient>();
                client.BaseAddress = baseAddress;

                var content = JsonConvert.SerializeObject(data);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(url, byteContent).ConfigureAwait(false);
                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine($"POST - url:{url}, HttpStatusCode:{response.StatusCode}, result:{result}");
                    throw new Exception($"{response.StatusCode}-{response.ReasonPhrase}");
                }
                Debug.WriteLine($"POST End, url:{url}, result:{result}");
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    string responseContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    throw new System.Exception($"response :{responseContent}", ex);
                }
                throw;
            }
        }

        protected async Task<T> GetAsync<T>(string url, object data) where T : class, new()
        {
            try
            {
                var client = ResolveType<HttpClient>();
                client.BaseAddress = baseAddress;

                var requestUrl = $"{url}?{GetQueryString(data)}";
                Debug.WriteLine($"GetAsync Start, requestUrl:{requestUrl}");
                var response = await client.GetAsync(requestUrl).ConfigureAwait(false);
                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine($"GET - url:{url}, HttpStatusCode:{response.StatusCode}, result:{result}");
                    throw new Exception($"{response.StatusCode}-{response.ReasonPhrase}");
                }
                Debug.WriteLine($"GetAsync End, requestUrl:{requestUrl}, HttpStatusCode:{response.StatusCode}, result:{result}");
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    string responseContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    throw new Exception($"Response :{responseContent}", ex);
                }
                throw;
            }
        }
        private static string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());
            return String.Join("&", properties.ToArray());
        }
    }
}
