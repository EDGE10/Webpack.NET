using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webpack.NET
{

	[Serializable]
	public class AssetNotFoundException : Exception
	{
		public AssetNotFoundException(string assetName, string assetType)
		: base($"Asset {assetName} with type {assetType} could not be found") { }
		
		protected AssetNotFoundException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
