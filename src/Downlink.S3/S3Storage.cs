using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Downlink.Core;
using Downlink.Core.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Downlink.S3
{
    public class S3Storage : IRemoteStorage
    {
        private readonly RegionEndpoint _region;
        private readonly AWSCredentials _credentials;

        public S3Storage(
            //IConfiguration configuration,
            AWSCredentials credentials,
            RegionEndpoint region,
            string bucket = "downlink-storage"
        )
        {
            _credentials = credentials;
            _region = region;
            BucketName = bucket;
        }
        private AmazonS3Client BuildClient() => new AmazonS3Client(_credentials, _region);
        public string BucketName { get; private set; }

        public async Task<IFileSource> GetFileAsync(VersionSpec version)
        {
            var pattern = BuildPattern(version);
            var files = await Search(pattern);
            if (!files.Any()) throw new VersionNotFoundException();
            var firstMatch = files.First();
            using(var client = BuildClient())
            {
                var req = new GetPreSignedUrlRequest {
                    BucketName = BucketName,
                    Key = firstMatch.Key,
                    Expires = DateTime.Now.AddMinutes(15)
                };
                var url = client.GetPreSignedURL(req);
                return new S3FileSource(new Uri(url)) {
                    Metadata = new FileMetadata(firstMatch.Size, firstMatch.Key.Split('/', '\\', ':').Last()),
                    Version = version
                };
            }
        }

        private async Task<IEnumerable<S3Object>> Search(string searchPattern, CancellationToken cancellationToken = default(CancellationToken)) {

            searchPattern = searchPattern?.Replace('\\', '/');
            string prefix = searchPattern;
            Regex patternRegex = null;
            int wildcardPos = searchPattern?.IndexOf('*') ?? -1;
            if (searchPattern != null && wildcardPos >= 0) {
                patternRegex = new Regex("^" + Regex.Escape(searchPattern).Replace("\\*", ".*?") + "$");
                int slashPos = searchPattern.LastIndexOf('/');
                prefix = slashPos >= 0 ? searchPattern.Substring(0, slashPos) : String.Empty;
            }
            prefix = prefix ?? String.Empty;

            var objects = new List<S3Object>();
            using (var client = BuildClient()) {
                var req = new ListObjectsRequest {
                    BucketName = BucketName,
                    Prefix = prefix
                };

                do {
                    var res = await client.ListObjectsAsync(req, cancellationToken).AnyContext();
                    if (res.IsTruncated)
                        req.Marker = res.NextMarker;
                    else
                        req = null;

                    // TODO: Implement paging
                    objects.AddRange(res.S3Objects.Where(blob => patternRegex == null || patternRegex.IsMatch(blob.Key)));
                } while (req != null);

                return objects;
            }
        }

        private static string BuildPattern(VersionSpec version)
        {
            var path = $"{version.Platform}/{version.Architecture}/*{version.ToString()}*";
            return path;
        }
    }
}
