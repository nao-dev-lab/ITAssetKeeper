using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.Device;

// Delete(機器情報削除)のビューモデル
public class DeviceDeleteViewModel
{
    [Display(Name = "機器管理ID")]
    public string ManagementId { get; set; }

    [Display(Name = "種別")]
    public string Category { get; set; }

    [Display(Name = "状態")]
    public string Status { get; set; }
}
