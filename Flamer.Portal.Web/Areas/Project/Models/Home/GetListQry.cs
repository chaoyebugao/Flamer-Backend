using Flamer.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Portal.Web.Areas.Project.Models.Home
{
    public class GetListQry : Paging
    {
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }

    }
}
