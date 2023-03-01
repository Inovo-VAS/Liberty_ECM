#region [ Using ]
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
#endregion

namespace CIMWebAPI
{
    public class Program
   {
      #region [ Main ]
      public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main function");
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Error in init");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }
      #endregion

      #region [ IHost Builder ]
      public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) => { config.AddJsonFile("InovoCIM.json", optional: false, reloadOnChange: false); })
        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();  
            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information); 
        })
        .UseNLog();
      #endregion
   }
}