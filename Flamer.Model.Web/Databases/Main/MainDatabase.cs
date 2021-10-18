using Flamer.Model.Web.Databases.Main.Account;
using Flamer.Model.Web.Databases.Main.Blob;
using Flamer.Model.Web.Databases.Main.Db;
using Flamer.Model.Web.Databases.Main.IPA;
using Flamer.Model.Web.Databases.Main.Projects;
using Microsoft.Extensions.Configuration;
using SQLite;
using System.IO;

namespace Flamer.Model.Web.Databases.Main
{
    public class MainDatabase
    {
        private readonly IConfiguration configuration;

        public MainDatabase(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string DirectoryPath => "data";

        public string DbPath
        {
            get
            {
                return Path.Combine(DirectoryPath, "main.db");
            }
        }

        public SQLiteAsyncConnection GetSQLiteConnection()
        {
            //TODO:数据库加密密钥
            var options = new SQLiteConnectionString(DbPath, true, key: configuration["SQLiteKey"]);
            var db = new SQLiteAsyncConnection(options);
            return db;
        }

        public void Init()
        {
            Directory.CreateDirectory(DirectoryPath);

            var db = GetSQLiteConnection();

            db.CreateTableAsync<SysUser>();
            db.CreateTableAsync<EmailActivation>();
            db.CreateTableAsync<Ticket>();

            db.CreateTableAsync<Project>();

            db.CreateTableAsync<DbScheme>();
            db.CreateTableAsync<DbUser>();
            db.CreateTableAsync<DbSchemeUserRelative>();

            db.CreateTableAsync<OssFile>();

            db.CreateTableAsync<IpaBundle>();
        }
    }
}
