using Flamer.Data.Repositories.IPA;
using Flamer.Data.Repositories.Projects;
using Flamer.Model.Web.Databases.Main.IPA;
using Flamer.Model.ViewModel.Blob;
using Flamer.Model.ViewModel.IPA;
using Flamer.Pagination;
using Flamer.Service.Domain.Blob;
using Flamer.Service.Email;
using Flamer.Service.ImageProxy.OptionBuilder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flamer.Service.Domain.IPA
{
    public class IpaService : IIpaService
    {
        private readonly IIpaBundleRepository ipaBundleRepository;
        private readonly IProjectRepository projectRepository;
        private readonly IOssService ossService;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;

        public IpaService(IIpaBundleRepository ipaBundleRepository,
            IProjectRepository projectRepository,
            IOssService ossService,
            IConfiguration configuration,
            IEmailService emailService)
        {
            this.ipaBundleRepository = ipaBundleRepository;
            this.projectRepository = projectRepository;
            this.ossService = ossService;
            this.configuration = configuration;
            this.emailService = emailService;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="projectCode">项目代码</param>
        /// <param name="identifier">包id</param>
        /// <param name="version">版本号</param>
        /// <param name="fullSizeImage">全尺寸显示图（文件sha1哈希）</param>
        /// <param name="softwarePackage">ipa（文件sha1哈希）</param>
        /// <param name="env">环境</param>
        /// <returns></returns>
        public async Task Add(string sysUserName, string projectCode, string identifier, string version, string fullSizeImage, string softwarePackage, string env)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new BizErrorException("包id为空");
            }

            if (!Version.TryParse(version, out _))
            {
                throw new BizErrorException("版本号无法解析");
            }

            if (string.IsNullOrEmpty(softwarePackage))
            {
                throw new BizErrorException("ipa文件为空");
            }

            var projectId = await projectRepository.GetId(projectCode);
            if (string.IsNullOrEmpty(projectId))
            {
                throw new BizErrorException($"指定项目未找到:{sysUserName}-{projectCode}");
            }

            var bundle = new IpaBundle()
            {
                CreateTime = DateTimeOffset.UtcNow,
                SysUserName = sysUserName,
                FullSizeImage = fullSizeImage,
                Identifier = identifier,
                ProjectId = projectId,
                Id = IdHelper.New(),
                SoftwarePackage = softwarePackage,
                Version = version,
                Env = env,
            };
            await ipaBundleRepository.Add(bundle);

            var noteTask = Task.Run(() =>
            {
                var subEmails = configuration["IpaAddSubscription"];
            });
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="id"></param>
        /// <param name="projectId"></param>
        /// <param name="identifier"></param>
        /// <param name="version"></param>
        /// <param name="fullSizeImage"></param>
        /// <param name="softwarePackage"></param>
        /// <returns></returns>
        public Task Edit(string sysUserName, string id, string projectId, string identifier, string version, string fullSizeImage, string softwarePackage)
        {
            if (!Version.TryParse(version, out _))
            {
                throw new BizErrorException("版本号无法解析");
            }

            var bundle = new IpaBundle()
            {
                ProjectId = projectId,
                FullSizeImage = fullSizeImage,
                Identifier = identifier,
                Id = IdHelper.New(),
                SoftwarePackage = softwarePackage,
                Version = version,
            };
            return ipaBundleRepository.Edit(sysUserName, id, bundle);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task Remove(string sysUserName, IEnumerable<string> ids)
        {
            return ipaBundleRepository.Remove(sysUserName, ids);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="paging">分页</param>
        /// <param name="projectId">所属项目</param>
        /// <param name="id">指定包id</param>
        /// <returns></returns>
        public Task<PagedList<IpaBundleVm>> GetList(string sysUserName, Paging paging,
            string projectId = null, string id = null)
        {
            return ipaBundleRepository.GetList(sysUserName, paging, projectId, id);
        }

        /// <summary>
        /// 获取简单历史记录列表
        /// </summary>
        /// <param name="paging">分页</param>
        /// <param name="projectCode">所属项目代码</param>
        /// <param name="sysUserName">所属用户名</param>
        /// <returns></returns>
        public Task<PagedList<IpaBundleHistoryVm>> GetHistoryList(Paging paging,
            string projectCode, string sysUserName = null)
        {
            return ipaBundleRepository.GetHistoryList(paging, projectCode, sysUserName);
        }

        /// <summary>
        /// 为安装页面获取
        /// </summary>
        /// <param name="projectCode">所属项目代码</param>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="id">指定包id，比较优先</param>
        /// <returns></returns>
        public async Task<IpaInstallVm> GetForInstallPage(string projectCode, string sysUserName = null, string id = null)
        {
            if (string.IsNullOrEmpty(projectCode))
            {
                throw new ArgumentNullException(nameof(projectCode));
            }

            if (string.IsNullOrEmpty(id))
            {
                id = await ipaBundleRepository.GetId(projectCode, sysUserName);
            }

            var bundleVm = await ipaBundleRepository.Get(id);
            if (bundleVm == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(bundleVm.FullSizeImage))
            {
                //如果ipa记录没指定图像，则使用项目Logo
                var projectLogo = await projectRepository.GetLogo(bundleVm.ProjectCode);
                bundleVm.FullSizeImage = projectLogo;
            }

            if (!string.IsNullOrEmpty(bundleVm.FullSizeImage))
            {
                var fullSizeImageInfo = await ossService.GetUrl(bundleVm.FullSizeImage, true, new ImageProxyModel()
                {
                    Width = 128,
                    Height = 128,
                });
                bundleVm.FullSizeImage = fullSizeImageInfo.Url;
            }

            var localipaUrl = await ossService.GetUrl(bundleVm.SoftwarePackage, true, minioChannel: OSS.Extensions.MinioChannels.Inner);

            return new IpaInstallVm()
            {
                Bundle = bundleVm,
                LocalIpaUrl = localipaUrl?.Url
            };
        }

        /// <summary>
        /// 为安装plist文件构建获取
        /// </summary>
        /// <param name="id">指定包id</param>
        /// <param name="isLocal">是否为本地安装查询</param>
        /// <returns></returns>
        public async Task<IpaBundleVm> GetForInstallTemplate(string id, bool isLocal = false)
        {
            var vm = await ipaBundleRepository.Get(id);
            vm.SoftwarePackageHash = vm.SoftwarePackage;

            if (string.IsNullOrEmpty(vm.FullSizeImage))
            {
                //如果ipa记录没指定图像，则使用项目Logo
                var projectLogo = await projectRepository.GetLogo(vm.ProjectCode);
                vm.FullSizeImage = projectLogo;
            }

            if (!string.IsNullOrEmpty(vm.FullSizeImage))
            {
                var fullSizeImageInfo = await ossService.GetUrl(vm.FullSizeImage, true);
                var fullSizeImage = fullSizeImageInfo.Url.AsImageProxyOptions().Width(512).Height(512);
                vm.FullSizeImage = fullSizeImage;
                var displayImage = fullSizeImageInfo.Url.AsImageProxyOptions().Width(64).Height(64);
                vm.DisplayImage = displayImage;
            }

            if (!isLocal)
            {
                var softwarePackageInfo = await ossService.GetUrl(vm.SoftwarePackage, true);
                vm.SoftwarePackage = softwarePackageInfo.Url;
            }

            return vm;
        }

    }

    public interface IIpaService
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="projectCode">项目代码</param>
        /// <param name="identifier">包id</param>
        /// <param name="version">版本号</param>
        /// <param name="fullSizeImage">全尺寸显示图（文件sha1哈希）</param>
        /// <param name="softwarePackage">ipa（文件sha1哈希）</param>
        /// <param name="env">环境</param>
        /// <returns></returns>
        Task Add(string sysUserName, string projectCode, string identifier, string version, string fullSizeImage, string softwarePackage, string env);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="id"></param>
        /// <param name="projectId"></param>
        /// <param name="identifier"></param>
        /// <param name="version"></param>
        /// <param name="fullSizeImage"></param>
        /// <param name="softwarePackage"></param>
        /// <returns></returns>
        Task Edit(string sysUserName, string id, string projectId, string identifier, string version, string fullSizeImage, string softwarePackage);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task Remove(string sysUserName, IEnumerable<string> ids);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="paging">分页</param>
        /// <param name="projectId">所属项目</param>
        /// <param name="id">指定包id</param>
        /// <returns></returns>
        Task<PagedList<IpaBundleVm>> GetList(string sysUserName, Paging paging,
            string projectId = null, string id = null);

        /// <summary>
        /// 获取简单历史记录列表
        /// </summary>
        /// <param name="paging">分页</param>
        /// <param name="projectCode">所属项目代码</param>
        /// <param name="sysUserName">所属用户名</param>
        /// <returns></returns>
        Task<PagedList<IpaBundleHistoryVm>> GetHistoryList(Paging paging,
            string projectCode, string sysUserName = null);

        /// <summary>
        /// 为安装页面获取
        /// </summary>
        /// <param name="projectCode">所属项目代码</param>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="id">指定包id，比较优先</param>
        /// <returns></returns>
        Task<IpaInstallVm> GetForInstallPage(string projectCode, string sysUserName = null, string id = null);

        /// <summary>
        /// 为安装plist文件构建获取
        /// </summary>
        /// <param name="id">指定包id</param>
        /// <param name="isLocal">是否为本地安装查询</param>
        /// <returns></returns>
        Task<IpaBundleVm> GetForInstallTemplate(string id, bool isLocal = false);
    }
}

