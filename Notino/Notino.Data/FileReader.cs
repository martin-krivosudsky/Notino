using Notino.Common.Models;
using System;
using System.IO;

namespace Notino.Data
{
    public class FileReader : IFileReader
    {
        public FileType GetFileType(string filePath)
        {
            string extension = Path.GetExtension(filePath).Remove(0, 1);

            return (FileType)Enum.Parse(typeof(FileType), extension, true);
        }

        public string Read(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}
