namespace Notino.Data
{
    public interface IFileReader
    {
        string ReadText(string filePath);
        string GetFileExtension(string filePath);
        byte[] ReadBytes(string filePath);
        bool FileExists(string filePath);
        string GetFileName(string filePath);
    }
}
