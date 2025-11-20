namespace ITAssetKeeper.Constants;

// 特殊値変換テーブル
// Enum では表現できない履歴専用の文字列を変換
public class SpecialDisplayDictionary
{
    public static readonly Dictionary<string, string> VALUE_MAP =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "Deleted", "削除" },
                { "Created", "新規登録" },
                // 今後追加する場合はここに足す
            };
}
