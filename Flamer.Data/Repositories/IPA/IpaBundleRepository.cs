using Flamer.Model.Web.Databases.Main.IPA;
using Flamer.Model.Web.Databases.Main.Projects;
using Flamer.Model.ViewModel.IPA;
using Flamer.Pagination;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flamer.Data.Repositories.IPA
{
    public class IpaBundleRepository : IIpaBundleRepository
    {
        private readonly SQLiteAsyncConnection connection;

        public IpaBundleRepository(SQLiteAsyncConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public Task<int> Add(IpaBundle bundle)
        {
            return connection.InsertAsync(bundle);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public async Task<int> Edit(string sysUserName, string id, IpaBundle bundle)
        {
            var ps = new object[] { sysUserName, id };

            var tIpaBundle = nameof(IpaBundle);

            var findSql = $"SELECT b.* FROM {tIpaBundle} AS b WHERE b.SysUserName = ? AND b.Id = ?";
            var dbBundle = await connection.FindWithQueryAsync<IpaBundle>(findSql, args: ps);

            dbBundle.ProjectId = bundle.ProjectId;
            dbBundle.FullSizeImage = bundle.FullSizeImage;
            dbBundle.Identifier = bundle.Identifier;
            dbBundle.SoftwarePackage = bundle.SoftwarePackage;
            dbBundle.Version = bundle.Version;

            return await connection.UpdateAsync(dbBundle);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<int> Remove(string sysUserName, IEnumerable<string> ids)
        {
            var ps = new object[] { sysUserName };

            var tIpaBundle = nameof(IpaBundle);

            var inIds = $"('{string.Join("', '", ids)}')";

            var sql = $@"
DELETE FROM {tIpaBundle} WHERE b.SysUserName = ? AND b.Id IN {inIds}
";

            return connection.ExecuteAsync(sql, ps);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="sysUserName">所属用户名</param>
        /// <param name="paging">分页</param>
        /// <param name="projectId">所属项目</param>
        /// <param name="id">指定包id</param>
        /// <returns></returns>
        public async Task<PagedList<IpaBundleVm>> GetList(string sysUserName, Paging paging,
            string projectId = null, string id = null)
        {
            var tIpaBundle = nameof(IpaBundle);
            var tProject = nameof(Project);

            var ps = new List<object>() { sysUserName };

            var projectIdWhere = "1=1";
            if (!string.IsNullOrEmpty(projectId))
            {
                projectIdWhere = "p.Id = ?";
                ps.Add(projectId);
            }

            var idWhere = "1=1";
            if (!string.IsNullOrEmpty(id))
            {
                idWhere = "b.Id = ?";
                ps.Add(id);
            }

            var tmpSql = $@"SELECT {{0}} FROM {tIpaBundle} AS b INNER JOIN {tProject} AS p ON p.Id = b.ProjectId WHERE b.SysUserName = ? AND {projectIdWhere} AND {idWhere}";

            var countSql = string.Format(tmpSql, "COUNT(1)");
            var count = await connection.ExecuteScalarAsync<long>(countSql, ps.ToArray());

            var listSql = $@"
{string.Format(tmpSql, $@"b.Id, b.ProjectId, b.CreateTime, b.SoftwarePackage, b.FullSizeImage, b.Version, b.Identifier, p.Name AS Title")}
{paging.PagingSql("b.CreateTime")}";
            var list = await connection.QueryAsync<IpaBundleVm>(listSql, ps.ToArray());
            return PagedList.Create(count, list);
        }

        /// <summary>
        /// 获取简单历史记录列表
        /// </summary>
        /// <param name="paging">分页</param>
        /// <param name="projectCode">所属项目代码</param>
        /// <param name="sysUserName">所属用户名</param>
        /// <returns></returns>
        public async Task<PagedList<IpaBundleHistoryVm>> GetHistoryList(Paging paging,
            string projectCode, string sysUserName = null)
        {
            var tIpaBundle = nameof(IpaBundle);
            var tProject = nameof(Project);

            var ps = new List<object>() { projectCode.ToLower() };

            var sysUserNameWhere = "1=1";
            if (!string.IsNullOrEmpty(sysUserName))
            {
                sysUserNameWhere = "b.SysUserName = ?";
                ps.Add(sysUserName);
            }

            var tmpSql = $@"SELECT {{0}} FROM {tIpaBundle} AS b INNER JOIN {tProject} AS p ON p.Id = b.ProjectId WHERE LOWER(p.Code) = ? AND {sysUserNameWhere}";

            var countSql = string.Format(tmpSql, "COUNT(1)");
            var count = await connection.ExecuteScalarAsync<long>(countSql, ps.ToArray());

            var listSql = $@"
{string.Format(tmpSql, $@"b.Id, b.SysUserName, b.CreateTime, b.Version, p.Name AS Title")}
{paging.PagingSql("b.CreateTime")}";
            var list = await connection.QueryAsync<IpaBundleHistoryVm>(listSql, ps.ToArray());
            return PagedList.Create(count, list);
        }

        /// <summary>
        /// 根据Id查找
        /// </summary>
        /// <param name="projectCode">项目代码</param>
        /// <param name="sysUserName">所属用户名</param>
        /// <returns></returns>
        public Task<string> GetId(string projectCode, string sysUserName = null)
        {
            var tIpaBundle = nameof(IpaBundle);
            var tProject = nameof(Project);

            var ps = new List<object>() { projectCode.ToLower() };

            var sysUserNameWhere = "1=1";
            if (!string.IsNullOrEmpty(sysUserName))
            {
                sysUserNameWhere = "LOWER(b.SysUserName) = ?";
                ps.Add(sysUserName.ToLower());
            }

            var sql = $@"
SELECT b.Id FROM {tIpaBundle} AS b INNER JOIN {tProject} AS p ON p.Id = b.ProjectId
WHERE LOWER(p.Code) = ? AND {sysUserNameWhere}
ORDER BY b.CreateTime DESC
LIMIT 1
";
            return connection.ExecuteScalarAsync<string>(sql, ps.ToArray());
        }

        /// <summary>
        /// 根据Id查找
        /// </summary>
        /// <param name="id">指定的id</param>
        /// <returns></returns>
        public Task<IpaBundleVm> Get(string id)
        {
            var tIpaBundle = nameof(IpaBundle);
            var tProject = nameof(Project);

            var ps = new List<object>() { id };

            var sql = $@"
SELECT b.Id, b.ProjectId, p.Code AS ProjectCode, b.SysUserName, b.CreateTime, b.SoftwarePackage, b.FullSizeImage, b.Version, b.Identifier, p.Name AS Title, b.Env
FROM {tIpaBundle} AS b INNER JOIN {tProject} AS p ON p.Id = b.ProjectId
WHERE b.Id = ?
ORDER BY b.CreateTime DESC
";

            return connection.FindWithQueryAsync<IpaBundleVm>(sql, ps.ToArray());
        }
    }


    public interface IIpaBundleRepository
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        Task<int> Add(IpaBundle bundle);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        Task<int> Edit(string sysUserName, string id, IpaBundle bundle);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<int> Remove(string sysUserName, IEnumerable<string> ids);

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
        /// 根据Id查找
        /// </summary>
        /// <param name="projectCode">项目代码</param>
        /// <param name="sysUserName">所属用户名</param>
        /// <returns></returns>
        Task<string> GetId(string projectCode, string sysUserName = null);

        /// <summary>
        /// 根据Id查找
        /// </summary>
        /// <param name="id">指定的id</param>
        /// <returns></returns>
        Task<IpaBundleVm> Get(string id);
    }

}
