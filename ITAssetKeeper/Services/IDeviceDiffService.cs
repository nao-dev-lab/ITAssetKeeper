using ITAssetKeeper.Models.Entities;

namespace ITAssetKeeper.Services;

// DeviceHistory の比較項目取得用
public interface IDeviceDiffService
{
    // 変更項目を取得する
    List<DeviceChange> GetChanges(Device before, Device after);
}

// 結果を保持する為のカスタムクラス
public class DeviceChange
{
    public string FieldName { get; set; }
    public string? BeforeValue { get; set; }
    public string? AfterValue { get; set; }
}
