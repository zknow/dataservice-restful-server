using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DataServer;

// Protocol Document: https://hackmd.io/DcFF2LRpRumTNxLmSPa2tw
public class Server
{
    public static void Main(string[] args)
    {
        // 防止Redis短時間忙碌造成Crash
        ThreadPool.SetMinThreads(200, 200);

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}