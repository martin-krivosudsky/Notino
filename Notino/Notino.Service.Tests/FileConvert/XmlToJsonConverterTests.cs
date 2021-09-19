using Notino.Common.Models;
using Notino.Service.FileConvert;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace Notino.Service.Tests.FileConvert
{
    public class XmlToJsonConverterTests
    {
        XmlToJsonConverter converter;

        [SetUp]
        public void Setup()
        {
            converter = new();
        }

        [Test]
        public void Convert_HappyPath()
        {
            string result = converter.Convert(FilesToConvert.XmlFile);
            result = Regex.Replace(result, @"\s+", "");
            string expected = Regex.Replace(FilesToConvert.JsonFile, @"\s+", "");

            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase(FileType.Json, FileType.Json, false)]
        [TestCase(FileType.Json, FileType.Xml, false)]
        [TestCase(FileType.Xml, FileType.Xml, false)]
        [TestCase(FileType.Xml, FileType.Json, true)]
        public void GetValidConverter(FileType source, FileType destination, bool expectedResult)
        {
            bool result = converter.IsValid(source, destination);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
