using System.ComponentModel.DataAnnotations;

namespace DataServer.Dtos.Request.User;

public class AccountUpdateRequest
{
    public string NickName { get; set; }

    public string Password { get; set; }

    [Phone]
    public string Phone { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public bool? IsPhoneVerified { get; set; }

    public bool? IsEmailVerified { get; set; }
}