using Newtonsoft.Json;
using Notino.Common.Models;
using Notino.Common.Service.FileConvert;
using System.Xml;

namespace Notino.Service.FileConvert
{
    public class XmlToJsonConverter : IConverter
    {
        public string Convert(string source)
        {
            XmlDocument doc = new();
            doc.LoadXml(source);
            return JsonConvert.SerializeXmlNode(doc);
        }

        public bool IsValid(FileType sourceType, FileType outputType)
        {
            return sourceType == FileType.Xml && outputType == FileType.Json;
        }
    }
}
