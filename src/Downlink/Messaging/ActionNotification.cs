using System;
using MediatR;

namespace Downlink.Messaging
{
    public class ActionNotification : MediatR.INotificationHandler<DownlinkResultNotification>
    {
        private Action<DownlinkResultNotification> _responseAction;
        internal ActionNotification(Action<DownlinkResultNotification> responseAction) {
            _responseAction = responseAction;
        }
        public void Handle(DownlinkResultNotification notification)
        {
            _responseAction?.Invoke(notification);
        }
    }
}