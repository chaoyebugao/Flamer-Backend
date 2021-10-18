using Flamer.Model.ViewModel;
using Flamer.Model.Web.Databases.Main.Account;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Data.Repositories.Account
{
    /// <summary>
    /// 系统用户
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly SQLiteAsyncConnection connection;

        public UserRepository(SQLiteAsyncConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sysUser"></param>
        /// <returns></returns>
        public Task<int> Add(SysUser sysUser)
        {
            return connection.InsertAsync(sysUser);
        }

        /// <summary>
        /// 设置为已激活的
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task SetActivated(string name)
        {
            var sysUser = await connection.GetAsync<SysUser>(name);
            sysUser.Activated = true;

            await connection.UpdateAsync(sysUser);
        }

        /// <summary>
        /// 用户名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> NameExists(string name)
        {
            var count = await connection.Table<SysUser>().CountAsync(m => m.Name.ToLower() == name.ToLower());
            return count > 0;
        }

        /// <summary>
        /// Email是否存在
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> EmailExists(string email)
        {
            var count = await connection.Table<SysUser>().CountAsync(m => m.Email.ToLower() == email.ToLower());
            return count > 0;
        }

        /// <summary>
        /// 根据主键获取
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<SysUser> Get(string name)
        {
            return connection.Table<SysUser>().FirstOrDefaultAsync(m => m.Name.ToLower() == name.ToLower());
        }

        /// <summary>
        /// 根据电子邮件获取用户名
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<string> GetName(string email)
        {
            var tSysUser = nameof(SysUser);

            var sql = $"SELECT Name FROM {tSysUser} WHERE Email = ?";
            return connection.ExecuteScalarAsync<string>(sql, email);
        }

        /// <summary>
        /// 根据用户名获取电子邮件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<string> GetEmail(string name)
        {
            var tSysUser = nameof(SysUser);

            var sql = $"SELECT Email FROM {tSysUser} WHERE Name = ?";
            return connection.ExecuteScalarAsync<string>(sql, name);
        }

        /// <summary>
        /// 根据用户名和密码哈希获取
        /// </summary>
        /// <param name="name"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task<SysUser> GetByName(string name, string passwordHash)
        {
            return connection.Table<SysUser>().FirstOrDefaultAsync(m => m.Name.ToLower() == name.ToLower() && m.PasswordHash == passwordHash);
        }

        /// <summary>
        /// 根据Email和密码哈希获取
        /// </summary>
        /// <param name="email"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task<SysUser> GetByEmail(string email, string passwordHash)
        {
            return connection.Table<SysUser>().FirstOrDefaultAsync(m => m.Email.ToLower() == email.ToLower() && m.PasswordHash == passwordHash);
        }

        /// <summary>
        /// 获取下拉列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SelectVm>> GetListForSelect(string keyword = null)
        {
            var qry = connection.Table<SysUser>();

            if (!string.IsNullOrEmpty(keyword))
            {
                qry = qry.Where(m => m.Name.Contains(keyword));
            }

            var count = await qry.CountAsync();
            var list = await qry.OrderByDescending(m => m.CreateTime).ToListAsync();

            var vmList = list.Select(m => new SelectVm()
            {
                Label = m.Name,
                Value = m.Name,
            });

            return vmList;
        }
    }


    /// <summary>
    /// 系统用户
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sysUser"></param>
        /// <returns></returns>
        Task<int> Add(SysUser sysUser);

        /// <summary>
        /// 设置为已激活的
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task SetActivated(string name);

        /// <summary>
        /// 用户名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> NameExists(string name);

        /// <summary>
        /// Email是否存在
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> EmailExists(string email);

        /// <summary>
        /// 根据Email获取用户名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<string> GetName(string email);

        /// <summary>
        /// 根据用户名获取电子邮件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<string> GetEmail(string name);

        /// <summary>
        /// 根据主键获取
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<SysUser> Get(string name);

        /// <summary>
        /// 根据用户名和密码哈希获取
        /// </summary>
        /// <param name="name"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        Task<SysUser> GetByName(string name, string passwordHash);

        /// <summary>
        /// 根据Email和密码哈希获取
        /// </summary>
        /// <param name="email"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        Task<SysUser> GetByEmail(string email, string passwordHash);

        /// <summary>
        /// 获取下拉列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<IEnumerable<SelectVm>> GetListForSelect(string keyword = null);
    }

}
