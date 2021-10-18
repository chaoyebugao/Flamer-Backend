using Flamer.Model.Web.Databases.Main.Db;
using Flamer.Model.ViewModel.Db;
using Flamer.Pagination;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Data.Repositories.Db
{
    public class DbSchemeRepository : IDbSchemeRepository
    {
        private readonly SQLiteAsyncConnection connection;

        public DbSchemeRepository(SQLiteAsyncConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="dbScheme"></param>
        /// <returns></returns>
        public Task<int> Add(DbScheme dbScheme)
        {
            return connection.InsertAsync(dbScheme);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="dbUser"></param>
        /// <returns></returns>
        public Task<int> Remove(string id)
        {
            return connection.Table<DbScheme>().Where(m => m.Id == id).DeleteAsync();
        }

        /// <summary>
        /// 是否存在有
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<bool> Exists(string projectId)
        {
            var count = await connection.Table<DbScheme>().CountAsync(m => m.ProjectId == projectId);
            return count > 0;
        }

        /// <summary>
        /// 还存有的数据库
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetList(IEnumerable<string> projectIds)
        {
            var list = await connection.Table<DbScheme>().Where(m => projectIds.Contains(m.ProjectId)).ToListAsync();
            return list.Select(m => m.ProjectId);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paging"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<PagedList<SchemeVm>> GetList(string sysUserName, Paging paging, string keyword)
        {
            var ps = new List<object>() { sysUserName };
            var keywordWhere = "1=1";
            if (!string.IsNullOrEmpty(keyword))
            {
                keywordWhere = "(u.Username LIKE @keyword OR s.Host LIKE ? OR s.Name LIKE ?)";
                var p = $"%{keyword}%";
                ps.Add(p);
                ps.Add(p);
            }

            var tmpSql = $@"SELECT {{0}} FROM DbUser AS u INNER JOIN DbScheme AS s ON s.Id = u.SchemeId WHERE s.SysUserName = ? AND {keywordWhere}";

            
            var countSql = string.Format(tmpSql, "COUNT(1)");
            var count = await connection.ExecuteScalarAsync<long>(countSql, ps);

            var listSql = $@"
{string.Format(tmpSql, $@" s.Id AS DbSchemeId, s.CreateTime AS DbSchemeCreateTime, u.Id AS DbUserId, s.Host, s.Name, s.VolPath")}
{paging.PagingSql()}";
            var list = await connection.QueryAsync<SchemeVm>(listSql, ps);

            return PagedList.Create(count, list);
        }
    }


    public interface IDbSchemeRepository
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="dbScheme"></param>
        /// <returns></returns>
        Task<int> Add(DbScheme dbScheme);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="dbUser"></param>
        /// <returns></returns>
        Task<int> Remove(string id);

        /// <summary>
        /// 是否存在有
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<bool> Exists(string projectId);

        /// <summary>
        /// 还存有的数据库
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetList(IEnumerable<string> projectIds);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paging"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<PagedList<SchemeVm>> GetList(string sysUserName, Paging paging, string keyword);
    }

}
