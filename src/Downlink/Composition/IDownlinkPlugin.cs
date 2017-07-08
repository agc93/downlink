using Downlink.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Composition
{
    public interface IDownlinkPlugin
    {
        void AddServices(IDownlinkBuilder builder);
    }
}