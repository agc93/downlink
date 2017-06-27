using Downlink.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.GitHub
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGitHubReleaseStorage(this IServiceCollection services, IConfiguration config) {
            return services.AddGitHubReleaseStorage(config.GetValue<string>("GitHubStorage:Repository"));
        }
        public static IServiceCollection AddGitHubReleaseStorage(this IServiceCollection services, string repoId)
        {
            services.AddSingleton<GitHubCredentials>(provider => new GitHubCredentials(string.Empty, repoId));
            services.AddSingleton<IGitHubClient, OctokitClient>();
            services.AddSingleton<IRemoteStorage>(provider => provider.GetService<IGitHubClient>());

            return services;
        }
    }
}
