using System;
using System.Linq;
using HttpDataServer.Core;
using StackExchange.Redis;

namespace HttpDataServer.Repository;

public class ValidationCodeRepo
{
    public IDatabase db => Server.DBMgr.Redis.DB;

    public int RespCode { get; set; } = Code.Success;

    public readonly double defaultExpireMinutes = 10;

    public bool CheckExists(string key)
    {
        return db.KeyExists(key);
    }

    public bool Get(string key, out HashEntry result)
    {
        result = default(HashEntry);
        try
        {
            result = db.HashGetAll(key).First();
            return true;
        }
        catch (System.Exception ex)
        {
            RespCode = Core.Code.DatabaseError;
            Console.WriteLine(ex.Message);
        }
        return false;
    }

    public bool Set(string key, string code, string value, double expireSec = 0)
    {
        try
        {
            bool success = false;
            TimeSpan expireSecTimeSpan;
            if (expireSec > 0)
            {
                expireSecTimeSpan = TimeSpan.FromSeconds(expireSec);
            }
            else
            {
                expireSecTimeSpan = TimeSpan.FromMinutes(defaultExpireMinutes);
            }

            if (db.HashExists(key, code))
            {
                success = true;
            }
            else if (db.HashSet(key, code, value))
            {
                success = db.KeyExpire(key, expireSecTimeSpan);
            }

            if (!success)
            {
                RespCode = Core.Code.VerificationCodeSetError;
            }

            return success;
        }
        catch (System.Exception ex)
        {
            RespCode = Core.Code.DatabaseError;
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}