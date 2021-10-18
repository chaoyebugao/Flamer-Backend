using Flamer.Model.Result.WebResults;
using Flamer.Portal.LocaCola.Extensions;
using Flamer.Portal.Web.Areas.Account.Models.User;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Portal.LocaCola.Services
{
    public class WebProcessor : IWebProcessor
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly HttpClient httpClient;
        private readonly CookieContainer cookieContainer;
        private readonly JsonSerializer serializer = new();

        internal static LoginRet LoginRet;
        internal static string SysUserName;

        public WebProcessor(HttpClient httpClient,
            CookieContainer cookieContainer)
        {
            this.httpClient = httpClient;
            this.cookieContainer = cookieContainer;
        }

        #region HttpMethods
        /// <summary>
        /// 获取文本
        /// </summary>
        /// <param name="uri">URL</param>
        /// <param name="requestParams">请求参数实例</param>
        /// <returns></returns>
        public async Task<string> GetPlainTextAsync(string uri, IDictionary<string, string> requestParams = null)
        {
            uri = requestParams?.Count > 0 ? uri + "?" + string.Join("&", requestParams.Select(m => $"{m.Key}={m.Value}")) : uri;

            httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
            cookieContainer.AttachTicket(httpClient, LoginRet, uri);
            var response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var ret = await reader.ReadToEndAsync();
            return ret;
        }
        /// <summary>
        /// 获取json并将其反序列化
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="uri">URL</param>
        /// <param name="requestParams">请求参数实例</param>
        /// <returns></returns>
        public async Task<TResult> GetJsonAsync<TResult>(string uri, IDictionary<string, string> requestParams = null)
        {
            uri = requestParams?.Count > 0 ? uri + "?" + string.Join("&", requestParams.Select(m => $"{m.Key}={m.Value}")) : uri;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
            cookieContainer.AttachTicket(httpClient, LoginRet, uri);
            var response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var json = new JsonTextReader(reader);
            return serializer.Deserialize<TResult>(json);
        }

        /// <summary>
        /// 提交数据并将结果反序列化
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="uri">URL</param>
        /// <param name="requestParams">请求参数实例</param>
        /// <returns></returns>
        public async Task<TResult> PostJsonAsync<TResult>(string uri, object requestParams)
        {
            var jsonReq = JsonConvert.SerializeObject(requestParams);
            var content = new StringContent(jsonReq);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            cookieContainer.AttachTicket(httpClient, LoginRet, uri);

            var response = await httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            using var json = new JsonTextReader(reader);
            return serializer.Deserialize<TResult>(json);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="uri">URL</param>
        /// <param name="requestParams">请求参数实例</param>
        /// <returns></returns>
        public async Task UploadAsync(string uri, FileInfo fileInfo)
        {
            //using var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            //using var fs = fileInfo.OpenRead();
            //using var stContent = new StreamContent(fs);
            //content.Add(stContent, fileInfo.Name);

            //using var message = await httpClient.PostAsync(uri, content);
            //var input = await message.Content.ReadAsStringAsync();

            //if (!string.IsNullOrEmpty(input))
            //{
            //    logger.Info(input);
            //}

        }

        #endregion

        private void CheckRet(BaseRet ret)
        {
            if (ret.Code != RetCodes.Success)
            {
                var msgRet = ret as MsgRet;
                if (string.IsNullOrEmpty(msgRet?.Msg))
                {
                    throw new Exception(ret.Code.ToString());
                }
                throw new Exception(msgRet.Msg);
            }
        }

        /// <summary>
        /// 请求附加数据结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="uri"></param>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        public async Task<TResult> Get4Data<TResult>(string uri, IDictionary<string, string> requestParams = null)
        {
            var ret = await GetJsonAsync<DataRet<TResult>>(uri, requestParams);
            CheckRet(ret);
            return ret.Data;
        }

        /// <summary>
        /// 请求附加数据结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task Post(string uri, object body)
        {
            var ret = await PostJsonAsync<MsgRet>(uri, body);
            CheckRet(ret);
            return;
        }

        /// <summary>
        /// 请求附加数据结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<TResult> Post4Data<TResult>(string uri, object body)
        {
            var ret = await PostJsonAsync<DataRet<TResult>>(uri, body);
            CheckRet(ret);
            return ret.Data;
        }

        /// <summary>
        /// 请求分行业数据结果
        /// </summary>
        /// <typeparam name="TResultItem"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<PagedRetNeat<TResultItem>> Post4Paged<TResultItem>(string uri, object body)
        {
            var ret = await PostJsonAsync<PagedRet<TResultItem>>(uri, body);
            CheckRet(ret);
            return new PagedRetNeat<TResultItem>()
            {
                 Total = ret.Total,
                 Items = ret.Items,
            };
        }

    }

    public interface IWebProcessor
    {
        #region HttpMethods
        /// <summary>
        /// 获取文本
        /// </summary>
        /// <param name="uri">URL</param>
        /// <param name="requestParams">请求参数实例</param>
        /// <returns></returns>
        Task<string> GetPlainTextAsync(string uri, IDictionary<string, string> requestParams = null);

        /// <summary>
        /// GET
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="uri">URL</param>
        /// <param name="requestParams">请求参数实例</param>
        /// <returns></returns>
        Task<TResult> GetJsonAsync<TResult>(string uri, IDictionary<string, string> requestParams = null);

        /// <summary>
        /// POST
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="uri">URL</param>
        /// <param name="requestParams">请求参数实例</param>
        /// <returns></returns>
        Task<TResult> PostJsonAsync<TResult>(string uri, object requestParams);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="uri">URL</param>
        /// <param name="requestParams">请求参数实例</param>
        /// <returns></returns>
        Task UploadAsync(string uri, FileInfo fileInfo);
        #endregion

        /// <summary>
        /// 请求附加数据结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="uri"></param>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        Task<TResult> Get4Data<TResult>(string uri, IDictionary<string, string> requestParams = null);

        /// <summary>
        /// 请求附加数据结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task Post(string uri, object body);

        /// <summary>
        /// 请求附加数据结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<TResult> Post4Data<TResult>(string uri, object body);

        /// <summary>
        /// 请求分行业数据结果
        /// </summary>
        /// <typeparam name="TResultItem"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<PagedRetNeat<TResultItem>> Post4Paged<TResultItem>(string uri, object body);

    }
}
