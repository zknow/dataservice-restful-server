using System;

namespace HttpDataServer.Database
{
    public class DatabaseManager
    {
        public MsSqlEngine Sql;
        public RedisEngine Redis;

        public DatabaseManager()
        {
            try
            {
                InitMsSql();
                InitRedis();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void InitMsSql()
        {
            Sql = new MsSqlEngine(Config.GetMsSqlConnectionString());
            Sql.CreateTabls();
        }

        private void InitRedis()
        {
            Redis = new RedisEngine(Config.GetRedisConnectionString(),
                int.Parse(Config.GetValueFromKey("ConnectionStrings:RedisDB")));
        }

        public void Dispose()
        {
            Sql.Dispose();
            Redis.Dispose();
        }
    }
}