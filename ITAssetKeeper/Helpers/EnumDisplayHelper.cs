using ITAssetKeeper.Constants;
using ITAssetKeeper.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Helpers;

public class EnumDisplayHelper
{
    // DB保存値(Enum or 特殊値)を表示用の文字列に変換
    public static string ResolveDisplayName<TEnum>(string value)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "-";
        }

        // Enum に該当する場合、Display 名に変換
        if (Enum.TryParse<TEnum>(value, true, out var enumValue))
        {
            return GetDisplayName(enumValue);
        }

        // 特殊値辞書に登録されている場合
        if (SpecialDisplayDictionary.VALUE_MAP.TryGetValue(value, out string mapped))
        {
            return mapped;
        }

        // どちらにも該当しない場合は値をそのまま返す
        return value;
    }


    // Enum → (内部名, 表示名) の Dictionary を生成
    // 例: StatusEnum.Active → { "Active", "稼働中" } のように内部名と表示名をペアにする
    public static Dictionary<string, string> EnumToDictionary<TEnum>(bool special = false, params TEnum[] exclude)
    where TEnum : struct, Enum
    {
        // Enum の全メンバーを取得し、TEnum 型にキャスト
        var allValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        IEnumerable<TEnum> filtered;

        // 除外対象が null または空配列の場合、全メンバーをそのまま利用
        if (exclude == null || exclude.Length == 0)
        {
            filtered = allValues;
        }
        else
        {
            // 除外対象が指定されている場合、そのメンバーを除外したコレクションを作成
            filtered = allValues.Where(x => !exclude.Contains(x));
        }

        // フィルタ済みの Enum 値を Dictionary<string,string> に変換
        // Key: Enum の内部名 (文字列)
        // Value: 表示名 (DisplayAttribute)
        var dict = filtered.ToDictionary(
            e => e.ToString(),
            e => GetDisplayName(e)
        );

        // 特殊辞書値が必要(bool special = true)であれば、それも追加
        if (special)
        {
            foreach (var item in SpecialDisplayDictionary.VALUE_MAP)
            {
                dict.Add(item.Key, item.Value);
            }
        }

        // 作成したdictionaryを返す
        return dict;
    }

    // 引数で受け取った Enum のメンバーから表示名を取得
    // DisplayAttribute があればその Name を返し、なければ内部名を返す
    private static string GetDisplayName<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        // Enum のメンバー情報を取得
        var memberInfo = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();

        // メンバー情報が取れなければ内部名を返す
        if (memberInfo == null)
        {
            return value.ToString();
        }

        // DisplayAttribute を取得
        var displayAttribute = memberInfo
            .GetCustomAttributes(typeof(DisplayAttribute), false)
            .Cast<DisplayAttribute>()
            .FirstOrDefault();

        // DisplayAttribute.Name があればそれを返し、なければ内部名を返す
        if (displayAttribute != null && displayAttribute.Name != null)
        {
            return displayAttribute.Name;
        }
        else
        {
            return value.ToString();
        }
    }

    // フリーワード検索用：
    // フリーワードが含まれる表示名に対応する生値を取得
    // 例: freeText = "稼働" → "Active"
    public static List<string> GetRawOfDisplayNameContainsText<TEnum>(string freeText)
        where TEnum : struct, Enum
    {
        // 引数で渡されたEnum の値:表示名の辞書を取得
        var rawDisplayNamePairs = EnumToDictionary<TEnum>();

        // rawDisplayNamePairs の値で、freeText が含まれるValueのKeyを取得
        var raws = rawDisplayNamePairs
            .Where(x => x.Value.Contains(freeText))
            .Select(x => x.Key)
            .ToList();

        // 引数で渡されたのがDeviceColumnsであれば、特殊値辞書内も探す
        if (typeof(TEnum).Name == nameof(DeviceColumns))
        {
            // 特殊値辞書 の値で、freeText が含まれるValueのKeyを取得
            var spRaws = SpecialDisplayDictionary.VALUE_MAP
                .Where(x => x.Value.Contains(freeText))
                .Select(x => x.Key)
                .ToList();

            // 結果のListを raws に追加
            raws.AddRange(spRaws);
        }

        // 取得した値が入ったListを返す
        return raws;
    }

    // SelectList生成用：
    // ビューで <select> 要素を生成する為、Enum → SelectList に変換
    public static SelectList ToSelectList<TEnum>()
        where TEnum : struct, Enum
    {
        // Enum を Dictionary に変換
        var dict = EnumToDictionary<TEnum>();

        // Dictionary を SelectList に変換して返す
        return new SelectList(dict, "Key", "Value");
    }

    // SelectList生成用：
    // SelectListを指定のビューモデルにセットする
    // setter(Action<SelectList>)を渡し、任意のプロパティに SelectList を設定
    public static void SetEnumSelectList<TEnum>(
        object viewModel,
        Action<SelectList> setter
    )
        where TEnum : struct, Enum
    {
        // Enum を SelectList に変換し、setterでビューモデルに設定
        var selectList = ToSelectList<TEnum>();
        setter(selectList);
    }
}
