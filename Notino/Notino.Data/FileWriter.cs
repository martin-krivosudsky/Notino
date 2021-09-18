using System;
using System.IO;
using System.Threading.Tasks;

namespace Notino.Data
{
    public class FileWriter : IFileWriter
    {
        public async Task WriteAsync(string filePath, byte[] data)
        {
            using (FileStream sourceStream = new(filePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(data.AsMemory(0, data.Length));
            };
        }
    }
}
