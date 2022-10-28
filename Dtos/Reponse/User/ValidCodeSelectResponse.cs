namespace DataServer.Dtos.Response.User;

public class ValidCodeSelectResponse : BasicResponse
{
    public int ValidationCode { get; set; }

    public string ExtraValue { get; set; }
}