using Newtonsoft.Json;
using Notino.Common.Models;
using Notino.Common.Service.FileConvert;
using System.IO;
using System.Xml;

namespace Notino.Service.FileConvert
{
    public class JsonToXmlConverter : IConverter
    {
        public string Convert(string source)
        {
            XmlDocument xml = JsonConvert.DeserializeXmlNode(source);

            using StringWriter stringWriter = new StringWriter();
            using XmlTextWriter textWriter = new XmlTextWriter(stringWriter);
            xml.WriteTo(textWriter);
            string xmlString = stringWriter.ToString();
            return xmlString;
        }

        public bool IsValid(FileType sourceType, FileType outputType)
        {
            return sourceType == FileType.Json && outputType == FileType.Xml;
        }
    }
}
