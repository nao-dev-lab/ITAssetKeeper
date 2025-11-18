using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.DeviceHistory;

// DeviceHistoryのビューモデル
public class DeviceHistoryViewModel
{
    // メタデータにアクセスする踏み台
    public DeviceHistoryDto DeviceHistoryDtoHeader { get; } = new();

    // ---ドロップダウン選択用 ---
    public string? SelectedChangeField { get; set; }
    public DeviceHistoryColumns SortKeyValue { get; set; } = DeviceHistoryColumns.HistoryId;
    public SortOrders SortOrderValue { get; set; } = SortOrders.Asc;

    // --- 検索結果 ---
    public List<DeviceHistoryDto> DeviceHistories { get; set; } = new();

    // --- 検索条件 ---
    [Display(Name = "履歴ID")]
    public string? HistoryId { get; set; }

    [Display(Name = "機器管理ID")]
    public string? ManagementId { get; set; }

    [Display(Name = "更新項目")]
    public SelectList? ChangeFieldItems { get; set; }

    [Display(Name = "更新前の値")]
    public string? BeforeValue { get; set; }

    [Display(Name = "更新後の値")]
    public string? AfterValue { get; set; }

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
