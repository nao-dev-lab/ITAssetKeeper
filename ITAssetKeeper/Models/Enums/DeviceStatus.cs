namespace ITAssetKeeper.Models.Enums;

// 機器状態用Enum
public enum DeviceStatus
{
    Active = 1,     // 稼働中
    Spare = 2,      // 予備
    Broken = 3,     // 故障
    Retiring = 4,   // 廃棄予定
    Retired = 5,    // 廃棄済
    Other = 99
}
