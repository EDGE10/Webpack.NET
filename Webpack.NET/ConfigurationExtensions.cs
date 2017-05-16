using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Webpack.NET
{
	public static class ConfigurationExtensions
	{
		[ExcludeFromCodeCoverage]
		public static void ConfigureWebpack(this HttpApplication application, params WebpackConfig[] configs)
		{
			if (application == null) throw new ArgumentNullException(nameof(application));

			new HttpApplicationStateWrapper(application.Application)
				.ConfigureWebpack(new Webpack(configs, new HttpServerUtilityWrapper(application.Server)));
		}

		internal static void ConfigureWebpack(this HttpApplicationStateBase application, IWebpack webpack)
		{
			if (application == null) throw new ArgumentNullException(nameof(application));
			if (webpack     == null) throw new ArgumentNullException(nameof(webpack));
			
			application.Lock();
			try
			{
				application[WebpackApplicationKey] = webpack;
			}
			finally
			{
				application.UnLock();
			}
		}

		internal static string WebpackApplicationKey = "WebpackApplicationKey";

		internal static IWebpack GetWebpack(this HttpApplicationStateBase application)
		{
			if (application == null) throw new ArgumentNullException(nameof(application));
			
			var webpack = application[WebpackApplicationKey] as IWebpack;
			if (webpack == null)
				throw new ApplicationException("Webpack has not been configured.  Have you called HttpApplication.ConfigureWebpack()?");

			return webpack;
		}
	}
}
