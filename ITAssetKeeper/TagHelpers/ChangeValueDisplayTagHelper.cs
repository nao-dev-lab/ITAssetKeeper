using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ITAssetKeeper.TagHelpers;

[HtmlTargetElement("change-value")]
public class ChangeValueDisplayTagHelper : TagHelper
{
    // 入力属性 
    public string? Before { get; set; }
    public string? After { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // タグ名を div にして扱う
        output.TagName = "div";

        // 値があればそのまま表示、なければ"-"を表示
        var beforeText = string.IsNullOrWhiteSpace(Before) ? "-" : Before;
        var afterText = string.IsNullOrWhiteSpace(After) ? "-" : After;

        // HTMLコンテンツを設定
        output.Content.SetHtmlContent($@"
            <div class='diff-before diff-value'>{beforeText}</div>
            <div class='diff-after diff-value'>{afterText}</div>
        ");
    }
}
