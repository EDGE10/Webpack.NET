using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webpack.NET
{
	internal class WebpackAssetsDictionary : Dictionary<string, WebpackAsset>
	{
		public string RootFolder { get; set; }

		internal static WebpackAssetsDictionary FromFile(string assetJsonPath)
		{
			var serializer = new JsonSerializer();
			using (var reader = new JsonTextReader(new StreamReader(assetJsonPath)))
				return serializer.Deserialize<WebpackAssetsDictionary>(reader);
		}
	}
}
