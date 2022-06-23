using HttpDataServer.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Tools;

namespace HttpDataServer;

public class Server
{
    private static Logrotator logrotator;
    public static DBManager DBManager;

    public static void Main(string[] args)
    {
        Config.LoadConfig();

        logrotator = new Logrotator();
        DBManager = new DBManager();

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