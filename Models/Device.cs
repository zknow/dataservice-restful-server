using System;
using LinqToDB.Mapping;

namespace HttpDataServer.Models;

[Table(Name = nameof(Device))]
public class Device
{
    [PrimaryKey, NotNull]
    public string FirebaseCode { get; set; }

    [Column, NotNull]
    public long UID { get; set; }

    [Column, Nullable]
    public string DeviceName { get; set; }

    [Column, Nullable]
    public int DeviceType { get; set; }

    [Column, Nullable]
    public string PublicKey { get; set; }

    [Column, Nullable]
    public DateTime? LoginTime { get; set; }
}