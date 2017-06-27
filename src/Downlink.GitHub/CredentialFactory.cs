using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.GitHub
{
    public static class CredentialFactory {

        public static GitHubCredentials BuildCredentials(System.IServiceProvider provider) {
            var config = provider.GetService<IConfiguration>();
            var repo = config.GetValue<string>("GitHubStorage:Repository", null) ??
                config.GetValue<string>("GitHub:Repository", null);
            if (string.IsNullOrWhiteSpace(repo)) throw new System.UnauthorizedAccessException();
            return new GitHubCredentials(string.Empty, repo);

        }
    }
}
