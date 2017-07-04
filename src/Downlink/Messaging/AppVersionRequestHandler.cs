using System;
using System.Threading.Tasks;
using Downlink.Core;
using MediatR;

namespace Downlink.Messaging
{
    public class AppVersionRequestHandler : IAsyncRequestHandler<AppVersionRequest, AppVersionResponseModel>
    {
        public AppVersionRequestHandler(IRemoteStorage storage) {
            Storage = storage;
            System.Console.WriteLine($"Storage == null: {Storage == null}");
        }

        private IRemoteStorage Storage {get;}

        public async Task<AppVersionResponseModel> Handle(AppVersionRequest message)
        {
            var source = await Storage.GetFileAsync(message.Version);
            return new AppVersionResponseModel(source);
        }
    }
}