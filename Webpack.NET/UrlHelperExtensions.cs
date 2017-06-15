using System;
using System.Web.Mvc;

namespace Webpack.NET
{
    /// <summary>
    /// Extension methods for <see cref="UrlHelper"/>.
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Gets the URL of the webpack asset.
        /// </summary>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="required">If set to <c>true</c> throws an <see cref="AssetNotFoundException" /> when the asset could not be found; otherwise, returns <c>null</c>.</param>
        /// <returns>
        /// The URL of the webpack asset.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">urlHelper</exception>
        public static string WebpackAsset(this UrlHelper urlHelper, string assetName = "main", string assetType = "js", bool required = true)
        {
            if (urlHelper == null) throw new ArgumentNullException(nameof(urlHelper));

            var webpack = urlHelper.RequestContext.HttpContext.Application.GetWebpack();
            return urlHelper.Content(webpack.GetAssetUrl(assetName, assetType, required));
        }

        /// <summary>
        /// Gets the absolute URL of the webpack asset.
        /// </summary>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="required">If set to <c>true</c> throws an <see cref="AssetNotFoundException" /> when the asset could not be found; otherwise, returns <c>null</c>.</param>
        /// <returns>
        /// The absolute URL of the webpack asset.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">urlHelper</exception>
        public static string AbsoluteWebpackAsset(this UrlHelper urlHelper, string assetName = "main", string assetType = "js", bool required = true)
        {
            if (urlHelper == null) throw new ArgumentNullException(nameof(urlHelper));

            var assetUrl = urlHelper.WebpackAsset(assetName, assetType, required);
            return new Uri(urlHelper.RequestContext.HttpContext.Request.Url, assetUrl).AbsoluteUri;
        }
    }
}
