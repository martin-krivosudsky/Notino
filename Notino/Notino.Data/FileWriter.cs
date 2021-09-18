using System;
using System.IO;
using System.Threading.Tasks;

namespace Notino.Data
{
    public class FileWriter : IFileWriter
    {
        public async Task WriteAsync(string folderPath, string fileName, byte[] data, bool createFolders)
        {
            if (createFolders)
            {
                Directory.CreateDirectory(folderPath);
            }

            using (FileStream sourceStream = new(folderPath + fileName,
                FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(data.AsMemory(0, data.Length)).ConfigureAwait(false);
            };
        }
    }
}
