using System;
using System.Linq;
using DataServer.Core;
using Serilog;
using StackExchange.Redis;

namespace DataServer.Repository;

public class ValidationCodeRepo
{
    public IDatabase db => Server.DBManager.Redis.DB;

    public ErrorCode ErrCode { get; set; } = ErrorCode.Success;

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
            ErrCode = ErrorCode.DatabaseError;
            Log.Error(ex, ErrCode.GetString());
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
                ErrCode = ErrorCode.VerificationCodeSetError;
            }

            return success;
        }
        catch (System.Exception ex)
        {
            ErrCode = ErrorCode.DatabaseError;
            Log.Error(ex, ErrCode.GetString());
            return false;
        }
    }
}