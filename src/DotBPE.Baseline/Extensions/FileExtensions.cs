using System.IO;

namespace DotBPE.Baseline.Extensions
{
    public static class FileExtensions
    {
        public static string GetFileExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(extension))
                extension = fileName.Substring(fileName.LastIndexOf('.'));

            return extension;
        }
    }
}
