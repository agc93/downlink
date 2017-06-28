# Version Matching

The process for going from a request, like `/v1.2/windows/x64` to an actual file on your storage backend we refer to as "version matching". There's two methods of version matching that depends on your choice of backend:

## Match Strategies

Each backend can make use of a *match strategy* to match a version request to a specific file on the backend. These match strategies are the more advanced method of version matching, but are also not portable between backends as they use the respective backends' underlying storage types.

Currently, Downlink doesn't use many match strategies, but they are very helpful for extending and customising.

## Pattern Matchers

Pattern matching is the much simpler alternative to match strategies, but has the benefit of being backend-agnostic, since it abstracts the domain-specific objects (i.e. release assets, block blobs etc) into a "path" that gets run through the pattern matcher. The first match is returned to the user.

> The main exception is the GitHub backend, which supports only the convention-driven GitHub-specific *Flat* strategy at this time.

## Compatibility

|Backend|Azure|AWS S3|GitHub|
|:-----:|:---:|:----:|:----:|
|Flat|✅|✅|✅*|
|Hierarchical|✅|✅|❌|
|FlatVersion|✅|✅|❌|
|FlatPlatform|✅|✅|❌|
|Search|❌|✅|❌|

## Configuration

As outlined in the individual backend's documentation, the convention is to use a `MatchStrategy` configuration key under the respective backend's section to control the version matching in use:

```json
{
    "AzureStorage": {
        "MatchStrategy": "Hierarchical"
    },
    "AWS": {
        "MatchStrategy": "FlatVersion"
    }
}
```

```yaml
AzureStorage:
  MatchStrategy: 'Hierarchical'
AWS:
  MatchStrategy: 'FlatPlatform'
```
