using System;
using Serilog;

namespace HttpDataServer.Database
{
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
                Sql.CreateTabls();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "SQL 初始化失敗");
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
                Log.Error(ex, "Redis 初始化失敗");
                Environment.Exit(1);
            }
        }

        public void Dispose()
        {
            Sql.Dispose();
            Redis.Dispose();
        }
    }
}