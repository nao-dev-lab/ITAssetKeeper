using ITAssetKeeper.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.Device;

// Index(機器一覧)のビューモデル
public class DeviceListViewModel
{
    // --- 検索条件 ---
    [Display(Name = "機器管理ID")]
    public string? ManagementId { get; set; }

    [Display(Name = "種別")]
    public string? Category { get; set; }

    [Display(Name = "用途")]
    public string? Purpose { get; set; }

    [Display(Name = "型番(モデル)")]
    public string? ModelNumber { get; set; }

    [Display(Name = "製造番号(シリアル)")]
    public string? SerialNumber { get; set; }

    [Display(Name = "ホスト名")]
    public string? HostName { get; set; }

    [Display(Name = "設置場所")]
    public string? Location { get; set; }

    [Display(Name = "使用者")]
    public string? UserName { get; set; }

    [Display(Name = "状態")]
    public string? Status { get; set; }

    [Display(Name = "購入日 (開始)")]
    public DateTime? PurchaseDateFrom { get; set; }

    [Display(Name = "購入日 (終了)")]
    public DateTime? PurchaseDateTo { get; set; }

    // --- 並び替え・ページング ---
    public string? SortKey { get; set; }
    public SortOrder? SortOrder { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // --- 検索結果 ---
    public List<DeviceDto> Devices { get; set; } = new();

    // --- 検索フォーム用ドロップダウン ---
    public SelectList? CategoryList { get; set; }
    public SelectList? PurposeList { get; set; }
    public SelectList? LocationList { get; set; }
    public SelectList? StatusList { get; set; }
}
