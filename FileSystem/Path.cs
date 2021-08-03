using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using UtilityStruct.FileSystem;
using UtilityStruct.Helper;
using static Files.Shared.PhysicalPath.Utilities.PathPolyfills;
using Files.Shared;
using Files.Shared.PhysicalPath.Utilities;

namespace UtilityStruct.FileSystem
{
    static class PathConstants
    {
        public const int MAXPATH = 260;

        public const char DirectorySeparatorChar = '/';
    }

    public ref struct Path
    {
        public enum ErrorCode
        {
            NullOrWhiteSpace,
            MaxPathLength,
            InvalidCharacters,
            OnlyContainsVolumeDesignator,
            BeginsWithVolumeDesignator,
            UnattachedVolumeDesignator,
        }

        public class Validator : Validater<string, ErrorCode>
        {
            public Validator(string value, ErrorCode code, Expression<Func<string, bool>> lambda) : base(value, code, lambda)
            {
            }
        }

        public Path(string path) : this(path.AsSpan())
        {
            if (Validate(path) is { Failed: { Count: { } failures } failed } valids && failures > 0)
                throw new MultiValidationException<string>(valids);
        }

        public Path(ReadOnlySpan<char> path)
        {
            Value = path;
            Name = System.IO.Path.GetFileName(path);
            Parent = System.IO.Path.GetDirectoryName(path);
            Root = System.IO.Path.GetPathRoot(path);
            EndsWithDirectorySeparator = PathPolyfills.EndsWithDirectorySeparator(path);
        }
        public ReadOnlySpan<char> Value { get; }

        private ReadOnlySpan<char> Root { get; }

        public ReadOnlySpan<char> Name { get; }

        public ReadOnlySpan<char> Parent { get; }

        public bool EndsWithDirectorySeparator { get; }

        public bool IsAbsolute => Value[0] == PathConstants.DirectorySeparatorChar;

        /// <summary>
        /// Gets a value indicating whether this path is relative by **not** starting with a leading `/`.
        /// </summary>
        /// <value><c>true</c> if this instance is relative; otherwise, <c>false</c>.</value>
        public bool IsRelative => !IsAbsolute;

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
            return new MultiValidater<string>(path,
                new[] {
                new Validator(path, ErrorCode.NullOrWhiteSpace, a=>!string.IsNullOrWhiteSpace(a)),
                new Validator(path, ErrorCode.MaxPathLength, a=>a.Length <= PathConstants.MAXPATH),
                new Validator(path, ErrorCode.OnlyContainsVolumeDesignator, a=>(a!= ":")),
                new Validator(path, ErrorCode.BeginsWithVolumeDesignator, a=>a[0]!=':'),
                new Validator(path, ErrorCode.OnlyContainsVolumeDesignator, a=>!(a.Contains(":") && a.IndexOf(':') != 1)),
                });
        }


        /// <summary>
        ///     Attempts to trim one trailing directory separator character from this path and
        ///     return the resulting path.
        /// </summary>
        /// <param name="result">
        ///     An <see langword="out"/> parameter which will, if the operation succeedes,
        ///     hold the new <see cref="Path"/> where one trailing directory separator has been
        ///     trimmed.
        ///     If this path doesn't have a trailing directory separator character, the same path
        ///     instance is returned.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the operation succeeded; <see langword="false"/> if not.
        /// </returns>
        public bool TryTrimEndingDirectorySeparator([NotNullWhen(true)] out Path result)
        {
            if (!EndsWithDirectorySeparator)
            {
                result = this;
                return true;
            }

            try
            {
                result = new Path(PathPolyfills.TrimEndingDirectorySeparator(this.Value));
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        ///     Trims one trailing directory separator character from this path and returns the
        ///     resulting path.
        ///     See remarks for details.
        /// </summary>
        /// <returns>
        ///     A new <see cref="Path"/> instance where one trailing directory separator has been
        ///     trimmed.
        ///     If this path doesn't have a trailing directory separator character, the same path
        ///     instance is returned.
        /// </returns>
        /// <remarks>
        ///     As described, the method trims exactly one trailing directory separator character
        ///     from the path (if one exists - otherwise, the same path is returned).
        ///     
        ///     In comparison to .NET's <c>System.IO.Path.TrimEndingDirectorySeparator(string path)</c>
        ///     method, <see cref="TrimEndingDirectorySeparator"/> also trims a directory separator
        ///     character if the path is a root path.
        ///     This can lead to trouble in certain scenarios, for example if the path is <c>"/"</c> on
        ///     Unix. Trimming this character would result in an empty path <c>""</c> which is illegal.
        ///     In such cases (and other scenarios where an invalid path is the result of trimming)
        ///     this method throws an <see cref="InvalidOperationException"/>.
        ///     If you are unsure whether trimming a path is possible, consider using
        ///     <see cref="TryTrimEndingDirectorySeparator(out Path?)"/>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     Trimming the trailing directory separator character is not possible because the
        ///     resulting path is invalid.
        /// </exception>
        private Path TrimEndingDirectorySeparatorImpl()
        {
            if (!EndsWithDirectorySeparator)
            {
                return this;
            }

            if (Value.Length == 1)
            {
                throw new InvalidOperationException(ExceptionStrings.StoragePath.TrimmingResultsInEmptyPath());
            }

            var trimmedPath = Value[0..^1];


            return new Path(trimmedPath);
        }

        ///// <summary>
        /////     Attempts to append the specified <paramref name="part"/> to the end of this path and
        /////     return the resulting path.
        ///// </summary>
        ///// <param name="part">
        /////     The part to be appended to the path.
        ///// </param>
        ///// <param name="result">
        /////     An <see langword="out"/> parameter which will, if the operation succeedes,
        /////     hold a new <see cref="Path"/> instance which represents the path after appending the
        /////     specified <paramref name="part"/>.
        ///// </param>
        ///// <returns>
        /////     <see langword="true"/> if the operation succeeded; <see langword="false"/> if not.
        ///// </returns>
        //public bool TryAppend(string? part, [NotNullWhen(true)] out Path result)
        //{
        //    if (part is null)
        //    {
        //        result = default;
        //        return false;
        //    }

        //    try
        //    {
        //        result = Append(part);
        //        return true;
        //    }
        //    catch
        //    {
        //        result = default;
        //        return false;
        //    }
        //}

        ///// <summary>
        /////     Appends the specified <paramref name="part"/> to the end of this path.
        ///// </summary>
        ///// <param name="part">
        /////     The part to be appended to the path.
        ///// </param>
        ///// <returns>
        /////     A new <see cref="Path"/> instance which represents the path after appending the
        /////     specified <paramref name="part"/>.
        ///// </returns>
        ///// <exception cref="ArgumentNullException">
        /////     <paramref name="part"/>
        ///// </exception>
        ///// <exception cref="ArgumentException">
        /////     Appending <paramref name="part"/> would result in an invalid path.
        ///// </exception>
        //public Path Append(ReadOnlySpan<char> part)
        //{
        //    if (part.Length == 0)
        //    {
        //        return this;
        //    }

        //    return new Path(SpanHelper.Concat(Value, part));
        //}

        /// <inheritdoc cref="TryCombine(string?, out Path?)"/>
        public bool TryCombine(Path other, [NotNullWhen(true)] out Path result) =>
            TryCombine(other, out result);

        ///// <summary>
        /////     Attempts to concatenate the two paths while also ensuring that <i>at least one</i> directory separator
        /////     character is inserted between them.
        /////     
        /////     If <paramref name="other"/> is rooted or starts with a directory separator character,
        /////     this path is discarded and the resulting path will simply be <paramref name="other"/>.
        /////     
        /////     See remarks of <see cref="Combine(string)"/> for details and examples.
        ///// </summary>
        ///// <param name="other">
        /////     Another path to be concatenated with this path.
        ///// </param>
        ///// <param name="result">
        /////     An <see langword="out"/> parameter which will, if the operation succeedes,
        /////     hold the resulting concatenated path.
        ///// </param>
        ///// <returns>
        /////     <see langword="true"/> if the operation succeeded; <see langword="false"/> if not.
        ///// </returns>
        //public bool TryCombine(ReadOnlySpan<char> other, [NotNullWhen(true)] out Path result)
        //{
        //    try
        //    {
        //        result = Combine(new Path(other));
        //        return true;
        //    }
        //    catch
        //    {
        //        result = default;
        //        return false;
        //    }
        //}


        ///// <summary>
        /////     Concatenates the two paths while also ensuring that <i>at least one</i> directory separator
        /////     character is present between them.
        /////     
        /////     If <paramref name="other"/> is rooted (i.e. it starts with a directory separator character),
        /////     this path is discarded and the resulting path will simply be <paramref name="other"/>.
        /////     
        /////     See remarks for details and examples.
        ///// </summary>
        ///// <param name="other">
        /////     Another path to be concatenated with this path.
        ///// </param>
        ///// <returns>The resulting concatenated path.</returns>
        ///// <remarks>
        /////     This method behaves like .NET's <see cref="System.IO.Path.Combine(string, string)"/>
        /////     method.
        /////     
        /////     In comparison to the alternatives (<see cref="Join(string)"/> and <see cref="Link(string)"/>),
        /////     <see cref="Combine(string)"/> discards this path if <paramref name="other"/> is rooted or
        /////     starts with a directory separator character.
        /////     Out of the three methods, <see cref="Combine(string)"/> is the method that might
        /////     remove the most information from the two specified paths.
        /////     
        /////     The following code demonstrates the behavior of <see cref="Combine(string)"/>:
        /////     
        /////     <code>
        /////     // Note: The code assumes that / is the file system's directory separator.
        /////     
        /////     Path first = fs.GetPath("firstPath");
        /////     first.Combine("secondPath");    // Returns "firstPath/secondPath".
        /////     first.Combine("/secondPath");   // Returns "/secondPath".
        /////     first.Combine("///secondPath"); // Returns "///secondPath".
        /////     
        /////     first = fs.GetPath("firstPath/");
        /////     first.Combine("secondPath");    // Returns "firstPath/secondPath".
        /////     first.Combine("/secondPath");   // Returns "/secondPath".
        /////     first.Combine("///secondPath"); // Returns "///secondPath".
        /////     
        /////     first = fs.GetPath("firstPath///");
        /////     first.Combine("secondPath");    // Returns "firstPath///secondPath".
        /////     first.Combine("/secondPath");   // Returns "/secondPath".
        /////     first.Combine("///secondPath"); // Returns "///secondPath".
        /////     
        /////     first = fs.GetPath("firstPath");
        /////     first.Join("");              // Returns "firstPath".
        ///// 
        /////     first = fs.GetPath("/");
        /////     first.Join("");              // Returns "/".
        /////     first.Join("/");             // Returns "/".
        /////     first.Join("//");            // Returns "//".
        /////     
        /////     first = fs.GetPath("//");
        /////     first.Join("");              // Returns "//".
        /////     first.Join("/");             // Returns "/".
        /////     first.Join("//");            // Returns "//".
        /////     </code>
        ///// </remarks>
        ///// <exception cref="ArgumentNullException">
        /////     <paramref name="other"/> is <see langword="null"/>-
        ///// </exception>
        ///// <exception cref="ArgumentException">
        /////     Concatenating the two paths results in a path with an invalid format.
        ///// </exception>
        ///// <seealso cref="Combine(Path)"/>
        ///// <seealso cref="Join(string)"/>
        ///// <seealso cref="Join(Path)"/>
        ///// <seealso cref="Link(string)"/>
        ///// <seealso cref="Link(Path)"/>
        //public Path Combine(Path other)
        //{

        //    if (other.Value.Length == 0)
        //    {
        //        return this;
        //    }

        //    var otherPath = System.IO.Path.GetFullPath(other);
        //    if (!(otherPath is null))
        //    {
        //        return new Path(otherPath);
        //    }

        //    return new Path(Join(other.Value));
        //}

        /// <inheritdoc cref="TryJoin(string?, out Path?)"/>
        public bool TryJoin(Path other, out Path result) => TryJoin(other, out result);


        /// <summary>
        ///     Attempts to concatenate the two paths while also ensuring that <i>at least one</i> directory separator
        ///     character is inserted between them.
        ///     
        ///     All leading/trailing directory separator chars of <paramref name="other"/> and this path
        ///     are preserved. Neither path is discarded.
        ///     
        ///     See remarks of <see cref="Join(string)"/> for details and examples.
        /// </summary>
        /// <param name="other">
        ///     Another path to be concatenated with this path.
        /// </param>
        /// <param name="result">
        ///     An <see langword="out"/> parameter which will, if the operation succeedes,
        ///     hold the resulting concatenated path.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the operation succeeded; <see langword="false"/> if not.
        /// </returns>
        public bool TryJoin(ReadOnlySpan<char> other, [NotNullWhen(true)] out Path result)
        {
            try
            {
                result = new Path(Join(other));
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        ///     Concatenates the two paths while also ensuring that <i>at least one</i> directory separator
        ///     character is present between them.
        ///     
        ///     All leading/trailing directory separator chars of <paramref name="other"/> and this path
        ///     are preserved. Neither path is discarded.
        ///     
        ///     See remarks for details and examples.
        /// </summary>
        /// <param name="other">
        ///     Another path to be concatenated with this path.
        /// </param>
        /// <returns>The resulting concatenated path.</returns>
        /// <remarks>
        ///     This method behaves like .NET's
        ///     <c>System.IO.Path.Join(ReadOnlySpan&lt;char&gt;, ReadOnlySpan&lt;char&gt;)</c> method.
        ///     
        ///     In comparison to the alternatives (<see cref="Combine(string)"/> and <see cref="Link(string)"/>),
        ///     <see cref="Join(string)"/> preserves any leading/trailing directory separator chars of
        ///     <paramref name="other"/> and this path.
        ///     In comparison to <see cref="Combine(string)"/> specifically, neither path is discarded.
        ///     Out of the three methods, <see cref="Join(string)"/> is the safest one as it does not
        ///     remove any characters from either path.
        ///     
        ///     The following code demonstrates the behavior of <see cref="Join(string)"/>:
        ///     
        ///     <code>
        ///     // Note: The code assumes that / is the file system's directory separator.
        ///     
        ///     Path first = fs.GetPath("firstPath");
        ///     first.Join("secondPath");    // Returns "firstPath/secondPath".
        ///     first.Join("/secondPath");   // Returns "firstPath/secondPath".
        ///     first.Join("///secondPath"); // Returns "firstPath///secondPath".
        ///     
        ///     first = fs.GetPath("firstPath/");
        ///     first.Join("secondPath");    // Returns "firstPath/secondPath".
        ///     first.Join("/secondPath");   // Returns "firstPath//secondPath".
        ///     first.Join("///secondPath"); // Returns "firstPath////secondPath".
        ///     
        ///     first = fs.GetPath("firstPath///");
        ///     first.Join("secondPath");    // Returns "firstPath///secondPath".
        ///     first.Join("/secondPath");   // Returns "firstPath////secondPath".
        ///     first.Join("///secondPath"); // Returns "firstPath//////secondPath".
        ///     
        ///     first = fs.GetPath("firstPath");
        ///     first.Join("");              // Returns "firstPath".
        /// 
        ///     first = fs.GetPath("/");
        ///     first.Join("");              // Returns "/".
        ///     first.Join("/");             // Returns "//".
        ///     first.Join("//");            // Returns "///".
        ///     
        ///     first = fs.GetPath("//");
        ///     first.Join("");              // Returns "//".
        ///     first.Join("/");             // Returns "///".
        ///     first.Join("//");            // Returns "////".
        ///     </code>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="other"/> is <see langword="null"/>-
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Concatenating the two paths results in a path with an invalid format.
        /// </exception>
        /// <seealso cref="Combine(string)"/>
        /// <seealso cref="Combine(Path)"/>
        /// <seealso cref="Join(Path)"/>
        /// <seealso cref="Link(string)"/>
        /// <seealso cref="Link(Path)"/>
        public Path Join(ReadOnlySpan<char> other)
        {
            if (other.Length == 0)
            {
                return this;
            }

            var hasSeparator = IsDirectorySeparator(Value[Value.Length - 1]) || IsDirectorySeparator(other[0]);

            var joinedPath = hasSeparator
                ? SpanHelper.Concat(Value, other)
                : SpanHelper.Concat(Value, new[] { PathConstants.DirectorySeparatorChar }, other);

            return new Path(joinedPath);
        }


        ///// <summary>
        /////     Attempts to concatenate the two paths while also ensuring that <i>exactly one</i> directory separator
        /////     character is inserted between them.
        /////     
        /////     Excess leading/trailing directory separators are removed from <paramref name="other"/>/this path
        /////     in order to end up with exactly one separator between them. Neither path is discarded.
        /////     
        /////     See remarks of <see cref="Link(string)"/> for details and examples.
        ///// </summary>
        ///// <param name="other">
        /////     Another path to be concatenated with this path.
        ///// </param>
        ///// <param name="result">
        /////     An <see langword="out"/> parameter which will, if the operation succeedes,
        /////     hold the resulting concatenated path.
        ///// </param>
        ///// <returns>
        /////     <see langword="true"/> if the operation succeeded; <see langword="false"/> if not.
        ///// </returns>
        //public bool TryLink(ReadOnlySpan<char> other, [NotNullWhen(true)] out ReadOnlySpan<char> result)
        //{

        //    try
        //    {
        //        result = Link(other);
        //        return true;
        //    }
        //    catch
        //    {
        //        result = null;
        //        return false;
        //    }
        //}

        ///// <inheritdoc cref="Link(string)"/>
        //public ReadOnlySpan<char> Link(Path other) => Link(other.Value);


        ///// <summary>
        /////     Concatenates the two paths while also ensuring that <i>exactly one</i> directory separator
        /////     character is present between them.
        /////     
        /////     Excess leading/trailing directory separators are removed from <paramref name="other"/>/this path
        /////     in order to end up with exactly one separator between them. Neither path is discarded.
        /////     
        /////     See remarks for details and examples.
        ///// </summary>
        ///// <param name="other">
        /////     Another path to be concatenated with this path.
        ///// </param>
        ///// <returns>The resulting concatenated path.</returns>
        ///// <remarks>
        /////     This method currently doesn't have an equivalent in the .NET Framework.
        /////     In essence, it behaves similarly to
        /////     <c>System.IO.Path.Join(ReadOnlySpan&lt;char&gt;, ReadOnlySpan&lt;char&gt;)</c>,
        /////     but with the difference that excess directory separators are removed between the two
        /////     paths.
        /////     
        /////     In comparison to the alternatives (<see cref="Combine(string)"/> and <see cref="Join(string)"/>),
        /////     <see cref="Link(string)"/> removes excess leading/trailing directory separator chars of
        /////     <paramref name="other"/>/this path before concatenating them. This ensures that
        /////     exactly one directory separator character is present between the two paths.
        /////     In comparison to the alternatives, this method is the ideal when dealing
        /////     with user input, as the result will, most likely, be a valid path without an
        /////     excess number of directory separator characters.
        /////     In comparison to <see cref="Join(string)"/> specifically, results like <c>firstPath//secondPath</c>
        /////     are not possible with this method.
        /////     
        /////     Be aware that using this method can change the meaning/format of <paramref name="other"/> if it
        /////     is a special path. If <paramref name="other"/> is, for example, a UNC path, trimming
        /////     the two leading directory separator chars <c>//</c> will inevitably change the path's meaning.
        /////     Then again, such a path (and absolute paths in general) should most likely not be
        /////     concatenated with other paths in the first place.
        /////     
        /////     The following code demonstrates the behavior of <see cref="Link(string)"/>:
        /////     
        /////     <code>
        /////     // Note: The code assumes that / is the file system's directory separator.
        /////     
        /////     Path first = fs.GetPath("firstPath");
        /////     first.Join("secondPath");    // Returns "firstPath/secondPath".
        /////     first.Join("/secondPath");   // Returns "firstPath/secondPath".
        /////     first.Join("///secondPath"); // Returns "firstPath/secondPath".
        /////     
        /////     first = fs.GetPath("firstPath/");
        /////     first.Join("secondPath");    // Returns "firstPath/secondPath".
        /////     first.Join("/secondPath");   // Returns "firstPath/secondPath".
        /////     first.Join("///secondPath"); // Returns "firstPath/secondPath".
        /////     
        /////     first = fs.GetPath("firstPath///");
        /////     first.Join("secondPath");    // Returns "firstPath/secondPath".
        /////     first.Join("/secondPath");   // Returns "firstPath/secondPath".
        /////     first.Join("///secondPath"); // Returns "firstPath/secondPath".
        /////     
        /////     first = fs.GetPath("firstPath");
        /////     first.Join("");              // Returns "firstPath".
        ///// 
        /////     first = fs.GetPath("/");
        /////     first.Join("");              // Returns "/".
        /////     first.Join("/");             // Returns "/".
        /////     first.Join("//");            // Returns "/".
        /////     
        /////     first = fs.GetPath("//");
        /////     first.Join("");              // Returns "//".
        /////     first.Join("/");             // Returns "/".
        /////     first.Join("//");            // Returns "/".
        /////     </code>
        ///// </remarks>
        ///// <exception cref="ArgumentNullException">
        /////     <paramref name="other"/> is <see langword="null"/>-
        ///// </exception>
        ///// <exception cref="ArgumentException">
        /////     Concatenating the two paths results in a path with an invalid format.
        ///// </exception>
        ///// <seealso cref="Combine(string)"/>
        ///// <seealso cref="Combine(Path)"/>
        ///// <seealso cref="Join(string)"/>
        ///// <seealso cref="Join(Path)"/>
        ///// <seealso cref="Link(Path)"/>
        //public ReadOnlySpan<char> Link(ReadOnlySpan<char> other)
        //{
        //    if (other.Length == 0)
        //    {
        //        return this.Value;
        //    }

        //    var part1 = Value.TrimEnd(PathConstants.DirectorySeparatorChar);
        //    var part2 = other.TrimStart(PathConstants.DirectorySeparatorChar);
        //    return SpanHelper.Concat(part1, new[] { PathConstants.DirectorySeparatorChar }, part2);
        //}

        ///// <inheritdoc cref="CompareTo(string?, StringComparison)"/>
        //public int CompareTo(Path? path, StringComparison stringComparison) =>
        //    CompareTo(this.ToString(), stringComparison);


        /// <summary>
        ///     Compares this path with another path based on the path strings.
        ///     The comparison is done using the specified <paramref name="stringComparison"/> value.
        /// </summary>
        /// <param name="path">Another path to be compared with this path.</param>
        /// <param name="stringComparison">
        ///     The <see cref="StringComparison"/> to be used for comparing the two paths.
        /// </param>
        /// <returns>
        ///     A negative value if this path precedes the other <paramref name="path"/>.
        ///     Zero if the two paths are considered equal.
        ///     A positive value if this path follows the other <paramref name="path"/>.
        /// </returns>
        public int CompareTo(ReadOnlySpan<char> path, StringComparison stringComparison) =>
            this.Value.CompareTo(path, stringComparison);



        ///// <inheritdoc cref="Equals(string?, StringComparison)"/>
        //public bool Equals(Path path, StringComparison stringComparison) =>
        //    Equals(path, stringComparison);




        /// <inheritdoc cref="operator /(Path, string)"/>
        public static Path operator /(Path path1, Path path2)
        {
            // The called overload validates for null.
            return (path1 / path2.Value);
        }

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
        public static Path operator /(Path path1, string path2)
        {
            return path1.Join(path2);
        }

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
        public static Path operator /(Path path1, ReadOnlySpan<char> path2)
        {
            return path1.Join(path2);
        }

        ///// <summary>
        /////     Appends the specified <paramref name="part"/> to the end of the <paramref name="path"/>.
        ///// </summary>
        ///// <param name="path">
        /////     The path to which the specified <paramref name="part"/> should be appended.
        ///// </param>
        ///// <param name="part">
        /////     The part to be appended to the path.
        ///// </param>
        ///// <returns>
        /////     A new <see cref="Path"/> instance which represents the path after appending the
        /////     specified <paramref name="part"/>.
        ///// </returns>
        ///// <exception cref="ArgumentNullException">
        /////     <paramref name="part"/>
        ///// </exception>
        ///// <exception cref="ArgumentException">
        /////     Appending <paramref name="part"/> would result in an invalid path.
        ///// </exception>
        ///// <seealso cref="Append(string)"/>
        //public static Path operator +(Path path, string part)
        //{
        //    _ = part ?? throw new ArgumentNullException(nameof(part));
        //    return path.Append(part);
        //}

        /// <summary>
        ///     Implicitly converts the specified <paramref name="path"/> to its underlying path string.
        /// </summary>
        /// <param name="path">The path to be converted.</param>
        /// <returns>
        ///     The path's underlying path string or <see langword="null"/> if <paramref name="path"/>
        ///     is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull("path")]
        public static implicit operator string(Path path) => path.Value.ToString();

        /// <summary>
        ///     Implicitly converts the specified <paramref name="path"/> to its underlying path string.
        /// </summary>
        /// <param name="path">The path to be converted.</param>
        /// <returns>
        ///     The path's underlying path string or <see langword="null"/> if <paramref name="path"/>
        ///     is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull("path")]
        public static implicit operator Path(string path) => new Path(path);

    }


    public ref struct AbsolutePath
    {
        public ReadOnlySpan<char> Value { get; }
        private ReadOnlySpan<char> RootPath { get; }
        public ReadOnlySpan<char> PathWithoutTrailingSeparator { get; }
        public ReadOnlySpan<char> Parent { get; }

        //public ReadOnlySpan<char> Name { get; }

        public AbsolutePath(string path)
        {
            Value = path;
            RootPath = System.IO.Path.GetPathRoot(Value);
            PathWithoutTrailingSeparator = PathPolyfills.TrimEndingDirectorySeparator(Value);
            Parent = System.IO.Path.GetDirectoryName(Value);
            //NameWithoutExtension = GetNameWithoutExtension(name);
            //            var extension = PhysicalPathHelper.GetExtensionWithoutTrailingExtensionSeparator(pathWithoutTrailingSeparator);
            //            var isPathFullyQualified = PathPolyfills.IsPathFullyQualified(path);

            //            Kind = isPathFullyQualified ? PathKind.Absolute : PathKind.Relative;
            //            Name = name;
            //            NameWithoutExtension = nameWithoutExtension;
            //            Extension = string.IsNullOrEmpty(extension) ? null : extension;
        }

        //public static MutliValidater<string, ErrorCode> GetValidator(string path)
        //{
        //    return new MutliValidater<string, ErrorCode>(path,
        //        new[] {
        //        new Validator(path, ErrorCode.NullOrWhiteSpace, a=>string.IsNullOrWhiteSpace(a)),
        //        new Validator(path, ErrorCode.MaxPathLength, a=>a.Length > PathConstants.MAXPATH),
        //        new Validator(path, ErrorCode.OnlyContainsVolumeDesignator, a=>(a== ":")),
        //        new Validator(path, ErrorCode.BeginsWithVolumeDesignator, a=>a[0]==':'),
        //        new Validator(path, ErrorCode.OnlyContainsVolumeDesignator, a=>a.Contains(":") && a.IndexOf(':') != 1),
        //        });
        //}


        public static bool IsValidPath(string path, bool allowRelativePaths = false)
        {
            try
            {
                return allowRelativePaths ?
                    System.IO.Path.IsPathRooted(path) :
                    string.IsNullOrEmpty(System.IO.Path.GetPathRoot(path)?.Trim('\\', '/')) == false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}





//namespace Files.Shared.PhysicalPath
//{
//    using System;
//    using System.Diagnostics;
//    using System.IO;
//    using Files;
//    using Files.Shared;
//    using Files.Shared.PhysicalPath.Utilities;

//    public class PhysicalPath : Path
//    {
//        // All properties which return a Path are wrapped in a Lazy<T> in order to not fully
//        // expand/walk a path tree whenever a path is initialized.
//        // Consider for example the path foo/bar/baz.
//        // Without Lazy<T>, three path instances would be created immediately through the Parent property.
//        private readonly Lazy<Path?> _rootLazy;
//        private readonly Lazy<Path?> _parentLazy;
//        private readonly Lazy<Path> _fullPathLazy;

//        public override PathKind Kind { get; }

//        public override Path? Root => _rootLazy.Value;

//        public override Path? Parent => _parentLazy.Value;

//        public override Path FullPath => _fullPathLazy.Value;

//        public override string Name { get; }

//        public override string NameWithoutExtension { get; }

//        public override string? Extension { get; }

//        internal PhysicalPath(string path, FileSystem fileSystem)
//            : base(fileSystem, path)
//        {
//            Debug.Assert(
//                ReferenceEquals(fileSystem.PathInformation, PhysicalPathHelper.PhysicalPathInformation),
//                $"When using the PhysicalPath, your file system should be using the corresponding " +
//                $"{nameof(PhysicalPathHelper.PhysicalPathInformation)}."
//            );

//            var fullPath = GetFullPathOrThrow(path);
//            var rootPath = Path.GetPathRoot(path);
//            var pathWithoutTrailingSeparator = PathPolyfills.TrimEndingDirectorySeparator(path);
//            var directoryPath = Path.GetDirectoryName(pathWithoutTrailingSeparator);
//            var name = Path.GetFileName(pathWithoutTrailingSeparator);
//            var nameWithoutExtension = GetNameWithoutExtension(name);
//            var extension = PhysicalPathHelper.GetExtensionWithoutTrailingExtensionSeparator(pathWithoutTrailingSeparator);
//            var isPathFullyQualified = PathPolyfills.IsPathFullyQualified(path);

//            Kind = isPathFullyQualified ? PathKind.Absolute : PathKind.Relative;
//            Name = name;
//            NameWithoutExtension = nameWithoutExtension;
//            Extension = string.IsNullOrEmpty(extension) ? null : extension;

//            _rootLazy = new Lazy<Path?>(
//                () => string.IsNullOrEmpty(rootPath) ? null : fileSystem.GetPath(rootPath)
//            );

//            _parentLazy = new Lazy<Path?>(
//                () => string.IsNullOrEmpty(directoryPath) ? null : fileSystem.GetPath(directoryPath)
//            );

//            _fullPathLazy = new Lazy<Path>(
//                () => fileSystem.GetPath(fullPath)
//            );

//        }


//        static string GetFullPathOrThrow(string path)
//        {
//            // This method, apart from returning the full path, has another goal:
//            // Validate that the path has the correct format.
//            // Path rules are incredibly complex depending on the current OS. It's best to not try
//            // and emulate these rules here, but to actually use the OS/FS APIs, i.e. GetFullPath, directly.
//            // One thing to note is that there are differences between the different .NET runtimes here.
//            // Older versions have stricter path validations and throw ArgumentExceptions here while
//            // newer implementations delay that and throw IOExceptions for invalid paths in the various
//            // File/Directory APIs.
//            // There is not much we can do about this. Again, emulating and artifically supporting this is
//            // incredibly complex. Therefore we simply allow this and document this fact.

//            try
//            {
//                return Path.GetFullPath(path);
//            }
//            catch (Exception ex) when (
//                   ex is ArgumentException
//                || ex is NotSupportedException
//                || ex is PathTooLongException
//            )
//            {
//                throw new ArgumentException(
//                    ExceptionStrings.Path.InvalidFormat(),
//                    nameof(path),
//                    ex
//                );
//            }
//        }

//        static string GetNameWithoutExtension(string name)
//        {
//            // Specification requires special handling for these two directories.
//            // Without this code, we'd return "" and ".", because Path.GetFileNameWithoutExtension
//            // trims one dot.
//            if (name == PhysicalPathHelper.CurrentDirectorySegment ||
//                name == PhysicalPathHelper.ParentDirectorySegment)
//            {
//                return name;
//            }
//            return Path.GetFileNameWithoutExtension(name);
//        }
//    }
//}


namespace Files.Shared.PhysicalPath.Utilities
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    ///     Provides static utility members for interacting with physical path strings.
    /// </summary>
    internal static partial class PhysicalPathHelper
    {
        internal const char ExtensionSeparatorChar = '.';
        internal const string CurrentDirectorySegment = ".";
        internal const string ParentDirectorySegment = "..";
        internal static readonly char[] DirectorySeparatorChars = new[]
            {
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar,
            }
            .Distinct()
            .ToArray();

        internal static readonly char[] InvalidNewNameCharacters =
            new[]
            {
                // Used for the newName parameter in methods like RenameAsync.
                // In essence, we want to avoid names like "foo/bar", i.e. relative paths.
                // We simply forbid directory separator chars in the name to achieve that.
                // Also forbid the volume separator, because it might also be a /.
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar,
                Path.VolumeSeparatorChar,
            }
            .Distinct()
            .ToArray();

        internal static string? GetExtensionWithoutTrailingExtensionSeparator(string? path)
        {
            return TrimTrailingExtensionSeparator(Path.GetExtension(path));

            static string? TrimTrailingExtensionSeparator(string? extension)
            {
                if (string.IsNullOrEmpty(extension) || extension![0] != ExtensionSeparatorChar)
                {
                    return extension;
                }
                return extension.Substring(1);
            }
        }

        internal static string GetTemporaryElementName()
        {
            var guidPart = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
            return $"{guidPart}~tmp";
        }
    }
}

namespace Files
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Files.Shared;

    /// <summary>
    ///     Provides information about special path characteristics in a file system implementation.
    /// </summary>
    public sealed class PathInformation
    {
        /// <summary>
        ///     Gets a list of characters which are not allowed to appear in a path targeting this
        ///     file system implementation.
        ///     
        ///     Depending on the underlying file system implementation, including such an invalid
        ///     character in a path will either throw during the creation of a <see cref="Path"/>
        ///     or while performing file I/O with that path.
        /// </summary>
        public IReadOnlyList<char> InvalidPathChars { get; }

        /// <summary>
        ///     Gets a list of characters which are not allowed to appear in the file name part of
        ///     a path targeting this file system implementation.
        ///     
        ///     Depending on the underlying file system implementation, including such an invalid
        ///     character in a path will either throw during the creation of a <see cref="Path"/>
        ///     or while performing file I/O with that path.
        /// </summary>
        public IReadOnlyList<char> InvalidFileNameChars { get; }

        /// <summary>
        ///     Gets a character which is used by this file system implementation to separate a path
        ///     into its various segments.
        /// </summary>
        public char DirectorySeparatorChar { get; }

        /// <summary>
        ///     Gets an alternative character which is used by this file system implementation to separate a path
        ///     into its various segments.
        ///     
        ///     Depending on the file system implementation, this property may return the same
        ///     character as <see cref="DirectorySeparatorChar"/>.
        /// </summary>
        public char AltDirectorySeparatorChar { get; }

        /// <summary>
        ///     Gets a distinct list which contains the file system's directory separator chars.
        ///     This list contains the <see cref="DirectorySeparatorChar"/> and <see cref="AltDirectorySeparatorChar"/>
        ///     if the characters are different. Otherwise, this list only contains a single directory
        ///     separator character.
        /// </summary>
        public IReadOnlyList<char> DirectorySeparatorChars { get; }

        /// <summary>
        ///     Gets a character which is used by this file system implementation to separate a
        ///     file name from a file extension.
        /// </summary>
        public char ExtensionSeparatorChar { get; }

        /// <summary>
        ///     Gets a character which is used by this file system implementation to separate
        ///     a volume from the rest of the path.
        /// </summary>
        public char VolumeSeparatorChar { get; }

        /// <summary>
        ///     Gets a string which is used by this file system implementation to refer to the
        ///     current directory in a path.
        ///     In most file system implementations, this is the <c>"."</c> string.
        /// </summary>
        public string CurrentDirectorySegment { get; }

        /// <summary>
        ///     Gets a string which is used by this file system implementation to refer to the
        ///     parent directory in a path.
        ///     In most file system implementations, this is the <c>".."</c> string.
        /// </summary>
        public string ParentDirectorySegment { get; }

        /// <summary>
        ///     Gets the <see cref="StringComparison"/> which is, by default, used by this file system
        ///     implementation to compare paths.
        ///     
        ///     Please be aware of the fact that this property is named "Default" for a reason and
        ///     might, in certain situations, not reflect the real string comparison used in a file
        ///     system.
        ///     See remarks for details.
        /// </summary>
        /// <remarks>
        ///     Depending on the file system implementation, it can very well happen that paths
        ///     are compared with different string comparisons in different locations.
        ///     This could, for example, be the case in a real, physical file system which mounts
        ///     another file system which uses a different string comparison. In such cases, this
        ///     property will lead to invalid comparisons.
        ///     For that reason, information about path equality, even with this property, should
        ///     always be treated with care.
        /// </remarks>
        public StringComparison DefaultStringComparison { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PathInformation"/> class.
        /// </summary>
        /// <param name="invalidPathChars">
        ///     A list of characters which are not allowed to appear in a path targeting this
        ///     file system implementation.
        /// </param>
        /// <param name="invalidFileNameChars">
        ///     A list of characters which are not allowed to appear in the file name part of
        ///     a path targeting this file system implementation.
        /// </param>
        /// <param name="directorySeparatorChar">
        ///     A character which is used by this file system implementation to separate a path
        ///     into its various segments.
        /// </param>
        /// <param name="altDirectorySeparatorChar">
        ///     An alternative character which is used by this file system implementation to separate a path
        ///     into its various segments.
        /// </param>
        /// <param name="extensionSeparatorChar">
        ///     A character which is used by this file system implementation to separate a
        ///     file name from a file extension.
        /// </param>
        /// <param name="volumeSeparatorChar">
        ///     A character which is used by this file system implementation to separate
        ///     a volume from the rest of the path.
        /// </param>
        /// <param name="currentDirectorySegment">
        ///     A string which is used by this file system implementation to refer to the
        ///     current directory in a path.
        /// </param>
        /// <param name="parentDirectorySegment">
        ///     A string which is used by this file system implementation to refer to the
        ///     parent directory in a path.
        /// </param>
        /// <param name="defaultStringComparison">
        ///     The <see cref="StringComparison"/> which is, by default, used by this file system
        ///     implementation to compare paths.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="invalidPathChars"/>, <paramref name="invalidFileNameChars"/>,
        ///     <paramref name="parentDirectorySegment"/> or <paramref name="currentDirectorySegment"/>
        ///     is <see langword="null"/>.
        /// </exception>
        public PathInformation(
            IEnumerable<char> invalidPathChars,
            IEnumerable<char> invalidFileNameChars,
            char directorySeparatorChar,
            char altDirectorySeparatorChar,
            char extensionSeparatorChar,
            char volumeSeparatorChar,
            string currentDirectorySegment,
            string parentDirectorySegment,
            StringComparison defaultStringComparison)
        {
            _ = invalidPathChars ?? throw new ArgumentNullException(nameof(invalidPathChars));
            _ = invalidFileNameChars ?? throw new ArgumentNullException(nameof(invalidFileNameChars));
            _ = parentDirectorySegment ?? throw new ArgumentNullException(nameof(parentDirectorySegment));
            _ = currentDirectorySegment ?? throw new ArgumentNullException(nameof(currentDirectorySegment));

            if (currentDirectorySegment.Length == 0)
            {
                throw new ArgumentException(ExceptionStrings.String.CannotBeEmpty(), nameof(currentDirectorySegment));
            }

            if (parentDirectorySegment.Length == 0)
            {
                throw new ArgumentException(ExceptionStrings.String.CannotBeEmpty(), nameof(parentDirectorySegment));
            }

            InvalidPathChars = new ReadOnlyCollection<char>(invalidPathChars.ToArray());
            InvalidFileNameChars = new ReadOnlyCollection<char>(invalidFileNameChars.ToArray());
            DirectorySeparatorChar = directorySeparatorChar;
            AltDirectorySeparatorChar = altDirectorySeparatorChar;
            ExtensionSeparatorChar = extensionSeparatorChar;
            VolumeSeparatorChar = volumeSeparatorChar;
            CurrentDirectorySegment = currentDirectorySegment;
            ParentDirectorySegment = parentDirectorySegment;
            DefaultStringComparison = defaultStringComparison;

            DirectorySeparatorChars = new[] { DirectorySeparatorChar, AltDirectorySeparatorChar }
                .Distinct()
                .ToList()
                .AsReadOnly();
        }
    }
}

namespace UtilityStruct
{

#pragma warning disable CA1036
    // Override methods on comparable types, i.e. implement <, >, <=, >= operators due to IComparable.
    // These operators are not implemented because .NET's string class also doesn't implement them.
    // Since a path is, at the end, just a string these operators are also not implemented here.




#pragma warning restore CA1036
}



// Parts of this file are copied from the source files of the dotnet/runtime repository and adapted/enhanced.
// The relevant files can be found at:
// https://github.com/dotnet/runtime
//
// The code is licensed by the .NET Foundation with the following license header:
// > Licensed to the .NET Foundation under one or more agreements.
// > The .NET Foundation licenses this file to you under the MIT license.
// > See the LICENSE file in the project root for more information.

namespace Files.Shared.PhysicalPath.Utilities
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal static class PathPolyfills
    {
#if NETCOREAPP2_0 || NETSTANDARD2_0 || UAP
        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/Path.cs#L419
        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/Path.cs#L643
        internal static string Join(string path1, string path2)
        {
            if (path1.Length == 0)
            {
                return path2;
            }
            
            if (path2.Length == 0)
            {
                return path1;
            }

            var hasSeparator = IsDirectorySeparator(path1[path1.Length - 1]) || IsDirectorySeparator(path2[0]);
            return hasSeparator
                ? $"{path1}{path2}"
                : $"{path1}{Path.DirectorySeparatorChar}{path2}";
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/Path.cs#L282
        internal static bool IsPathFullyQualified(string path)
        {
            return !IsPartiallyQualified(path);

            static bool IsPartiallyQualified(string path)
            {
                return Platform.Current switch
                {
                    PlatformID.Win32NT => WindowsImpl(path),
                    PlatformID.Unix => UnixImpl(path),
                    _ => throw new PlatformNotSupportedException(),
                };

                // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Windows.cs#L271
                static bool WindowsImpl(string path)
                {
                    if (path.Length < 2)
                    {
                        // It isn't fixed, it must be relative.  There is no way to specify a fixed
                        // path with one character (or less).
                        return true;
                    }

                    if (IsDirectorySeparator(path[0]))
                    {
                        // There is no valid way to specify a relative path with two initial slashes or
                        // \? as ? isn't valid for drive relative paths and \??\ is equivalent to \\?\
                        return !(path[1] == '?' || IsDirectorySeparator(path[1]));
                    }

                    // The only way to specify a fixed path that doesn't begin with two slashes
                    // is the drive, colon, slash format- i.e. C:\
                    return !((path.Length >= 3)
                        && (path[1] == Path.VolumeSeparatorChar)
                        && IsDirectorySeparator(path[2])
                        // To match old behavior we'll check the drive character for validity as the path is technically
                        // not qualified if you don't have a valid drive. "=:\" is the "=" file's default data stream.
                        && WindowsIsValidDriveChar(path[0]));
                }

                // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Unix.cs#L77
                static bool UnixImpl(string path)
                {
                    return !Path.IsPathRooted(path);
                }
            }
        }
#else
        internal static string Join(string path1, string path2)
        {
            return Path.Join(path1, path2);
        }

        internal static bool IsPathFullyQualified(string path)
        {
            return Path.IsPathFullyQualified(path);
        }
#endif

#if NETSTANDARD2_1 || NETCOREAPP2_2 || NETCOREAPP2_1 || NETCOREAPP2_0 || NETSTANDARD2_0 || UAP
        private const int WindowsDevicePrefixLength = 4;
        private const int WindowsUncPrefixLength = 2;
        private const int WindowsUncExtendedPrefixLength = 8;

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.cs#L21
        internal static bool EndsWithDirectorySeparator(string path)
        {
            return path.Length != 0 && IsDirectorySeparator(path[path.Length - 1]);
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.cs#L21
        internal static bool EndsWithDirectorySeparator(ReadOnlySpan<char> path)
        {
            return path.Length != 0 && IsDirectorySeparator(path[^1]);
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.cs#L226
        internal static string TrimEndingDirectorySeparator(string path)
        {
            if (EndsWithDirectorySeparator(path) && !IsRoot(path))
            {
                return path[0..^1];
            }
            else
            {
                return path;
            }
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.cs#L226
        internal static ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path)
        {
            if (EndsWithDirectorySeparator(path) && !IsRoot(path))
            {
                return path[0..^1];
            }
            else
            {
                return path;
            }
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.cs#L28
        private static bool IsRoot(string path)
        {
            return path.Length == GetRootLength(path);
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.cs#L28
        private static bool IsRoot(ReadOnlySpan<char> path)
        {
            return path.Length == GetRootLength(path);
        }


        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Windows.cs#L202
        static int GetRootLength(string path)
        {
            var pathLength = path.Length;
            var i = 0;

            var deviceSyntax = IsDevice(path);
            var deviceUnc = deviceSyntax && WindowsIsDeviceUNC(path);

            if ((!deviceSyntax || deviceUnc) && pathLength > 0 && IsDirectorySeparator(path[0]))
            {
                // UNC or simple rooted path (e.g. "\foo", NOT "\\?\C:\foo")
                if (deviceUnc || (pathLength > 1 && IsDirectorySeparator(path[1])))
                {
                    // UNC (\\?\UNC\ or \\), scan past server\share

                    // Start past the prefix ("\\" or "\\?\UNC\")
                    i = deviceUnc ? WindowsUncExtendedPrefixLength : WindowsUncPrefixLength;

                    // Skip two separators at most
                    var n = 2;
                    while (i < pathLength && (!IsDirectorySeparator(path[i]) || --n > 0))
                        i++;
                }
                else
                {
                    // Current drive rooted (e.g. "\foo")
                    i = 1;
                }
            }
            else if (deviceSyntax)
            {
                // Device path (e.g. "\\?\.", "\\.\")
                // Skip any characters following the prefix that aren't a separator
                i = WindowsDevicePrefixLength;
                while (i < pathLength && !IsDirectorySeparator(path[i]))
                    i++;

                // If there is another separator take it, as long as we have had at least one
                // non-separator after the prefix (e.g. don't take "\\?\\", but take "\\?\a\")
                if (i < pathLength && i > WindowsDevicePrefixLength && IsDirectorySeparator(path[i]))
                    i++;
            }
            else if (
                   pathLength >= 2
                && path[1] == Path.VolumeSeparatorChar
                && WindowsIsValidDriveChar(path[0])
            )
            {
                // Valid drive specified path ("C:", "D:", etc.)
                i = 2;

                // If the colon is followed by a directory separator, move past it (e.g "C:\")
                if (pathLength > 2 && IsDirectorySeparator(path[2]))
                    i++;
            }

            return i;
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Windows.cs#L202
        static int GetRootLength(ReadOnlySpan<char> path)
        {
            var pathLength = path.Length;
            var i = 0;

            var deviceSyntax = IsDevice(path);
            var deviceUnc = deviceSyntax && WindowsIsDeviceUNC(path);

            if ((!deviceSyntax || deviceUnc) && pathLength > 0 && IsDirectorySeparator(path[0]))
            {
                // UNC or simple rooted path (e.g. "\foo", NOT "\\?\C:\foo")
                if (deviceUnc || (pathLength > 1 && IsDirectorySeparator(path[1])))
                {
                    // UNC (\\?\UNC\ or \\), scan past server\share

                    // Start past the prefix ("\\" or "\\?\UNC\")
                    i = deviceUnc ? WindowsUncExtendedPrefixLength : WindowsUncPrefixLength;

                    // Skip two separators at most
                    var n = 2;
                    while (i < pathLength && (!IsDirectorySeparator(path[i]) || --n > 0))
                        i++;
                }
                else
                {
                    // Current drive rooted (e.g. "\foo")
                    i = 1;
                }
            }
            else if (deviceSyntax)
            {
                // Device path (e.g. "\\?\.", "\\.\")
                // Skip any characters following the prefix that aren't a separator
                i = WindowsDevicePrefixLength;
                while (i < pathLength && !IsDirectorySeparator(path[i]))
                    i++;

                // If there is another separator take it, as long as we have had at least one
                // non-separator after the prefix (e.g. don't take "\\?\\", but take "\\?\a\")
                if (i < pathLength && i > WindowsDevicePrefixLength && IsDirectorySeparator(path[i]))
                    i++;
            }
            else if (
                   pathLength >= 2
                && path[1] == Path.VolumeSeparatorChar
                && WindowsIsValidDriveChar(path[0])
            )
            {
                // Valid drive specified path ("C:", "D:", etc.)
                i = 2;

                // If the colon is followed by a directory separator, move past it (e.g "C:\")
                if (pathLength > 2 && IsDirectorySeparator(path[2]))
                    i++;
            }

            return i;
        }



        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Windows.cs#L301
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDirectorySeparator(char c)
        {
            return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Windows.cs#L132
        private static bool IsDevice(string path)
        {
            // If the path begins with any two separators is will be recognized and normalized and prepped with
            // "\??\" for internal usage correctly. "\??\" is recognized and handled, "/??/" is not.
            return WindowsIsExtended(path) ||
                (
                       path.Length >= WindowsDevicePrefixLength
                    && IsDirectorySeparator(path[0])
                    && IsDirectorySeparator(path[1])
                    && (path[2] == '.' || path[2] == '?')
                    && IsDirectorySeparator(path[3])
                );
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Windows.cs#L132
        private static bool IsDevice(ReadOnlySpan<char> path)
        {
            // If the path begins with any two separators is will be recognized and normalized and prepped with
            // "\??\" for internal usage correctly. "\??\" is recognized and handled, "/??/" is not.
            return WindowsIsExtended(path) ||
                (
                       path.Length >= WindowsDevicePrefixLength
                    && IsDirectorySeparator(path[0])
                    && IsDirectorySeparator(path[1])
                    && (path[2] == '.' || path[2] == '?')
                    && IsDirectorySeparator(path[3])
                );
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Windows.cs#L165
        private static bool WindowsIsExtended(string path)
        {
            // While paths like "//?/C:/" will work, they're treated the same as "\\.\" paths.
            // Skipping of normalization will *only* occur if back slashes ('\') are used.
            return path.Length >= WindowsDevicePrefixLength
                && path[0] == '\\'
                && (path[1] == '\\' || path[1] == '?')
                && path[2] == '?'
                && path[3] == '\\';
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Windows.cs#L165
        private static bool WindowsIsExtended(ReadOnlySpan<char> path)
        {
            // While paths like "//?/C:/" will work, they're treated the same as "\\.\" paths.
            // Skipping of normalization will *only* occur if back slashes ('\') are used.
            return path.Length >= WindowsDevicePrefixLength
                && path[0] == '\\'
                && (path[1] == '\\' || path[1] == '?')
                && path[2] == '?'
                && path[3] == '\\';
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Windows.cs#L150
        private static bool WindowsIsDeviceUNC(string path)
        {
            return path.Length >= WindowsUncExtendedPrefixLength
                && IsDevice(path)
                && IsDirectorySeparator(path[7])
                && path[4] == 'U'
                && path[5] == 'N'
                && path[6] == 'C';
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Windows.cs#L150
        private static bool WindowsIsDeviceUNC(ReadOnlySpan<char> path)
        {
            return path.Length >= WindowsUncExtendedPrefixLength
                && IsDevice(path)
                && IsDirectorySeparator(path[7])
                && path[4] == 'U'
                && path[5] == 'N'
                && path[6] == 'C';
        }

        // See https://github.com/dotnet/runtime/blob/f30675618fc379e112376acc6f1efa53733ee881/src/libraries/System.Private.CoreLib/src/System/IO/PathInternal.Windows.cs#L70
        private static bool WindowsIsValidDriveChar(char value)
        {
            return (value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z');
        }
#else
        internal static bool EndsWithDirectorySeparator(string path)
        {
            return Path.EndsInDirectorySeparator(path);
        }

        internal static string TrimEndingDirectorySeparator(string path)
        {
            return Path.TrimEndingDirectorySeparator(path);
        }
#endif


    }
}


namespace Files.Shared
{
    using System;

    internal static class ExceptionStrings
    {
        internal static class String
        {
            internal static string CannotBeEmpty() =>
                "The specified string must not be empty.";
        }

        internal static class Enum
        {
            internal static string UndefinedValue<T>(T value) where T : struct, System.Enum =>
                $"{value} is not a valid {typeof(T).FullName} value.";

            internal static string UnsupportedValue<T>(T value) where T : struct, System.Enum =>
                $"{typeof(T).FullName}.{value} is not supported.";
        }

        internal static class Comparable
        {
            internal static string TypeIsNotSupported(Type type) =>
                $"The object cannot be compared to objects of type {type.FullName}.";
        }

        internal static class Stream
        {
            internal static string NotReadable() =>
                "The stream does not support reading.";

            internal static string NotWriteable() =>
                "The stream does not support writing.";
        }

        internal static class FsCompatibility
        {
            internal static string StoragePathTypeNotSupported() =>
                $"The specified {nameof(StoragePath)} has a type that is incompatible with the " +
                $"current file system implementation. " +
                $"Ensure that you are not accidently using multiple file system implementations at " +
                $"the same or that you are appropriately converting the paths between multiple " +
                $"different implementations.";
        }

        //internal static class FileSystem
        //{
        //    internal static string KnownFolderNotSupported(KnownFolder value) =>
        //        $"The file system doesn't support or provide a folder matching the " +
        //        $"\"{nameof(KnownFolder)}.{value}\" value.";
        //}

        internal static class StoragePath
        {
            internal static string InvalidFormat() =>
                $"The specified path has an invalid format.";

            internal static string TrimmingResultsInEmptyPath() =>
                $"Trimming the trailing directory separator results in an empty path.";

            internal static string TrimmingResultsInInvalidPath() =>
                "Trimming the trailing directory separator results in an invalid path.";

            internal static string PathHasNoParent() =>
                $"The path is required to have a parent, but the {nameof(StoragePath.PathHasNoParent)} " +
                $"property is null.";
        }

        internal static class StorageElement
        {
            internal static string CannotCopyToSameLocation() =>
                "Copying the element to the same location is not possible.";

            internal static string CannotCopyToRootLocation() =>
                "Copying the element to a root location is not possible.";

            internal static string CannotMoveToSameLocation() =>
                "Moving the element to the same location is not possible.";

            internal static string CannotMoveToRootLocation() =>
                "Moving the element to a root location is not possible.";

            internal static string CannotMoveFromRootLocation() =>
                "A rooted element cannot be moved to another location.";
        }

        internal static class StorageFile
        {
            internal static string HasNoParentPath() =>
                "The parent folder of the current file could not be resolved from the path. " +
                "This is most likely an error in the underlying file system implementation, " +
                "because each file should have a parent folder.";

            internal static string CannotInitializeWithRootFolderPath() =>
                "The specified path points to a root folder which cannot identify a file.";

            internal static string ParentFolderDoesNotExist() =>
                "The file's parent folder does not exist.";

            internal static string ConflictingFolderExistsAtFileLocation() =>
                "The operation failed because a folder exists at the file's location (or at the " +
                "destination folder if this was a copy or move operation).";

            //internal static string NewNameContainsInvalidChar(PathInformation pathInformation) =>
            //    $"The specified name contains one or more invalid characters. " +
            //    $"Invalid characters are:\n" +
            //    $"- The directory separator character '{pathInformation.DirectorySeparatorChar}'\n" +
            //    $"- The alternative directory separator character '{pathInformation.AltDirectorySeparatorChar}'\n" +
            //    $"- The volume separator character '{pathInformation.VolumeSeparatorChar}'\n" +
            //    $"\n" +
            //    $"You can use the {nameof(Files.FileSystem)}.{nameof(Files.FileSystem.PathInformation)} property " +
            //    $"of this file's {nameof(Files.StorageFile.FileSystem)} property to determine which characters" +
            //    $"are allowed.";

            internal static string CannotMoveToSameLocation() =>
                "Moving the file to the same location is not possible.";

            internal static string CannotCopyToRootLocation() =>
                "Copying the file to a root location is not possible.";

            internal static string CannotMoveToRootLocation() =>
                "Moving the file to a root location is not possible.";

            //internal static string NotFound(Files.StoragePath path) =>
            //    $"The file at \"{path}\" does not exist.";

            //internal static string ParentNotFound(Files.StoragePath path) =>
            //    $"One or more parent folder(s) of the file at \"{path}\" do(es) not exist.";

            internal static string FileIsLocked() =>
                "The file is currently locked and thus cannot be modified.";
        }

        internal static class StorageFolder
        {
            internal static string CreateFailFolderAlreadyExists() =>
                "Creating the folder failed because another folder already exists at the location.";

            internal static string ConflictingFileExistsAtFolderLocation() =>
                "The operation failed because a file exists at the folder's location (or at the " +
                "destination folder if this was a copy or move operation).";

            internal static string CopyConflictingFolderExistsAtDestination() =>
                "Another folder already exists at the destination.";

            //internal static string NewNameContainsInvalidChar(PathInformation pathInformation) =>
            //    $"The specified name contains one or more invalid characters. " +
            //    $"Invalid characters are:\n" +
            //    $"- The directory separator character '{pathInformation.DirectorySeparatorChar}'\n" +
            //    $"- The alternative directory separator character '{pathInformation.AltDirectorySeparatorChar}'\n" +
            //    $"- The volume separator character '{pathInformation.VolumeSeparatorChar}'\n" +
            //    $"\n" +
            //    $"You can use the {"FileSystem"}.{"PathInformation"} property " +
            //    $"of this folder's {nameof(Files.StorageFolder.FileSystem)} property to determine which characters" +
            //    $"are allowed.";

            internal static string CannotCopyToSameLocation() =>
                "Copying the folder to the same location is not possible.";

            internal static string CannotMoveToSameLocation() =>
                "Moving the folder to the same location is not possible.";

            internal static string CannotMoveToRootLocation() =>
                "Moving the folder to a root location is not possible.";

            internal static string CannotMoveParentFolderIntoChildFolder() =>
                "Moving a parent folder into a child folder is not possible.";

            //internal static string NotFound(Files.StoragePath path) =>
            //    $"The folder at \"{path}\" does not exist.";

            //internal static string ParentNotFound(Path path) =>
            //    $"One or more parent folder(s) of the folder at \"{path}\" do(es) not exist.";
        }

        internal static class WindowsStorageCompatibility
        {
            internal static string WindowsStorageElementHasNoPath() =>
                $"The current operation failed because an underlying Windows.Storage.IStorageItem " +
                $"which is used by this file system implementation does not provide an actual physical " +
                $"path within the file system. " +
                $"This can happen if the file is not stored in the actual file system.";
        }

        internal static class InMemoryFileSystem
        {
            internal static string MemberIncompatibleWithInstance() =>
                "The specified member has been created by another InMemoryFileSystem instance. " +
                "An InMemoryFileSystem instance is only compatible with members created by itself. " +
                "Ensure that you are not creating and mixing multiple InMemoryFileSystem instances " +
                "at the same time.";
        }
    }
}