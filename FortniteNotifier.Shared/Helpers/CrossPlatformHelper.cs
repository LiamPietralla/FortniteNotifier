using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteNotifier.Shared.Helpers
{
    // Contains assorted methods to ease cross platform use of certain methods
    public static class CrossPlatformHelper
    {
        /// <summary>
        /// The Path split character for all platforms
        /// </summary>
        static char[] _pathSplitCharacters = new char[] { '/', '\\' };

        /// <summary>
        /// Path.Combine is not safe to use in a multi-platform environment. It does not check if inputs have the same path split, so can cause potential issues.
        /// This method will ensure that the path is split correctly for the current platform by taking in a known safe base path (hopefully generated using 'AppContext.BaseDirectory' or similar)
        /// and then combining it with the path(s) to be joined, by splitting on any path split characters. Each section is then combined using Path.Combine whcih will now return the correct output.
        /// </summary>
        /// <param name="basePath">The known valid base path</param>
        /// <param name="paths">Any combination of paths, of any format (e.g. Linux / Windows)</param>
        /// <returns></returns>
        public static string PathCombine(string basePath, params string[] paths)
        {
            string[][] splits = paths.Select(s => s.Split(_pathSplitCharacters)).ToArray();
            int totalLength = splits.Sum(arr => arr.Length);
            string[] segments = new string[totalLength + 1];
            segments[0] = basePath;
            int i = 0;
            foreach (var split in splits)
            {
                foreach (var value in split)
                {
                    i++;
                    segments[i] = value;
                }
            }
            return Path.Combine(segments);
        }
    }
}
