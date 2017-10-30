using Downlink.Core;
using Downlink.Core.Diagnostics;

namespace Downlink.Messaging
{
    internal static class MessagingExtensions
    {
        internal static DownlinkResultNotification ToNotification(this NotFoundException exception, VersionSpec spec) {
            return DownlinkResultNotification.FromException(exception, spec);
        }
    }
}