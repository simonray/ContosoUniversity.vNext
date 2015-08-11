using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Infrastructure.TagHelpers
{
    [TargetElement("pager", Attributes = ActionAttributeName)]
    [TargetElement("pager", Attributes = ControllerAttributeName)]
    [TargetElement("pager", Attributes = PageIndexAttributeName)]
    [TargetElement("pager", Attributes = PageSizeAttributeName)]
    [TargetElement("pager", Attributes = TotalAttributeName)]
    [TargetElement("pager", Attributes = LinksAttributeName)]
    public class PagerTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        protected IUrlHelper UrlHelper { get; set; }

        private const string ActionAttributeName = "asp-action";
        [HtmlAttributeName(ActionAttributeName)]
        public string Action { get; set; }

        private const string ControllerAttributeName = "asp-controller";
        [HtmlAttributeName(ControllerAttributeName)]
        public string Controller { get; set; }

        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";
        [HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
        public IDictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private const string PageIndexAttributeName = "page-index";
        [HtmlAttributeName(PageIndexAttributeName)]
        public int PageIndex { get; set; } = 0;

        private const string PageSizeAttributeName = "page-size";
        [HtmlAttributeName(PageSizeAttributeName)]
        public int PageSize { get; set; } = 10;

        private const string TotalAttributeName = "total";
        [HtmlAttributeName(TotalAttributeName)]
        public int Total { get; set; } = 0;

        private const string LinksAttributeName = "links";
        [HtmlAttributeName(LinksAttributeName)]
        public int NumberOfLinks { get; set; } = 5;

        public PagerTagHelper(IUrlHelper urlHelper)
        {
            UrlHelper = urlHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var routeValues = RouteValues.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value, StringComparer.OrdinalIgnoreCase);
            var htmlAttributes = new Dictionary<string, object>();

            output.TagName = null;
            output.Content.SetContent(CreatePager(routeValues, PageIndex, Total, PageSize, NumberOfLinks, htmlAttributes));

            await base.ProcessAsync(context, output);
        }

        private string CreatePager(IDictionary<string, object> routeValues, int pageIndex, int totalItems, int pageSize = 10, int numberOfLinks = 5, IDictionary<string, object> htmlAttributes = null)
        {
            if (totalItems <= 0)
                return string.Empty;

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var lastPageNumber = (int)Math.Ceiling((double)pageIndex / numberOfLinks) * numberOfLinks;
            var firstPageNumber = lastPageNumber - (numberOfLinks - 1);
            var hasPreviousPage = pageIndex > 1;
            var hasNextPage = pageIndex < totalPages;
            if (lastPageNumber > totalPages)
            {
                lastPageNumber = totalPages;
            }
            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");
            ul.InnerHtml += AddLink(1, routeValues, false, pageIndex == 1, "<<", "First Page", htmlAttributes);
            ul.InnerHtml += AddLink(pageIndex - 1, routeValues, false, !hasPreviousPage, "<", "Previous Page", htmlAttributes);
            for (int i = firstPageNumber; i <= lastPageNumber; i++)
            {
                ul.InnerHtml += AddLink(i, routeValues, i == pageIndex, false, i.ToString(), i.ToString(), htmlAttributes);
            }
            ul.InnerHtml += AddLink(pageIndex + 1, routeValues, false, !hasNextPage, ">", "Next Page", htmlAttributes);
            ul.InnerHtml += AddLink(totalPages, routeValues, false, pageIndex == totalPages, ">>", "Last Page", htmlAttributes);
            return ul.ToString();
        }

        private TagBuilder AddLink(int index, IDictionary<string, object> routeValues, bool active, bool disabled, string linkText, string tooltip, IDictionary<string, object> htmlAttributes)
        {
            var li = new TagBuilder("li");

            li.MergeAttribute("title", tooltip);
            if (active)
                li.AddCssClass("active");

            TagBuilder anchor = new TagBuilder("a");
            anchor.InnerHtml += linkText;
            anchor.MergeAttributes(htmlAttributes);

            if (disabled)
                li.AddCssClass("disabled");
            else
            {
                routeValues["page"] = index;
                anchor.Attributes.Add("href", UrlHelper.Action(Action, Controller, routeValues));
            }

            li.InnerHtml += anchor;
            return li;
        }
    }
}
