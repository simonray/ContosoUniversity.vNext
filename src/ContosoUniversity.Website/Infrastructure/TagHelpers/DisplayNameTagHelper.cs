using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using System;
using System.Threading.Tasks;

namespace ContosoUniversity.Infrastructure.TagHelpers
{
    [TargetElement("display-name", Attributes = ForAttributeName)]
    public class DisplayNameTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
        [HtmlAttributeNotBound]
        protected IHtmlHelper HtmlHelper { get; set; }

        private const string ForAttributeName = "asp-for";
        public ModelExpression AspFor { get; set; }

        public DisplayNameTagHelper(IHtmlHelper htmlHelper)
        {
            HtmlHelper = htmlHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            (HtmlHelper as ICanHasViewContext)?.Contextualize(ViewContext);
            output.TagName = null;
            output.Content.SetContent(HtmlHelper.DisplayName(AspFor.Metadata.PropertyName).SplitCamelCase());
            output.Content.Append(await context.GetChildContentAsync());
        }
    }
}