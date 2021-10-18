using Flamer.Data.Repositories.Db;
using Flamer.Data.Repositories.Projects;
using Flamer.Model.Web.Databases.Main.Projects;
using Flamer.Model.ViewModel.Projects;
using Flamer.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flamer.Model.ViewModel;

namespace Flamer.Service.Domain.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository projectRepository;
        private readonly IDbSchemeRepository dbSchemeRepository;

        public ProjectService(IProjectRepository projectRepository,
            IDbSchemeRepository dbSchemeRepository)
        {
            this.projectRepository = projectRepository;
            this.dbSchemeRepository = dbSchemeRepository;
        }

        public async Task Add(string creator, string code, string name, string logo)
        {
            var codeExisted = await projectRepository.CodeExists(creator, code);
            if (codeExisted)
            {
                throw new Exception("已存在的代码");
            }

            var project = new Project()
            {
                Id = IdHelper.New(),
                CreateTime = DateTimeOffset.UtcNow,
                Code = code,
                Name = name,
                Creator = creator,
                Logo = logo,
            };

            await projectRepository.Add(project);
        }

        public async Task Edit(string id, string code, string name, string logo)
        {
            var codeExisted = await projectRepository.CodeExists(code, id);
            if (codeExisted)
            {
                throw new Exception("已存在的代码");
            }

            var project = new Project()
            {
                Code = code,
                Name = name,
                Logo = logo,
            };

            await projectRepository.Edit(id, project);
        }

        public async Task Remove(IEnumerable<string> projectIds)
        {
            var existedIds = await dbSchemeRepository.GetList(projectIds);
            if (existedIds.Count() != 0)
            {
                throw new BizErrorException("项目仍关联有数据库");
            }

            await projectRepository.Remove(projectIds);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="paging"></param>
        /// <param name="creator"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public Task<PagedList<ProjectVm>> GetList(Paging paging, string creator = null, string keyword = null)
        {
            return projectRepository.GetList(paging, creator, keyword);
        }

        /// <summary>
        /// 获取下拉列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public Task<IEnumerable<SelectVm>> GetListForSelect(string keyword = null)
        {
            return projectRepository.GetListForSelect(keyword);
        }
    }


    public interface IProjectService
    {
        Task Add(string sysUserName, string code, string name, string logo);

        Task Edit(string id, string code, string name, string logo);

        Task Remove(IEnumerable<string> projectIds);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="paging"></param>
        /// <param name="creator"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<PagedList<ProjectVm>> GetList(Paging paging, string creator = null, string keyword = null);

        /// <summary>
        /// 获取下拉列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<IEnumerable<SelectVm>> GetListForSelect(string keyword = null);
    }

}
