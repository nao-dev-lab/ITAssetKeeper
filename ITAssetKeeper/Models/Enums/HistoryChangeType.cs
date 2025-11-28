using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.Enums;

public enum HistoryChangeType
{
    [Display(Name = "新規登録")]
    Created,

    [Display(Name = "更新")]
    Updated,

    [Display(Name = "削除")]
    Deleted
}
