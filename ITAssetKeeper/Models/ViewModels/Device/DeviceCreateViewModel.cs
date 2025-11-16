using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.ViewModels.Device;


// Create(機器登録)のビューモデル
public class DeviceCreateViewModel
{
    // ---ドロップダウン選択用 ---
    [Required(ErrorMessage = "項目の選択が必要です")]
    public string SelectedCategory { get; set; }

    [Required(ErrorMessage = "項目の選択が必要です")]
    public string SelectedPurpose { get; set; }

    [Required(ErrorMessage = "項目の選択が必要です")]
    public string SelectedStatus { get; set; }

    // --- 入力(表示)項目 ---
    [Display(Name = "機器管理ID")]
    public string? ManagementId { get; set; }

    [Display(Name = "種別")]
    public SelectList? CategoryItems { get; set; }

    [Display(Name = "用途")]
    public SelectList? PurposeItems { get; set; }

    [Required(ErrorMessage = "入力が必要です")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "{0} は{2}から{1}文字の範囲で入力してください")]
    [RegularExpression(@"^[a-zA-Z0-9\-._/]+$", ErrorMessage = "半角英数字と - . _ / のみ使用できます")]
    [Display(Name = "型番(モデル)")]
    public string ModelNumber { get; set; }

    [Required(ErrorMessage = "入力が必要です")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "{0} は{2}から{1}文字の範囲で入力してください")]
    [RegularExpression(@"^[a-zA-Z0-9\-._/]+$", ErrorMessage = "半角英数字と - . _ / のみ使用できます")]
    [Display(Name = "製造番号(シリアル)")]
    public string SerialNumber { get; set; }

    [Display(Name = "ホスト名")]
    [RegularExpression(@"^[a-zA-Z0-9\-._]+$", ErrorMessage = "半角英数字と - . _ のみ使用できます")]
    public string? HostName { get; set; }

    [Required(ErrorMessage = "入力が必要です")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "{0} は{2}から{1}文字の範囲で入力してください")]
    [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "半角英数字と - のみ使用できます")]
    [Display(Name = "設置場所")]
    public string Location { get; set; }

    [Display(Name = "使用者")]
    [RegularExpression(@"^[a-zA-Z0-9\-._]+$", ErrorMessage = "半角英数字と - . _ のみ使用できます")]
    public string? UserName { get; set; }

    [Display(Name = "状態")]
    public SelectList? StatusItems { get; set; }

    [Display(Name = "メモ")]
    public string? Memo { get; set; }

    [Required(ErrorMessage = "日付の選択が必要です")]
    [Display(Name = "購入日")]
    public DateTime PurchaseDate { get; set; }
}
