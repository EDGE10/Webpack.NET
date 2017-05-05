using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webpack.NET
{
	/// <summary>
	/// Configuration properties for Webpack integration.
	/// </summary>
	public class WebpackConfig
	{
		/// <summary>
		/// Gets or sets the server-relative path to the assets.json file output by https://github.com/kossnocorp/assets-Webpack-plugin
		/// e.g. "~/scripts/Webpack-assets.json"
		/// </summary>
		public string AssetManifestPath { get; set; }

		/// <summary>
		/// Gets or sets the server-relative path to the output of the Webpack assets.
		/// e.g. "~/scripts"
		/// </summary>
		public string AssetOutputPath { get; set; }
	}
}
