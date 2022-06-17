namespace HttpDataServer.Dtos.Account;

public class AccountUpdateDto
{
    public string NickName { get; set; }

    public string Password { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public bool? IsPhoneVerified { get; set; }

    public bool? IsEmailVerified { get; set; }
}