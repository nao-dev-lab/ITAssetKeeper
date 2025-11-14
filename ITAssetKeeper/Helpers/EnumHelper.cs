using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.Helpers;

public class EnumHelper
{
    // Enum → (内部名, 表示名) の Dictionary を生成
    public static Dictionary<string, string> ToDictionary<TEnum>()
        where TEnum : struct, Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .ToDictionary(
                e => e.ToString(),
                e => GetDisplayName(e)
            );
    }

    // Enum → 表示名取得
    private static string GetDisplayName<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        var memberInfo = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();

        if (memberInfo == null)
        {
            return value.ToString();
        }

        var displayAttribute = memberInfo
            .GetCustomAttributes(typeof(DisplayAttribute), false)
            .Cast<DisplayAttribute>()
            .FirstOrDefault();

        return displayAttribute?.Name ?? value.ToString();
    }

    // Enum → SelectList（最終的にビューで使う形）
    public static SelectList ToSelectList<TEnum>()
        where TEnum : struct, Enum
    {
        var dict = ToDictionary<TEnum>();
        return new SelectList(dict, "Key", "Value");
    }
}
