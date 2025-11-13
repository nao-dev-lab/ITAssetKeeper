namespace ITAssetKeeper.Models.Enums;

// 機器種別用Enum
public enum DeviceCategory
{
    Laptop = 1,       // ノートPC
    DesktopPc = 2,    // デスクトップPC
    Switch_L2 = 3,    // L2スイッチ
    Switch_L3 = 4,    // L3スイッチ
    Router_Core = 5,  // 業務用ルーター(Cisco,YAMAHA RTX/NVR,Juniperなど)
    Router_Soho = 6,  // 小規模用ルーター(Buffalo,Elecom,NEC Aterm など)
    Server = 7,       // サーバー
    Firewall = 8,     // ファイアウォール
    Ups = 9,          // UPS
    Other = 99
}
