using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Webpack.NET
{
    /// <summary>
    /// Extension methods for <see cref="HtmlHelper"/>.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Gets the stylesheet tag for the webpack asset.
        /// </summary>
        /// <param name="htmlHelper">The URL helper.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="required">If set to <c>true</c> throws an <see cref="AssetNotFoundException" /> when the asset could not be found; otherwise, returns <c>null</c>.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns>
        /// The stylesheet tag of the webpack asset.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">htmlHelper</exception>
        public static IHtmlString WebpackStyleSheet(this HtmlHelper htmlHelper, string assetName = "main", string assetType = "css", bool required = true, object htmlAttributes = null)
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));

            // Get asset URL
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var assetUrl = urlHelper.WebpackAsset(assetName, assetType, required);

            // If not required, return nothing if not found
            if (assetUrl == null) return null;

            // Create tag builder
            var builder = new TagBuilder("link");

            // Add attributes
            builder.MergeAttribute("rel", "stylesheet");
            builder.MergeAttribute("href", assetUrl);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            // Render tag
            return htmlHelper.Raw(builder.ToString(TagRenderMode.SelfClosing));
        }

        /// <summary>
        /// Gets the script tag for the webpack asset.
        /// </summary>
        /// <param name="htmlHelper">The URL helper.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="required">If set to <c>true</c> throws an <see cref="AssetNotFoundException" /> when the asset could not be found; otherwise, returns <c>null</c>.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns>
        /// The script tag of the webpack asset.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">htmlHelper</exception>
        public static IHtmlString WebpackScript(this HtmlHelper htmlHelper, string assetName = "main", string assetType = "js", bool required = true, object htmlAttributes = null)
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));

            // Get asset URL
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var assetUrl = urlHelper.WebpackAsset(assetName, assetType, required);

            // If not required, return nothing if not found
            if (assetUrl == null) return null;

            // Create tag builder
            var builder = new TagBuilder("script");

            // Add attributes
            builder.MergeAttribute("src", assetUrl);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            // Render tag
            return htmlHelper.Raw(builder.ToString());
        }
    }
}
