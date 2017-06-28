using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Downlink.Core;
using Amazon.Runtime;

namespace Downlink.S3
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddS3Storage(this IServiceCollection services)
        {
            services.AddConfiguration();
            services.AddSingleton<IRemoteStorage>(S3FileStorageFactory.BuildS3FileStorage);
            return services;
        }
    }

    public static class S3FileStorageFactory
    {
        internal static IServiceCollection AddConfiguration(this IServiceCollection services) {
            services.AddSingleton<S3Configuration>(BuildConfiguration);
            return services;
        }
        public static IRemoteStorage BuildS3FileStorage(System.IServiceProvider provider)
        {
            var config = provider.GetService<IConfiguration>();
            var strat = config.GetValue("AWS:SearchStrategy", "Search");
            if (strat.ToLower() == "search") {
                return new S3SearchStorage(config, provider.GetService<S3Configuration>());
            } else {
                return new S3Storage(
                    config,
                    provider.GetService<S3Configuration>(),
                    provider.GetServices<IPatternMatcher>(),
                    provider.GetServices<S3MatchStrategy>());
            }
        }

        internal static S3Configuration BuildConfiguration(System.IServiceProvider provider) {
            var config = provider.GetService<IConfiguration>();
            var opts = config.GetAWSOptions();
            var bucket = config.GetValue<string>("AWS:Bucket");
            return new S3Configuration(opts.Credentials, opts.Region, string.IsNullOrWhiteSpace(bucket) ? "downlink-storage" : bucket);
        }
    }
}