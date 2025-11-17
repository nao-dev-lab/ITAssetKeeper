using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.Enums;

public enum SortOrders
{
    [Display(Name = "昇順")]
    Asc,

    [Display(Name = "降順")]
    Desc
}
