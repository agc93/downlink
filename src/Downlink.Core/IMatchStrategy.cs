using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core.IO;

namespace Downlink.Core
{
    public interface IMatchStrategy<TObject> where TObject : class
    {
        string Name { get; }
        Task<IFileSource> MatchAsync(IEnumerable<TObject> items, VersionSpec version);
    }

    public static class MatchStrategyExtensions {
        public static T GetFor<T, TObject>(this IEnumerable<T> strategies, string name) where T : IMatchStrategy<TObject> where TObject : class {
            return strategies.FirstOrDefault(s => s.Name.ToLower() == name.ToLower());
        }

        public static IPatternMatcher GetFor(this IEnumerable<IPatternMatcher> patterns, string name) {
            return patterns.FirstOrDefault(s => s.Name.ToLower() == name.ToLower());
        }
    }
}