using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using ITAssetKeeper.Models.Enums;

namespace ITAssetKeeper.Models.ViewModels.Device;

// Create(機器登録)・Edit(機器情報編集)・Details(機器情報)共通のビューモデル
public class DeviceManageViewModel
{
    // ---ドロップダウン選択用 ---
    public string? SelectedCategory { get; set; }
    public string? SelectedPurpose { get; set; }
    public string? SelectedStatus { get; set; }

    // --- Create / Edit / Details の判定 ---
    // 保険で明示的にセットしておく
    public ViewMode Mode { get; set; } = ViewMode.Details;

    // --- 入力(表示)項目 ---
    [Display(Name = "機器管理ID")]
    public string? ManagementId { get; set; }

    [Required]
    [Display(Name = "種別")]
    public SelectList? Category { get; set; }

    [Required]
    [Display(Name = "用途")]
    public SelectList? Purpose { get; set; }

    [Required]
    [Display(Name = "型番(モデル)")]
    public string ModelNumber { get; set; }

    [Required]
    [Display(Name = "製造番号(シリアル)")]
    public string SerialNumber { get; set; }

    [Display(Name = "ホスト名")]
    public string? HostName { get; set; }

    [Required]
    [Display(Name = "設置場所")]
    public string? Location { get; set; }

    [Display(Name = "使用者")]
    public string? UserName { get; set; }

    [Required]
    [Display(Name = "状態")]
    public SelectList? Status { get; set; }

    [Display(Name = "メモ")]
    public string? Memo { get; set; }

    [Required]
    [Display(Name = "購入日")]
    public DateTime PurchaseDate { get; set; }
}
