using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.Device;

// Delete(機器情報削除)のビューモデル
public class DeviceDeleteViewModel
{
    // --- 削除対象識別用 ---
    public int HiddenId { get; set; }

    // --- 確認表示用 ---
    [Display(Name = "機器管理ID")]
    public string ManagementId { get; set; }

    [Display(Name = "種別")]
    public string Category { get; set; }

    [Display(Name = "状態")]
    public string Status { get; set; }

    [Display(Name = "使用者")]
    public string? UserName { get; set; }
}
