using Downlink.Core;

namespace Downlink.Messaging
{
    public class AppVersionResponseModel
    {
        internal AppVersionResponseModel(IFileSource source) {
            Source = source;
        }

        internal System.Exception Exception {get;}

        internal IFileSource Source { get; }

    }
}