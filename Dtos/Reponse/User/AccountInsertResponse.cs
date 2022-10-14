using System;
using System.ComponentModel.DataAnnotations;
using DataServer.Dtos.Response;
using DataServer.Dtos.Sql;

//https://ithelp.ithome.com.tw/articles/10194337
namespace DataServer.Dtos.Request.User;

public class AccountInsertResponse : BasicResponse
{
    public long UID { get; set; }
}