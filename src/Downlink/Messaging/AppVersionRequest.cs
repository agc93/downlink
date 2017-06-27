using Downlink.Core;
using MediatR;

namespace Downlink.Messaging
{
    public class AppVersionRequest : IRequest<AppVersionResponseModel>
    {
        public AppVersionRequest(VersionSpec version, string format = null)
        {
            Version = version;
            Format = format;
        }
        public VersionSpec Version { get; }

        public string Format { get; }
    }
}