using Flamer.Model.Web.Databases.Main.Db;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flamer.Data.Repositories.Db
{
    public class DbUserRepository : IDbUserRepository
    {
        private readonly SQLiteAsyncConnection connection;

        public DbUserRepository(SQLiteAsyncConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="dbUser"></param>
        /// <returns></returns>
        public Task<int> Add(IEnumerable<DbUser> dbUsers)
        {
            return connection.InsertAllAsync(dbUsers);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="dbUser"></param>
        /// <returns></returns>
        public Task<int> Remove(string id)
        {
            return connection.Table<DbUser>().Where(m => m.Id == id).DeleteAsync();
        }

    }

    public interface IDbUserRepository
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="dbUser"></param>
        /// <returns></returns>
        Task<int> Add(IEnumerable<DbUser> dbUsers);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="dbUser"></param>
        /// <returns></returns>
        Task<int> Remove(string id);

    }

}
