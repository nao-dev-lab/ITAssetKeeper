using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.Enums;

public enum DeviceHistoryColumns
{
    [Display(Name = "履歴ID")]
    HistoryId,

    [Display(Name = "更新種別")]
    ChangeType,

    [Display(Name = "更新者")]
    UpdatedBy,

    [Display(Name = "更新日時")]
    UpdatedAt,

    [Display(Name = "機器管理ID")]
    ManagementIdAtHistory,

    [Display(Name = "種別")]
    CategoryAtHistory,

    [Display(Name = "用途")]
    PurposeAtHistory,

    [Display(Name = "型番(モデル)")]
    ModelNumberAtHistory,

    [Display(Name = "製造番号(シリアル)")]
    SerialNumberAtHistory,

    [Display(Name = "ホスト名")]
    HostNameAtHistory,

    [Display(Name = "設置場所")]
    LocationAtHistory,

    [Display(Name = "使用者")]
    UserNameAtHistory,

    [Display(Name = "状態")]
    StatusAtHistory,

    [Display(Name = "購入日")]
    PurchaseDateAtHistory,

    [Display(Name = "登録日")]
    CreatedAtHistory,

    [Display(Name = "削除日")]
    DeletedAt,

    [Display(Name = "削除者")]
    DeletedBy
}
