using System.Threading;
using DataServer.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Tools;

namespace DataServer;

public class Server
{
    private static Logrotator logrotator;
    public static DBManager DBManager;

    public static void Main(string[] args)
    {
        Config.LoadConfig();

        logrotator = new Logrotator();
        DBManager = new DBManager();

        // 防止Redis短時間忙碌造成Crash
        ThreadPool.SetMinThreads(200, 200);

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }

    public static void Dispose()
    {
        DBManager.Dispose();
        logrotator.Dispose();
        DBManager = null;
        logrotator = null;
    }
}