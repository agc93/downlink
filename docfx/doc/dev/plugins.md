# Building and using plugins

Downlink includes a built-in plugin system to easily extend Downlink with customised logic and support for additional services.

Note that plugins aren't the only way to extend Downlink! If you're [self hosting](./hosting.md) Downlink, you can use the `IDownlinkBuilder` overloads to quickly add individual components like scheme clients or storage backends individually. Unlike that method, plugins can be used to register *any* service and *any number* of services with Downlink in one hit. This makes plugins perfect for bundling linked components together (like a new storage backend with it's own version semantics).

> [!TIP]
> You can look at plugins as a "*bundle*" of extension points for Downlink, distributed and loaded together

## Plugin basics

At it's heart, a plugin consists of an `IDownlinkPlugin` implementation. That's what the `AddPlugin` method accepts, and is what the host uses to load up your plugin. In practice, the `IDownlinkPlugin` implementation should be your entry point, responsible for adding any other extension points your plugin is installing to the services container.

> [!TIP]
> It may be helpful to define your `IDownlinkPlugin` implementation in the `Downlink.Hosting` namespace. Your users won't need to add a `using` statement!

## Building a plugin

To get started building a plugin, create a new .NET Standard class library and add a reference to the `Downlink` and `Downlink.Core` packages. Next you'll want to add your plugin's resources. This will usually be things like a [storage backend](./storage.md), a pattern matcher, a [scheme client](./scheme-clients.md) or any other Downlink service such as a response handler. For this example, we're going to assume you've created a new storage backend called `MyAwesomeStorage` that needs a custom scheme client called `MyAwesomeSchemeClient`.

To package these into a plugin, create a new class (e.g. `MyAwesomePlugin`) and implement `IDownlinkPlugin` (from the `Downlink.Composition` namespace). For example:

```csharp
public class MyAwesomePlugin : IDownlinkPlugin
{
    public void AddServices(IDownlinkBuilder builder)
    {
    }
}
```

In the `AddServices` method, call the builder's `Add*` methods to add your new awesome services to the Downlink container. Note that if, for example, your storage backend also needed an instance of `MyAwesomeService` injected into the constructor you can add that directly to the container:

> [!WARNING]
> If you want to add services directly to the container (i.e. not using the Downlink builder methods), you'll need to install the `Microsoft.Extensions.DependencyInjection.Abstractions` package


```csharp
public void AddServices(IDownlinkBuilder builder)
{
    builder.Services.AddSingleton<MyAwesomeService>();
    builder.AddStorage<MyAwesomeStorage>();
    builder.AddSchemeClient<MyAwesomeSchemeClient>();
}
```

Now, to add your plugin to your self-hosted app, just add a call to `AddPlugin` to your app's `Startup.cs` in the `ConfigureServices` method.

```csharp
// in Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc()
        .AddDownlink(b => b.AddPlugin<MyAwesomePlugin>());
}
```

That's it! Your plugin's implementation of `IDownlinkBuilder` will automatically get invoked, adding your services to the container. Update your configuration as needed and start your app again to test it out.

## Default plugin discovery

The default Downlink host (the one packaged in the Docker image, and in binary packages) also supports an experimental new feature: local plugin discovery. What this means is that you can drop a `.dll` file in the app folder that matches the conventions and `Downlink.Host` will attempt to automatically locate and load it!

To make this an opt-in feature, there's two changes you need to make for this to work. First, the plugin assembly's file name must start with `Downlink.Extensions.` and be located in the app directory (that's `/downlink` for Docker users). Second, you must opt-in to this feature as it's currently experimental. This is done using the `Experimental:EnableLocalPlugins` variable.

Environment Variable: `DOWNLINK__Experimental__EnableLocalPlugins`

JSON:

```json
{
    "Experimental": {
        "EnableLocalPlugins": true
    }
}
```

YAML:

```yaml
Experimental:
  EnableLocalPlugins: true
```

If matching assemblies are found, they will be scanned for an `IDownlinkPlugin` implementation. If it finds one, that is loaded as a plugin and the rest of the assembly is ignored. If there is no plugin declared, Downlink will automatically scan for any storage backends (`IRemoteStorage`), pattern matchers (`IPatternMatcher`), response handlers (`IResponseHandler`) or scheme clients (`ISchemeClient`) and automatically load them into the app container.