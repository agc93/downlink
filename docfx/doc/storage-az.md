# Azure Storage for Downlink

The `Downlink.AzureStorage` library (bundled with Downlink) adds support for Azure Storage as a remote storage backend.

## Activating the Azure Storage backend

> Check the [configuration guide](./configuration.md) for full details

You can quickly enable the Azure Storage backend by setting the **`Storage`** configuration key to "Azure" or "AzureStorage".

This will invoke the `AddAzureStorage()` method and add Azure Storage to your application.

## Configuration

The Azure Storage backend requires two things be added to your app configuration (i.e. `downlink.json` or `downlink.yml`): the connection string used for your Azure Storage account, and the name of the container to retrieve from.

### Connection String

```json
{
  "ConnectionStrings": {
    "AzureStorage": "DefaultEndpointsProtocol=https;AccountName=storagesample;AccountKey=<account-key>+"
  },
}
```

```yaml
ConnectionStrings :
  AzureStorage: 'DefaultEndpointsProtocol=https;AccountName=storagesample;AccountKey=<account-key>+'
```

Follow the instructions in the [official documentation](https://docs.microsoft.com/en-us/azure/storage/storage-configure-connection-string) to get your connection string.

### Container Name

```json
{
    "AzureStorage": {
        "Container": "container-name-here"
    }
}
```

```yaml
AzureStorage:
  Container: 'container-name-here'
```

## How it works

Much like the [S3 backend](./storage-s3.md), the Azure Storage backend relies on hierarchical storage of app artifacts, and defaults to using the `container-name/version/platform/architecture` form. For example:

```text
# Path inside container
/v1.2/windows/x64/myapp.msi
```

Will be resolved for the `/v1.2/windows/x64` path. Note that this mode requires each "directory" to contain only one file (Downlink will just return the first file in the matching directory).

### Alternate modes

You can also provide the `MatchStrategy` option under `AzureStorage` configuration to control the matching behaviour.

Using `FlatVersion` results in a request for `/v1.2/windows/x64` being mapped to the `/v1.2` directory in the container, and the first file with `windows` and `x64` in the name will be returned. Likewise, `FlatPlatform` will use the same method for the first file in the `/windows` directory.

Finally the `Flat` strategy just matches all files in the top level of the container against all three properties, and is not recommended if at all possible.

> The default behaviour can be specified using the `Hierarchical` strategy config

### Force Name Matching

Set the `AzureStorage/ForceNameMatching` option to `true` to also force any matches to include the requested version in the file name. This is useful if you have more than one file in the target folder (such as metadata etc).
