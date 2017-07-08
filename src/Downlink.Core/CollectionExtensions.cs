using System.Collections.Generic;
using System.Linq;

namespace Downlink.Core
{
    public static class CollectionExtensions {
        public static T GetFor<T, TObject>(this IEnumerable<T> strategies, string name) where T : IMatchStrategy<TObject> where TObject : class {
            return strategies.FirstOrDefault(s => s.Name.ToLower() == name.ToLower());
        }

        public static IPatternMatcher GetFor(this IEnumerable<IPatternMatcher> patterns, string name) {
            return patterns.FirstOrDefault(s => s.Name.ToLower() == name.ToLower());
        }

        public static IRemoteStorage GetStorageFor(this IEnumerable<IRemoteStorage> storage, string name) {
            return storage.FirstOrDefault(s => s.Name.ToLower() == name.ToLower())
                ?? storage.FirstOrDefault(s => s.Name.Replace(" ", string.Empty).Trim() == name.ToLower());
        }
    }
}