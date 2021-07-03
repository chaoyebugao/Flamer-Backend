using Flamer.Data.ViewModels.Projects;
using Flammer.Model.Backend.Databases.Main.Projects;
using Flammer.Pagination;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Data.Repositories.Projects
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly SQLiteAsyncConnection connection;

        public ProjectRepository(SQLiteAsyncConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Task<int> Add(Project project)
        {
            return connection.InsertAsync(project);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public async Task<int> Edit(string sysUserName, string id, Project project)
        {
            var dbProject = await connection.Table<Project>().FirstOrDefaultAsync(m => m.SysUserName == sysUserName && m.Id == id);
            dbProject.Code = project.Code;
            dbProject.Name = project.Name;
            dbProject.Logo = project.Logo;

            return await connection.UpdateAsync(dbProject);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="projectIds"></param>
        /// <returns></returns>
        public Task<int> Remove(string sysUserName, IEnumerable<string> projectIds)
        {
            return connection.Table<Project>().Where(m => m.SysUserName == sysUserName && projectIds.Contains(m.Id))
                .DeleteAsync();
        }

        /// <summary>
        /// 代码是否已存在
        /// </summary>
        /// <param name="sysUserName">用户名</param>
        /// <param name="code">代码</param>
        /// <param name="excludeId">排除的id</param>
        /// <returns></returns>
        public async Task<bool> CodeExists(string sysUserName, string code, string excludeId = null)
        {
            var tProject = nameof(Project);

            var ps = new List<object>()
            {
                sysUserName,
                code.ToLower(),
            };

            var excludeIdWhere = "1=1";
            if (!string.IsNullOrEmpty(excludeId))
            {
                excludeIdWhere = "Id != ?";
                ps.Add(excludeId);
            }

            var sql = $"SELECT COUNT(1) FROM {tProject} WHERE SysUserName = ? AND LOWER(Code) = ? AND {excludeIdWhere}";

            var count = await connection.ExecuteScalarAsync<int>(sql, ps.ToArray());
            return count > 0;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="paging"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<PagedList<ProjectVm>> GetList(string sysUserName, Paging paging, string keyword)
        {
            var qry = connection.Table<Project>().Where(m => m.SysUserName == sysUserName);

            if (!string.IsNullOrEmpty(keyword))
            {
                qry = qry.Where(m => m.Name.Contains(keyword));
            }

            var count = await qry.CountAsync();
            var list = await qry.OrderByDescending(m => m.CreateTime).Skip((paging.Page - 1) * paging.Limit).Take(paging.Limit).ToListAsync();

            var vmList = list.Select(m => new ProjectVm()
            {
                Id = m.Id,
                CreateTime = m.CreateTime,
                Code = m.Code,
                Name = m.Name,
                Logo = m.Logo,
            });

            return PagedList.Create(count, vmList);
        }

        /// <summary>
        /// 根据项目代码查找项目id
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="projectCode">项目代码</param>
        /// <returns></returns>
        public async Task<string> GetProjectId(string sysUserName, string projectCode)
        {
            var project = await connection.Table<Project>().FirstOrDefaultAsync(m => m.SysUserName == sysUserName && m.Code.ToLower() == projectCode.ToLower());

            return project?.Id;
        }
    }


    public interface IProjectRepository
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<int> Add(Project project);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<int> Edit(string sysUserName, string id, Project project);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="projectCodes"></param>
        /// <returns></returns>
        Task<int> Remove(string sysUserName, IEnumerable<string> projectCodes);

        /// <summary>
        /// 代码是否已存在
        /// </summary>
        /// <param name="sysUserName">用户名</param>
        /// <param name="code">代码</param>
        /// <param name="excludeId">排除的id</param>
        /// <returns></returns>
        Task<bool> CodeExists(string sysUserName, string code, string excludeId = null);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="paging"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<PagedList<ProjectVm>> GetList(string sysUserName, Paging paging, string keyword);

        /// <summary>
        /// 根据项目代码查找项目id
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="projectCode">项目代码</param>
        /// <returns></returns>
        Task<string> GetProjectId(string sysUserName, string projectCode);
    }

}
