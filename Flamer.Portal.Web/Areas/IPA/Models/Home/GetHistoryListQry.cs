using Flamer.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Portal.Web.Areas.IPA.Models.Home
{
    public class GetHistoryListQry : Paging
    {
        /// <summary>
        /// 所属用户名
        /// </summary>
        public string SysUserName { get; set; }

        /// <summary>
        /// 所属项目代码
        /// </summary>
        public string ProjectCode { get; set; }
    }
}
