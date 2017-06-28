using Amazon;
using Amazon.Runtime;

namespace Downlink.S3
{
    public class S3Configuration {
        internal S3Configuration(AWSCredentials creds, RegionEndpoint region, string bucketName) {
            Credentials = creds;
            Region = region;
            BucketName = bucketName;
        }
        internal AWSCredentials Credentials {get;}
        internal RegionEndpoint Region {get;}
        internal string BucketName {get;}
    }
}