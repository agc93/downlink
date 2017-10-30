using System.Linq;
using Downlink.Core;
using Downlink.Core.IO;

namespace Downlink.Infrastructure
{
    public class MimeFormatParser : IFormatParser
    {
        public string GetFormat(Path fullPath)
        {
            return HeyRed.Mime.MimeGuesser.GuessExtension(fullPath);
        }
    }
}