using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flammer.Portal.Web.Areas.Db.Models.Home
{
    public class AddSchemeSub
    {
        public string ProjectId { get; set; }

        public string VolPath { get; set; }
        public string Host { get; set; }
        public string Name { get; set; }

        public IDictionary<string, string> Users { get; set; }
    }

}
