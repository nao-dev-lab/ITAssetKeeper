using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.Device;

// Index(機器一覧)のビューモデル
public class DeviceListViewModel
{
    // メタデータにアクセスする踏み台
    public DeviceDto DeviceDtoHeader { get; } = new();

    // ---ドロップダウン選択用 ---
    public string? SelectedCategory { get; set; }
    public string? SelectedPurpose { get; set; }
    public string? SelectedStatus { get; set; }
    public SortKeyColums SortKeyValue { get; set; } = SortKeyColums.ManagementId;
    public SortOrder SortOrderValue { get; set; } = Enums.SortOrder.Asc;

    // --- 検索条件 ---
    [Display(Name = "機器管理ID")]
    public string? ManagementId { get; set; }

    [Display(Name = "種別")]
    public SelectList? Category { get; set; }

    [Display(Name = "用途")]
    public SelectList? Purpose { get; set; }

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
    public SelectList? Status { get; set; }

    [Display(Name = "購入日 (開始)")]
    public DateTime? PurchaseDateFrom { get; set; }

    [Display(Name = "購入日 (終了)")]
    public DateTime? PurchaseDateTo { get; set; }

    [Display(Name = "購入日")]
    public DateTime? PurchaseDate { get; set; }

    // --- 並び替え・ページング ---
    [Display(Name = "並び替え基準")]
    public SelectList? SortKeyList { get; set; }

    [Display(Name = "並び替え")]
    public SelectList? SortOrderList { get; set; }
    
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // --- 検索結果 ---
    public List<DeviceDto> Devices { get; set; } = new();
}
