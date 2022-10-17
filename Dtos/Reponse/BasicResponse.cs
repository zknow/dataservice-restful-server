using DataServer.Core;

namespace DataServer.Dtos.Response;

public class BasicResponse
{
    public ErrorCode ErrorCode { get; set; } = ErrorCode.ParametereError;

    public string Message => ErrorCode.GetString();
}