using ITAssetKeeper.Models.ViewModels.Device;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.DeviceHistory;

public class DeviceHistoryDetailViewModel
{
    // --- 履歴メタ情報 ---
    [Display(Name = "履歴ID")]
    public string HistoryId { get; set; }

    [Display(Name = "更新項目")]
    public string ChangeType { get; set; } // Created / Updated / Deleted

    [Display(Name = "更新者")]
    public string UpdatedBy { get; set; }

    [Display(Name = "更新日時")]
    public DateTime UpdatedAt { get; set; }

    // --- Snapshot ---
    public DeviceSnapshotDto AfterSnapshot { get; set; }
    public DeviceSnapshotDto? BeforeSnapshot { get; set; } // Updated のときだけ使用

    // --- 差分一覧(Updated のときのみ値が入る) ---
    public List<DeviceHistoryFieldDiffDto> Fields { get; set; } = new();

    // DisplayNameFor のメタデータ用
    public DeviceSnapshotDto DeviceSnapshotDtoHeader { get; } = new();

    // --- 表示専用プロパティ(読み取り専用) --- 
    public string UpdatedAtText => UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss");

    // --- バッジ用プロパティ --- 
    public string ChangeTypeClass =>
        ChangeType switch
        {
            "新規登録" => "badge-created",
            "更新" => "badge-updated",
            "削除" => "badge-deleted",
            _ => ""
        };
}
