using System; 
using log4net;
using System.Net;
using System.Xml;
using System.IO;
using System.Collections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using log4net.Config;
using System.Reflection;
using Npgsql;

namespace KKday.Web.OCBT.AppCode
{
    public sealed class Website
    {
        public static readonly Website Instance = new Website();
  
        // DI for Configuration (appsettings.cs)
        public IConfiguration Configuration { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public readonly ILog loggerOrg = LogManager.GetLogger(typeof(Website));
        public Log4netHelper logger = new Log4netHelper(LogManager.GetLogger(typeof(Website)), "OCBT");
        public string AWSaccessKey;
        public string AWSaccessSecret;
        public string AWSbucket;
        public string AWSregionEP;

        public string SqlConnectionString { get; private set; }
        public string AesCryptKey { get; private set; }
        // 主機站台識別
        private string _stationID;
        public string StationID
        {
            get { return _stationID; }
        }

        // ClaimPrinciple 版本序號
        public string PrincipleVersion
        {
            get { return "1.0.0.1"; } // O.C.B.T 建立
        }
        private string _OCBT_DB = "";
        public string OCBT_DB
        {
            get { return _OCBT_DB; }
        }

        private Website()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public void Init(IConfiguration config)//, IHostingEnvironment env)
        {
            this.Configuration = config;
            //this._ERP_DB = Configuration["ConnectionStrings:PostgreSQL"];

            AWSaccessKey = Configuration["AWSS3:AccessKey"];
            AWSaccessSecret = Configuration["AWSS3:AccessSecretKey"];
            AWSbucket = Configuration["AWSS3:Bucket"];
            AWSregionEP = Configuration["AWSS3:RegionEP"];

            _stationID = Dns.GetHostName();

            //NpgsqlConnection npg_conn = new NpgsqlConnection(_ERP_DB);

            LoadLog4netConfig();
            LoadOCBTDBConfig();

            logger.Debug("StartUp....!");
        }

        private void LoadLog4netConfig()
        {
            try
            {
                string logPath = Configuration["log4netPath:path"];

                var repository = LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                         typeof(log4net.Repository.Hierarchy.Hierarchy)
                     );

                log4net.GlobalContext.Properties["LogFileName"] = logPath;
                log4net.GlobalContext.Properties["hostname"] = Environment.MachineName;
                log4net.LogicalThreadContext.Properties["app_env"] = Configuration["app_env"];
                XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));

                // var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
                //Array.ForEach(repository.GetAppenders(), appender =>
                //{

                //     //Check appsetting.json => log4net.Appender.Name is "RollingFile"
                //    if (appender.Name.Equals("RollingFile") &&
                //             appender.GetType() == typeof(log4net.Appender.RollingFileAppender))
                //    {
                //        Console.WriteLine(appender.Name);
                //        ((log4net.Appender.RollingFileAppender)appender).File = logPath;
                //        ((log4net.Appender.RollingFileAppender)appender).ActivateOptions();
                //    }
                //    if (appender.Name.Equals("FileAppender") &&
                //         appender.GetType() == typeof(log4net.Appender.FileAppender))
                //    {
                //        Console.WriteLine(appender.Name);
                //        ((log4net.Appender.FileAppender)appender).File = logPath;
                //        ((log4net.Appender.FileAppender)appender).ActivateOptions();
                //    }
                //});
            }
            catch (Exception ex)
            {
                string qq = ex.Message.ToString();
            }




        }

        private void LoadOCBTDBConfig()
        {
            Console.WriteLine($"pg連線字串：{Configuration["ConnectionStrings:NpgsqlConnection"]}");
            this._OCBT_DB = Configuration["ConnectionStrings:NpgsqlConnection"];

            string szLog4NetCfgFile = string.Format("{0}\\log4net.config", Directory.GetCurrentDirectory());

            _stationID = Dns.GetHostName();

            NpgsqlConnection npg_conn = new NpgsqlConnection(_OCBT_DB);
        }

    }
}

