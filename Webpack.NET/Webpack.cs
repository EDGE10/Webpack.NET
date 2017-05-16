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
		private Lazy<WebpackAssetsDictionary> _assets;

		public Webpack(WebpackConfig config, HttpServerUtilityBase server)
		{
			if (server == null) throw new ArgumentNullException(nameof(server));

			this.Config = config ?? throw new ArgumentNullException(nameof(config));
			_assets     = new Lazy<WebpackAssetsDictionary>(() => 
				WebpackAssetsDictionary.FromFile(server.MapPath(config.AssetManifestPath)));
		}

		public WebpackConfig Config { get; private set; }

		public string GetAssetUrl(string assetName, string assetType)
		{
			return _assets.Value[assetName][assetType];
		}
	}
}
