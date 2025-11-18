using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.Enums;

public enum DeviceHistoryColumns
{
    [Display(Name = "履歴ID")]
    HistoryId,

    [Display(Name = "機器管理ID")]
    ManagementId,

    [Display(Name = "更新項目")]
    ChangeField,

    [Display(Name = "更新前の値")]
    BeforeValue,

    [Display(Name = "更新後の値")]
    AfterValue,

    [Display(Name = "更新者")]
    UpdatedBy,

    [Display(Name = "更新日時")]
    UpdatedAt
}
