namespace Webpack.NET
{
	/// <summary>
	/// Configuration properties for webpack integration.
	/// </summary>
	public class WebpackConfig
	{
		/// <summary>
		/// Gets or sets the server-relative path to the JSON file output by https://github.com/kossnocorp/assets-webpack-plugin, eg. "~/scripts/webpack-assets.json"
		/// </summary>
		public string AssetManifestPath { get; set; }

		/// <summary>
		/// Gets or sets the server-relative path to the output of the webpack assets, eg. "~/scripts".
		/// </summary>
		public string AssetOutputPath { get; set; }
	}
}
