using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Webpack.NET
{
	/// <summary>
	/// Represents the webpack assets manifest.
	/// </summary>
	/// <seealso cref="System.Collections.Generic.Dictionary{System.String, Webpack.NET.WebpackAsset}" />
	internal class WebpackAssetsDictionary : Dictionary<string, WebpackAsset>
	{
		/// <summary>
		/// Gets or sets the root folder.
		/// </summary>
		/// <value>
		/// The root folder.
		/// </value>
		public string RootFolder { get; set; }

		/// <summary>
		/// Reads the webpack assets manifest from the file.
		/// </summary>
		/// <param name="assetFilePath">The webpack asset manifest file path.</param>
		/// <returns>
		/// The webpack assets manifest.
		/// </returns>
		internal static WebpackAssetsDictionary FromFile(string assetFilePath)
		{
			var serializer = new JsonSerializer();
			using (var reader = new JsonTextReader(new StreamReader(assetFilePath)))
			{
				return serializer.Deserialize<WebpackAssetsDictionary>(reader);
			}
		}
	}
}
