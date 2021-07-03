using Flamer.Data.Repositories.Db;
using Flamer.Data.Repositories.Projects;
using Flamer.Data.ViewModels.Projects;
using Flammer.Model.Backend.Databases.Main.Projects;
using Flammer.Pagination;
using Flammer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task Add(string sysUserName, string code, string name, string logo)
        {
            var codeExisted = await projectRepository.CodeExists(sysUserName, code);
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
                SysUserName = sysUserName,
                Logo = logo,
            };

            await projectRepository.Add(project);
        }

        public async Task Edit(string sysUserName, string id, string code, string name, string logo)
        {
            var codeExisted = await projectRepository.CodeExists(sysUserName, code, id);
            if (codeExisted)
            {
                throw new Exception("已存在的代码");
            }

            var project = new Project()
            {
                Code = code,
                Name = name,
                SysUserName = sysUserName,
                Logo = logo,
            };

            await projectRepository.Edit(sysUserName, id, project);
        }

        public async Task Remove(string sysUserName, IEnumerable<string> projectIds)
        {
            var existedIds = await dbSchemeRepository.GetList(projectIds);
            if (existedIds.Count() != 0)
            {
                throw new BizErrorException("项目仍关联有数据库");
            }

            await projectRepository.Remove(sysUserName, projectIds);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="paging"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public Task<PagedList<ProjectVm>> GetList(string sysUserName, Paging paging, string keyword)
        {
            return projectRepository.GetList(sysUserName, paging, keyword);
        }
    }


    public interface IProjectService
    {
        Task Add(string sysUserName, string code, string name, string logo);

        Task Edit(string sysUserName, string id, string code, string name, string logo);

        Task Remove(string sysUserName, IEnumerable<string> projectIds);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="paging"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<PagedList<ProjectVm>> GetList(string sysUserName, Paging paging, string keyword);
    }

}
