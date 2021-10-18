using Flamer.Model.Web.Databases.Main.Projects;
using Flamer.Model.ViewModel.Projects;
using Flamer.Pagination;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flamer.Model.ViewModel;

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
        public async Task<int> Edit(string id, Project project)
        {
            var dbProject = await connection.Table<Project>().FirstOrDefaultAsync(m => m.Id == id);
            dbProject.Code = project.Code;
            dbProject.Name = project.Name;
            dbProject.Logo = project.Logo;

            return await connection.UpdateAsync(dbProject);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<int> Remove(IEnumerable<string> ids)
        {
            return connection.Table<Project>().Where(m => ids.Contains(m.Id))
                .DeleteAsync();
        }

        /// <summary>
        /// 代码是否已存在
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="excludeId">排除的id</param>
        /// <returns></returns>
        public async Task<bool> CodeExists(string code, string excludeId = null)
        {
            var tProject = nameof(Project);

            var ps = new List<object>()
            {
                code.ToLower(),
            };

            var excludeIdWhere = "1=1";
            if (!string.IsNullOrEmpty(excludeId))
            {
                excludeIdWhere = "Id != ?";
                ps.Add(excludeId);
            }

            var sql = $"SELECT COUNT(1) FROM {tProject} WHERE LOWER(Code) = ? AND {excludeIdWhere}";

            var count = await connection.ExecuteScalarAsync<int>(sql, ps.ToArray());
            return count > 0;
        }

        /// <summary>
        /// 根据代码获取Id
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns></returns>
        public Task<string> GetId(string code)
        {
            var tProject = nameof(Project);

            var ps = new List<object>()
            {
                code.ToLower(),
            };

            var sql = $"SELECT Id FROM {tProject} WHERE LOWER(Code) = ? LIMIT 1";

            return connection.ExecuteScalarAsync<string>(sql, ps.ToArray());
        }

        /// <summary>
        /// 根据代码获取Logo
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns></returns>
        public Task<string> GetLogo(string code)
        {
            var tProject = nameof(Project);

            var ps = new List<object>()
            {
                code.ToLower(),
            };

            var sql = $"SELECT Logo FROM {tProject} WHERE LOWER(Code) = ? LIMIT 1";

            return connection.ExecuteScalarAsync<string>(sql, ps.ToArray());
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="paging"></param>
        /// <param name="creator"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<PagedList<ProjectVm>> GetList(Paging paging, string creator = null, string keyword = null)
        {
            var qry = connection.Table<Project>();

            if (!string.IsNullOrEmpty(creator))
            {
                qry = qry.Where(m => m.Creator == creator);
            }

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
        /// 获取下拉列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SelectVm>> GetListForSelect(string keyword = null)
        {
            var qry = connection.Table<Project>();

            if (!string.IsNullOrEmpty(keyword))
            {
                qry = qry.Where(m => m.Name.Contains(keyword));
            }

            var count = await qry.CountAsync();
            var list = await qry.OrderByDescending(m => m.CreateTime).ToListAsync();

            var vmList = list.Select(m => new SelectVm()
            {
                Label = m.Name,
                Value = m.Code,
            });

            return vmList;
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
        Task<int> Edit(string id, Project project);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<int> Remove(IEnumerable<string> ids);

        /// <summary>
        /// 代码是否已存在
        /// </summary>s
        /// <param name="code">代码</param>
        /// <param name="excludeId">排除的id</param>
        /// <returns></returns>
        Task<bool> CodeExists(string code, string excludeId = null);

        /// <summary>
        /// 根据代码获取Id
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns></returns>
        Task<string> GetId(string code);

        /// <summary>
        /// 根据代码获取Logo
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns></returns>
        Task<string> GetLogo(string code);

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
