using System;
using System.ComponentModel.DataAnnotations;
using HttpDataServer.Models;

//https://ithelp.ithome.com.tw/articles/10194337
namespace HttpDataServer.Dtos.Account;

public class AccountPostDto
{
    [Required]
    public string AccountName { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string NickName { get; set; }

    [Phone]
    public string Phone { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string DeviceName { get; set; }

    [Required]
    public int DeviceType { get; set; }

    [Required]
    public string PublicKey { get; set; }

    [Required]
    public string FirebaseCode { get; set; }

    [Required]
    public DateTime LoginTime { get; set; }

    public Models.Account ToPlayer(long uid)
    {
        return new Models.Account
        {
            UID = uid,
            AccountName = this.AccountName,
            Password = this.Password,
            NickName = this.NickName,
            Phone = this.Phone,
            Email = this.Email,
        };
    }

    public Device ToDevice(long uid)
    {
        return new Device
        {
            UID = uid,
            DeviceName = this.DeviceName,
            DeviceType = this.DeviceType,
            PublicKey = this.PublicKey,
            FirebaseCode = this.FirebaseCode,
            LoginTime = this.LoginTime,
        };
    }
}