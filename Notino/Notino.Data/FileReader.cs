﻿using System.IO;

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
    }
}
