using Flamer.Data.Repositories.Account;
using Flamer.Service.Common.Cache;
using Flamer.Service.Domain.Account.ViewModels;
using Flammer.Model.Backend.Databases.Main.Account;
using Flammer.Service.Email;
using Flammer.Utility.Base62;
using Flammer.Utility.Extensions;
using Flammer.Utility.Security;
using Flammer.Utility.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Flammer.Service.Domain.User
{
    public class UserService : IUserService
    {
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly IMemoryCache memoryCache;
        private readonly IUserRepository userRepository;
        private readonly IEmailActivationRepository emailActivationRepository;
        private readonly ITicketRepository ticketRepository;

        public UserService(IConfiguration configuration,
            IEmailService emailService,
            IMemoryCache memoryCache,
            IUserRepository userRepository,
            IEmailActivationRepository emailActivationRepository,
            ITicketRepository ticketRepository)
        {
            this.configuration = configuration;
            this.emailService = emailService;
            this.memoryCache = memoryCache;
            this.userRepository = userRepository;
            this.emailActivationRepository = emailActivationRepository;
            this.ticketRepository = ticketRepository;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task SignUpAsync(string name, string email, string password)
        {
            //只能英文和数字，且只能英文开头
            var namePattern = $"[a-zA-z][a-zA-Z0-9]*";
            if (!Regex.Match(name, namePattern).Success)
            {
                throw new BizErrorException("用户名只能英文和数字，并且英文开头");
            }

            var nameExists = await userRepository.NameExists(name);
            if (nameExists)
            {
                throw new BizErrorException("已存在的用户名");
            }

            var emailExists = await userRepository.EmailExists(email);
            if (emailExists)
            {
                throw new BizErrorException("已存在的Email");
            }

            var actvToken = Guid.NewGuid().ToBase62();
            var hmacSha256Secret = configuration["HmacSha256Secret"];
            var pwdHash = HmacSha256Hasher.CreateAsBase62(name + password, hmacSha256Secret);
            var actvTime = DateTime.Now.ToUnixSeconds();
            var actvNonce = StringUtil.GetRandomString(10);
            var actvSig = HmacSha256Hasher.CreateAsBase62($"{actvToken}{actvTime}{actvNonce}", hmacSha256Secret);
            var actvUrl = $"{configuration["WebAddr"]}/api/account/user/activate?t={actvToken}&ts={actvTime}&s={actvSig}&n={actvNonce}";

            await emailService.SendSignUpActivationAsync(name, email, actvUrl);
            var emailActivation = new EmailActivation()
            {
                CreateTime = DateTimeOffset.UtcNow,
                ExpireAt = DateTimeOffset.UtcNow.AddHours(1),
                Token = actvToken,
                SysUserName = name,
            };

            await emailActivationRepository.Add(emailActivation);

            var user = new SysUser()
            {
                CreateTime = DateTimeOffset.UtcNow,
                Email = email,
                Name = name,
                PasswordHash = pwdHash,
            };

            

            await userRepository.Add(user);

        }

        /// <summary>
        /// 注册激活
        /// </summary>
        /// <param name="token"></param>
        /// <param name="time"></param>
        /// <param name="signature"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        public async Task SignUpActivateAsync(string token, ulong time, string signature, string nonce)
        {
            var timestamp = time.ToDateTime();
            var now = DateTime.Now;
            if (timestamp < now.AddMinutes(-10) || timestamp > now.AddMinutes(10))
            {
                throw new UnauthorizedAccessException();
            }

            var hmacSha256Secret = configuration["HmacSha256Secret"];
            var actvSig = HmacSha256Hasher.CreateAsBase62($"{token}{time}{nonce}", hmacSha256Secret);
            if (actvSig != signature)
            {
                throw new UnauthorizedAccessException();
            }

            var emailActivation = await emailActivationRepository.Get(token);
            if (emailActivation == null)
            {
                throw new UnauthorizedAccessException("激活失败，未找到相关激活记录");
            }
            if (emailActivation.Activated)
            {
                throw new UnauthorizedAccessException("激活失败，已使用过的链接");
            }
            if (emailActivation.ExpireAt < DateTimeOffset.UtcNow)
            {
                throw new UnauthorizedAccessException("激活失败，激活链接已过期");
            }

            await emailActivationRepository.SetActivated(emailActivation);
            await userRepository.SetActivated(emailActivation.SysUserName);

        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<Ticket> LoginAsync(string loginName, string password)
        {
            //TODO:替换为正则表达式来判断
            var isLoginByEmail = loginName.Contains("@");

            var hmacSha256Secret = configuration["HmacSha256Secret"];

            string name;
            string email;

            if (isLoginByEmail)
            {
                email = loginName;
                name = await userRepository.GetName(email);
            }
            else
            {
                name = loginName;
                email = await userRepository.GetEmail(name);
            }
            if (name == null || email == null)
            {
                throw new BizErrorException("登录名或密码错误");
            }

            var pwdHash = HmacSha256Hasher.CreateAsBase62(name + password, hmacSha256Secret);

            var user = await userRepository.GetByEmail(email, pwdHash);
            if (user == null)
            {
                throw new BizErrorException("登录名或密码错误");
            }

            if (!user.Activated)
            {
                throw new BizErrorException("用户未激活，请先激活");
            }

            var ticket = new Ticket()
            {
                CreateTime = DateTimeOffset.UtcNow,
                ExpireAt = DateTimeOffset.UtcNow.AddDays(50),
                Token = Guid.NewGuid().ToBase62(),
                SysUserName = user.Name,
            };

            await ticketRepository.Add(ticket);

            memoryCache.Set(CacheKeys.Token, ticket, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30),
            });

            return ticket;
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task LogoutAsync(string token)
        {
            return ticketRepository.Remove(token);
        }

        /// <summary>
        /// 验证票据
        /// </summary>
        /// <param name="token"></param>
        public async Task VerifyTokenAsync(string token)
        {
            var hasCache = memoryCache.TryGetValue(CacheKeys.Token, out Ticket ticket);
            if (!hasCache)
            {
                ticket = await ticketRepository.Get(token);

                if (ticket == null)
                {
                    throw new BizErrorException("无效的登录");
                }

                memoryCache.Set(CacheKeys.Token, ticket, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30),
                });
            }

            if (ticket.ExpireAt < DateTimeOffset.UtcNow)
            {
                throw new BizErrorException("登录已过期，请重新登录");
            }

        }

        /// <summary>
        /// 获取账户信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<UserInfo> GetInfoAsync(string name)
        {
            var user = await userRepository.Get(name);
            if (user == null)
            {
                return null;
            }

            return new UserInfo()
            {
                Name = user.Name,
                Email = user.Email,
                Avatar = "https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif?imageView2/1/w/80/h/80",
            };
        }

        /// <summary>
        /// 根据令牌获取用户id
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        public async Task<string> GetNameByTokenAsync(string token)
        {
            var hasCache = memoryCache.TryGetValue(CacheKeys.TokenSysUserName, out string sysUserName);
            if (!hasCache)
            {
                var ticket = await ticketRepository.Get(token);
                if (ticket == null || ticket.ExpireAt < DateTimeOffset.UtcNow)
                {
                    throw new BizErrorException("获取用户错误");
                }

                sysUserName = ticket.SysUserName;

                memoryCache.Set(CacheKeys.TokenSysUserName, sysUserName, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30),
                });
            }

            return sysUserName;
        }
    }


    public interface IUserService
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task SignUpAsync(string name, string email, string password);

        /// <summary>
        /// 注册激活
        /// </summary>
        /// <param name="token"></param>
        /// <param name="time"></param>
        /// <param name="signature"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        Task SignUpActivateAsync(string token, ulong time, string signature, string nonce);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns>票据Token</returns>
        Task<Ticket> LoginAsync(string loginName, string password);

        /// <summary>
        /// 注销登录
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task LogoutAsync(string token);

        /// <summary>
        /// 验证票据
        /// </summary>
        /// <param name="token"></param>
        Task VerifyTokenAsync(string token);

        /// <summary>
        /// 获取账户信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<UserInfo> GetInfoAsync(string name);

        /// <summary>
        /// 根据令牌获取用户id
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        Task<string> GetNameByTokenAsync(string token);
    }

}
