using Flamer.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Portal.Web.Areas.IPA.Models.Home
{
    public class GetListQry : Paging
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>
        public string ProjectId { get; set; }
    }
}
