using System.Collections.Generic;

namespace DataServer.Core;

public static class ErrorMsg
{
    public static string GetString(this ErrorCode code)
    {
        return MsgDict.GetValueOrDefault(code, "未定義的ErrorCode");
    }

    private static Dictionary<ErrorCode, string> MsgDict = new Dictionary<ErrorCode, string>
    {
        { ErrorCode.Success , "OK"},
        { ErrorCode.ParametereError , "參數錯誤"},

        { ErrorCode.AccountNotFount , "查無帳號"},
        { ErrorCode.AccountCreateFailed , "帳號新增失敗"},
        { ErrorCode.AccountUpdateFailed , "帳號更新失敗"},

        { ErrorCode.DeviceNotFound , "查無裝置"},
        { ErrorCode.DeviceUpdateFailed , "裝置資訊更新失敗"},

        { ErrorCode.VerificationCodeNotFound , "驗證碼不存在"},
        { ErrorCode.VerificationCodeSetError , "驗證碼設定失敗"},

        { ErrorCode.DatabaseError , "資料庫錯誤"},
    };
}