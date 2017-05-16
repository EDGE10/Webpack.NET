using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Webpack.NET
{
	/// <summary>
	/// Extension methods for <see cref="UrlHelper"/>.
	/// </summary>
	public static class UrlHelperExtensions
	{
		public static string WebpackAsset(this UrlHelper urlHelper, string assetName = "main", string assetType = "js")
		{
			if (urlHelper == null) throw new ArgumentNullException(nameof(urlHelper));

			var webpack = urlHelper.RequestContext.HttpContext.Application.GetWebpack();
			return urlHelper.Content(webpack.GetAssetUrl(assetName, assetType));
		}

		public static string AbsoluteWebpackAsset(this UrlHelper urlHelper, string assetName = "main", string assetType = "js")
		{
			if (urlHelper == null) throw new ArgumentNullException(nameof(urlHelper));

			var webpack  = urlHelper.RequestContext.HttpContext.Application.GetWebpack();
			var relative = urlHelper.Content(webpack.GetAssetUrl(assetName, assetType));
			return new Uri(urlHelper.RequestContext.HttpContext.Request.Url, relative).ToString();
		}
	}
}
