using Flamer.Model.Web.Databases.Main.Blob;
using SQLite;
using System.Threading.Tasks;

namespace Flamer.Data.Repositories.Blob
{
    public class OssFileRepository : IOssFileRepository
    {
        private readonly SQLiteAsyncConnection connection;

        public OssFileRepository(SQLiteAsyncConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Task<int> Add(OssFile file)
        {
            return connection.InsertAsync(file);
        }

        ///// <summary>
        ///// 哈希是否已存在
        ///// </summary>
        ///// <param name="hash"></param>
        ///// <returns></returns>
        //public async Task<bool> HashExists(string hash)
        //{
        //    var count = await connection.Table<OssFile>().CountAsync(m => m.Hash == hash);
        //    return count > 0;
        //}

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public Task<OssFile> Get(string hash)
        {
            return connection.Table<OssFile>().FirstOrDefaultAsync(m => m.Hash == hash);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public Task<OssFile> Get(string sysUserName, string hash)
        {
            return connection.Table<OssFile>().FirstOrDefaultAsync(m => m.SysUserName == sysUserName && m.Hash == hash);
        }
    }

    /// <summary>
    /// 哈希是否已存在
    /// </summary>
    /// <param name="hash"></param>
    /// <returns></returns>
    public interface IOssFileRepository
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<int> Add(OssFile file);

        ///// <summary>
        ///// 哈希是否已存在
        ///// </summary>
        ///// <param name="hash"></param>
        ///// <returns></returns>
        //Task<bool> HashExists(string hash);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task<OssFile> Get(string hash);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="sysUserName"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task<OssFile> Get(string sysUserName, string hash);
    }

}
