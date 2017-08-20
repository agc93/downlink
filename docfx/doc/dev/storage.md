# Building a Storage Backend for Downlink

Downlink is specifically designed to allow you to store app artifacts on whatever storage you want, just providing an abstraction over your chosen storage platform. Out of the box, we support GitHub Releases, Azure Storage, AWS S3, and local files, but you may want to use another different storage backend.

## Introduction

> [!TIP]
> Make sure to read the [Developer's Guide](./developers.md) first!

As covered in the guide, a storage backend in Downlink is essentially just an `IRemoteStorage` implementation that returns an `IFileSource` for a given `VersionSpec`. It's that simple.

### Version Matching

If you're planning on distributing your custom backend for use by others, it's probably a good idea to implement support for pattern matchers so that users can customise the version matching behaviour. That's covered in the example below below, but also makes your backend's implementation much cleaner and more logical.

## Building a backend

> [!NOTE]
> This example assumes [self hosting](./hosting.md). If you're building for a [plugin](./plugins.md) the process is similar.

Since this is not the simplest process and requires a bit of explanation, the process is covered in full detail in [this how-to document](./storage-howto.md).

## Activating a new backend

Custom storage backends are activated in a two-step process: adding the backend to the app and activating it in configuration.

To add the app when self-hosting, just call `AddStorage` from the builder in your `Startup.cs`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc()
        .AddDownlink(b => b.AddStorage<MyAwesomeStorage>());
}
```

This will register your storage backend with Downlink, but you also need to set the value of the `Storage` configuration key to the name of your storage backend (that is, the value of the `Name` property).