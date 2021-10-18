using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Portal.Web.Areas.IPA.Models.Home
{
    public class GetQry
    {
        public string projectCode { get; set; }

        /// <summary>
        /// ipa包id，较优先
        /// </summary>
        public string id { get; set; }

        public string sysUserName { get; set; }
    }
}
