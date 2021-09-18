namespace Notino.Common.Models
{
    public enum ResponseCode
    {
        Success = 0,
        FileNotFound = 1,
        ConversionNotSupported = 2,
        InvalidFile = 3,
        UnableToCreateFile = 4,
        FileAlreadyInDesiredFormat = 5
    }
}
