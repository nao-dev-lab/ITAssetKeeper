using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Models.Entities;

// Identity用のクラス
public class ApplicationUser : IdentityUser
{
    [Required]
    [Display(Name = "パスワード有効期限")]
    public DateTime PasswordExpirationDate { get; set; }

    [Required]
    [Display(Name = "アカウント状態")]
    public bool IsActive { get; set; }

    [Display(Name = "メモ")]
    public string? Memo { get; set; }
}
