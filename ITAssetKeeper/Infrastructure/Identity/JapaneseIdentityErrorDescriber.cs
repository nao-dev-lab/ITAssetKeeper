using Microsoft.AspNetCore.Identity;

namespace ITAssetKeeper.Infrastructure.Identity;

// エラー文を日本語に変換するクラス
public class JapaneseIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError() { return new IdentityError { Code = nameof(DefaultError), Description = $"不明なエラーが発生しました。" }; }
    public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "データが他のプロセスで変更されたため、更新できませんでした。" }; }
    public override IdentityError PasswordMismatch() { return new IdentityError { Code = nameof(PasswordMismatch), Description = "パスワードが間違っています。" }; }
    public override IdentityError InvalidToken() { return new IdentityError { Code = nameof(InvalidToken), Description = "無効なトークンです。" }; }
    public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "このログイン名を持つユーザーは既に存在します。" }; }
    public override IdentityError InvalidUserName(string userName) { return new IdentityError { Code = nameof(InvalidUserName), Description = $"ユーザー名 '{userName}' は無効です。英数字のみ使用できます。" }; }
    public override IdentityError InvalidEmail(string email) { return new IdentityError { Code = nameof(InvalidEmail), Description = $"メールアドレス '{email}' は無効です。" }; }
    public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = nameof(DuplicateUserName), Description = $"ユーザー名 '{userName}' は既に使用されています。" }; }
    public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = nameof(DuplicateEmail), Description = $"メールアドレス '{email}' は既に使用されています。" }; }
    public override IdentityError InvalidRoleName(string role) { return new IdentityError { Code = nameof(InvalidRoleName), Description = $"ロール名 '{role}' は無効です。" }; }
    public override IdentityError DuplicateRoleName(string role) { return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"ロール名 '{role}' は既に使用されています。" }; }
    public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "ユーザーは既にパスワードを設定済みです。" }; }
    public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "このユーザーに対してロックアウトは有効化されていません。" }; }
    public override IdentityError UserAlreadyInRole(string role) { return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"ユーザーは既にロール '{role}' に属しています。" }; }
    public override IdentityError UserNotInRole(string role) { return new IdentityError { Code = nameof(UserNotInRole), Description = $"ユーザーはロール '{role}' に属していません。" }; }
    public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = nameof(PasswordTooShort), Description = $"パスワードは少なくとも {length} 文字以上である必要があります。" }; }
    public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "パスワードには少なくとも1つの記号 (英数字以外の文字) を含める必要があります。" }; }
    public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "パスワードには少なくとも1つの数字 ('0'～'9') を含める必要があります。" }; }
    public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "パスワードには少なくとも1つの小文字 ('a'～'z') が含まれている必要があります。" }; }
    public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "パスワードには少なくとも1つ以上の大文字 ('A'～'Z') を含める必要があります。" }; }
    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) { return new IdentityError { Code = nameof(PasswordRequiresUniqueChars), Description = $"パスワードは少なくとも {uniqueChars} 種類の異なる文字を使用する必要があります。" }; }
    public override IdentityError RecoveryCodeRedemptionFailed(){ return new IdentityError { Code = nameof(RecoveryCodeRedemptionFailed), Description = "リカバリーコードの使用に失敗しました。" }; }
    //public override IdentityError DuplicateLogin(string login) { return new IdentityError {Code = nameof(DuplicateLogin), Description = $"ログイン '{login}' は既に登録されています。" }; }
}
