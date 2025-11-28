using ITAssetKeeper.Models.Enums;

namespace ITAssetKeeper.Models.ViewModels.DeviceHistory;

// 履歴情報の差分表示用DTO
public class DeviceHistoryFieldDiffDto
{
    public string FieldName { get; set; }           // 内部的な項目名
    public string DisplayFieldName { get; set; }    // ↑の表示用の項目名
    public string? BeforeValue { get; set; }        // 変更前の値
    public string? AfterValue { get; set; }         // 変更後の値
    public bool IsChanged { get; set; }             // 差分有無

    // --- ステータス専用のバッジ用クラス名 ---
    public string StatusClass
    {
        get
        {
            // Status 以外ならバッジを使わない
            if (FieldName != nameof(DeviceColumns.Status))
                return "";

            return AfterValue switch
            {
                "稼働中" => "badge-active",
                "予備" => "badge-spare",
                "故障" => "badge-broken",
                "廃棄予定" => "badge-retiring",
                "廃棄済" => "badge-retired",
                _ => ""
            };
        }
    }
}
