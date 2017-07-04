using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Downlink.Core;

namespace Downlink.Local
{
    public abstract class LocalFileMatchStrategy : IMatchStrategy<FileSystemInfo>
    {
        protected LocalFileMatchStrategy(string name) {
            Name = name;
        }
        public string Name { get; }

        public abstract Task<IFileSource> MatchAsync(System.Collections.Generic.IEnumerable<FileSystemInfo> items, VersionSpec version);
    }
}