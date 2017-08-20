using System;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Hosting
{
    public interface IDownlinkBuilder
    {
        IServiceCollection Services {get;}
        void Build();
    }
}
