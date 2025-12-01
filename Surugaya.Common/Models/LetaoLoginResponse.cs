namespace Surugaya.Common.Models;

/// <summary>
/// 樂淘登入 API 回應
/// </summary>
public class LetaoLoginResponse
{
    /// <summary>
    /// 回應代碼
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 回應訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 詳細資料
    /// </summary>
    public LetaoLoginDetails? Details { get; set; }
}

/// <summary>
/// 登入詳細資料
/// </summary>
public class LetaoLoginDetails
{
    /// <summary>
    /// 資料內容
    /// </summary>
    public LetaoLoginData? Data { get; set; }
}

/// <summary>
/// 登入資料
/// </summary>
public class LetaoLoginData
{
    /// <summary>
    /// GUID (Session ID)
    /// </summary>
    public string Guid { get; set; } = string.Empty;

    /// <summary>
    /// GUID 過期時間
    /// </summary>
    public string GuidExpired { get; set; } = string.Empty;
}
