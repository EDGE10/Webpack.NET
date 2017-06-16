using System;
using System.IO;
using NUnit.Framework;

namespace Webpack.NET.Tests
{
	[TestFixture]
	public class TestWebpackAssetsDictionary
	{
		private string _tempFile;

		[SetUp]
		public void Setup()
		{
			_tempFile = Path.GetTempFileName();
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_tempFile))
				File.Delete(_tempFile);
		}

		[Test]
		public void FromFile_Throws_On_Invalid_Filename()
		{
			Assert.Throws<ArgumentNullException>(() => WebpackAssetsDictionary.FromFile(null));
			Assert.Throws<ArgumentException>(() => WebpackAssetsDictionary.FromFile(string.Empty));
		}

		[Test]
		public void FromFile_Throws_On_NonExistant_File()
		{
			Assert.Throws<FileNotFoundException>(() => WebpackAssetsDictionary.FromFile("non-existant.json"));
		}

		[Test]
		public void FromFile_Reads_Valid_Content()
		{
			File.WriteAllText(_tempFile, @"
			{
				""file"": { ""js"": ""file.hash.js"", ""other"": ""file.hash.other"" },
				""file2"": { ""js"": ""file2.hash.js"" }
			}");

			var assets = WebpackAssetsDictionary.FromFile(_tempFile);

			Assert.That(assets["file"]["js"], Is.EqualTo("file.hash.js"));
			Assert.That(assets["file"]["other"], Is.EqualTo("file.hash.other"));
			Assert.That(assets["file2"]["js"], Is.EqualTo("file2.hash.js"));
		}
	}
}
