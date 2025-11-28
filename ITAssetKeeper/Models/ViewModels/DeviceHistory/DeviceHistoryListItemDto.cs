using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.DeviceHistory;

// Index一覧表示用 DTO
public class DeviceHistoryListItemDto
{

    public int Id { get; set; }

    [Display(Name = "履歴ID")]
    public string HistoryId { get; set; }

    [Display(Name = "機器管理ID")]
    public string ManagementId { get; set; }

    [Display(Name = "更新項目")]
    public string ChangeType { get; set; }  // (Created / Updated / Deleted)

    [Display(Name = "更新者")]
    public string UpdatedBy { get; set; }

    [Display(Name = "更新日")]
    public DateTime UpdatedAt { get; set; }

    [Display(Name = "種別")]
    public string CategoryAtHistory { get; set; }

    [Display(Name = "用途")]
    public string PurposeAtHistory { get; set; }

    [Display(Name = "型番(モデル)")]
    public string ModelNumberAtHistory { get; set; }

    [Display(Name = "製造番号(シリアル)")]
    public string SerialNumberAtHistory { get; set; }

    [Display(Name = "ホスト名")]
    public string HostNameAtHistory { get; set; }

    [Display(Name = "設置場所")]
    public string LocationAtHistory { get; set; }

    [Display(Name = "使用者")]
    public string UserNameAtHistory { get; set; }

    [Display(Name = "状態")]
    public string StatusAtHistory { get; set; }

    [Display(Name = "メモ")]
    public string MemoAtHistory { get; set; }

    [Display(Name = "購入日")]
    public DateTime PurchaseDateAtHistory { get; set; }

    [Display(Name = "登録日")]
    public DateTime CreatedAtHistory { get; set; }


    // 表示専用プロパティ(読み取り専用)
    public string UpdatedAtText => UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss");
    public string PurchaseDateAtHistoryText => PurchaseDateAtHistory.ToString("yyyy/MM/dd");

    // バッジ用プロパティ
    public string ChangeTypeClass =>
        ChangeType switch
        {
            "新規登録" => "badge-created",
            "更新" => "badge-updated",
            "削除" => "badge-deleted",
            _ => ""
        };

    public string StatusClass =>
        StatusAtHistory switch
        {
            "稼働中" => "badge-active",
            "予備" => "badge-spare",
            "故障" => "badge-broken",
            "廃棄予定" => "badge-retiring",
            "廃棄済" => "badge-retired",
            _ => ""
        };
}
