# GitHub Release Storage for Downlink

The `Downlink.GitHub` library (bundled with Downlink) adds support for GitHub releases as a remote storage backend.

## Activating the GitHub Release backend

> Check the [configuration guide](./configuration.md) for full details

You can quickly enable the GitHub Release backend by setting the **`Storage`** configuration key to "GitHub".

This will invoke the `AddGitHubReleaseStorage()` method and add GitHub Release-backed storage to your application.

## Configuration

The GitHub Release backend requires some configuration for the repository to use, and optionally for GitHub Enterprise. The backend defaults to using GitHub.com, but providing a `ServerUrl` configuration key will use GitHub Enterprise instead.

```json
{
    "GitHubStorage": {
        "Repository": "agc93/downlink"
    }
}
```

```yaml
GitHubStorage:
  Repository: 'agc93/downlink'
  ServerUrl: 'https://github.myenterprise.com'
```

## How it works

The GitHub Release backend is quite unique from other currently supported backends in not having any notion of folders or hierarchy, since it uses the assets attached to a given release. Due to this, it relies on naming conventions in asset files to work correctly.

For example, for an app name '*myapp*', a request to `/v1.2/windows/x64` would search for an asset named `myapp_v1.2_windows_x64.exe`, or equivalent. The important parts are that segments are separated by an underscore (`_`) and only the first segment is ignored. As such, the following two URLs are functionally equivalent:

```text
my-awesome-app-with-features-and-things_v1_macos_x64.app
myapp_v1_macos_x64.app
```

while the following are different:

```text
my-awesome-app_v2_macos_x64.app -> /v2/macos/x64
my-awesome-app_patch_macos_x64.app -> /patch/macos/x64
```

> Remember that as with any other backends, the easiest way to 'skip' a field, such as when you are only building for one architecture, is to use a single value like `any` or `default`.

### Alternate modes

You can also provide the `MatchStrategy` option under `GitHubStorage` configuration to control the matching behaviour.

Note that due to the limited nature of GitHub Releases, it is recommended to not change the version matching method unless absolutely required, as even the flat matching documented here is adjusted for GitHub compared to other backends.