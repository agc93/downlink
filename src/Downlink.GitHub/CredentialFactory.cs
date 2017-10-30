using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Downlink.GitHub
{
    public static class CredentialFactory {

        public static GitHubCredentials BuildCredentials(System.IServiceProvider provider)
        {
            var config = provider.GetService<IConfiguration>();
            return BuildCredentials(config, provider.GetService<ILogger<GitHubCredentials>>());
        }

        public static GitHubCredentials BuildCredentials(IConfiguration config, ILogger<GitHubCredentials> logger = null)
        {
            var repo = config.GetValue<string>("GitHubStorage:Repository", string.Empty) ??
                            config.GetValue<string>("GitHub:Repository", string.Empty);
            var token = config.GetValue<string>("GitHubStorage:ApiToken", string.Empty) ??
                            config.GetValue<string>("GitHub:ApiToken", string.Empty);
            return new GitHubCredentials(token, repo);
        }
    }
}
