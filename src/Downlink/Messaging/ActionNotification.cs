using System;
using MediatR;

namespace Downlink.Messaging
{
    public class ActionNotification : MediatR.INotificationHandler<AppVersionResponseModel>
    {
        private Action<AppVersionResponseModel> _responseAction;
        internal ActionNotification(Action<AppVersionResponseModel> responseAction) {
            _responseAction = responseAction;
        }
        public void Handle(AppVersionResponseModel notification)
        {
            _responseAction?.Invoke(notification);
        }
    }
}