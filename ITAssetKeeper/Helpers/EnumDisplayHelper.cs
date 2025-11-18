using ITAssetKeeper.Constants;
using ITAssetKeeper.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Helpers;

public class EnumDisplayHelper
{
    // 履歴用：ChangeField を見て「どのEnum/変換を使うか」決める
    public static string ResolveHistoryDisplay(string changeField, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        switch (changeField)
        {
            // ChangeField = Category
            case nameof(DeviceColumns.Category):
                return ResolveDisplayName<DeviceCategory>(value);

            // ChangeField = Purpose
            case nameof(DeviceColumns.Purpose):
                return ResolveDisplayName<DevicePurpose>(value);

            // ChangeField = Status
            case nameof(DeviceColumns.Status):
                // IsDeleted が true の場合、履歴上のStatusを Deleted(削除済)に変換
                return ResolveDisplayName<DeviceStatus>(value);

            // ChangeField = PurchaseDate 
            case nameof(DeviceColumns.PurchaseDate):
                // TryParseでチェックしてからパースする
                if (DateTime.TryParse(value, out var dtPurchase))
                {
                    return dtPurchase.ToString("yyyy/MM/dd");
                }
                // NGの場合そのまま返す
                return value;

            case nameof(DeviceColumns.UpdatedAt):
                // TryParseでチェックしてからパースする
                if (DateTime.TryParse(value, out var dtUpdated))
                {
                    return dtUpdated.ToString("yyyy/MM/dd");
                }
                // NGの場合そのまま返す
                return value;

            default:
                // それ以外(Location, UserName など)は DB の文字列のまま
                return value;
        }
    }

    // DB保存値(Enum or 特殊値)を表示用の文字列に変換
    public static string ResolveDisplayName<TEnum>(string value)
    where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
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
    public static Dictionary<string, string> ToDictionary<TEnum>(params TEnum[] exclude)
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
        return filtered.ToDictionary(
            e => e.ToString(),
            e => GetDisplayName(e)
        );
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

    // ビューで <select> 要素を生成する為、Enum → SelectList に変換
    public static SelectList ToSelectList<TEnum>()
        where TEnum : struct, Enum
    {
        // Enum を Dictionary に変換
        var dict = ToDictionary<TEnum>();

        // Dictionary を SelectList に変換して返す
        return new SelectList(dict, "Key", "Value");
    }

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
