# Hosting Downlink

Want to use Downlink without a separate site? Want to integrate Downlink into an existing ASP.NET Core app? Now you can!

Downlink has been built to integrate cleanly with ASP.NET MVC Core. When you run Downlink directly (such as from the Docker image), you are actually running aginst [Downlink.Host](xref:Downlink.Startup), which is a very basic ASP.NET Core app with Downlink pre-configured for use.

Note that Downlink requires MVC, so plain ASP.NET Core (without MVC installed) is not supported.

## Getting Started

The easiest way of understanding Downlink's hosting model is to show it.

### Example

To quickly setup a Downlink instance, create a new empty ASP.NET MVC Core app (such as using `dotnet new empty`). Once that has finished, you should have a `Program.cs` with the basic boilerplate code. First, add `ConfigureDownlink()` before the `Build()` method is called. You may need to add `using Downlink.Hosting` to your namespaces.


```csharp
// in Program.cs
public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureDownlink() // <-- add this line!
            .UseStartup<Startup>()
            .Build();
}
```

This registers the Downlink configuration defaults with the app.

Next, in your `Startup.cs` file, just add the `AddDownlink()` method to your `ConfigureServices` method. Again, you may need to add `using Downlink.Hosting` to your namespace imports.

```csharp
// in Startup.cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc().AddDownlink(); // <-- add AddDownlink() after the AddMvc() call
        // services.AddMvcCore().AddDownlink(); // Downlink also supports the AddMvcCore equivalent
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseMvc(); // you need this line if it is not already here
    }
}
```

**That's it!** You've just added Downlink to a new ASP.NET MVC Core app. Run the app and the Downlink parts will be automatically registered, and you can browse to `myapp:5000/version-here/platform-here/arch-here` to get your app downloads. Note that we haven't added any of the configuration Downlink needs so make sure to follow the configuration steps outlined in the [Getting Started guide](../user/getting-started.md) and the [Configuration guide](../user/configuration.md).