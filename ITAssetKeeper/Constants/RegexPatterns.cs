namespace ITAssetKeeper.Constants;

public class RegexPatterns
{
    public const string MODEL_NUMBER_PATTERN = @"^(?!.* {2})[a-zA-Z0-9._/\- ]+$";
    public const string SERIAL_NUMBER_PATTERN = @"^[a-zA-Z0-9\-._/]+$";
    public const string HOST_USER_NAME_PATTERN = @"^[a-zA-Z0-9\-._]+$";
    public const string LOCATION_PATTERN = @"^[a-zA-Z0-9\-]+$";
}
