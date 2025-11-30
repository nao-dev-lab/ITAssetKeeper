using ITAssetKeeper.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.DeviceHistory;

// DeviceHistoryのビューモデル
public class DeviceHistoryListViewModel
{
    // メタデータにアクセスする踏み台
    public DeviceHistoryListItemDto DeviceHistoryListItemDtoHeader { get; } = new();

    // ---ドロップダウン選択用 ---
    public string? SelectedChangeType { get; set; }
    public string? SelectedCategoryAtHistory { get; set; }
    public string? SelectedPurposeAtHistory { get; set; }
    public string? SelectedStatusAtHistory { get; set; }

    public DeviceHistoryColumns SortKeyValue { get; set; } = DeviceHistoryColumns.HistoryId;
    public SortOrders SortOrderValue { get; set; } = SortOrders.Desc;

    // --- 検索結果 ---
    public List<DeviceHistoryListItemDto> Histories { get; set; } = new();

    // --- 検索条件 ---
    [Display(Name = "履歴ID")]
    public string? HistoryId { get; set; }

    [Display(Name = "機器管理ID")]
    public string? ManagementId { get; set; }

    [Display(Name = "更新種別")]
    public SelectList? ChangeTypeItems { get; set; }

    [Display(Name = "更新者")]
    public string? UpdatedBy { get; set; }

    [Display(Name = "更新日 (開始)")]
    [DataType(DataType.Date)]
    public DateTime? UpdatedDateFrom { get; set; }

    [Display(Name = "更新日 (終了)")]
    [DataType(DataType.Date)]
    public DateTime? UpdatedDateTo { get; set; }

    [Display(Name = "更新日時")]
    public DateTime? UpdatedAt { get; set; }

    [Display(Name = "種別")]
    public SelectList? CategoryAtHistoryItems { get; set; }
    
    [Display(Name = "用途")]
    public SelectList? PurposeAtHistoryItems { get; set; }

    [Display(Name = "型番(モデル)")]
    public string? ModelNumberAtHistory { get; set; }

    [Display(Name = "製造番号(シリアル)")]
    public string? SerialNumberAtHistory { get; set; }
    
    [Display(Name = "ホスト名")]
    public string? HostNameAtHistory { get; set; }
    
    [Display(Name = "設置場所")]
    public string? LocationAtHistory { get; set; }

    [Display(Name = "使用者")]
    public string? UserNameAtHistory { get; set; }
    
    [Display(Name = "状態")]
    public SelectList? StatusAtHistoryItems { get; set; }

    [Display(Name = "購入日 (開始)")]
    [DataType(DataType.Date)]
    public DateTime? PurchaseDateFrom { get; set; }

    [Display(Name = "購入日 (終了)")]
    [DataType(DataType.Date)]
    public DateTime? PurchaseDateTo { get; set; }

    [Display(Name = "購入日")]
    public DateTime? PurchaseDateAtHistory { get; set; }

    [Display(Name = "登録日 (開始)")]
    [DataType(DataType.Date)]
    public DateTime? CreatedDateFrom { get; set; }

    [Display(Name = "登録日 (終了)")]
    [DataType(DataType.Date)]
    public DateTime? CreatedDateTo { get; set; }

    [Display(Name = "登録日")]
    public DateTime? CreatedAtHistory { get; set; }

    [Display(Name = "フリーワード")]
    public string? FreeText { get; set; }

    // --- 検索フォーム折り畳み判別用 ---
    public bool IsSearchExecuted { get; set; }

    // --- どちらの検索フォームを使用しているかの判別用 ---
    public string ActiveSearchType { get; set; } = "simple";

    //  --- 検索フォームに項目が入って検索されたかをチェック  ---
    public bool HasAnyFilter =>
        !string.IsNullOrWhiteSpace(HistoryId)
        || !string.IsNullOrWhiteSpace(ManagementId)
        || !string.IsNullOrWhiteSpace(SelectedChangeType)
        || !string.IsNullOrWhiteSpace(SelectedCategoryAtHistory)
        || !string.IsNullOrWhiteSpace(SelectedPurposeAtHistory)
        || !string.IsNullOrWhiteSpace(UpdatedBy)
        || !string.IsNullOrWhiteSpace(ModelNumberAtHistory)
        || !string.IsNullOrWhiteSpace(SerialNumberAtHistory)
        || !string.IsNullOrWhiteSpace(HostNameAtHistory)
        || !string.IsNullOrWhiteSpace(LocationAtHistory)
        || !string.IsNullOrWhiteSpace(UserNameAtHistory)
        || PurchaseDateFrom != null
        || PurchaseDateTo != null
        || CreatedDateFrom != null
        || CreatedDateTo != null
        || UpdatedDateFrom != null
        || UpdatedDateTo != null;

    // --- 並び替え ---
    [Display(Name = "並び替え基準")]
    public SelectList? SortKeyList { get; set; }

    [Display(Name = "並び替え順")]
    public SelectList? SortOrderList { get; set; }

    // --- ページング ---
    public int PageNumber { get; set; } = 1; // 現在のページ番号
    public int PageSize { get; set; } = 15; // 1ページに表示するサイズ
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
