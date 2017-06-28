# AWS S3 Storage for Downlink

The `Downlink.S3` library (bundled with Downlink) adds support for AWS S3 as a remote storage backend.

## Activating the AWS S3 backend

> Check the [configuration guide](./configuration.md) for full details

You can quickly enable the AWS S3 backend by setting the **`Storage`** configuration key to "AWS" or "S3".

This will invoke the `AddS3Storage()` method and add AWS S3 Storage to your application.

## Configuration

The AWS S3 backend requires configuration for the credential profile and bucket name you want to use to be added to your app configuration (i.e. `downlink.json` or `downlink.yml`).

```json
{
  "AWS": {
    "Profile": "local-creds-profile",
    "Region": "us-west-2",
    "Bucket": "downlink"
  },
}
```

```yaml
AWS:
  Profile: 'local-test-profile'
  Region: 'us-west-2'
  Bucket: 'downlink'
```

## How it works

Much like the [Azure backend](./storage-az.md), the S3 backend uses hierarchical storage of app artifacts, and defaults to using the `container-name/version/platform/architecture` form. For example:

```text
# Path inside bucket
/v1.2/windows/x64/myapp.msi
```

Will be resolved for the `/v1.2/windows/x64` path. Note that this mode requires each "directory" to contain only one file (Downlink will just return the first file in the matching directory).

### Alternate modes

You can also provide the `MatchStrategy` option under `AWS` configuration to control the matching behaviour.

Using `FlatVersion` results in a request for `/v1.2/windows/x64` being mapped to the `/v1.2` directory in the container, and the first file with `windows` and `x64` in the name will be returned. Likewise, `FlatPlatform` will use the same method for the first file in the `/windows` directory.

Finally the `Flat` strategy just matches all files in the top level of the container against all three properties, and is not recommended if at all possible.

> The default behaviour can be specified using the `Search` strategy config

### Force Name Matching

Set the `AWS/ForceNameMatching` option to `true` to also force any matches to include the requested version in the file name. This is useful if you have more than one file in the target folder (such as metadata etc).