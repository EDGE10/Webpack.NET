using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace Webpack.NET
{
    /// <summary>
    /// Configuration extensions for setting up webpack.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// The webpack application key.
        /// </summary>
        internal static string WebpackApplicationKey = "WebpackApplicationKey";

        /// <summary>
        /// Configures webpack.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="configurations">The webpack configurations.</param>
        /// <exception cref="System.ArgumentNullException">application</exception>
        [ExcludeFromCodeCoverage]
        public static void ConfigureWebpack(this HttpApplication application, params WebpackConfig[] configurations)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));

            new HttpApplicationStateWrapper(application.Application)
                    .ConfigureWebpack(new Webpack(configurations, new HttpServerUtilityWrapper(application.Server)));
        }

        /// <summary>
        /// Configures webpack.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="webpack">The webpack instance.</param>
        /// <exception cref="System.ArgumentNullException">
        /// application
        /// or
        /// webpack
        /// </exception>
        internal static void ConfigureWebpack(this HttpApplicationStateBase application, IWebpack webpack)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));
            if (webpack == null) throw new ArgumentNullException(nameof(webpack));

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

        /// <summary>
        /// Gets the webpack instance.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>
        /// The webpack instance.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Webpack has not been configured, have you called HttpApplication.ConfigureWebpack()?</exception>
        internal static IWebpack GetWebpack(this HttpApplicationStateBase application)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));

            var webpack = application[WebpackApplicationKey] as IWebpack;
            if (webpack == null) throw new InvalidOperationException("Webpack has not been configured, have you called HttpApplication.ConfigureWebpack()?");

            return webpack;
        }
    }
}
