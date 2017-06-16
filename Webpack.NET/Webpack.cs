using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webpack.NET
{
	/// <summary>
	/// Implementation of configured webpack instance.
	/// </summary>
	/// <seealso cref="Webpack.NET.IWebpack" />
	internal class Webpack : IWebpack
	{
		/// <summary>
		/// The cached assets.
		/// </summary>
		private readonly Lazy<IEnumerable<WebpackAssetsDictionary>> assets;

		/// <summary>
		/// Initializes a new instance of the <see cref="Webpack" /> class.
		/// </summary>
		/// <param name="configurations">The webpack configurations.</param>
		/// <param name="httpServerUtility">The HTTP server utility.</param>
		/// <exception cref="System.ArgumentNullException">configs
		/// or
		/// server</exception>
		public Webpack(IEnumerable<WebpackConfig> configurations, HttpServerUtilityBase httpServerUtility)
		{
			if (configurations == null) throw new ArgumentNullException(nameof(configurations));
			if (httpServerUtility == null) throw new ArgumentNullException(nameof(httpServerUtility));

			this.assets = new Lazy<IEnumerable<WebpackAssetsDictionary>>(() => configurations
				.Select(config => GetAssetDictionaryForConfig(config, httpServerUtility))
				.ToList());
		}

		/// <summary>
		/// Gets the webpack asset dictionary for the specified <paramref name="configuration"/>.
		/// </summary>
		/// <param name="configuration">The webpack configuration.</param>
		/// <param name="httpServerUtility">The HTTP server utility.</param>
		/// <returns>
		/// The webpack asset dictionary.
		/// </returns>
		private static WebpackAssetsDictionary GetAssetDictionaryForConfig(WebpackConfig configuration, HttpServerUtilityBase httpServerUtility)
		{
			var assets = WebpackAssetsDictionary.FromFile(httpServerUtility.MapPath(configuration.AssetManifestPath));
			assets.RootFolder = configuration.AssetOutputPath;

			return assets;
		}

		/// <summary>
		/// Gets the asset URL.
		/// </summary>
		/// <param name="assetName">Name of the asset.</param>
		/// <param name="assetType">Type of the asset.</param>
		/// <param name="required">If set to <c>true</c> throws an <see cref="AssetNotFoundException" /> when the asset could not be found; otherwise, returns <c>null</c>.</param>
		/// <returns>
		/// The asset URL.
		/// </returns>
		/// <exception cref="AssetNotFoundException">Asset '<paramref name="assetName" />' with type '<paramref name="assetType" />' could not be found.</exception>
		public string GetAssetUrl(string assetName, string assetType, bool required = true)
		{
			var matchingDictionary = this.assets.Value
				.Where(a => a.ContainsKey(assetName))
				.FirstOrDefault();

			string assetUrl = null;
			var noAssetFound = matchingDictionary == null || !matchingDictionary[assetName].TryGetValue(assetType, out assetUrl);
			if (noAssetFound)
			{
				if (required) throw new AssetNotFoundException(assetName, assetType);

				return null;
			}

			string rootFolder = matchingDictionary.RootFolder;
			if (String.IsNullOrEmpty(rootFolder) || Uri.IsWellFormedUriString(assetUrl, UriKind.Absolute))
			{
				// No root folder set or asset is already an absolute URL
				return assetUrl;
			}
			else
			{
				// Combine root folder and asset URL
				return $"{rootFolder}/{assetUrl}";
			}
		}
	}
}
