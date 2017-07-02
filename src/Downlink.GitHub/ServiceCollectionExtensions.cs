using Downlink.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Downlink.GitHub
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGitHubReleaseStorage(this IServiceCollection services)
        {
            services.AddSingleton<GitHubCredentials>(CredentialFactory.BuildCredentials);
            services.AddSingleton<IGitHubClient, OctokitClient>();
            services.AddSingleton<GitHubMatchStrategy, FlatMatchStrategy>();
            services.AddSingleton<IRemoteStorage>(provider => provider.GetService<IGitHubClient>());

            return services;
        }
    }

    public static class ConfigurationExtensions {
        internal static List<string> GetList(this IConfiguration config, string key) {
            var values = config.GetSection(key);
            if (!values.AsEnumerable().Any()) {
                return null;
            }
            return values.GetChildren().Select(v => v.Value).ToList();
        }

        internal static IEnumerable<string> GetList(this IConfiguration config, string key, IEnumerable<string> defaultValue) {
            return config.GetList(key) ?? defaultValue;
        }
    }
}
