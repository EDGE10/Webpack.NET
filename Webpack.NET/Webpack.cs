using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Webpack.NET
{
	internal class Webpack : IWebpack
	{
		private Lazy<IEnumerable<WebpackAssetsDictionary>> _assets;

		public Webpack(IEnumerable<WebpackConfig> configs, HttpServerUtilityBase server)
		{
			if (configs == null) throw new ArgumentNullException(nameof(configs));
			if (server == null) throw new ArgumentNullException(nameof(server));

			_assets = new Lazy<IEnumerable<WebpackAssetsDictionary>>(() => configs
				.Select(config => GetAssetDictionaryForConfig(server, config))
				.ToList());
		}

		private static WebpackAssetsDictionary GetAssetDictionaryForConfig(HttpServerUtilityBase server, WebpackConfig config)
		{
			var assets        = WebpackAssetsDictionary.FromFile(server.MapPath(config.AssetManifestPath));
			assets.RootFolder = config.AssetOutputPath;
			return assets;
		}

		public string GetAssetUrl(string assetName, string assetType)
		{
			var matchingDictionary = _assets.Value
				.Where(a => a.ContainsKey(assetName))
				.FirstOrDefault();

			var noAssetFound = matchingDictionary == null || !matchingDictionary[assetName].ContainsKey(assetType);
			if (noAssetFound)
				throw new AssetNotFoundException(assetName, assetType);

			return $"{matchingDictionary.RootFolder}/{matchingDictionary[assetName][assetType]}";
		}
	}
}
