namespace Notino.Data
{
    public interface IFileReader
    {
        string ReadText(string filePath);
        string GetFileExtension(string filePath);
        byte[] ReadBytes(string filePath);
    }
}
