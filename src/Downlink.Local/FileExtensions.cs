using System;
using System.IO;

namespace Downlink.Local
{
    internal static class FileExtensions
    {
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="basePath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="targetPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        internal static string MakeRelativeTo(this string targetPath, string basePath)
        {
            if (string.IsNullOrEmpty(basePath)) throw new ArgumentNullException(nameof(basePath));
            if (string.IsNullOrEmpty(targetPath)) throw new ArgumentNullException(nameof(targetPath));

            Uri fromUri = new Uri(basePath);
            Uri toUri = new Uri(targetPath);

            if (fromUri.Scheme != toUri.Scheme) { return targetPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        internal static string MakeRelativeTo(this FileSystemInfo targetFile, string basePath) {
            return targetFile.FullName.MakeRelativeTo(basePath);
        }

        internal static string MakeRelativeTo(this string targetFile, FileSystemInfo basePath) {
            return targetFile.MakeRelativeTo(basePath.FullName);
        }

        internal static string MakeRelativeTo(this FileSystemInfo targetFile, FileSystemInfo basePath) {
            return targetFile.FullName.MakeRelativeTo(basePath.FullName);
        }
    }
}