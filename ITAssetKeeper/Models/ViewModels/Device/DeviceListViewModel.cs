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
    public DeviceColumns SortKeyValue { get; set; } = DeviceColumns.ManagementId;
    public SortOrders SortOrderValue { get; set; } = SortOrders.Asc;

    // --- 検索結果 ---
    public List<DeviceDto> Devices { get; set; } = new();

    // --- 検索条件 ---
    [Display(Name = "機器管理ID")]
    public string? ManagementId { get; set; }

    [Display(Name = "種別")]
    public SelectList? CategoryItems { get; set; }

    [Display(Name = "用途")]
    public SelectList? PurposeItems { get; set; }

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
    public SelectList? StatusItems { get; set; }

    [Display(Name = "購入日 (開始)")]
    [DataType(DataType.Date)]
    public DateTime? PurchaseDateFrom { get; set; }

    [Display(Name = "購入日 (終了)")]
    [DataType(DataType.Date)]
    public DateTime? PurchaseDateTo { get; set; }

    [Display(Name = "購入日")]
    public DateTime? PurchaseDate { get; set; }

    [Display(Name = "更新日 (開始)")]
    [DataType(DataType.Date)]
    public DateTime? UpdatedDateFrom { get; set; }

    [Display(Name = "更新日 (終了)")]
    [DataType(DataType.Date)]
    public DateTime? UpdatedDateTo { get; set; }

    [Display(Name = "更新日")]
    public DateTime? UpdatedAt { get; set; }

    [Display(Name = "登録日 (開始)")]
    [DataType(DataType.Date)]
    public DateTime? CreatedDateFrom { get; set; }

    [Display(Name = "登録日 (終了)")]
    [DataType(DataType.Date)]
    public DateTime? CreatedDateTo { get; set; }

    [Display(Name = "登録日")]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "フリーワード")]
    public string? FreeText { get; set; }

    // 検索フォーム折り畳み判別用
    public bool IsSearchExecuted { get; set; }

    // 検索フォームに項目が入って検索されたかをチェック
    public bool HasAnyFilter =>
    !string.IsNullOrWhiteSpace(ManagementId)
    || !string.IsNullOrWhiteSpace(SelectedCategory)
    || !string.IsNullOrWhiteSpace(SelectedPurpose)
    || !string.IsNullOrWhiteSpace(ModelNumber)
    || !string.IsNullOrWhiteSpace(SerialNumber)
    || !string.IsNullOrWhiteSpace(HostName)
    || !string.IsNullOrWhiteSpace(Location)
    || !string.IsNullOrWhiteSpace(UserName)
    || !string.IsNullOrWhiteSpace(SelectedStatus)
    || PurchaseDateFrom != null
    || PurchaseDateTo != null
    || CreatedDateFrom != null
    || CreatedDateTo != null
    || UpdatedDateFrom != null
    || UpdatedDateTo != null;

    // --- 並び替え ---
    [Display(Name = "並び替え基準")]
    public SelectList? SortKeyList { get; set; }

    [Display(Name = "並び替え")]
    public SelectList? SortOrderList { get; set; }

    // --- ページング ---
    public int PageNumber { get; set; } = 1; // 現在のページ番号
    public int PageSize { get; set; } = 10; // 1ページに表示するサイズ
    public int TotalCount { get; set; }     // トータルページ数

    // --- ページング用計算プロパティ ---
    // 合計ページ数を取得
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    // 現在表示されているページの前にまだ表示するページが存在するか
    public bool HasPreviousPage => PageNumber > 1;
    // 現在表示されているページの後にまだ表示するページが存在するか
    public bool HasNextPage => PageNumber < TotalPages;
    
    // ページ番号リンクの生成
    public IEnumerable<int> PageRange
    {
        get
        {
            // 総ページが 0 または 現在ページが範囲外 → 空を返す
            if (TotalPages <= 0 || PageNumber > TotalPages)
            {
                return Enumerable.Empty<int>();
            }

            int start = Math.Max(1, PageNumber - 2);
            int end = Math.Min(TotalPages, PageNumber + 2);
            return Enumerable.Range(start, end - start + 1);
        }
    }
}
