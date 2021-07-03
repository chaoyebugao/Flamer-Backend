using Flammer.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flammer.Portal.Web.Areas.Project.Models.Home
{
    public class GetListQry : Paging
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }

    }
}
