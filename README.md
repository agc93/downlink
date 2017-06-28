# Downlink

## Introduction

Downlink is a simple API used to create predictable, stable and consistent download links for software releases/projects.

In short, Downlink lets you give a link to your end user like `http://myapp.com/v1.2/windows/x64` and Downlink will return version 1.2 of your app for 64-bit Windows, regardless of where your app binaries/packages/installers/images/artifacts are actually stored.

Downlink is an abstraction over your chosen release storage platform so your links are always dependable and predictable, even if you (or your chosen platform) change how you store your app.

Out of the box, Downlink supports serving packages from GitHub Releases (with a naming convention), Azure Storage, AWS S3 Storage, or the local filesystem of the Downlink server.

### Technology

Downlink is built entirely using ASP.NET Core and is supported on Windows, Linux and macOS, including a pre-built Docker image.

## Documentation

Complete documentation, including user and developer guides, are available [on GitHub](https://agc93.github.io/downlink/).