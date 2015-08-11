using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using System.Collections.Generic;

namespace ContosoUniversity.Infrastructure.TagHelpers
{
    /// <summary>
    /// <see cref="ITagHelper"/> implementation targeting &lt;select&gt; elements with an <c>asp-for</c> attribute.
    /// </summary>
    /// <remarks>
    /// Alternative &lt;select&gt; that supports a non-model expression and option labels.
    /// </remarks>
    [TargetElement("select", Attributes = ExpressionAttributeName)]
    public class SelectTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
        [HtmlAttributeNotBound]
        public IHtmlGenerator Generator { get; set; }

        private const string ExpressionAttributeName = "asp-expression";
        [HtmlAttributeName(ExpressionAttributeName)]
        public string AspExpression { get; set; }

        public string AspCurrent { get; set; }
        public string AspOptionLabel { get; set; }
        public IEnumerable<SelectListItem> AspItems { get; set; }

        public SelectTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            var tagBuilder = Generator.GenerateSelect(
                ViewContext,
                null,
                optionLabel: AspOptionLabel,
                expression: AspExpression,
                selectList: AspItems,
                currentValues: new string[] { AspCurrent, },
                allowMultiple: false,
                htmlAttributes: null);

            tagBuilder.AddCssClass(
                context.AllAttributes.ContainsName("class") == true ? 
                context.AllAttributes["class"].Value.ToString() : string.Empty);

            output.Content.SetContent(tagBuilder.ToString());
        }
    }
}
