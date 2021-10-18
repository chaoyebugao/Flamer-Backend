using Flamer.Data.Repositories.Db;
using Flamer.Model.Web.Databases.Main.Db;
using Flamer.Model.ViewModel.Db;
using Flamer.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Flamer.Service.Domain.Db
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IDbSchemeRepository dbSchemeRepository;
        private readonly IDbUserRepository dbUserRepository;
        private readonly IDbSchemeUserRelativeRepository dbSchemeUserRelativeRepository;

        public DatabaseService(IDbSchemeRepository dbSchemeRepository,
            IDbUserRepository dbUserRepository,
            IDbSchemeUserRelativeRepository dbSchemeUserRelativeRepository)
        {
            this.dbSchemeRepository = dbSchemeRepository;
            this.dbUserRepository = dbUserRepository;
            this.dbSchemeUserRelativeRepository = dbSchemeUserRelativeRepository;
        }

        /// <summary>
        /// 添加数据库实例
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="projectId"></param>
        /// <param name="volPath"></param>
        /// <param name="host"></param>
        /// <param name="name"></param>
        /// <param name="schemeUsers"></param>
        /// <returns></returns>
        public async Task AddSchemeAsync(string sysUserName, string projectId, string volPath, string host, string name, IEnumerable<NetworkCredential> schemeUsers = null)
        {
            var dbScheme = new DbScheme()
            {
                CreateTime = DateTimeOffset.UtcNow,
                Id = IdHelper.New(),
                Host = host,
                Name = name,
                ProjectId = projectId,
                VolPath = volPath,
                SysUserName = sysUserName,
            };

            var dbUsers = schemeUsers?.Select(m => new DbUser()
            {
                CreateTime = DateTimeOffset.UtcNow,
                Id = IdHelper.New(),
                Password = m.Password,
                Username = m.UserName,
            });

            //TODO:添加关联到关系表

            await dbSchemeRepository.Add(dbScheme);
            if (dbUsers?.Count() > 0)
            {
                await dbUserRepository.Add(dbUsers);
            }
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="usernamePassword"></param>
        /// <returns></returns>
        public Task AddUserAsync(string schemeId, NetworkCredential usernamePassword)
        {
            var dbUser = new DbUser()
            {
                CreateTime = DateTimeOffset.UtcNow,
                Id = IdHelper.New(),
                Password = usernamePassword.Password,
                Username = usernamePassword.UserName,
            };
            return dbUserRepository.Add(new DbUser[] { dbUser });

        }

        /// <summary>
        /// 删除数据库实例
        /// </summary>
        /// <param name="schemeId"></param>
        public Task RemoveUserAsync(string userIdId)
        {
            return dbUserRepository.Remove(userIdId);
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
            var pagedList = await dbSchemeRepository.GetList(sysUserName, paging, keyword);

            var now = DateTime.Now;
            foreach (var m in pagedList.List)
            {
                var dumpCmd = $@"echo '主机:' && read -s HOST && echo '用户:' && read -s USERNAME && echo '密码:' && read -s PASSWORD && echo '数据库:' && read -s SCHEME && docker run --rm -it -v {m.VolPath}/sql:/sql imega/mysql-client mysqldump --user=$USERNAME --password=$PASSWORD --host=$HOST --protocol=tcp --port=3306 --skip-lock-tables  $SCHEME | gzip> {m.VolPath}/backup/db_$SCHEME_{now:yyyyMMddHHmmss}.sql.gz && cp {m.VolPath}/backup/db_$SCHEME_{now:yyyyMMddHHmmss}.sql.gz /tmp/
{m.Host}
{m.Username}
{m.Password}
{m.Name}
";

                var consoleCmd = $@"echo '主机:' && read -s HOST && echo '用户:' && read -s USERNAME && echo '密码:' && read -s PASSWORD && echo '数据库:' && read -s SCHEME && docker run --rm -it -v {m.VolPath}/sql:/sql imega/mysql-client mysql -u$USERNAME -p$PASSWORD -P3306 -h$HOST -v --database=$SCHEME
{m.Host}
{m.Username}
{m.Password}
{m.Name}
";
            }


            return pagedList;

        }


    }


    public interface IDatabaseService
    {
        /// <summary>
        /// 添加数据库实例
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="projectId"></param>
        /// <param name="volPath"></param>
        /// <param name="host"></param>
        /// <param name="name"></param>
        /// <param name="schemeUsers"></param>
        /// <returns></returns>
        Task AddSchemeAsync(string sysUserName, string projectId, string volPath, string host, string name, IEnumerable<NetworkCredential> schemeUsers = null);

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="usernamePassword"></param>
        /// <returns></returns>
        Task AddUserAsync(string schemeId, NetworkCredential usernamePassword);

        /// <summary>
        /// 删除数据库实例
        /// </summary>
        /// <param name="schemeId"></param>
        Task RemoveUserAsync(string userIdId);

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
