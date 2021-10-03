using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CustomHttps
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context,services)=>{
                    HostConfig.CertPath=context.Configuration["CertPath"];
                    HostConfig.CertPassword=context.Configuration["CertPassword"];
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var host =Dns.GetHostEntry("weather.io");
                    webBuilder.UseKestrel(opt=>{
                        // opt.ListenAnyIP(5000);
                        opt.Listen(host.AddressList[0],5000);
                        opt.ListenAnyIP(5001, listOptions=>{
                            listOptions.UseHttps(HostConfig.CertPath, HostConfig.CertPassword);
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class HostConfig{
        public static string CertPath { get; set; }
        
        public static string CertPassword { get; set; }
        
    }
}
