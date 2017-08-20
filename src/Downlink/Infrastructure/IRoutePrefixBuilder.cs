using System;
using Microsoft.Extensions.Configuration;

namespace Downlink.Infrastructure
{
    public interface IRoutePrefixBuilder
    {
        string GetPrefix();
    }

    public class ConfigurationRoutePrefixBuilder : IRoutePrefixBuilder
    {
        private readonly IConfiguration _config;

        public ConfigurationRoutePrefixBuilder(IConfiguration config) {
            _config = config;
        }
        public string GetPrefix()
        {
            return _config.GetValue("DownlinkPrefix", _config.GetValue<string>("Downlink:Prefix", null) ?? string.Empty);
        }
    }

    public class StaticRoutePrefixBuilder : IRoutePrefixBuilder
    {
        private readonly string _prefix;

        internal StaticRoutePrefixBuilder(string prefix) {
            _prefix = prefix;
        }
        public string GetPrefix() => _prefix ?? string.Empty;
    }
}