namespace Notino.Common.Helpers
{
    public static class FilenameHelper
    {
        public static string RenameExtension(string source, string oldExtension, string newExtension)
        {
            int place = source.LastIndexOf(oldExtension);

            if (place == -1)
                return source;

            string result = source.Remove(place, oldExtension.Length).Insert(place, newExtension);
            return result;
        }
    }
}
