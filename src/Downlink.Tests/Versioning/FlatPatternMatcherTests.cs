using System.Collections.Generic;
using Downlink.Core;
using Downlink.Core.Runtime;
using Shouldly;
using Xunit;

namespace Downlink.Tests.Versioning
{
    public sealed class FlatPatternMatcherTests
    {
        [Fact]
        public void ShouldCreatePatternMatcher() {
            var match = new FlatPatternMatcher();

            match.ShouldNotBeNull();
            match.Name.ShouldBe("Flat");
        }

        [Theory, MemberData(nameof(MatchCases))]
        public void ShouldMatchVersion(string input, VersionSpec spec) {
            var matcher = new FlatPatternMatcher();

            var result = matcher.Match(input, spec);

            result.ShouldBeTrue();
        }

        public static IEnumerable<object> MatchCases {
            get {
                return new object[] {
                    new object[] { "app_1_windows_any", new VersionSpec("1", "windows", null) },
                    //new object[] { "app_2_linux.file", new VersionSpec("2", "linux", "")}, // issue #2
                    new object[] { "/path/app_3_osx_x64", new VersionSpec("3", "osx", "x64")}, //default SpecParser only does _
                    new object[] { "/path/app-name-1-and-more_4_win_arm.exe", new VersionSpec("4", "win", "arm")}
                };
            }
        }
    }
}