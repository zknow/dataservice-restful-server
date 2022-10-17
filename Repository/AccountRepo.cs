using System;
using System.Linq;
using DataServer.Core;
using DataServer.Database;
using DataServer.Dtos.Request.User;
using DataServer.Dtos.Sql;
using LinqToDB;
using Serilog;
using StackExchange.Redis;

namespace DataServer.Repository;

public class AccountRepo
{
    public MsSqlEngine db => DBManager.Instance.Sql;
    public IDatabase cache => DBManager.Instance.Redis.DB;

    public ErrorCode ErrCode { get; set; } = ErrorCode.Success;

    public IQueryable<Account> GetAll()
    {
        try
        {
            return db.Accounts.Select(x => x);
        }
        catch (System.Exception ex)
        {
            ErrCode = ErrorCode.DatabaseError;
            Log.Error(ex, ErrCode.GetString());
            return null;
        }
    }

    public Account Get(long uid)
    {
        try
        {
            var account = db.Accounts.FirstOrDefault(x => x.UID == uid);
            if (account == null)
            {
                ErrCode = ErrorCode.AccountNotFount;
            }
            return account;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, ErrorCode.DatabaseError.GetString());
            return null;
        }
    }

    public bool Insert(Account player, Device device)
    {
        try
        {
            using (var tran = db.BeginTransaction())
            {
                if (db.Insert(player) > 0)
                {
                    if (db.Insert(device) > 0)
                    {
                        tran.Commit();
                        return true;
                    }
                }
                ErrCode = ErrorCode.AccountCreateFailed;
                return false;
            }
        }
        catch (System.Exception ex)
        {
            ErrCode = ErrorCode.DatabaseError;
            Log.Error(ex, ErrCode.GetString());
            return false;
        }
    }

    public bool Update(long uid, AccountUpdateRequest updateData)
    {
        try
        {
            var selector = db.Accounts.Where(p => p.UID == uid).FirstOrDefault();
            if (selector == null)
            {
                ErrCode = ErrorCode.AccountNotFount;
                return false;
            }

            selector.NickName = string.IsNullOrEmpty(updateData.NickName) ? selector.NickName : updateData.NickName;
            selector.Password = string.IsNullOrEmpty(updateData.Password) ? selector.Password : updateData.Password;
            selector.Phone = string.IsNullOrEmpty(updateData.Phone) ? selector.Phone : updateData.Phone;
            selector.Email = string.IsNullOrEmpty(updateData.Email) ? selector.Email : updateData.Email;
            selector.IsPhoneVerified = updateData.IsPhoneVerified == null ? selector.IsPhoneVerified : updateData.IsPhoneVerified;
            selector.IsEmailVerified = updateData.IsEmailVerified == null ? selector.IsEmailVerified : updateData.IsEmailVerified;

            if (db.Update(selector) <= 0)
            {
                ErrCode = ErrorCode.AccountUpdateFailed;
                return false;
            }
            return true;
        }
        catch (System.Exception ex)
        {
            ErrCode = ErrorCode.DatabaseError;
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public bool UpdatePassword(long uid, string password)
    {
        try
        {
            var selector = db.Accounts.Where(p => p.UID == uid);
            if (selector.Count() == 0)
            {
                ErrCode = ErrorCode.AccountNotFount;
                return false;
            }
            var updatable = selector.Set(p => p.Password, password);
            return updatable.Update() > 0;
        }
        catch (System.Exception ex)
        {
            ErrCode = ErrorCode.DatabaseError;
            Log.Error(ex, ErrCode.GetString());
            return false;
        }
    }

    public bool UpdateNickName(long uid, string nickName)
    {
        try
        {
            var selector = db.Accounts.Where(p => p.UID == uid);
            if (selector.Count() == 0)
            {
                ErrCode = ErrorCode.AccountNotFount;
                return false;
            }
            var updatable = selector.Set(p => p.NickName, nickName);
            return updatable.Update() > 0;
        }
        catch (System.Exception ex)
        {
            ErrCode = ErrorCode.DatabaseError;
            Log.Error(ex, ErrCode.GetString());
            return false;
        }
    }

    public bool UpdateContact(long uid, string phone, string email)
    {
        try
        {
            var selector = db.Accounts.Where(p => p.UID == uid);
            if (selector.Count() == 0)
            {
                ErrCode = ErrorCode.AccountNotFount;
                return false;
            }
            if (!string.IsNullOrWhiteSpace(phone))
            {
                var updatable = selector.Set(p => p.Phone, phone);
                if (updatable.Update() <= 0)
                {
                    ErrCode = ErrorCode.AccountUpdateFailed;
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                var updatable = selector.Set(p => p.Email, email);
                if (updatable.Update() <= 0)
                {
                    ErrCode = ErrorCode.AccountUpdateFailed;
                    return false;
                }
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

    public bool UpdateContactVarified(long uid, bool? isPhoneVarified, bool? isEmailVarified)
    {
        try
        {
            var selector = db.Accounts.Where(p => p.UID == uid);
            if (selector.Count() == 0)
            {
                ErrCode = ErrorCode.AccountNotFount;
                return false;
            }
            if (isPhoneVarified != null)
            {
                var updatable = selector.Set(p => p.IsPhoneVerified, isPhoneVarified);
                if (updatable.Update() <= 0)
                {
                    ErrCode = ErrorCode.AccountUpdateFailed;
                    return false;
                }
            }
            if (isEmailVarified != null)
            {
                var updatable = selector.Set(p => p.IsEmailVerified, isEmailVarified);
                if (updatable.Update() <= 0)
                {
                    ErrCode = ErrorCode.AccountUpdateFailed;
                    return false;
                }
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

    public bool TryGetUserSN(out long sn)
    {
        try
        {
            sn = cache.HashIncrement("UserData", "SN");
            return true;
        }
        catch (System.Exception ex)
        {
            sn = -1;
            ErrCode = ErrorCode.DatabaseError;
            Log.Error(ex, ErrCode.GetString());
            return false;
        }
    }
}