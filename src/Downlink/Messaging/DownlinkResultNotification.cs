using Downlink.Core;
using Downlink.Core.Diagnostics;

namespace Downlink.Messaging
{
    public class DownlinkResultNotification : MediatR.INotification
    {
        public VersionSpec Version {get;}
        public AppVersionResponseModel Response {get;}
        public NotFoundException Exception {get; private set;}

        public DownlinkResultNotification(VersionSpec spec, AppVersionResponseModel res)
        {
            this.Version = spec;
            this.Response = res;
        }

        internal static DownlinkResultNotification FromException(NotFoundException exception, VersionSpec spec) {
            return new DownlinkResultNotification(spec, null) {
                Exception = exception
            };
        }
    }
}