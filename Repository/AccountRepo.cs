using System;
using System.Linq;
using HttpDataServer.Core;
using HttpDataServer.Database;
using HttpDataServer.Dtos.Account;
using HttpDataServer.Models;
using LinqToDB;
using Serilog;
using StackExchange.Redis;

namespace HttpDataServer.Repository;

public class AccountRepo
{
    public MsSqlEngine db => Server.DBManager.Sql;
    public IDatabase cache => Server.DBManager.Redis.DB;

    public int RespCode { get; set; } = Code.Success;

    public AccountRepo()
    {
        try
        {
            if (CacheData.UserSN == -1)
            {
                CacheData.UserSN = (long)cache.HashGet("UserData", "SN");
            }
        }
        catch (System.Exception ex)
        {
            CacheData.UserSN = -1;
            RespCode = Code.DatabaseError;
            Log.Error(ex, Code.Message(RespCode));
        }
    }

    public IQueryable<Account> GetAll()
    {
        try
        {
            return db.Accounts.Select(x => x);
        }
        catch (System.Exception ex)
        {
            RespCode = Core.Code.DatabaseError;
            Log.Error(ex, Code.Message(RespCode));
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
                RespCode = Code.AccountNotFount;
            }
            return account;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, Code.Message(Code.DatabaseError));
            return null;
        }
    }

    public bool Insert(Account player, Device device)
    {
        try
        {
            if (CacheData.UserSN == -1)
            {
                RespCode = Code.AccountCreateFailed;
                return false;
            }

            using (var tran = db.BeginTransaction())
            {
                if (db.Insert(player) > 0)
                {
                    if (db.Insert(device) > 0)
                    {
                        if (UserSnIncrement())
                        {
                            tran.Commit();
                            return true;
                        }
                    }
                }
                RespCode = Code.AccountCreateFailed;
                return false;
            }
        }
        catch (System.Exception ex)
        {
            RespCode = Code.DatabaseError;
            Log.Error(ex, Code.Message(RespCode));
            return false;
        }
    }

    public bool Update(long uid, AccountUpdateDto updateData)
    {
        try
        {
            var selector = db.Accounts.Where(p => p.UID == uid).FirstOrDefault();
            if (selector == null)
            {
                RespCode = Code.AccountNotFount;
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
                RespCode = Code.AccountUpdateFailed;
                return false;
            }
            return true;
        }
        catch (System.Exception ex)
        {
            RespCode = Code.DatabaseError;
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
                RespCode = Core.Code.AccountNotFount;
                return false;
            }
            var updatable = selector.Set(p => p.Password, password);
            return updatable.Update() > 0;
        }
        catch (System.Exception ex)
        {
            RespCode = Code.DatabaseError;
            Log.Error(ex, Code.Message(RespCode));
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
                RespCode = Code.AccountNotFount;
                return false;
            }
            var updatable = selector.Set(p => p.NickName, nickName);
            return updatable.Update() > 0;
        }
        catch (System.Exception ex)
        {
            RespCode = Code.DatabaseError;
            Log.Error(ex, Code.Message(RespCode));
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
                RespCode = Core.Code.AccountNotFount;
                return false;
            }
            if (!string.IsNullOrWhiteSpace(phone))
            {
                var updatable = selector.Set(p => p.Phone, phone);
                if (updatable.Update() <= 0)
                {
                    RespCode = Core.Code.AccountUpdateFailed;
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                var updatable = selector.Set(p => p.Email, email);
                if (updatable.Update() <= 0)
                {
                    RespCode = Code.AccountUpdateFailed;
                    return false;
                }
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

    public bool UpdateContactVarified(long uid, bool? isPhoneVarified, bool? isEmailVarified)
    {
        try
        {
            var selector = db.Accounts.Where(p => p.UID == uid);
            if (selector.Count() == 0)
            {
                RespCode = Core.Code.AccountNotFount;
                return false;
            }
            if (isPhoneVarified != null)
            {
                var updatable = selector.Set(p => p.IsPhoneVerified, isPhoneVarified);
                if (updatable.Update() <= 0)
                {
                    RespCode = Code.AccountUpdateFailed;
                    return false;
                }
            }
            if (isEmailVarified != null)
            {
                var updatable = selector.Set(p => p.IsEmailVerified, isEmailVarified);
                if (updatable.Update() <= 0)
                {
                    RespCode = Code.AccountUpdateFailed;
                    return false;
                }
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

    public bool UserSnIncrement()
    {
        try
        {
            CacheData.UserSN = cache.HashIncrement("UserData", "SN");
            return true;
        }
        catch (System.Exception ex)
        {
            RespCode = Code.DatabaseError;
            Log.Error(ex, Code.Message(RespCode));
            return false;
        }
    }
}