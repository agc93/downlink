# Development Guide

> [!TIP]
> You can check the [Developer Reference](../api/index.md) above for full API source reference, directly from the source code.

## Building

Building this project is super-simple: run `build.ps1` if you're on Windows, or `build.sh` if you're on Linux.

> To build the documentation, you may also need `wkhtmltopdf` installed.

### Packaging

To do a complete build of the tool *and* build all packages, you will need to have Docker installed and available on your host (and accessible to the user running the build script). The build script will complete all the packaging steps in a series of Docker containers so this may be a slow process the first time you run it as the relevant images are pulled from the Hub.

## Introduction

Downlink has a lot of moving parts, so the code base may appear quite daunting at first, but fear not! While there are a lot of individual components, they are mostly quite simple and a lot of the plumbing/boilerplate is already taken care of by framework code.

### Extension points

Downlink does include some specifically designed extensions points, but the  `IRemoteStorage` and `ISchemeClient` interfaces are the ones most developers will be interacting with, and are explained in more detail below.

## Storage

Downlink is primarily a gateway which means the app artifacts must actually be stored on a remote backend. In Downlink, that backend is an implementation of the `IRemoteStorage` interface, a simple abstraction over any storage service.

The `IRemoteStorage` interface is deliberately simplistic, relying on returning a single `Uri` for the requested version, or throwing an implementation of `NotFoundException` (from the `Downlink.Core.Diagnostics` namespace) if it can't find a match.

Returning a `Uri` rather than the artifact itself enables Downlink's built-in proxying support allowing for end users to hit the storage backend directly, or to proxy everything through Downlink itself.

### Storage URIs

Note that the `Uri` returned from an `IRemoteStorage` implementation can be any valid URI, and it will be passed directly to the response handler in Downlink itself (an `IResponseHandler` implementation). Out of the box, URIs with the 'http', 'https' or 'file' URIs are automatically handled and returned to the user. If Downlink doesn't know how to handle a URI scheme, it will return a *412 Precondition Failed* status.

To add support for another scheme (most often if you are also developing a new backend), you need to implement `ISchemeClient`. To make it easier, you can inherit from `SchemeClient` which takes out a bit of boilerplate.

Your `ISchemeClient` implementation will need to return an `IActionResult` (just like a Controller action method), and that will be returned to the user. Note that your scheme client will *only* be invoked for the schemes it supports.

## Dependency Injection

This app includes dependency injection support for most of the framework and app code. Simply include the types you need in your constructor, and the app will attempt to resolve this dependency from the DI container. Note this will only work where you have registered your types with the container (such as in `Startup.cs`).