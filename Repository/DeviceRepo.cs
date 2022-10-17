using System;
using System.Linq;
using DataServer.Core;
using DataServer.Database;
using DataServer.Dtos.Request.User;
using LinqToDB;
using Serilog;

namespace DataServer.Repository;

public class DeviceRepo
{
    public MsSqlEngine db => DBManager.Instance.Sql;
    public ErrorCode ErrCode { get; set; } = ErrorCode.Success;

    public bool Update(string firebaseCode, DeviceUpdateRequest deviceInfo)
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
                ErrCode = ErrorCode.DeviceUpdateFailed;
                return false;
            }

            return true;
        }
        catch (System.Exception ex)
        {
            ErrCode = ErrorCode.DatabaseError;
            Log.Error(ex, ErrCode.GetString());
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
                ErrCode = ErrorCode.DeviceNotFound;
                return false;
            }

            var updatable = selector.Set(p => p.LoginTime, loginTime);
            var success = updatable.Update() > 0;
            if (!success)
            {
                ErrCode = ErrorCode.DeviceUpdateFailed;
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