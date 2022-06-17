using System.Collections.Generic;

namespace HttpDataServer.Core;

public partial class Code
{
    public static Dictionary<int, string> MessageDict = new Dictionary<int, string>
    {
        { Success , "OK"},
        { Error , "一般錯誤"},
        { ParametereError , "參數錯誤"},
        { UidError , "UID錯誤"},

        { AccountNotFount , "查無帳號"},
        { AccountUpdateFailed , "帳號更新失敗"},

        { DeviceNotFound , "查無裝置"},
        { DeviceUpdateFailed , "裝置資訊更新失敗"},

        { VerificationCodeNotFound , "驗證碼不存在"},
        { VerificationCodeSetError , "驗證碼設定失敗"},

        { DatabaseError , "資料庫錯誤"},
    };

    public static string Message(int code)
    {
        return MessageDict.GetValueOrDefault(code, "未定義的ErrorCode");
    }
}



