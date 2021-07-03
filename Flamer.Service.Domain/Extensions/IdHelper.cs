using Flammer.Utility.Base62;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Flamer.Service.Domain
{
    public static class IdHelper
    {
        public static string New()
        {
            var id = YitIdHelper.NextId();
            return id.ToBase62();
        }
    }
}
