using Flamer.Model.Web.Databases.Main.Account;
using SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Data.Repositories.Account
{
    /// <summary>
    /// 票据
    /// </summary>
    public class TicketRepository : ITicketRepository
    {
        private readonly SQLiteAsyncConnection connection;

        public TicketRepository(SQLiteAsyncConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public Task<int> Add(Ticket ticket)
        {
            return connection.InsertAsync(ticket);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<int> Remove(string token)
        {
            return connection.Table<Ticket>().Where(m => m.Token == token).DeleteAsync();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<Ticket> Get(string token)
        {
            return connection.Table<Ticket>().FirstOrDefaultAsync(m => m.Token == token);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string> GetSysUserName(string token)
        {
            var list = await connection.QueryScalarsAsync<string>($"SELECT {nameof(Ticket.SysUserName)} FROM {nameof(Ticket)} WHERE {nameof(Ticket.Token)} = ?", token);

            return list?.FirstOrDefault();
        }
    }


    /// <summary>
    /// 票据
    /// </summary>
    public interface ITicketRepository
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        Task<int> Add(Ticket ticket);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<int> Remove(string token);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Ticket> Get(string token);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetSysUserName(string token);
    }

}
