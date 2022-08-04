namespace HttpDataServer.Core;

public static class CacheData
{
    public static long UserSN = -1;

    public static object UserLocker = new object();
}