using Downlink.Core;
using Shouldly;
using Xunit;
using System;

namespace Downlink.Tests
{
    public sealed class FileMetadataTests
    {
        [Fact]
        public void ShouldThrowOnNegativeSize() {
            double size = -1;
            var name = "name";

            Should.Throw<ArgumentOutOfRangeException>(() => new FileMetadata(size, name));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2147483648)]
        public void ShouldNotThrowForEmptyFileSize(double size) {
            var metadata = new FileMetadata(size, "name");

            metadata.ShouldNotBeNull();
            metadata.SizeInBytes.ShouldBe(size);
        }

        [Fact]
        public void ShouldThrowOnNullFileName() {
            var size = 1;

            Should.Throw<ArgumentNullException>(() => new FileMetadata(size, null));
        }

        [Fact]
        public void ShouldDefaultPublicToTrue() {
            var meta = new FileMetadata(1, "name");

            meta.Public.ShouldBeTrue();
        }
    }
}