using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Webpack.NET
{
	internal class Webpack
	{
		private Lazy<WebpackAssetsDictionary> _assets;

		public Webpack(WebpackConfig config, HttpServerUtilityBase server)
		{
			this.Config = config;
			_assets     = new Lazy<WebpackAssetsDictionary>(() => 
				WebpackAssetsDictionary.FromFile(
					server.MapPath(config.AssetManifestPath)));
		}

		public WebpackConfig Config { get; private set; }

		internal WebpackAssetsDictionary Assets { get => _assets.Value; }
	}
}
