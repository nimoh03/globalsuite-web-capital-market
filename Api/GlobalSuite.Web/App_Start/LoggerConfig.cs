using System.IO;
using System.Web;
using Serilog;
using Serilog;

namespace GlobalSuite.Web
{
    public class LoggerConfig
    {
        public static void Configure()
        {
            var path = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data", "Logs", "logs.txt");
          var loggerConfiguration = new LoggerConfiguration()
              .WriteTo.File(path).CreateLogger();

          Log.Logger = loggerConfiguration;

        }
    }
}