using Flammer.Presentation.Web.Areas.Account.Models.User;
using Flammer.Presentation.Web.Mgr.Models.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Flammer.Presentation.Web.Tests.Areas.Account.Controllers
{
    public class UserController : TestBase
    {
        [Fact]
        public async Task SignUpAsync()
        {
            var sub = new SignUpSub()
            {
                Email = "firesheng@qq.com",
                Password = "123456",
            };
            var result = await PostAsync<SuccessRet>("api/account/user/signup", sub);

            Assert.NotNull(result);
            Assert.True(result.Code == RetCodes.Success);
            
        }
    }
}
