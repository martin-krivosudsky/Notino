namespace Notino.Service.Tests.FileConvert
{
    internal class FilesToConvert
    {
        internal const string JsonFile = @"
{""employees"":
    {""employee"":[{""firstName"":""John"",""lastName"":""Doe""},
    {""firstName"":""Anna"",""lastName"":""Smith""},
    {""firstName"":""Peter"",""lastName"":""Jones""}]}}";
        internal const string XmlFile = @"
<employees>
  <employee>
    <firstName>John</firstName> <lastName>Doe</lastName>
  </employee>
  <employee>
    <firstName>Anna</firstName> <lastName>Smith</lastName>
  </employee>
  <employee>
    <firstName>Peter</firstName> <lastName>Jones</lastName>
  </employee>
</employees>";
    }
}
