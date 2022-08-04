using System;
using HttpDataServer.Models;

namespace HttpDataServer.Dtos.Account;

public class DeviceUpdateDto
{
    public long UID { get; set; }

    public string DeviceName { get; set; }

    public int DeviceType { get; set; }

    public string PublicKey { get; set; }

    public DateTime? LoginTime { get; set; }

    public Device ToDevice(string firebaseCode)
    {
        return new Device
        {
            FirebaseCode = firebaseCode,
            UID = this.UID,
            DeviceName = this.DeviceName,
            DeviceType = this.DeviceType,
            PublicKey = this.PublicKey,
            LoginTime = this.LoginTime,
        };
    }
}