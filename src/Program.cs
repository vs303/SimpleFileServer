using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Hosting;


namespace Saro.FileServer
{
    public class Program
    {
        public static IConfigurationRoot s_Config { get; private set; }

        public static void Main(string[] args)
        {
            var configDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            Directory.SetCurrentDirectory(configDir);

            s_Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .Build();

            CreateHostBuilder(args).Build().Run();
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseWindowsService(options =>
                {
                    options.ServiceName = "fileServer";
                });
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseUrls(s_Config["urls"])
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    //所有controller都不限制post的body大小
                    options.Limits.MaxRequestBodySize = null;
                });
        }
    }
}