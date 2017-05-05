using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Webpack.NET
{
	public static class ConfigurationExtensions
	{

		public static void ConfigureWebpack(this HttpApplication application, WebpackConfig config)
		{
			application.Application.Lock();
			try
			{
				application.Application[WebpackApplicationKey] = new Webpack(config, new HttpServerUtilityWrapper(application.Server));
			}
			finally
			{
				application.Application.UnLock();
			}
		}

		internal static string WebpackApplicationKey = "WebpackApplicationKey";

		internal static Webpack GetWebpack(this HttpApplication application)
		{
			var Webpack = application.Application[WebpackApplicationKey] as Webpack;
			if (Webpack == null)
				throw new ApplicationException("Webpack has not been configured.  Have you called HttpApplication.ConfigureWebpack()?");

			return Webpack;
		}
	}
}
