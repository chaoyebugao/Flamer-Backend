using Flamer.Model.ViewModel.Blob;
using Flamer.Portal.LocalWeb.Areas.Blob.Models.Oss;
using Flamer.Portal.Web.Areas.Blob.Models.Oss;
using Flamer.Portal.Web.Areas.IPA.Models.Home;
using System.IO;
using System.Threading.Tasks;

namespace Flamer.Portal.LocaCola.Services
{
    public class IpaService : IIpaService
    {
        private readonly IWebProcessor webProcessor;

        public IpaService(IWebProcessor webProcessor)
        {
            this.webProcessor = webProcessor;
        }

        /// <summary>
        /// 保存记录
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        public Task Add(AddSub sub)
        {
            return webProcessor.Post("/api/ipa/add", sub);
        }

    }

    public interface IIpaService
    {
        /// <summary>
        /// 保存记录
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        Task Add(AddSub sub);
    }
}
