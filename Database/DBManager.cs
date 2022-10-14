using System;
using System.Data;
using Serilog;

namespace DataServer.Database;

public class DBManager
{
    public MsSqlEngine Sql;
    public RedisEngine Redis;

    public DBManager()
    {
        InitMsSql();
        InitRedis();
    }

    private void InitMsSql()
    {
        try
        {
            Sql = new MsSqlEngine(Config.GetMsSqlConnectionString());
            if (Sql.Connection.State != ConnectionState.Open)
            {
                Log.Fatal($"SQL 連接失敗, State : {(System.Data.ConnectionState)Sql.Connection.State}");
                Environment.Exit(1);
            }
            Sql.CreateTabls();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "SQL 初始化失敗");
            Environment.Exit(1);
        }
    }

    private void InitRedis()
    {
        try
        {
            Redis = new RedisEngine(Config.GetRedisConnectionString(),
              int.Parse(Config.GetValueFromKey("ConnectionStrings:RedisDB")));
            Redis.DB.Ping();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Redis 初始化失敗");
            Environment.Exit(1);
        }
    }

    public void Dispose()
    {
        Sql.Dispose();
        Redis.Dispose();
    }
}