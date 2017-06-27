# Frequently Asked Questions

<!-- content-here -->

### But, why?

In short, because I wanted to.

Downlink was designed and built as a simple distribution mechanism for tools like [Git Profile Manager](https://github.com/agc93/git-profile-manager) where there may be a lot of binaries, versions and packages available.

Since "storing files" is one of the few things "the cloud" has actually worked out, Downlink serves as an abstraction over them, so you can continue to use GitHub releases or Azure Storage or S3 buckets etc, while making links stable, predictable and agnostic to the underlying storage.

Essentially, you can give your users links and never worry about things like website migrations breaking your download links.

### Is it SEO friendly?

Not even close. Downlink is much closer to an API gateway than a web app and (at this time) doesn't even include a UI. As such, it's not good for SEO since it mostly just redirects to other locations anyway.

You should make your website SEO friendly and let Downlink do the one thing it's good at: predictable, stable download links.

### What platforms are supported?

Downlink is built with ASP.NET Core meaning it can be run essentially anywhere .NET Core is supported.

That being said, at this time, there are no pre-built platform packages available so the Docker image is still the recommended way to get started.

### But I already use GitHub releases!

Then keep doing so! Downlink doesn't replace things like GitHub releases, it merely serves as an API over the top of them. That means you can run still use GitHub to publish your releases, and if you follow our [conventions](./storage-gh.md) Downlink will pick up straight from your existing and new releases.

If you use Downlink, you don't *have* to continue using GitHub Releases for ever! It also means you can rename your repository/organisation or change away from GitHub all together without breaking the download links.