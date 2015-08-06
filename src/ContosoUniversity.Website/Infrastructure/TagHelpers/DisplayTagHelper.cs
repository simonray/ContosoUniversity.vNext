using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using System.Threading.Tasks;

namespace ContosoUniversity.Infrastructure.TagHelpers
{
    [TargetElement("display", Attributes = ForAttributeName)]
    public class DisplayTagHelper : TagHelper
    {
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected IHtmlHelper HtmlHelper { get; set; }

        private const string ForAttributeName = "asp-for";
        public ModelExpression AspFor { get; set; }

        public DisplayTagHelper(IHtmlHelper htmlHelper)
        {
            HtmlHelper = htmlHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            (HtmlHelper as ICanHasViewContext)?.Contextualize(ViewContext);
            output.TagName = null;

            if (AspFor.Metadata?.TemplateHint != null)
                output.Content.SetContent(
                    HtmlHelper.Partial($"DisplayTemplates/{AspFor.Metadata.TemplateHint}", AspFor.Model).ToString());
            else
                output.Content.SetContent(
                    string.Format(System.Globalization.CultureInfo.CurrentCulture,
                        AspFor.Metadata?.DisplayFormatString ?? "{0}", AspFor.Model));

            output.Content.Append(await context.GetChildContentAsync());
        }
    }
}
