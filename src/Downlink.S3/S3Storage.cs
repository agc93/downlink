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
            IConfiguration configuration,
            S3Configuration opts,
            IEnumerable<IPatternMatcher> patternMatchers,
            IEnumerable<S3MatchStrategy> strategies
        )
        {
            _credentials = opts.Credentials;
            _region = opts.Region;
            BucketName = opts.BucketName;
            var stratName = configuration.GetSection("AWS").GetSection("MatchStrategy").Value;
            stratName = string.IsNullOrWhiteSpace(stratName) ? "Hierarchical" : stratName;
            MatchStrategy = strategies.GetFor<S3MatchStrategy, S3Object>(stratName);
            PatternMatcher = patternMatchers.GetFor(stratName);
        }
        private AmazonS3Client BuildClient() => new AmazonS3Client(_credentials, _region);
        public string BucketName { get; private set; }

        private S3MatchStrategy MatchStrategy { get; }
        private IPatternMatcher PatternMatcher { get; }

        public async Task<IFileSource> GetFileAsync(VersionSpec version)
        {
            var blobs = new List<S3Object>();
            using (var client = BuildClient())
            {
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = BucketName,
                    MaxKeys = 10
                };
                ListObjectsV2Response response;
                do
                {
                    response = await client.ListObjectsV2Async(request);
                    blobs.AddRange(response.S3Objects);
                    request.ContinuationToken = response.NextContinuationToken;

                } while (response.IsTruncated == true);
            }
            if (MatchStrategy == null)
            {
                var blob = blobs.FirstOrDefault(
                    o => PatternMatcher.Match(o.Key, version)
                );
                if (blob == null) throw new VersionNotFoundException();
                return ToSource(version, blob);
            }
            var match = await MatchStrategy.MatchAsync(blobs, version);
            return match;
        }

        private IFileSource ToSource(VersionSpec version, S3Object firstMatch)
        {
            using (var client = BuildClient())
            {
                var req = new GetPreSignedUrlRequest
                {
                    BucketName = BucketName,
                    Key = firstMatch.Key,
                    Expires = DateTime.Now.AddMinutes(15)
                };
                var url = client.GetPreSignedURL(req);
                return new S3FileSource(new Uri(url))
                {
                    Metadata = new FileMetadata(firstMatch.Size, firstMatch.Key.Split('/', '\\', ':').Last()),
                    Version = version
                };
            }
        }
    }
}
