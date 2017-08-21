using System.Collections.Generic;
using Downlink.Core;
using Downlink.Core.Runtime;
using Shouldly;
using Xunit;

namespace Downlink.Tests.Versioning
{
    public class RuntimePatternMatcherTests
    {
        [Fact]
        public void ShouldCreatePatternMatcher() {
            var match = new RuntimePatternMatcher();

            match.ShouldNotBeNull();
            match.Name.ShouldBe("Runtime");
        }

        [Theory, MemberData(nameof(MatchCases))]
        public void ShouldMatchVersion(string input, VersionSpec spec) {
            var matcher = new RuntimePatternMatcher();

            var result = matcher.Match(input, spec);

            result.ShouldBeTrue();
        }

        [Theory, MemberData(nameof(VersionedMatchCases))]
        public void ShouldMatchVersionWithNameMatching(string input, VersionSpec spec) {
            var matcher = new RuntimePatternMatcher(true);

            var result = matcher.Match(input, spec);

            result.ShouldBeTrue();
        }

        public static IEnumerable<object> MatchCases {
            get {
                return new object[] {
                    new object[] { "/1/win10-x64/app.exe", new VersionSpec("1", "win10", "x64")},
                    new object[] { "/2.1/dotnet-any/Downlink.dll", new VersionSpec("2.1", "dotnet", null)},
                    new object[] { "/3/osx.10.12-x64/x64.file", new VersionSpec("3", "osx.10.12", "x64")},
                    new object[] { "/4/debian.8-x64/app-4-x64.tgz", new VersionSpec("4", "debian.8", "x64")}
                };
            }
        }

        public static IEnumerable<object> VersionedMatchCases {
            get {
                return new object[] {
                    new object[] { "v1/win7-x32/app-v1.exe", new VersionSpec("v1", "win7", "x32") },
                    new object[] { "2.5/linux-arm/myapp-2.5.1", new VersionSpec("2.5", "linux", "arm")},
                    new object[] { "3/osx.10.11-x64/osx-pkg_with_patch_3.dmg", new VersionSpec("3", "osx.10.11", "x64")}
                };
            }
        }
    }
}