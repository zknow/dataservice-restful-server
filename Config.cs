using System;
using System.IO;
using Microsoft.Extensions.Configuration;

//Doc:https://www.rocksaying.tw/archives/2019/dotNET-Core-%E7%AD%86%E8%A8%98-ASP.NET-Core-appsettings.json-%E8%88%87%E5%9F%B7%E8%A1%8C%E7%92%B0%E5%A2%83.html
public class Config
{
    private static IConfigurationRoot conf;

    public static void LoadConfig()
    {
        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
        conf = builder.Build();
    }

    public static string GetMsSqlConnectionString()
    {
        return conf.GetConnectionString("MssqlConnectionStrings");
    }

    public static string GetRedisConnectionString()
    {
        return conf.GetConnectionString("RedisConnectionStrings");
    }

    public static string GetValueFromKey(string key)
    {
        return conf.GetSection(key).Value;
    }
}