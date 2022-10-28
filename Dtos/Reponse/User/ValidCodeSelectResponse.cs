namespace DataServer.Dtos.Response.User;

public class ValidCodeSelectResponse : BasicResponse
{
    public string ValidationCode { get; set; }

    public string ExtraValue { get; set; }
}