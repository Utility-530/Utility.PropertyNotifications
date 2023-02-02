using Files.Shared.PhysicalPath.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Structs.Infrastructure
{
    public static class PathHelper
    {

        /// <summary>
        /// <a href="https://stackoverflow.com/questions/1395205/better-way-to-check-if-a-path-is-a-file-or-a-directory"></a>
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsFileNameCharactersValid(ReadOnlySpan<char> filename)
        {
            bool containedInvalidCharacters = false;

            for (int i = 0; i < filename.Length; i++)
            {
                int n = filename[i];
                if (
                    (n == 0x22) || // "
                    (n == 0x3c) || // <
                    (n == 0x3e) || // >
                    (n == 0x7c) || // |
                    (n == 0x3a) || // : 
                    (n == 0x2a) || // * 
                    (n == 0x3f) || // ? 
                    (n == 0x5c) || // \ 
                    (n == 0x2f) || // /
                    (n < 0x20)    // the control characters
                  )
                {
                    containedInvalidCharacters = true;
                }
            }

            return containedInvalidCharacters == false;
        }

        public static ReadOnlySpan<char> GetNameWithoutExtension(ReadOnlySpan<char> name)
        {
            // Specification requires special handling for these two directories.
            // Without this code, we'd return "" and ".", because Path.GetFileNameWithoutExtension
            // trims one dot.
            if (name == PhysicalPathHelper.CurrentDirectorySegment ||
                name == PhysicalPathHelper.ParentDirectorySegment)
            {
                return name;
            }
            return System.IO.Path.GetFileNameWithoutExtension(name);
        }
    }
}
