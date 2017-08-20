# Routing in Downlink

Downlink by default has a very aggressive route template: `{version}/{platform?}/{arch?}` meaning it will often match routes you didn't want it to.

To correct this, Downlink (since 0.2) includes support for **prefixing** the route template used for Downlink's actions.

## Configuration

> [!TIP]
> This method is also supported in the Docker image (even though it is not generally needed)

By default, Downlink will look for a configuration key called `DownlinkPrefix` and prepend the value of that to Downlink's action routes:

```yaml
DownlinkPrefix: 'download'
```

```json
{
    "DownlinkPrefix": "download"
}
```

This will change the template to `download/{version}/{platform?}/{arch?}`, effectively isolating Downlink form other routes in your application.

## Code (for self-hosting)

If you're hosting Downlink, you can also add a prefix in your startup code:

```csharp
// in Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().AddDownlink(d => d.UseRoutePrefix("download"));
}
```

This will result in the same route template as the first example.

Internally, this is replacing the `ConfigurationRoutePrefixBuilder` with the `StaticRoutePrefixBuilder`.

## Extensibility

This functionality is specifically designed for extensibility and is built around a super-simple `IRoutePrefixBuilder` interface:

```csharp
string GetPrefix();
```

If you have your own implementation of the `IRoutePrefixBuilder` interface, register it using the `UseRouteBuilder` builder method and Downlink will invoke it instead on app start:

```csharp
// in Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().AddDownlink(d => d.UseRouteBuilder<MyAwesomeBuilder>());
}
```

### Advanced extensibility

> [!WARNING]
> This is only recommended for advanced users who really know what they're doing: you can completely break your app this way.

If you need super-finegrained control over Downlink's routing behaviour, you can also re-implement the convention used for Downlink routing. This is mostly useful if you need to make non-trivial edits to the route template used by Downlink.

Downlink provides the `IDownlinkRouteConvention` interface to override the conventions applied on app start. If you register an implementation of `IDownlinkRouteConvention` in the Downlink builder, it will be added to the app's convention map and applied on app start.

> [!WARNING]
> Your `IDownlinkRouteConvention` implementation will be applied to **the whole app**, not just Downlink controllers and actions.

You can find the default convention implementation in [`DownlinkRouteConvention`](xref:DownlinkRouteConvention).