// This implementation was borrowed unashamedly from the Cake project.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Downlink.Core.IO
{
    public class Path
    {
        private static readonly char[] _invalidPathCharacters;

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath { get; }

        /// <summary>
        /// Gets a value indicating whether this path is relative.
        /// </summary>
        /// <value>
        /// <c>true</c> if this path is relative; otherwise, <c>false</c>.
        /// </value>
        public bool IsRelative { get; }

        /// <summary>
        /// Gets the segments making up the path.
        /// </summary>
        /// <value>The segments making up the path.</value>
        public string[] Segments { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Path"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        protected Path(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be empty.", nameof(path));
            }

            // Validate the path.
            foreach (var character in path)
            {
                if (_invalidPathCharacters.Contains(character))
                {
                    const string format = "Illegal characters in path ({0}).";
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, format, character), nameof(path));
                }
            }

            FullPath = path.Replace('\\', '/').Trim();
            FullPath = FullPath == "./" ? string.Empty : FullPath;

            // Remove relative part of a path.
            if (FullPath.StartsWith("./", StringComparison.Ordinal))
            {
                FullPath = FullPath.Substring(2);
            }

            // Remove trailing slashes.
            FullPath = FullPath.TrimEnd('/', '\\');

            if (FullPath.EndsWith(":", StringComparison.OrdinalIgnoreCase))
            {
                FullPath = string.Concat(FullPath, "/");
            }

            // Relative path?
            IsRelative = !System.IO.Path.IsPathRooted(FullPath);

            // Extract path segments.
            Segments = FullPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (FullPath.StartsWith("/") && Segments.Length > 0)
            {
                Segments[0] = "/" + Segments[0];
            }
        }

        static Path()
        {
            _invalidPathCharacters = System.IO.Path.GetInvalidPathChars().Concat(new[] { '*', '?' }).ToArray();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this path.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return FullPath;
        }

        /// <summary>
        /// Gets the directory part of the path.
        /// </summary>
        /// <returns>The directory part of the path.</returns>
        public Path GetDirectory()
        {
            var directory = System.IO.Path.GetDirectoryName(FullPath);
            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = "./";
            }
            return new Path(directory);
        }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <returns>The filename.</returns>
        public string GetFilename()
        {
            var filename = System.IO.Path.GetFileName(FullPath);
            return filename;
        }

        /// <summary>
        /// Gets the filename without its extension.
        /// </summary>
        /// <returns>The filename without its extension.</returns>
        public string GetFilenameWithoutExtension()
        {
            var filename = System.IO.Path.GetFileNameWithoutExtension(FullPath);
            return filename;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Path"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A <see cref="Path"/>.</returns>
        public static implicit operator Path(string path)
        {
            return new Path(path);
        }

        public static implicit operator string(Path p)
        {
            return p.ToString();
        }

        /// <summary>
        /// Collapses a <see cref="Path"/> containing ellipses.
        /// </summary>
        /// <returns>A collapsed <see cref="Path"/>.</returns>
        public Path Collapse()
        {
            return new Path(PathCollapser.Collapse(this));
        }

        public void Normalize() {

        }

        internal static class PathCollapser
        {
            public static string Collapse(Path path)
            {
                if (path == null)
                {
                    throw new ArgumentNullException(nameof(path));
                }
                var stack = new Stack<string>();
                var segments = path.FullPath.Split('/', '\\');
                foreach (var segment in segments)
                {
                    if (segment == ".")
                    {
                        continue;
                    }
                    if (segment == "..")
                    {
                        if (stack.Count > 1)
                        {
                            stack.Pop();
                        }
                        continue;
                    }
                    stack.Push(segment);
                }
                string collapsed = string.Join("/", stack.Reverse());
                return collapsed == string.Empty ? "." : collapsed;
            }
        }
    }
}