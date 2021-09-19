using Notino.Common;
using System.Collections.Generic;
using System.IO;

namespace Notino.Data
{
    public class FileReader : IFileReader
    {
        public string GetFileExtension(string filePath)
        {
            return Path.GetExtension(filePath).Remove(0, 1);
        }

        public string ReadText(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public byte[] ReadBytes(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public IEnumerable<Common.Models.FileInfo> GetFilesInfo()
        {
            var result = new List<Common.Models.FileInfo>();

            foreach (string file in Directory.EnumerateFiles(Constants.StoragePath, "*.*", SearchOption.AllDirectories))
            {
                FileInfo fi = new(file);
                Common.Models.FileInfo fileInfo = new()
                {
                    Name = fi.Name,
                    Path = fi.FullName?.Replace(Constants.StoragePath, ""),
                    FileType = fi.Extension?.Replace(".", "")?.ToUpper(),
                    Size = fi.Length
                };
                result.Add(fileInfo);
            }

            return result;
        }
    }
}
