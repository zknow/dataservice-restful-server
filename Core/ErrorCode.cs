namespace DataServer.Core;

public enum ErrorCode
{
    // System
    Success = 0,
    ParametereError = 1,

    // Account
    AccountNotFount = 1000,
    AccountCreateFailed = 1001,
    AccountUpdateFailed = 1002,

    // Device
    DeviceNotFound = 2000,
    DeviceUpdateFailed = 2001,

    // ValidationCode
    VerificationCodeNotFound = 3000,
    VerificationCodeSetError = 3001,

    // Database
    DatabaseError = 5000
}