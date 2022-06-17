using StackExchange.Redis;

namespace HttpDataServer.Database;

public class RedisEngine
{
    private ConnectionMultiplexer redis;
    private int targetDB;

    public IDatabase DB => redis.GetDatabase(targetDB);

    public RedisEngine(string connString, int targetDB = 0)
    {
        redis = ConnectionMultiplexer.Connect(connString);
        this.targetDB = targetDB;
    }

    public void Dispose()
    {
        redis.Dispose();
    }
}