using System.Linq;
using Downlink.Core.IO;

namespace Downlink.Core
{
    public interface IFormatParser
    {
        string GetFormat(Path fullPath);
    }

    public class PathFormatParser : IFormatParser
    {
        public string GetFormat(Path fullPath)
        {
            return System.IO.Path.GetExtension(fullPath);
        }
    }

    public class SimpleFormatParser : IFormatParser
    {
        public string GetFormat(Path fullPath)
        {
            return string.Join(".", fullPath.ToString().Split('.').SkipWhile(segment => segment.Length > 3));
        }
    }
}