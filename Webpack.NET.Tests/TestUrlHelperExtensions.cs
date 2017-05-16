using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;

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
		public void WebpackAsset_Gets_Expected_Url()
		{
			var urlHelper = SetupUrlHelper("http://server/");
			var webpack   = new Mock<IWebpack>();
			webpack.Setup(w => w.GetAssetUrl("asset-name", "ext")).Returns("/scripts/assets/asset.hash.js");
			urlHelper.RequestContext.HttpContext.Application.ConfigureWebpack(webpack.Object);
			
			Assert.That(urlHelper.WebpackAsset("asset-name", "ext"), Is.EqualTo("/scripts/assets/asset.hash.js"));
			Assert.That(urlHelper.AbsoluteWebpackAsset("asset-name", "ext"), Is.EqualTo("http://server/scripts/assets/asset.hash.js"));
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
