using System;
using System.Web;
using Moq;
using NUnit.Framework;

namespace Webpack.NET.Tests
{
    [TestFixture]
    public class TestConfigurationExtensions
    {
        [Test]
        public void ConfigureWebpack_Throws_On_Null_Parameters()
        {
            HttpApplication application = null;
            Assert.Throws<ArgumentNullException>(() => application.ConfigureWebpack(new WebpackConfig()));
            Assert.Throws<ArgumentNullException>(() => new HttpApplication().ConfigureWebpack(null));
        }

        [Test]
        public void ConfigureWebpack_Internal_Throws_On_Null_Parameters()
        {
            var application = new Mock<HttpApplicationStateBase>();
            var webpack = new Mock<IWebpack>().Object;

            Assert.Throws<ArgumentNullException>(() => ((HttpApplicationStateBase)null).ConfigureWebpack(webpack));
            Assert.Throws<ArgumentNullException>(() => application.Object.ConfigureWebpack(null));
        }

        [Test]
        public void ConfigureWebpack_Populates_Application()
        {
            var webpack = new Mock<IWebpack>().Object;
            var application = new MockHttpApplicationState();
            var server = new Mock<HttpServerUtilityBase>();

            application.ConfigureWebpack(webpack);

            Assert.That(application.GetWebpack(), Is.SameAs(webpack));
        }

        [Test]
        public void GetWebpack_Throws_On_Null_Application()
        {
            HttpApplicationStateBase application = null;
            Assert.Throws<ArgumentNullException>(() => application.GetWebpack());
        }

        [Test]
        public void GetWebpack_Throws_On_Missing_Webpack()
        {
            var application = new Mock<HttpApplicationStateBase>();
            var error = Assert.Throws<ApplicationException>(() => application.Object.GetWebpack());
            Assert.That(error.Message, Is.EqualTo("Webpack has not been configured, have you called HttpApplication.ConfigureWebpack()?"));
        }
    }
}
