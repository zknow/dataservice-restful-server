namespace HttpDataServer.Dtos.RespDto;

public class RespDto
{
    private int code;
    public int Code
    {
        get { return code; }
        set
        {
            code = value;
            Message = Core.Code.Message(code);
        }
    }

    public string Message { get; set; }

    public object Data { get; set; }

    public RespDto()
    {
        Code = Core.Code.Success;
    }
}