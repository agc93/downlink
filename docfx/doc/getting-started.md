# Getting Started with Downlink

To get started quickly with Downlink, use the Docker image.

Once you've got Docker set up and working, you can get started very quickly with Downlink and use the powerful [configuration](./configuration.md) to easily control the behaviour.

As an example of how easy Downlink is to get started, we're going to set up a Downlink container to serve releases for the [Hugo](http://gohugo.io) static site generator. Currently, their "Download" link just sends you to the GitHub Releases page for the [hugo repository](https://github.com/gohugoio/hugo/releases), so we can easily put Downlink in front of these using the [GitHub Releases backend](./storage-gh.md).

First, create a config file using either YAML or JSON.

For `config.yml`:

```yaml
Storage: GitHub
GitHubStorage:
  Repository: gohugoio/hugo
  SplitCharacters:
    - '_'
    - '-'
```

or for `config.json`:

```json
{
    "Storage": "GitHub",
    "GitHubStorage": {
        "Repository": "gohugoio/hugo",
        "SplitCharacters": ["_", "-"]
    }
}
```

> [!NOTE]
> We need the extra `SplitCharacters` configuration as Hugo names their packages with a mix of `_` and `-` characters.

Now, from a command line, run the Downlink container:

```bash
docker run -it --rm -p 80:80 -v $PWD/config.yml:/downlink/config.yml agc93/downlink:latest
# or for JSON
docker run -it --rm -p 80:80 -v $PWD/config.json:/downlink/config.json agc93/downlink:latest
```

The new container will start, the configuration loader will read in your new configuration file and automatically add the GitHub Releases backend, pointing at the [gohugoio/hugo](https://github.com/gohugo.io) repository's releases.

That's it! You're now running Downlink. To prove it's working, on your PC, browse to [`http://localhost/v0.24.1/Windows/64bit`](http://localhost/v0.24.1/Windows/64bit) and watch as your browser automatically downloads the 64-bit Windows release of v0.24.1 of Hugo from GitHub.

> [!TIP]
> You can verify this by checking the Network tab of your browser's developer tools, or using `curl`/`wget`

You can try again with [`http://localhost/v0.23/macOS/32bit`](http://localhost/v0.23/macOS/32bit) or [`http://localhost/v0.22.1/OpenBSD/64bit`](http://localhost/v0.22.1/OpenBSD/64bit) to get the relevant files for other versions, platforms, or architectures.

Want to stop the redirecting and download straight from Downlink? Easy! Stop the container using Ctrl-C (it will be automatically removed), and run again, this time providing an extra configuration key from an environment variable:

```bash
docker run -it --rm -p 80:80 -v $PWD/config.yml:/downlink/config.yml -e DOWNLINK:ProxyRemoteFiles=true agc93/downlink:latest
# or for JSON
docker run -it --rm -p 80:80 -v $PWD/config.json:/downlink/config.json -e DOWNLINK:ProxyRemoteFiles=true agc93/downlink:latest
```

Note that extra `-e` argument? That adds the `ProxyRemoteFiles` variable to your Downlink configuration (and sets it to `true`). Downlink will now automatically proxy remote files (from whatever storage backend you chose) and serve it back to the user directly from Downlink. Again, browse to the same address as earlier and you should receive the binary download, no redirect required!

> [!TIP]
> You can mix and match configuration! You could add `ProxyRemoteFiles` to your YAML/JSON configuration file, or recreate the whole config file in environment variables, or anything in between.

Notice how we didn't change a single thing about Hugo's downloads or releases? But now we can easily link directly to any version, platform or architecture. That's what Downlink is about!