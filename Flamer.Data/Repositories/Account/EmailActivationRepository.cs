using Flamer.Model.Web.Databases.Main.Account;
using SQLite;
using System.Threading.Tasks;

namespace Flamer.Data.Repositories.Account
{
    /// <summary>
    /// 邮件激活
    /// </summary>
    public class EmailActivationRepository : IEmailActivationRepository
    {
        private readonly SQLiteAsyncConnection connection;

        public EmailActivationRepository(SQLiteAsyncConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="emailActivation"></param>
        /// <returns></returns>
        public Task<int> Add(EmailActivation emailActivation)
        {
            return connection.InsertAsync(emailActivation);
        }

        /// <summary>
        /// 设置为已激活的
        /// </summary>
        /// <param name="emailActivation"></param>
        /// <returns></returns>
        public async Task SetActivated(EmailActivation emailActivation)
        {
            emailActivation.Activated = true;

            await connection.UpdateAsync(emailActivation);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<EmailActivation> Get(string token)
        {
            return connection.Table<EmailActivation>().FirstOrDefaultAsync(m => m.Token == token);
        }
    }


    /// <summary>
    /// 邮件激活
    /// </summary>
    public interface IEmailActivationRepository
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="emailActivation"></param>
        /// <returns></returns>
        Task<int> Add(EmailActivation emailActivation);

        /// <summary>
        /// 设置为已激活的
        /// </summary>
        /// <param name="emailActivation"></param>
        /// <returns></returns>
        Task SetActivated(EmailActivation emailActivation);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<EmailActivation> Get(string token);
    }

}
