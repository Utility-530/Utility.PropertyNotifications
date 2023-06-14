using System;
using System.Diagnostics.CodeAnalysis;

namespace Utility.Structs.FileSystem
{
    public ref struct Directory
    {
        public Path Value { get; }

        public Directory(string directory) : this(directory.AsSpan())
        {
            if (Validate(directory) is { Failed: { Count: { } failures } failed } valids && failures > 0)
                throw new MultiValidationException<string>(valids);
        }

        public Directory(ReadOnlySpan<char> directory)
        {
            this.Value = new Path(directory);
        }

        /// <summary>
        /// <a href="https://stackoverflow.com/questions/1395205/better-way-to-check-if-a-path-is-a-file-or-a-directory"></a>
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static MultiValidationResult<string> Validate(string path)
        {
            return GetValidator(path).Validate();
        }

        public static MultiValidater<string> GetValidator(string path)
        {
            return Path.GetValidator(path);
        }

        /// <summary>
        /// <a href="https://stackoverflow.com/questions/1395205/better-way-to-check-if-a-path-is-a-file-or-a-directory"></a>
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool DirectoryContainsInvalidCharacters(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            bool containedInvalidCharacters = false;

            for (int i = 0; i < path.Length; i++)
            {
                int n = path[i];
                if (
                    (n == 0x22) || // "
                    (n == 0x3c) || // <
                    (n == 0x3e) || // >
                    (n == 0x7c) || // |
                    (n < 0x20)    // the control characters
                  )
                {
                    containedInvalidCharacters = true;
                }
            }

            return containedInvalidCharacters;
        }

        /// <summary>
        ///     Implicitly converts the specified <paramref name="directory"/> to its underlying path string.
        /// </summary>
        /// <param name="directory">The path to be converted.</param>
        /// <returns>
        ///     The path's underlying path string or <see langword="null"/> if <paramref name="directory"/>
        ///     is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull("directory")]
        public static implicit operator string(Directory directory) => directory.Value.Value.ToString();

        /// <summary>
        ///     Implicitly converts the specified <paramref name="directory"/> to its underlying path string.
        /// </summary>
        /// <param name="directory">The path to be converted.</param>
        /// <returns>
        ///     The path's underlying path string or <see langword="null"/> if <paramref name="directory"/>
        ///     is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull("directory")]
        public static implicit operator Directory(string directory) => new Directory(directory);

        /// <summary>
        ///     Concatenates the two paths via the <see cref="Join(string)"/> method.
        ///     Please see <see cref="Join(string)"/> for details on the specifics of the concatenation.
        /// </summary>
        /// <param name="path1">The first path.</param>
        /// <param name="path2">The second path.</param>
        /// <returns>The resulting concatenated path.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="path1"/> or <paramref name="path2"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Concatenating the two paths results in a path with an invalid format.
        /// </exception>
        /// <seealso cref="Join(string)"/>
        /// <seealso cref="Join(Path)"/>
        public static Path operator /(Directory path1, FileName path2)
        {
            return path1.Value.Join(path2.Value);
        }
    }
}
