using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;

namespace Webpack.NET.Tests
{
    [TestFixture]
    public class TestHtmlHelperExtensions
    {
        [Test]
        public void WebpackScript_Throws_On_Null_HtmlHelper()
        {
            HtmlHelper htmlHelper = null;
            Assert.Throws<ArgumentNullException>(() => htmlHelper.WebpackScript());
        }

        [Test]
        public void WebpackScript_Throws_On_No_Matching_Resource()
        {
            var htmlHelper = SetupHtmlHelper("http://server/");
            var webpack = new Mock<IWebpack>();
            webpack.Setup(w => w.GetAssetUrl("non-existant", "non-existant", true)).Throws<AssetNotFoundException>();
            htmlHelper.ViewContext.RequestContext.HttpContext.Application.ConfigureWebpack(webpack.Object);

            Assert.Throws<AssetNotFoundException>(() => htmlHelper.WebpackScript("non-existant", "non-existant"));
        }

        [Test]
        public void WebpackScript_Returns_Expected_Html()
        {
            var htmlHelper = SetupHtmlHelper("http://server/");
            var webpack = new Mock<IWebpack>();
            webpack.Setup(w => w.GetAssetUrl("asset-name", "ext", true)).Returns("/scripts/assets/asset.hash.js");
            webpack.Setup(w => w.GetAssetUrl("asset-name-querystring", "ext", true)).Returns("/scripts/assets/asset.hash.js?i=1%2b1");
            htmlHelper.ViewContext.RequestContext.HttpContext.Application.ConfigureWebpack(webpack.Object);

            Assert.That(htmlHelper.WebpackScript("asset-name", "ext").ToHtmlString(), Is.EqualTo("<script src=\"/scripts/assets/asset.hash.js\"></script>"));
            Assert.That(htmlHelper.WebpackScript("asset-name-querystring", "ext").ToHtmlString(), Is.EqualTo("<script src=\"/scripts/assets/asset.hash.js?i=1%2b1\"></script>"));
        }

        [Test]
        public void WebpackScript_Returns_Null_When_Not_Required_And_No_Matching_Resource()
        {
            var htmlHelper = SetupHtmlHelper("http://server/");
            var webpack = new Mock<IWebpack>();
            webpack.Setup(w => w.GetAssetUrl("non-existant", "non-existant", false)).Returns((string)null);
            htmlHelper.ViewContext.RequestContext.HttpContext.Application.ConfigureWebpack(webpack.Object);

            Assert.That(htmlHelper.WebpackScript("non-existant", "non-existant", false), Is.Null);
        }

        [Test]
        public void WebpackStyleSheet_Throws_On_Null_HtmlHelper()
        {
            HtmlHelper htmlHelper = null;
            Assert.Throws<ArgumentNullException>(() => htmlHelper.WebpackStyleSheet());
        }

        [Test]
        public void WebpackStyleSheet_Throws_On_No_Matching_Resource()
        {
            var htmlHelper = SetupHtmlHelper("http://server/");
            var webpack = new Mock<IWebpack>();
            webpack.Setup(w => w.GetAssetUrl("non-existant", "non-existant", true)).Throws<AssetNotFoundException>();
            htmlHelper.ViewContext.RequestContext.HttpContext.Application.ConfigureWebpack(webpack.Object);

            Assert.Throws<AssetNotFoundException>(() => htmlHelper.WebpackStyleSheet("non-existant", "non-existant"));
        }

        [Test]
        public void WebpackStyleSheet_Returns_Expected_Html()
        {
            var htmlHelper = SetupHtmlHelper("http://server/");
            var webpack = new Mock<IWebpack>();
            webpack.Setup(w => w.GetAssetUrl("asset-name", "ext", true)).Returns("/scripts/assets/asset.hash.css");
            webpack.Setup(w => w.GetAssetUrl("asset-name-querystring", "ext", true)).Returns("/scripts/assets/asset.hash.css?i=1%2b1");
            htmlHelper.ViewContext.RequestContext.HttpContext.Application.ConfigureWebpack(webpack.Object);

            Assert.That(htmlHelper.WebpackStyleSheet("asset-name", "ext").ToHtmlString(), Is.EqualTo("<link href=\"/scripts/assets/asset.hash.css\" rel=\"stylesheet\" />"));
            Assert.That(htmlHelper.WebpackStyleSheet("asset-name-querystring", "ext").ToHtmlString(), Is.EqualTo("<link href=\"/scripts/assets/asset.hash.css?i=1%2b1\" rel=\"stylesheet\" />"));
        }

        [Test]
        public void WebpackStyleSheet_Returns_Null_When_Not_Required_And_No_Matching_Resource()
        {
            var htmlHelper = SetupHtmlHelper("http://server/");
            var webpack = new Mock<IWebpack>();
            webpack.Setup(w => w.GetAssetUrl("non-existant", "non-existant", false)).Returns((string)null);
            htmlHelper.ViewContext.RequestContext.HttpContext.Application.ConfigureWebpack(webpack.Object);

            Assert.That(htmlHelper.WebpackStyleSheet("non-existant", "non-existant", false), Is.Null);
        }

        public static HtmlHelper SetupHtmlHelper(string baseUrl)
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

            var viewContext = new Mock<ViewContext>();
            viewContext.SetupGet(x => x.HttpContext).Returns(context.Object);

            var view = new Mock<IView>();

            var viewDataContainer = new Mock<IViewDataContainer>();

            return new HtmlHelper(viewContext.Object, viewDataContainer.Object, routes);
        }
    }
}
