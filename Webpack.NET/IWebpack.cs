namespace Webpack.NET
{
	internal interface IWebpack
	{
		WebpackAssetsDictionary Assets { get; }
		WebpackConfig Config { get; }
	}
}