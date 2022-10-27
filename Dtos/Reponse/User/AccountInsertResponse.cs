using DataServer.Dtos.Response;

namespace DataServer.Dtos.Request.User;

public class AccountInsertResponse : BasicResponse
{
    public long UID { get; set; }
}