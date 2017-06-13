using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Moq;
using NUnit.Framework;

namespace Webpack.NET.Tests
{
    [TestFixture]
    public class TestWebpack
    {
        private WebpackConfig _config;
        private Mock<HttpServerUtilityBase> _server;
        private List<string> _tempFiles;

        [SetUp]
        public void Setup()
        {
            _config = new WebpackConfig();
            _server = new Mock<HttpServerUtilityBase>();
            _tempFiles = new List<string>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var tempFile in _tempFiles)
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
        }

        [Test]
        public void Constructor_Throws_On_Null_Parameters()
        {
            Assert.Throws<ArgumentNullException>(() => new Webpack(null, _server.Object));
            Assert.Throws<ArgumentNullException>(() => new Webpack(new[] { new WebpackConfig() }, null));
        }

        [Test]
        public void Assets_Are_Lazily_Loaded()
        {
            var webpack = new Webpack(new[] { _config }, _server.Object);
            _server.Verify(s => s.MapPath(It.IsAny<string>()), Times.Never());

            _config.AssetManifestPath = "~/scripts/manifest.json";
            _config.AssetOutputPath = "~/dist";

            SetupManifestFile(_config.AssetManifestPath, @"{ ""file"": { ""js"": ""file.js"" } }");

            Assert.That(webpack.GetAssetUrl("file", "js"), Is.EqualTo("~/dist/file.js"));
            _server.Verify(s => s.MapPath(It.IsAny<string>()), Times.Once());

            Assert.That(webpack.GetAssetUrl("file", "js"), Is.EqualTo("~/dist/file.js"));
            _server.Verify(s => s.MapPath(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void Assets_Can_Be_Retrieved_From_Multiple_Configurations()
        {
            var config1 = new WebpackConfig { AssetManifestPath = "~/scripts/manifest1.json", AssetOutputPath = "~/dist/1" };
            var config2 = new WebpackConfig { AssetManifestPath = "~/scripts/manifest2.json", AssetOutputPath = "~/dist/2" };

            SetupManifestFile(config1.AssetManifestPath, @"{ ""code"": { ""js"": ""file.1.js"" } }");
            SetupManifestFile(config2.AssetManifestPath, @"{ ""style"": { ""css"": ""file.2.css"" } }");

            var webpack = new Webpack(new[] { config1, config2 }, _server.Object);
            Assert.That(webpack.GetAssetUrl("code", "js"), Is.EqualTo("~/dist/1/file.1.js"));
            Assert.That(webpack.GetAssetUrl("style", "css"), Is.EqualTo("~/dist/2/file.2.css"));
        }

        [Test]
        public void GetAssetUrl_Returns_Null_When_No_Configurations_Specified()
        {
            var webpack = new Webpack(new WebpackConfig[0], _server.Object);
            Assert.Throws<AssetNotFoundException>(() => webpack.GetAssetUrl("any", "any"));
        }

        [Test]
        public void GetAssetUrl_Returns_Null_When_No_Matching_Resource()
        {
            _config.AssetManifestPath = "~/scripts/manifest.json";
            SetupManifestFile(_config.AssetManifestPath, @"{ ""file"": { ""js"": ""file.js"" } }");

            var webpack = new Webpack(new[] { _config }, _server.Object);
            Assert.Throws<AssetNotFoundException>(() => webpack.GetAssetUrl("non-existant", "js"));
            Assert.Throws<AssetNotFoundException>(() => webpack.GetAssetUrl("file", "non-existant"));
            Assert.Throws<AssetNotFoundException>(() => webpack.GetAssetUrl("non-existant", "non-existant"));
        }

        private void SetupManifestFile(string manifestPath, string manifestContent)
        {
            var tempFile = Path.GetTempFileName();
            _tempFiles.Add(tempFile);
            _server.Setup(s => s.MapPath(manifestPath)).Returns(tempFile);
            File.WriteAllText(tempFile, manifestContent);
        }
    }
}
