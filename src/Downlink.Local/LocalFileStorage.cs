using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core;
using Downlink.Core.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Downlink.Local
{
    public class LocalFileStorage : IRemoteStorage
    {
        private readonly ILogger<LocalFileStorage> _logger;
        private readonly bool _forceNameMatching;

        public string Name => "Local";

        public LocalFileStorage(
            IConfiguration configuration,
            IEnumerable<IPatternMatcher> matchers,
            IEnumerable<LocalFileMatchStrategy> strategies,
            ILogger<LocalFileStorage> logger
            )
        {
            _logger = logger;
            var path = configuration.GetValue(
                "LocalStorage:PackageRoot",
                Path.Combine(Directory.GetCurrentDirectory(), "Packages")
            );
            path = path.EndsWith(Path.DirectorySeparatorChar.ToString()) ? path : path + Path.DirectorySeparatorChar;
            _forceNameMatching = configuration.GetValue("LocalStorage:ForceNameMatching", false);
            var stratName = configuration.GetValue("LocalStorage:MatchStrategy", "Hierarchical");
            MatchStrategy = strategies.GetFor<LocalFileMatchStrategy, FileSystemInfo>(stratName);
            PatternMatcher = matchers.GetFor(stratName);
            _logger.LogInformation("Starting local file storage using path '{0}' and '{1}' matcher.", path, PatternMatcher?.Name);
            PackageRoot = new DirectoryInfo(path);
        }

        private DirectoryInfo PackageRoot { get; }
        private IPatternMatcher PatternMatcher { get; }
        private LocalFileMatchStrategy MatchStrategy { get; }
        public async Task<IFileSource> GetFileAsync(VersionSpec version)
        {
            if (!Directory.Exists(PackageRoot.FullName))
            {
                _logger.LogWarning($"Could not resolve path: '{PackageRoot}'");
                throw new VersionNotFoundException(PackageRoot.FullName);
            }
            var files = PackageRoot.GetFileSystemInfos("*", SearchOption.AllDirectories).ToList();
            if (!files.Any())
            {
                _logger.LogWarning("No files found in target root '{0}'", PackageRoot);
                throw new VersionNotFoundException();
            }
            if (MatchStrategy == null)
            {
                var matching = files
                    .OfType<FileInfo>()
                    .FirstOrDefault(f => PatternMatcher.Match(f.MakeRelativeTo(PackageRoot), version));
                if (matching == null) 
                {
                    throw new VersionNotFoundException($"No matches found from {files.Count} using {PatternMatcher?.Name} matching!");
                }
                return new LocalFileSource(version, matching);
            }
            var match = await MatchStrategy.MatchAsync(files, version);
            return match;
        }
    }
}
