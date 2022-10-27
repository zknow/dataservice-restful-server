using DataServer.Dtos.Response;

namespace DataServer.Dtos.Request.User;

public class AccountUpdateResponse : BasicResponse
{
    public long UID { get; set; }
}