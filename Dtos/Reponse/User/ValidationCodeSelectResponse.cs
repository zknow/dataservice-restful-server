namespace DataServer.Dtos.Response.User;

public class ValidationCodeSelectResponse : BasicResponse
{
    public int ValidationCode { get; set; }

    public string ExtraValue { get; set; }
}