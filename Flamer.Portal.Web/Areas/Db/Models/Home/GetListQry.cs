using Flammer.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flammer.Portal.Web.Areas.Db.Models.Home
{
    public class GetListQry : Paging
    {
        
        public string Keyword { get; set; }
    }
}
