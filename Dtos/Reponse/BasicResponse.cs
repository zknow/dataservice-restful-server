using DataServer.Core;

namespace DataServer.Dtos.Response;

//https://ithelp.ithome.com.tw/articles/10194337
public class BasicResponse
{
    public ErrorCode ErrorCode { get; set; } = ErrorCode.ParametereError;

    public string Message => ErrorCode.GetString();
}