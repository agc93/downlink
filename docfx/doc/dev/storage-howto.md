# How to build a custom storage backend for Downlink

Creating a storage adapter, while it may seem complex, is actually fairly straightforward. With the use of pattern matchers, you can even cut down on logic.

## Create your new backend

First, create a new class (e.g. `MyAwesomeStorage`) and implement `IRemoteStorage`.

```csharp
public class MyAwesomeStorage : IRemoteStorage
{
    public string Name => "Awesome";

    public Task<IFileSource> GetFileAsync(VersionSpec version)
    {
        throw new NotImplementedException();
    }
}
```

The `Name` property is important as it is the name your users will set 'Storage' to in their configuration.

When a user requests a version (such as `/v1.2/windows/x64`) and your storage backend is active, your `GetFileAsync` method will be invoked with a `VersionSpec` object for the current request.

If your storage class needs additional dependencies, just declare them in the constructor. They will be resolved from the DI container on the first request.

```csharp
public class MyAwesomeStorage : IRemoteStorage
{
    // trimmed
    public MyAwesomeStorage(IConfiguration config, ILogger<MyAwesomeStorage> logger) {
        // your dependencies will be injected on first request
    }
    //trimmed
}
```

## Understanding the `IFileSource`

The `IFileSource` interface represents the location where Downlink can find the matching app artifact for the requested version. Your storage backend will need to implement it's own file source type, with any logic needed to populate the object. For a good example of this, check out the [`LocalFileSource`](xref:Downlink.Local.LocalFileSource) class.

Note that one of the most important parts of the `IFileSource` is obviously the `FileUri` property. This is the URI that Downlink will fetch to serve (or redirect to) your app artifact. If the URI has a `file://`, `http://` or `https://` scheme, Downlink can automatically serve it up using the built-in services. If your app needs additional logic, you can use a 'scheme client' (an `ISchemeClient` implementation) to include any additional logic. Check the [scheme client docs](./scheme-clients.md) for more on this.

If possible, it's recommended to return one of the supported URI schemes and let Downlink handle it.

## Pattern matcher support

To make working with your backend easier and cut down on the logic you need to re-implement in your backend, it's recommended to use pattern matchers. To do this, import an `IEnumerable<IPatternMatcher>` in your constructor and use a configuration key, like `MyAwesomeStorage:MatchStrategy` to find a matching implementation. 

> [!WARNING]
> You will need to install the `Microsoft.Extensions.Configuration.Abstractions` and `Microsoft.Extensions.Configuration.Binder` packages

For example:

```csharp
public class MyAwesomeStorage : IRemoteStorage
{
    private readonly IPatternMatcher _matcher;

    public MyAwesomeStorage(
        Microsoft.Extensions.Configuration.IConfiguration config,
        IEnumerable<IPatternMatcher> matchers
    ) {
        var name = config.GetValue("MyAwesomeStorage:MatchStrategy", string.Empty);
        _matcher = matchers.GetFor(name); // this will be null if not found!
    }
    // trimmed
}
```

Now in your `GetFileAsync` method, use a pattern matcher if available, or use your own logic:

```csharp
public class MyAwesomeStorage : IRemoteStorage
{
    //trimmed (as above)
    public Task<IFileSource> GetFileAsync(VersionSpec version)
    {
        var files = GetMyAwesomeFiles(); // get a list of files from your storage backend
        var paths = files.Select(f => ToPath(f)); // see below!
        if (_matcher != null) { // use a matcher if present
            var matchingFile = paths.FirstOrDefault(p => _matcher.Match(p, version));
            return new MyAwesomeFileSource(matchingFile);
        }
    }
}
```

In order to make pattern matchers backend-agnostic, they accept a `Path` instance representing the relative path of the current file in the remote storage. Mapping a domain-specific object to a `Path` depends on your storage, but it is usually a relative path from your storage's "root" to the current file. Check the existing storage implementations for ideas.

## Adding and activating your storage backend

Once you've finished the logic for your new backend, you need to add it to the running Downlink app and activate it.

> [!TIP]
> If you're also using other components like scheme clients, you may want to consider packaging it as a [plugin](./plugins.md).

In your Downlink host, just add a call to `AddStorage` in the builder in `Startup.cs`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc()
        .AddDownlink(b => b.AddStorage<MyAwesomeStorage>());
}
```

and finally set the `Storage` configuration item to the value of your new implementation's `Name` property: *"Awesome"* in our example.