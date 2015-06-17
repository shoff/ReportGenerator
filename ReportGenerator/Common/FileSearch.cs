﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Palmmedia.ReportGenerator.Common
{
    /// <summary>
    /// Searches files based on file pattern with support for wildcards.
    /// </summary>
    public static class FileSearch
    {
        /// <summary>
        /// Gets the files matching the given file pattern..
        /// </summary>
        /// <param name="pattern">The file pattern.</param>
        /// <returns>The files.</returns>
        public static IEnumerable<string> GetFiles(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentException("Pattern must not be empty.", "pattern");
            }

            if (pattern.Intersect(Path.GetInvalidPathChars()).Any())
            {
                throw new ArgumentException("Pattern contains invalid character.", "pattern");
            }

            pattern = pattern.Replace('/', Path.DirectorySeparatorChar);

            bool pathRooted = Path.IsPathRooted(pattern);

            if (!pathRooted)
            {
                pattern = Path.Combine(Directory.GetCurrentDirectory(), pattern);
            }

            string[] parts = pattern.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            if (pathRooted && parts.Length < 2)
            {
                throw new ArgumentException("Pattern in no valid file pattern.", "pattern");
            }

            bool directoryIsUNCPath = pattern.StartsWith(@"\\", StringComparison.Ordinal);

            if (directoryIsUNCPath)
            {
                parts[0] = @"\\" + parts[0];
            }

            if (parts[0].EndsWith(":"))
            {
                parts[0] = parts[0] + Path.DirectorySeparatorChar;
            }

            string[] directoryParts = parts.Take(parts.Length - 1).ToArray();

            string filePattern = parts.Last();

            foreach (string directory in GetDirectories(directoryParts[0], directoryParts, 1, directoryIsUNCPath))
            {
                if (Directory.Exists(directory))
                {
                    foreach (string file in Directory.EnumerateFiles(directory, filePattern))
                    {
                        yield return file;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the directories matching the given directory parts.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="directoryParts">The directory parts.</param>
        /// <param name="currentIndex">Index of the current part.</param>
        /// <param name="directoryIsUNCPath">if set to <c>true</c> directory is UNC path.</param>
        /// <returns>The directories.</returns>
        private static IEnumerable<string> GetDirectories(string directory, string[] directoryParts, int currentIndex, bool directoryIsUNCPath)
        {
            // UNC paths need special treatment Directory.Exists("\\SomeUNCPath") returns false if no subdirectory is specified.
            if (!Directory.Exists(directory) && !(currentIndex == 1 && directoryIsUNCPath))
            {
                yield break;
            }

            if (currentIndex >= directoryParts.Length)
            {
                yield return directory;
            }
            else if (directoryParts[currentIndex].Contains("*"))
            {
                var subDirectories = Directory.EnumerateDirectories(directory, directoryParts[currentIndex]);

                foreach (var subDirectory in subDirectories)
                {
                    var subsubDirectories = GetDirectories(subDirectory, directoryParts, currentIndex + 1, directoryIsUNCPath);

                    foreach (var subsubDirectory in subsubDirectories)
                    {
                        yield return subsubDirectory;
                    }
                }
            }
            else
            {
                directory = Path.Combine(directory, directoryParts[currentIndex]);

                var subsubDirectories = GetDirectories(directory, directoryParts, currentIndex + 1, directoryIsUNCPath);

                foreach (var subsubDirectory in subsubDirectories)
                {
                    yield return subsubDirectory;
                }
            }
        }
    }
}
