using Downlink.Core;
using Shouldly;
using Xunit;

namespace Downlink.Tests
{
    public sealed class VersionSpecTests
    {
        public sealed class TheConstructorTests
        {
            [Theory]
            [InlineData("")]
            [InlineData("platform")]
            public void ShouldNotThrowForEmptyPlatform(string platform)
            {
                var spec = new VersionSpec("version", platform, string.Empty);

                spec.ShouldNotBeNull();
                spec.Platform.ShouldBe(platform);
            }

            [Theory]
            [InlineData("")]
            [InlineData("arch")]
            public void ShouldNotThrowForEmptyArchitecture(string arch)
            {
                var spec = new VersionSpec("version", "platform", arch);
                spec.ShouldNotBeNull();
                spec.Architecture.ShouldBe(arch);
            }

            [Fact]
            public void ShouldSetNullPlatform()
            {
                var spec = new VersionSpec("version", null, string.Empty);
                spec.ShouldNotBeNull();
                spec.Platform.ShouldNotBeNull();
                spec.Platform.ShouldBe("any");
            }

            [Fact]
            public void ShouldSetNullArchitecture()
            {
                var spec = new VersionSpec("version", string.Empty, null);
                spec.ShouldNotBeNull();
                spec.Architecture.ShouldNotBeNull();
                spec.Architecture.ShouldBe("any");
            }

        }

        public sealed class TheSummaryTests
        {
            [Theory]
            [InlineData("1", "windows", "64")]
            [InlineData("2", "linux", "arm")]
            public void ShouldReturnCorrectSummary(string version, string platform, string architecture) {
                var spec = new VersionSpec(version, platform, architecture);

                var summ = spec.Summary;

                summ.ShouldContain(version);
                summ.ShouldContain($"{platform}/{architecture}");
            }

            [Fact]
            public void ShouldSpecifyUnknownPlatform() {
                var spec = new VersionSpec("version", "", "arch");

                var summ = spec?.Summary;

                summ.ShouldNotBeNullOrWhiteSpace();
                summ.ShouldContain("unknown/arch");
            }

            [Fact]
            public void ShouldSpecifyUnknownArchitecture() {
                var spec = new VersionSpec("version", "platform", "");

                var summ = spec?.Summary;

                summ.ShouldNotBeNullOrWhiteSpace();
                summ.ShouldContain("platform/unknown");
            }
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1.0.0-alpha")]
        public void ShouldImplicitlyConvertToString(string version)
        {
            var spec = new VersionSpec(version, "platform", "arch");

            string impl = spec;

            impl.ShouldBe(version);
        }

        [Theory]
        [InlineData("1", "", "")]
        [InlineData("2.0.0-beta", "windows", "x64")]
        public void ShouldReturnVersionStringFromToString(string version, string platform, string arch)
        {
            VersionSpec spec = new VersionSpec(version, platform, arch);
            spec.ShouldNotBeNull();
            var str = spec.ToString();
            str.ShouldBe(version);
        }
    }
}