using System;
using System.Linq;
using HttpDataServer.Core;
using HttpDataServer.Database;
using HttpDataServer.Dtos.Account;
using LinqToDB;
using Serilog;

namespace HttpDataServer.Repository;

public class DeviceRepo
{
    public MsSqlEngine db => Server.DBManager.Sql;
    public int RespCode { get; set; } = Code.Success;

    public bool Update(string firebaseCode, DeviceUpdateDto deviceInfo)
    {
        try
        {
            bool success = false;
            var existing = db.Devices.Any(d => d.FirebaseCode == firebaseCode);
            if (!existing)
            {
                var insertDevice = deviceInfo.ToDevice(firebaseCode);
                success = db.Insert(insertDevice) > 0;
            }
            else
            {
                var updateDevice = deviceInfo.ToDevice(firebaseCode);
                success = db.Update(updateDevice) > 0;
            }

            if (!success)
            {
                RespCode = Code.DeviceUpdateFailed;
                return false;
            }

            return true;
        }
        catch (System.Exception ex)
        {
            RespCode = Code.DatabaseError;
            Log.Error(ex, Code.Message(RespCode));
            return false;
        }
    }

    public bool UpdateTime(string firebaseCode, DateTime loginTime)
    {
        try
        {
            var selector = db.Devices.Where(p => p.FirebaseCode == firebaseCode);
            if (selector.Count() == 0)
            {
                RespCode = Code.DeviceNotFound;
                return false;
            }

            var updatable = selector.Set(p => p.LoginTime, loginTime);
            var success = updatable.Update() > 0;
            if (!success)
            {
                RespCode = Code.DeviceUpdateFailed;
            }
            return success;
        }
        catch (System.Exception ex)
        {
            RespCode = Code.DatabaseError;
            Log.Error(ex, Code.Message(RespCode));
            return false;
        }
    }
}