# Downlink Configuration

Downlink is specifically designed to be easy to configure with your choice of source and format.

Currently, Downlink supports configuration provided by files in either JSON or YAML format, or from environment variables (including in the Docker image).

This means that the following are all interchangeable:


```yaml
Storage: GitHub
GitHubStorage:
  Repository: agc93/downlink
```

```json
{
    "Storage": "GitHub",
    "GitHubStorage": {
        "Repository": "agc93/downlink"
    }
}
```

```bash
export DOWNLINK:Storage=GitHub
export DOWNLINK:GitHubStorage:Repository="agc93/downlink"
# You can also use double-underscores
export DOWNLINK__GitHubStorage__Repository="agc93/downlink"
```

## Configuration Files

When using JSON or YAML files, Downlink will automatically search a number of paths for suitable config files (note these are relative to the app directory):

- `./appsettings.json`
- `./downlink.yml` or `./downlink.json`
- `./config.yml` or `./config.json`
- `./config/downlink.yml` or `./config/downlink.json`

## Environment variables

Additionally, any environment variables that start with `DOWNLINK_`, `DOWNLINK__` or `DOWNLINK:` are automatically added to the configuration.

These are especially useful when working with the Docker container

## Priority

Note that since configuration is loaded sequentially, configuration values can be overriden. The order of loading is as follows:

- Configuration files:
  - `appsettings.json`
  - `downlink.json`/`downlink.yml`
  - `config.json`/`config.yml`
  - `./config/downlink.json`/`./config/downlink.yml`
- Environment Variables
- Command Line (if present)

> [!TIP]
> It's recommended to use configuration files for more complex configuration (such as Azure connection strings and complex paths), but environment variables can be used to easily override simple settings.

## Experimental options

There are some features in Downlink that, for various reasons, are still experimental and have to be enabled explicitly. These features are generally enabled using a top-level `Experimental` configuration key:

For example:

```json
{
  "Experimental": {
    "GitHubLatestVersion": true
  }
}
```

```yaml
Experimental:
  EnableLocalPlugins: true
  InjectContext: true
```