using DataServer.Dtos.Sql;

namespace DataServer.Dtos.Response.User;

public class AccountSelectResponse : BasicResponse
{
    public Account Data { get; set; }
}