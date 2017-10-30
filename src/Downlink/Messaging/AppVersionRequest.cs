using Downlink.Core;
using MediatR;

namespace Downlink.Messaging
{
    public class AppVersionRequest : IRequest<AppVersionResponseModel>
    {
        public AppVersionRequest(VersionSpec version, string format = null)
        {
            Version = version;
            if (Version.Format == null || (string.IsNullOrWhiteSpace(Version.Format) && !string.IsNullOrWhiteSpace(format))) {
                Version.Format = format;
            }
        }
        public VersionSpec Version { get; }
    }
}