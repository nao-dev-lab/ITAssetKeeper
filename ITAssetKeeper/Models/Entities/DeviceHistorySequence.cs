namespace ITAssetKeeper.Models.Entities;

// HistoryId衝突回避用：採番テーブル
public class DeviceHistorySequence
{
    public int Id { get; set; }   // 固定で「1」だけ使う
    public int LastUsedNumber { get; set; }
}
