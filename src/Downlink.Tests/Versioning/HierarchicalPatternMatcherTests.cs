using System.Collections.Generic;
using System.Linq;
using Downlink.Core;
using Downlink.Core.Runtime;
using Shouldly;
using Xunit;

namespace Downlink.Tests.Versioning
{
    public sealed class HierarchicalPatternMatcherTests
    {
        [Fact]
        public void ShouldCreatePatternMatcher() {
            var match = new HierarchicalPatternMatcher();

            match.ShouldNotBeNull();
            match.Name.ShouldBe("Hierarchical");
        }

        [Theory, MemberData(nameof(MatchCases))]
        public void ShouldMatchVersion(string input, VersionSpec spec) {
            var matcher = new HierarchicalPatternMatcher();

            var result = matcher.Match(input, spec);

            result.ShouldBeTrue();
        }

        [Theory, MemberData(nameof(VersionedMatchCases))]
        public void ShouldMatchVersionWithNameMatching(string input, VersionSpec spec) {
            var matcher = new HierarchicalPatternMatcher(true);

            var result = matcher.Match(input, spec);

            result.ShouldBeTrue();
        }

        public static IEnumerable<object> MatchCases {
            get {
                var list = new object[] {
                    new object[] { "1/windows/any/file", new VersionSpec("1", "windows", null) },
                    //new object[] { "app_2_linux.file", new VersionSpec("2", "linux", "")}, // issue #2
                    new object[] { "/3/osx/x64.file", new VersionSpec("3", "osx", "x64")}, //default SpecParser only does _
                    new object[] { "/4/windows/x64/app-4-x64.exe", new VersionSpec("4", "windows", "x64")}
                };
                return VersionedMatchCases.Concat(list);
            }
        }

        public static IEnumerable<object> VersionedMatchCases {
            get {
                return new object[] {
                    new object[] { "v1/windows/any/app-v1.exe", new VersionSpec("v1", "windows", null) },
                    new object[] { "2.5/linux/ia32/myapp-2.5.1", new VersionSpec("2.5", "linux", "ia32")},
                    new object[] { "3/macos/ppc/osx-pkg_with_patch_3.dmg", new VersionSpec("3", "macos", "ppc")}
                };
            }
        }
    }
}