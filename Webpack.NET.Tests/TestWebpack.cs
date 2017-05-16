using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Webpack.NET.Tests
{
	[TestFixture]
	public class TestWebpack
	{
		private WebpackConfig _config;
		private Mock<HttpServerUtilityBase> _server;
		private string _tempFile;

		[SetUp]
		public void Setup()
		{
			_config   = new WebpackConfig();
			_server   = new Mock<HttpServerUtilityBase>();
			_tempFile = Path.GetTempFileName();
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_tempFile))
				File.Delete(_tempFile);
		}

		[Test]
		public void Constructor_Throws_On_Null_Parameters()
		{
			Assert.Throws<ArgumentNullException>(() => new Webpack(null, _server.Object));
			Assert.Throws<ArgumentNullException>(() => new Webpack(new WebpackConfig(), null));
		}

		[Test]
		public void Config_Exposes_Constructor_Value()
		{
			var webpack = new Webpack(_config, _server.Object);
			Assert.That(webpack.Config, Is.SameAs(_config));
		}

		[Test]
		public void Assets_Are_Lazily_Loaded()
		{
			var webpack = new Webpack(_config, _server.Object);
			_server.Verify(s => s.MapPath(It.IsAny<string>()), Times.Never());

			_server.Setup(s => s.MapPath("~/scripts/manifest.json")).Returns(_tempFile);
			_config.AssetManifestPath = "~/scripts/manifest.json";
			File.WriteAllText(_tempFile, @"{ ""file"": { ""js"": ""file.js"" } }");

			Assert.That(webpack.GetAssetUrl("file", "js"), Is.EqualTo("file.js"));
			_server.Verify(s => s.MapPath(It.IsAny<string>()), Times.Once());

			Assert.That(webpack.GetAssetUrl("file", "js"), Is.EqualTo("file.js"));
			_server.Verify(s => s.MapPath(It.IsAny<string>()), Times.Once());
		}
	}
}
