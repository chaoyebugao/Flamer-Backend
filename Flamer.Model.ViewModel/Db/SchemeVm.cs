using System;

namespace Flamer.Model.ViewModel.Db
{
    public class SchemeVm
    {

        public string DbSchemeId { get; set; }

        public string DbUserId { get; set; }

        public DateTimeOffset DbSchemeCreateTime { get; set; }

        public string VolPath { get; set; }

        public string Host { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string DumpCmd { get; set; }

        public string ConsoleCmd { get; set; }
    }
}
