namespace ITAssetKeeper.Constants;

// Identity用定数クラス
public class ApplicationIdentityConstants
{
    public static readonly int PasswordValidDays = 42;
    public static readonly string DefaultName = "admin";
    public static readonly string DefaultEmail = "admin@example.com";
    public static readonly string DefaultPassword = "Admin@123";
    public enum Roles { Admin, Editor, Viewer } 
}
