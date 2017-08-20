

string BuildVersion(string fallbackVersion) {
    var PackageVersion = string.Empty;
    try {
        Information("Attempting GitVersion...");
        var versionInfo = GitVersion();
        PackageVersion = versionInfo.NuGetVersionV2;
    } catch {
        Information($"Falling back to version: {fallbackVersion}");
        PackageVersion = fallbackVersion;
    } finally {
        Information($"Building for version '{PackageVersion}'");
    }
    return PackageVersion;
}