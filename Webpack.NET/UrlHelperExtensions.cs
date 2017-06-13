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
		/// <returns>
		/// The URL of the webpack asset.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">urlHelper</exception>
		public static string WebpackAsset(this UrlHelper urlHelper, string assetName = "main", string assetType = "js")
		{
			if (urlHelper == null) throw new ArgumentNullException(nameof(urlHelper));

			var webpack = urlHelper.RequestContext.HttpContext.Application.GetWebpack();
			return urlHelper.Content(webpack.GetAssetUrl(assetName, assetType));
		}

		/// <summary>
		/// Gets the absolute URL of the webpack asset.
		/// </summary>
		/// <param name="urlHelper">The URL helper.</param>
		/// <param name="assetName">Name of the asset.</param>
		/// <param name="assetType">Type of the asset.</param>
		/// <returns>
		/// The absolute URL of the webpack asset.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">urlHelper</exception>
		public static string AbsoluteWebpackAsset(this UrlHelper urlHelper, string assetName = "main", string assetType = "js")
		{
			if (urlHelper == null) throw new ArgumentNullException(nameof(urlHelper));

			var asset = urlHelper.WebpackAsset(assetName, assetType);
			return new Uri(urlHelper.RequestContext.HttpContext.Request.Url, asset).AbsoluteUri;
		}
	}
}
