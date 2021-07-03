using Flamer.Data.Repositories.IPA;
using Flamer.Data.ViewModels.IPA;
using Flamer.Service.Domain.Blob;
using Flamer.Service.Domain.Blob.Models;
using Flamer.Service.ImageProxy.OptionBuilder;
using Flammer.Model.Backend.Databases.Main.IPA;
using Flammer.Pagination;
using Flammer.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flamer.Service.Domain.IPA
{
    public class IpaService : IIpaService
    {
        private readonly IIpaBundleRepository ipaBundleRepository;
        private readonly IOssService ossService;

        public IpaService(IIpaBundleRepository ipaBundleRepository,
            IOssService ossService)
        {
            this.ipaBundleRepository = ipaBundleRepository;
            this.ossService = ossService;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="projectId">项目id</param>
        /// <param name="identifier">包id</param>
        /// <param name="version">版本号</param>
        /// <param name="fullSizeImage">全尺寸显示图（文件sha1哈希）</param>
        /// <param name="softwarePackage">ipa（文件sha1哈希）</param>
        /// <returns></returns>
        public Task Add(string projectId, string identifier, string version, string fullSizeImage, string softwarePackage)
        {
            if (!Version.TryParse(version, out _))
            {
                throw new BizErrorException("版本号无法解析");
            }

            var bundle = new IpaBundle()
            {
                CreateTime = DateTimeOffset.UtcNow,
                FullSizeImage = fullSizeImage,
                Identifier = identifier,
                ProjectId = projectId,
                Id = IdHelper.New(),
                SoftwarePackage = softwarePackage,
                Version = version,
            };
            return ipaBundleRepository.Add(bundle);
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
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="paging">分页</param>
        /// <param name="projectCode">所属项目代码</param>
        /// <returns></returns>
        public Task<PagedList<IpaBundleHistoryVm>> GetHistoryList(string sysUserName, Paging paging,
            string projectCode)
        {
            return ipaBundleRepository.GetHistoryList(sysUserName, paging, projectCode);
        }

        /// <summary>
        /// 为安装页面获取
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="projectCode">所属项目代码</param>
        /// <param name="id">指定包id</param>
        /// <returns></returns>
        public async Task<IpaBundleVm> GetForInstallPage(string sysUserName, string projectCode, string id = null)
        {
            var vm = await ipaBundleRepository.GetByProjectCode(sysUserName, projectCode, id);
            if (vm == null)
            {
                return null;
            }

            var fullSizeImageInfo = await ossService.GetUrl(vm.FullSizeImage, true, new ImageProxyModel()
            {
                Width = 128,
                Height = 128,
            });
            vm.FullSizeImage = fullSizeImageInfo.Url;

            return vm;
        }

        /// <summary>
        /// 为安装plist文件构建获取
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="projectCode">所属项目代码</param>
        /// <param name="id">指定包id</param>
        /// <param name="isLocal">是否为本地安装查询</param>
        /// <returns></returns>
        public async Task<IpaBundleVm> GetForInstallTemplate(string sysUserName, string projectCode, string id = null, bool isLocal = false)
        {
            var vm = await ipaBundleRepository.GetByProjectCode(sysUserName, projectCode, id);

            var fullSizeImageInfo = await ossService.GetUrl(vm.FullSizeImage, true);
            var fullSizeImage = fullSizeImageInfo.Url.AsImageProxyOptions().Width(512).Height(512);
            vm.FullSizeImage = fullSizeImage;
            var displayImage = fullSizeImageInfo.Url.AsImageProxyOptions().Width(64).Height(64);
            vm.DisplayImage = displayImage;

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
        /// <param name="projectId">项目id</param>
        /// <param name="identifier">包id</param>
        /// <param name="version">版本号</param>
        /// <param name="fullSizeImage">全尺寸显示图（文件sha1哈希）</param>
        /// <param name="softwarePackage">ipa（文件sha1哈希）</param>
        /// <returns></returns>
        Task Add(string projectId, string identifier, string version, string fullSizeImage, string softwarePackage);

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
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="paging">分页</param>
        /// <param name="projectCode">所属项目代码</param>
        /// <returns></returns>
        Task<PagedList<IpaBundleHistoryVm>> GetHistoryList(string sysUserName, Paging paging,
            string projectCode);

        /// <summary>
        /// 为安装页面获取
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="projectCode">所属项目代码</param>
        /// <param name="id">指定包id</param>
        /// <returns></returns>
        Task<IpaBundleVm> GetForInstallPage(string sysUserName, string projectCode, string id = null);

        /// <summary>
        /// 为安装plist文件构建获取
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="projectCode">所属项目代码</param>
        /// <param name="id">指定包id</param>
        /// <param name="isLocal">是否为本地安装查询</param>
        /// <returns></returns>
        Task<IpaBundleVm> GetForInstallTemplate(string sysUserName, string projectCode, string id = null, bool isLocal = false);
    }
}

