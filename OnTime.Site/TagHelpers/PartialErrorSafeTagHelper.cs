using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace OnTime.Site.TagHelpers;

[HtmlTargetElement("partial-error-safe", TagStructure = TagStructure.WithoutEndTag, Attributes = "name")]
public class PartialErrorSafeTagHelper : TagHelper
{
    private readonly ILogger<PartialErrorSafeTagHelper> logger;
    private readonly IHtmlHelper htmlHelper;

    public PartialErrorSafeTagHelper(ILogger<PartialErrorSafeTagHelper> logger, IHtmlHelper htmlHelper)
    {
        this.logger = logger;
        this.htmlHelper = htmlHelper;
    }

    [HtmlAttributeName("name")]
    private string Name { get; set; } = string.Empty;

    [HtmlAttributeName("model")]
    private object? Model { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        try
        {
            // Contextualize the HtmlHelper so it knows about the current request execution context
            (this.htmlHelper as IViewContextAware)?.Contextualize(this.ViewContext);

            // Completely replace the custom <partial-safe> tag with standard output contents
            output.TagName = null;

            // Attempt to render the partial view into a string writer
            using (var writer = new StringWriter())
            {
                var viewBuffer = await this.htmlHelper.PartialAsync(this.Name, this.Model);
                viewBuffer.WriteTo(writer, HtmlEncoder.Default);

                output.Content.SetHtmlContent(writer.ToString());
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
        }
    }
}