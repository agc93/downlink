using System;
using Downlink.Composition;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Hosting
{
    public class DownlinkContextPlugin : IDownlinkPlugin
    {
        private readonly IConfiguration _config;

        public DownlinkContextPlugin(IConfiguration configuration) {
            _config = configuration;
        }

        public void AddServices(IDownlinkBuilder builder)
        {
            if (_config.GetValue("Experimental:InjectContext", false)) {
                builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            }
        }
    }
}