using System;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;

namespace Webpack.NET.Tests
{
    [TestFixture]
    public class TestUrlHelperExtensions
    {
        [Test]
        public void WebpackAsset_Throws_On_Null_UrlHelper()
        {
            UrlHelper urlHelper = null;
            Assert.Throws<ArgumentNullException>(() => urlHelper.WebpackAsset());
            Assert.Throws<ArgumentNullException>(() => urlHelper.AbsoluteWebpackAsset());
        }

        [Test]
        public void WebpackAsset_Throws_On_No_Matching_Resource()
        {
            var urlHelper = SetupUrlHelper("http://server/");
            var webpack = new Mock<IWebpack>();
            webpack.Setup(w => w.GetAssetUrl("non-existant", "non-existant", true)).Throws<AssetNotFoundException>();
            urlHelper.RequestContext.HttpContext.Application.ConfigureWebpack(webpack.Object);

            Assert.Throws<AssetNotFoundException>(() => urlHelper.WebpackAsset("non-existant", "non-existant"));
            Assert.Throws<AssetNotFoundException>(() => urlHelper.AbsoluteWebpackAsset("non-existant", "non-existant"));
        }

        [Test]
        public void WebpackAsset_Returns_Expected_Url()
        {
            var urlHelper = SetupUrlHelper("http://server/");
            var webpack = new Mock<IWebpack>();
            webpack.Setup(w => w.GetAssetUrl("asset-name", "ext", true)).Returns("/scripts/assets/asset.hash.js");
            webpack.Setup(w => w.GetAssetUrl("asset-name", "ext", false)).Returns("/scripts/assets/asset.hash.js");
            webpack.Setup(w => w.GetAssetUrl("asset-name-querystring", "ext", true)).Returns("/scripts/assets/asset.hash.js?i=1%2b1");
            webpack.Setup(w => w.GetAssetUrl("asset-name-querystring", "ext", false)).Returns("/scripts/assets/asset.hash.js?i=1%2b1");
            urlHelper.RequestContext.HttpContext.Application.ConfigureWebpack(webpack.Object);

            Assert.That(urlHelper.WebpackAsset("asset-name", "ext"), Is.EqualTo("/scripts/assets/asset.hash.js"));
            Assert.That(urlHelper.WebpackAsset("asset-name", "ext", false), Is.EqualTo("/scripts/assets/asset.hash.js"));
            Assert.That(urlHelper.AbsoluteWebpackAsset("asset-name", "ext"), Is.EqualTo("http://server/scripts/assets/asset.hash.js"));
            Assert.That(urlHelper.AbsoluteWebpackAsset("asset-name", "ext", false), Is.EqualTo("http://server/scripts/assets/asset.hash.js"));
            Assert.That(urlHelper.WebpackAsset("asset-name-querystring", "ext"), Is.EqualTo("/scripts/assets/asset.hash.js?i=1%2b1"));
            Assert.That(urlHelper.WebpackAsset("asset-name-querystring", "ext", false), Is.EqualTo("/scripts/assets/asset.hash.js?i=1%2b1"));
            Assert.That(urlHelper.AbsoluteWebpackAsset("asset-name-querystring", "ext"), Is.EqualTo("http://server/scripts/assets/asset.hash.js?i=1%2b1"));
            Assert.That(urlHelper.AbsoluteWebpackAsset("asset-name-querystring", "ext", false), Is.EqualTo("http://server/scripts/assets/asset.hash.js?i=1%2b1"));
        }

        [Test]
        public void WebpackAsset_Returns_Null_When_Not_Required_And_No_Matching_Resource()
        {
            var urlHelper = SetupUrlHelper("http://server/");
            var webpack = new Mock<IWebpack>();
            webpack.Setup(w => w.GetAssetUrl("non-existant", "non-existant", false)).Returns((string)null);
            urlHelper.RequestContext.HttpContext.Application.ConfigureWebpack(webpack.Object);

            Assert.That(urlHelper.WebpackAsset("non-existant", "non-existant", false), Is.Null);
            Assert.That(urlHelper.AbsoluteWebpackAsset("non-existant", "non-existant", false), Is.Null);
        }

        public static UrlHelper SetupUrlHelper(string baseUrl)
        {
            var routes = new RouteCollection();

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.ApplicationPath).Returns("/");
            request.SetupGet(x => x.Url).Returns(new Uri(baseUrl, UriKind.Absolute));
            request.SetupGet(x => x.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());

            var response = new Mock<HttpResponseBase>();
            response.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns((string url) => url);
            response.Setup(x => x.AddCacheDependency(It.IsAny<CacheDependency>()));

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);

            var application = new MockHttpApplicationState();
            context.SetupGet(x => x.Application).Returns(application);

            return new UrlHelper(new RequestContext(context.Object, new RouteData()), routes);
        }
    }
}
