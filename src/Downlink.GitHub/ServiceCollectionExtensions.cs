using Downlink.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Downlink.GitHub
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGitHubReleaseStorage(this IServiceCollection services)
        {
            services.AddSingleton<GitHubCredentials>(CredentialFactory.BuildCredentials);
            services.AddSingleton<IGitHubClient, OctokitClient>();
            services.AddSingleton<IRemoteStorage>(provider => provider.GetService<IGitHubClient>());

            return services;
        }
    }
}
