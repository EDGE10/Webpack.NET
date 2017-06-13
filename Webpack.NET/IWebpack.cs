namespace Webpack.NET
{
	/// <summary>
	/// Represents a configured webpack instance.
	/// </summary>
	internal interface IWebpack
	{
		/// <summary>
		/// Gets the asset URL.
		/// </summary>
		/// <param name="assetName">Name of the asset.</param>
		/// <param name="assetType">Type of the asset.</param>
		/// <returns>
		/// The asset URL.
		/// </returns>
		string GetAssetUrl(string assetName, string assetType);
	}
}