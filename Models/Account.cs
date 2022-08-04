using System;
using LinqToDB.Mapping;

namespace HttpDataServer.Models;

[Table(Name = nameof(Account))]
public class Account
{
    [PrimaryKey, NotNull]
    public long UID { get; set; }

    [Column, Nullable]
    public string NickName { get; set; }

    [Column, Nullable]
    public string AccountName { get; set; }

    [Column, Nullable]
    public string Password { get; set; }

    [Column, Nullable]
    public string Phone { get; set; }

    [Column, Nullable]
    public string Email { get; set; }

    [Column, Nullable]
    public bool? IsPhoneVerified { get; set; }

    [Column, Nullable]
    public bool? IsEmailVerified { get; set; }
}