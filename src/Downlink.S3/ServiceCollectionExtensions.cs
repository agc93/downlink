using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Downlink.Core;

namespace Downlink.S3
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddS3Storage(this IServiceCollection services)
        {
            //services.AddSingleton<IFileStorage>(S3FileStorageFactory.BuildS3FileStorage);
            services.AddSingleton<IRemoteStorage>(S3FileStorageFactory.BuildS3FileStorage);
            return services;
        }
    }

    public static class S3FileStorageFactory
    {
        public static IRemoteStorage BuildS3FileStorage(System.IServiceProvider provider)
        {
            var config = provider.GetService<IConfiguration>();
            var opts = config.GetAWSOptions();
            var bucket = config.GetSection("AWS").GetSection("Bucket").Value;
            return new S3Storage(
                opts.Credentials,
                opts.Region,
                string.IsNullOrWhiteSpace(bucket)
                    ? "downlink-storage"
                    : bucket);
        }
    }
}