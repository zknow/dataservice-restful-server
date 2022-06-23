using HttpDataServer.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HttpDataServer;

public class Server
{
    public static DatabaseManager DBMgr;

    public static void Main(string[] args)
    {
        Config.LoadConfig();

        DBMgr = new DatabaseManager();

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}