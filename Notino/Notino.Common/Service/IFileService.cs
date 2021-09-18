﻿using Notino.Common.Models;
using System.Threading.Tasks;

namespace Notino.Common.Service
{
    public interface IFileService
    {
        Task<Response> SaveFile(FileDto file);
        Task<Response> Convert(string filePath, FileType desiredType);
        byte[] GetFile(string path);
        bool FileExist(string filePath);
        Response SaveFileFromUrl(string url, string filePath, string fileName);
        Response SendByEmail(string filePath, string email);
    }
}
