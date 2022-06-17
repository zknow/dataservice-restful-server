namespace HttpDataServer.Core;

public partial class Code
{
    // System
    public const int Success = 0;
    public const int Error = 1;

    // General
    public const int ParametereError = 100;
    public const int UidError = 101;

    // Account
    public const int AccountNotFount = 1000;
    public const int AccountUpdateFailed = 1001;

    // Device
    public const int DeviceNotFound = 2000;
    public const int DeviceUpdateFailed = 2001;

    // ValidationCode
    public const int VerificationCodeNotFound = 3000;
    public const int VerificationCodeSetError = 3001;

    public const int DatabaseError = 5000;
}