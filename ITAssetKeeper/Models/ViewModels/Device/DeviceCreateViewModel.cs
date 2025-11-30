using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using ITAssetKeeper.Constants;

namespace ITAssetKeeper.Models.ViewModels.Device;


// Create(機器登録)のビューモデル
public class DeviceCreateViewModel
{
    // ---ドロップダウン選択用 ---
    [Required(ErrorMessage = ValidationMessages.REQUIRED_DROPDOWN)]
    public string SelectedCategory { get; set; }

    [Required(ErrorMessage = ValidationMessages.REQUIRED_DROPDOWN)]
    public string SelectedPurpose { get; set; }

    [Required(ErrorMessage = ValidationMessages.REQUIRED_DROPDOWN)]
    public string SelectedStatus { get; set; }

    // --- 入力(表示)項目 ---
    [Display(Name = "機器管理ID")]
    public string? ManagementId { get; set; }

    [Display(Name = "種別")]
    public SelectList? CategoryItems { get; set; }

    [Display(Name = "用途")]
    public SelectList? PurposeItems { get; set; }

    [Required(ErrorMessage = ValidationMessages.REQUIRED_FIELD_SHORT)]
    [StringLength(100, MinimumLength = 1, ErrorMessage = ValidationMessages.STRING_LENGTH)]
    [RegularExpression(RegexPatterns.MODEL_NUMBER_PATTERN, ErrorMessage = ValidationMessages.INVALID_MODEL_NUMBER)]
    [Display(Name = "型番(モデル)")]
    public string ModelNumber { get; set; }

    [Required(ErrorMessage = ValidationMessages.REQUIRED_FIELD_SHORT)]
    [StringLength(100, MinimumLength = 1, ErrorMessage = ValidationMessages.STRING_LENGTH)]
    [RegularExpression(RegexPatterns.SERIAL_NUMBER_PATTERN, ErrorMessage = ValidationMessages.INVALID_SERIAL_NUMBER)]
    [Display(Name = "製造番号(シリアル)")]
    public string SerialNumber { get; set; }

    [Display(Name = "ホスト名")]
    [RegularExpression(RegexPatterns.HOST_USER_NAME_PATTERN, ErrorMessage = ValidationMessages.INVALID_HOST_USER_NAME)]
    public string? HostName { get; set; }

    [Required(ErrorMessage = ValidationMessages.REQUIRED_FIELD_SHORT)]
    [StringLength(200, MinimumLength = 1, ErrorMessage = ValidationMessages.STRING_LENGTH)]
    [RegularExpression(RegexPatterns.LOCATION_PATTERN, ErrorMessage = ValidationMessages.INVALID_LOCATION)]
    [Display(Name = "設置場所")]
    public string Location { get; set; }

    [Display(Name = "使用者")]
    [RegularExpression(RegexPatterns.HOST_USER_NAME_PATTERN, ErrorMessage = ValidationMessages.INVALID_HOST_USER_NAME)]
    public string? UserName { get; set; }

    [Display(Name = "状態")]
    public SelectList? StatusItems { get; set; }

    [Display(Name = "メモ")]
    public string? Memo { get; set; }

    [Required(ErrorMessage = ValidationMessages.REQUIRED_DATE)]
    [DataType(DataType.Date)]
    [Display(Name = "購入日")]
    public DateTime? PurchaseDate { get; set; }
}
