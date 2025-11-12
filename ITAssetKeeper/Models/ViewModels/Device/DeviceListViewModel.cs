namespace ITAssetKeeper.Models.ViewModels.Device;

public class DeviceListViewModel
{
    // DeviceテーブルにあるデータをDevice型リストで格納
    public List<Entities.Device>? AllDevices { get; set; }

    // ドロップダウンリストで選択された値を格納
    public string? SelectedCategory { get; set; }
    public string? SelectedPurpose { get; set; }
    public string? SelectedLocation { get; set; }
    public string? SelectedStatus { get; set; }

    // まだ途中
}
