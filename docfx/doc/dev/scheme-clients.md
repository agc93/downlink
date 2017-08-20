# Scheme Clients

As outlined in the storage docs, the storage backend is responsible for simply returning a URI where Downlink can find the requested artifact. Downlink uses *scheme clients* to resolve those URIs back to a result that can be returned to a user. The scheme client is invoked by the response handler, but you will likely not need to interact directly with response handlers, and it's best to leave the default handlers in place.

Out of the box, Downlink includes logic to correctly handle any `file://`, `http://` or `https://` scheme returned from a storage backend. To add support for another scheme (most often if you are also developing a new backend), you need to implement `ISchemeClient`. To make it easier, you can inherit from `SchemeClient` which takes out a bit of boilerplate.

Your `ISchemeClient` implementation will need to return an `IActionResult` (just like a Controller action method), and that will be returned to the user. Note that your scheme client will *only* be invoked for the schemes it explicitly supports.

> [!TIP]
> You'll need to install the `Microsoft.AspNetCore.Mvc.Abstractions` package as well.

## Example

Scheme clients are generally quite simple. For example, here is the scheme client for handling `file://` URIs (included in Downlink):

```csharp
public class FileSchemeClient : SchemeClient
{
    public FileSchemeClient() : base("file") { }
    public override Task<IActionResult> GetContentAsync(IFileSource file)
    {
        return Task.FromResult(new FileStreamResult(System.IO.File.OpenRead(file.FileUri.AbsolutePath), "application/octet-stream") as IActionResult);
    }
}
```

or the client for handling `http://`/`https://` URIs (also included by default):

```csharp
// trimmed and simplified for brevity and readability
public class HttpDownloadClient : SchemeClient
{
    public HttpDownloadClient(ILogger<HttpDownloadClient> logger) : base(new[] { "http", "https" })
    {
        DownloadLocation = System.IO.Path.GetTempPath();
    }

    public string DownloadLocation { get; private set; }

    public override async Task<IActionResult> GetContentAsync(IFileSource file)
    {
        using (var client = new HttpClient())
        {
            var fileStream = await client.GetStreamAsync(requestUri);
        }
        return new FileStreamResult(fileStream, "application/octet-stream") {
            FileDownloadName = file.Metadata.FileName
        };
    }
}
```

## Activating new scheme clients

New scheme client implementations needed to be added to Downlink either through inclusion in a [plugin](./plugins.md), or using the builder in `Startup.cs`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc()
        .AddDownlink(b => b.AddSchemeClient<MyAwesomeSchemeClient>());
}
```

Downlink will then call your scheme client when any storage backend returns a URI with your client's scheme.

> [!NOTE]
> Your scheme client will **only** be invoked for URIs that you explicitly support (in the `Schemes` property, or the constructor for `SchemeClient` implementations)