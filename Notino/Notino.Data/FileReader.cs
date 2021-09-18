using Notino.Common.Models;
using System;
using System.IO;

namespace Notino.Data
{
    public class FileReader : IFileReader
    {
        public string GetFileExtension(string filePath)
        {
            return Path.GetExtension(filePath).Remove(0, 1);
        }

        public string Read(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}
