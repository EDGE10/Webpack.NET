namespace Webpack.NET
{
	internal interface IWebpack
	{
		string GetAssetUrl(string assetName, string assetType);
	}
}