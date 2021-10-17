
//namespace Files.Shared.PhysicalStoragePath
//{
//    using System;
//    using System.Diagnostics;
//    using System.IO;
//    using Files;
//    using Files.Shared;
//    using Files.Shared.PhysicalStoragePath.Utilities;

//    public class PhysicalStoragePath : StoragePath
//    {
//        // All properties which return a StoragePath are wrapped in a Lazy<T> in order to not fully
//        // expand/walk a path tree whenever a path is initialized.
//        // Consider for example the path foo/bar/baz.
//        // Without Lazy<T>, three path instances would be created immediately through the Parent property.
//        private readonly Lazy<StoragePath?> _rootLazy;
//        private readonly Lazy<StoragePath?> _parentLazy;
//        private readonly Lazy<StoragePath> _fullPathLazy;

//        public override PathKind Kind { get; }

//        public override StoragePath? Root => _rootLazy.Value;

//        public override StoragePath? Parent => _parentLazy.Value;

//        public override StoragePath FullPath => _fullPathLazy.Value;

//        public override string Name { get; }

//        public override string NameWithoutExtension { get; }

//        public override string? Extension { get; }

//        internal PhysicalStoragePath(string path, FileSystem fileSystem)
//            : base(fileSystem, path)
//        {
//            Debug.Assert(
//                ReferenceEquals(fileSystem.PathInformation, PhysicalPathHelper.PhysicalPathInformation),
//                $"When using the PhysicalStoragePath, your file system should be using the corresponding " +
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

//            _rootLazy = new Lazy<StoragePath?>(
//                () => string.IsNullOrEmpty(rootPath) ? null : fileSystem.GetPath(rootPath)
//            );

//            _parentLazy = new Lazy<StoragePath?>(
//                () => string.IsNullOrEmpty(directoryPath) ? null : fileSystem.GetPath(directoryPath)
//            );

//            _fullPathLazy = new Lazy<StoragePath>(
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
//                    ExceptionStrings.StoragePath.InvalidFormat(),
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


//namespace Files.Shared.PhysicalStoragePath.Utilities
//{
//    using System;
//    using System.Globalization;
//    using System.IO;
//    using System.Linq;

//    /// <summary>
//    ///     Provides static utility members for interacting with physical path strings.
//    /// </summary>
//    internal static partial class PhysicalPathHelper
//    {
//        internal const char ExtensionSeparatorChar = '.';
//        internal const string CurrentDirectorySegment = ".";
//        internal const string ParentDirectorySegment = "..";
//        internal static readonly char[] DirectorySeparatorChars = new[]
//            {
//                Path.DirectorySeparatorChar,
//                Path.AltDirectorySeparatorChar,
//            }
//            .Distinct()
//            .ToArray();

//        internal static readonly char[] InvalidNewNameCharacters =
//            new[]
//            {
//                // Used for the newName parameter in methods like RenameAsync.
//                // In essence, we want to avoid names like "foo/bar", i.e. relative paths.
//                // We simply forbid directory separator chars in the name to achieve that.
//                // Also forbid the volume separator, because it might also be a /.
//                Path.DirectorySeparatorChar,
//                Path.AltDirectorySeparatorChar,
//                Path.VolumeSeparatorChar,
//            }
//            .Distinct()
//            .ToArray();

//        internal static readonly PathInformation PhysicalPathInformation = new PathInformation(
//            Path.GetInvalidPathChars(),
//            Path.GetInvalidFileNameChars(),
//            Path.DirectorySeparatorChar,
//            Path.AltDirectorySeparatorChar,
//            extensionSeparatorChar: ExtensionSeparatorChar,
//            Path.VolumeSeparatorChar,
//            currentDirectorySegment: CurrentDirectorySegment,
//            parentDirectorySegment: ParentDirectorySegment,
//            defaultStringComparison: StringComparison
//        );

//        internal static string? GetExtensionWithoutTrailingExtensionSeparator(string? path)
//        {
//            return TrimTrailingExtensionSeparator(Path.GetExtension(path));

//            static string? TrimTrailingExtensionSeparator(string? extension)
//            {
//                if (string.IsNullOrEmpty(extension) || extension![0] != ExtensionSeparatorChar)
//                {
//                    return extension;
//                }
//                return extension.Substring(1);
//            }
//        }

//        internal static string GetTemporaryElementName()
//        {
//            var guidPart = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
//            return $"{guidPart}~tmp";
//        }
//    }
//}

//namespace Files
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Collections.ObjectModel;
//    using System.Linq;
//    using Files.Shared;

//    /// <summary>
//    ///     Provides information about special path characteristics in a file system implementation.
//    /// </summary>
//    public sealed class PathInformation
//    {
//        /// <summary>
//        ///     Gets a list of characters which are not allowed to appear in a path targeting this
//        ///     file system implementation.
//        ///     
//        ///     Depending on the underlying file system implementation, including such an invalid
//        ///     character in a path will either throw during the creation of a <see cref="StoragePath"/>
//        ///     or while performing file I/O with that path.
//        /// </summary>
//        public IReadOnlyList<char> InvalidPathChars { get; }

//        /// <summary>
//        ///     Gets a list of characters which are not allowed to appear in the file name part of
//        ///     a path targeting this file system implementation.
//        ///     
//        ///     Depending on the underlying file system implementation, including such an invalid
//        ///     character in a path will either throw during the creation of a <see cref="StoragePath"/>
//        ///     or while performing file I/O with that path.
//        /// </summary>
//        public IReadOnlyList<char> InvalidFileNameChars { get; }

//        /// <summary>
//        ///     Gets a character which is used by this file system implementation to separate a path
//        ///     into its various segments.
//        /// </summary>
//        public char DirectorySeparatorChar { get; }

//        /// <summary>
//        ///     Gets an alternative character which is used by this file system implementation to separate a path
//        ///     into its various segments.
//        ///     
//        ///     Depending on the file system implementation, this property may return the same
//        ///     character as <see cref="DirectorySeparatorChar"/>.
//        /// </summary>
//        public char AltDirectorySeparatorChar { get; }

//        /// <summary>
//        ///     Gets a distinct list which contains the file system's directory separator chars.
//        ///     This list contains the <see cref="DirectorySeparatorChar"/> and <see cref="AltDirectorySeparatorChar"/>
//        ///     if the characters are different. Otherwise, this list only contains a single directory
//        ///     separator character.
//        /// </summary>
//        public IReadOnlyList<char> DirectorySeparatorChars { get; }

//        /// <summary>
//        ///     Gets a character which is used by this file system implementation to separate a
//        ///     file name from a file extension.
//        /// </summary>
//        public char ExtensionSeparatorChar { get; }

//        /// <summary>
//        ///     Gets a character which is used by this file system implementation to separate
//        ///     a volume from the rest of the path.
//        /// </summary>
//        public char VolumeSeparatorChar { get; }

//        /// <summary>
//        ///     Gets a string which is used by this file system implementation to refer to the
//        ///     current directory in a path.
//        ///     In most file system implementations, this is the <c>"."</c> string.
//        /// </summary>
//        public string CurrentDirectorySegment { get; }

//        /// <summary>
//        ///     Gets a string which is used by this file system implementation to refer to the
//        ///     parent directory in a path.
//        ///     In most file system implementations, this is the <c>".."</c> string.
//        /// </summary>
//        public string ParentDirectorySegment { get; }

//        /// <summary>
//        ///     Gets the <see cref="StringComparison"/> which is, by default, used by this file system
//        ///     implementation to compare paths.
//        ///     
//        ///     Please be aware of the fact that this property is named "Default" for a reason and
//        ///     might, in certain situations, not reflect the real string comparison used in a file
//        ///     system.
//        ///     See remarks for details.
//        /// </summary>
//        /// <remarks>
//        ///     Depending on the file system implementation, it can very well happen that paths
//        ///     are compared with different string comparisons in different locations.
//        ///     This could, for example, be the case in a real, physical file system which mounts
//        ///     another file system which uses a different string comparison. In such cases, this
//        ///     property will lead to invalid comparisons.
//        ///     For that reason, information about path equality, even with this property, should
//        ///     always be treated with care.
//        /// </remarks>
//        public StringComparison DefaultStringComparison { get; }

//        /// <summary>
//        ///     Initializes a new instance of the <see cref="PathInformation"/> class.
//        /// </summary>
//        /// <param name="invalidPathChars">
//        ///     A list of characters which are not allowed to appear in a path targeting this
//        ///     file system implementation.
//        /// </param>
//        /// <param name="invalidFileNameChars">
//        ///     A list of characters which are not allowed to appear in the file name part of
//        ///     a path targeting this file system implementation.
//        /// </param>
//        /// <param name="directorySeparatorChar">
//        ///     A character which is used by this file system implementation to separate a path
//        ///     into its various segments.
//        /// </param>
//        /// <param name="altDirectorySeparatorChar">
//        ///     An alternative character which is used by this file system implementation to separate a path
//        ///     into its various segments.
//        /// </param>
//        /// <param name="extensionSeparatorChar">
//        ///     A character which is used by this file system implementation to separate a
//        ///     file name from a file extension.
//        /// </param>
//        /// <param name="volumeSeparatorChar">
//        ///     A character which is used by this file system implementation to separate
//        ///     a volume from the rest of the path.
//        /// </param>
//        /// <param name="currentDirectorySegment">
//        ///     A string which is used by this file system implementation to refer to the
//        ///     current directory in a path.
//        /// </param>
//        /// <param name="parentDirectorySegment">
//        ///     A string which is used by this file system implementation to refer to the
//        ///     parent directory in a path.
//        /// </param>
//        /// <param name="defaultStringComparison">
//        ///     The <see cref="StringComparison"/> which is, by default, used by this file system
//        ///     implementation to compare paths.
//        /// </param>
//        /// <exception cref="ArgumentNullException">
//        ///     <paramref name="invalidPathChars"/>, <paramref name="invalidFileNameChars"/>,
//        ///     <paramref name="parentDirectorySegment"/> or <paramref name="currentDirectorySegment"/>
//        ///     is <see langword="null"/>.
//        /// </exception>
//        public PathInformation(
//            IEnumerable<char> invalidPathChars,
//            IEnumerable<char> invalidFileNameChars,
//            char directorySeparatorChar,
//            char altDirectorySeparatorChar,
//            char extensionSeparatorChar,
//            char volumeSeparatorChar,
//            string currentDirectorySegment,
//            string parentDirectorySegment,
//            StringComparison defaultStringComparison)
//        {
//            _ = invalidPathChars ?? throw new ArgumentNullException(nameof(invalidPathChars));
//            _ = invalidFileNameChars ?? throw new ArgumentNullException(nameof(invalidFileNameChars));
//            _ = parentDirectorySegment ?? throw new ArgumentNullException(nameof(parentDirectorySegment));
//            _ = currentDirectorySegment ?? throw new ArgumentNullException(nameof(currentDirectorySegment));

//            if (currentDirectorySegment.Length == 0)
//            {
//                throw new ArgumentException(ExceptionStrings.String.CannotBeEmpty(), nameof(currentDirectorySegment));
//            }

//            if (parentDirectorySegment.Length == 0)
//            {
//                throw new ArgumentException(ExceptionStrings.String.CannotBeEmpty(), nameof(parentDirectorySegment));
//            }

//            InvalidPathChars = new ReadOnlyCollection<char>(invalidPathChars.ToArray());
//            InvalidFileNameChars = new ReadOnlyCollection<char>(invalidFileNameChars.ToArray());
//            DirectorySeparatorChar = directorySeparatorChar;
//            AltDirectorySeparatorChar = altDirectorySeparatorChar;
//            ExtensionSeparatorChar = extensionSeparatorChar;
//            VolumeSeparatorChar = volumeSeparatorChar;
//            CurrentDirectorySegment = currentDirectorySegment;
//            ParentDirectorySegment = parentDirectorySegment;
//            DefaultStringComparison = defaultStringComparison;

//            DirectorySeparatorChars = new[] { DirectorySeparatorChar, AltDirectorySeparatorChar }
//                .Distinct()
//                .ToList()
//                .AsReadOnly();
//        }
//    }
//}

//namespace Files
//{
//    using System;
//    using System.Diagnostics;
//    using System.Diagnostics.CodeAnalysis;
//    using System.Linq;
//    using System.Runtime.CompilerServices;
//    using Files.Shared;

//#pragma warning disable CA1036
//    // Override methods on comparable types, i.e. implement <, >, <=, >= operators due to IComparable.
//    // These operators are not implemented because .NET's string class also doesn't implement them.
//    // Since a path is, at the end, just a string these operators are also not implemented here.

//    /// <summary>
//    ///     An immutable representation of a path which points to an element in a file system.
//    ///     Instances can be created via the <see cref="Files.FileSystem"/> class.
//    ///     See remarks for details.
//    /// </summary>
//    /// <remarks>
//    ///     A <see cref="StoragePath"/> is, in essence, a wrapper around a simple string.
//    ///     Depending on the underlying file system implementation, the class is able to interpret
//    ///     this underlying string as a path and therefore return path specific information, e.g.
//    ///     whether the path is absolute or relative.
//    ///     
//    ///     In addition, the <see cref="StoragePath"/> allows you to perform file system specific
//    ///     path operations, for example concatenating two paths.
//    ///     
//    ///     <see cref="StoragePath"/> instances can be obtained via the <see cref="FileSystem"/>
//    ///     class or via properties/methods like <see cref="StorageElement.Path"/>.
//    ///     
//    ///     Both users and implementers of this class should be aware that a <see cref="StoragePath"/>
//    ///     is, at its core, nothing else but a layer on top of a string. The string is and remains
//    ///     the lowest common denominator between different file system implementations.
//    ///     This is, for example, important if you wish to implement conversion logic between
//    ///     different <see cref="Files.FileSystem"/> implementations. Methods like
//    ///     <see cref="Files.FileSystem.GetFile(StoragePath)"/> throw an exception if the provided
//    ///     <see cref="StoragePath"/> was created by a <see cref="Files.FileSystem"/> instance
//    ///     of a different type. This can be avoided by falling back to the underlying string of
//    ///     the <see cref="StoragePath"/> and by creating a new <see cref="StoragePath"/> of the
//    ///     appropriate type with that string.
//    /// </remarks>
//    public abstract class StoragePath :
//        IFileSystemElement,
//        IEquatable<string?>,
//        IEquatable<StoragePath?>,
//        IComparable,
//        IComparable<string?>,
//        IComparable<StoragePath?>
//    {
//        private readonly Lazy<StoragePath> _pathWithoutTrailingDirectorySeparatorLazy;

//        /// <summary>
//        ///     The file system with which this path has been initialized.
//        /// </summary>
//        /// <seealso cref="ToString"/>
//        public FileSystem FileSystem { get; }

//        /// <summary>
//        ///     Gets the underlying string with which this <see cref="StoragePath"/> instance has
//        ///     been initialized.
//        /// </summary>
//        protected string UnderlyingString { get; }

//        /// <summary>
//        ///     Gets the number of characters in the underlying string with which this path has
//        ///     been initialized.
//        /// </summary>
//        public int Length => UnderlyingString.Length;

//        /// <summary>
//        ///     Gets a value indicating whether this path is an absolute or relative path.
//        /// </summary>
//        public abstract PathKind Kind { get; }

//        /// <summary>
//        ///     Gets a <see cref="StoragePath"/> which represent's this path's root directory.
//        ///     If the path doesn't have a root directory (for example if it is a relative path),
//        ///     this returns <see langword="null"/>.
//        /// </summary>
//        public abstract StoragePath? Root { get; }

//        /// <summary>
//        ///     Gets a <see cref="StoragePath"/> which represent's this path's parent directory.
//        ///     If the path doesn't have a parent directory (for example if it represents a root
//        ///     directory), this returns <see langword="null"/>.
//        ///     
//        ///     If the path ends with a single trailing directory separator, this separator is ignored
//        ///     and the parent is determined based on the segment before that separator.
//        ///     
//        ///     Once the parent directory has been determined, all trailing directory separator
//        ///     characters are removed (as an example, the parent of <c>"foo///bar"</c> is <c>"foo"</c>).
//        /// </summary>
//        public abstract StoragePath? Parent { get; }

//        /// <summary>
//        ///     Gets a <see cref="StoragePath"/> instance which represents a fully resolved, absolute path
//        ///     based on this path.
//        /// </summary>
//        public abstract StoragePath FullPath { get; }

//        /// <summary>
//        ///     Gets the full name of the path, i.e. the last segment, including the extension.
//        ///     
//        ///     If the path ends with a single trailing directory separator, this separator is ignored
//        ///     and the name is determined based on the segment before that separator.
//        /// </summary>
//        public abstract string Name { get; }

//        /// <summary>
//        ///     Gets the name of the path, i.e. the last segment, without the extension.
//        ///     
//        ///     If the path ends with a single trailing directory separator, this separator is ignored
//        ///     and the name is determined based on the segment before that separator.
//        /// </summary>
//        public abstract string NameWithoutExtension { get; }

//        /// <summary>
//        ///     Gets the extension of the path without the file system's extension separator.
//        ///     If the path doesn't have an extension, this returns <see langword="null"/>.
//        ///     
//        ///     If the path ends with a single trailing directory separator, this separator is ignored
//        ///     and the extension is determined based on the segment before that separator.
//        /// </summary>
//        public abstract string? Extension { get; }

//        /// <summary>
//        ///     Gets a value indicating whether the path starts with a directory separator character.
//        /// </summary>
//        public bool StartsWithDirectorySeparator =>
//            IsDirectorySeparator(UnderlyingString[0]);

//        /// <summary>
//        ///     Gets a value indicating whether the path ends with a directory separator character.
//        /// </summary>
//        public bool EndsWithDirectorySeparator =>
//            IsDirectorySeparator(UnderlyingString[UnderlyingString.Length - 1]);

//        /// <summary>
//        ///     Initializes a new <see cref="StoragePath"/> instance from the specified <paramref name="path"/> string.
//        /// </summary>
//        /// <param name="fileSystem">
//        ///     The file system with which this <see cref="StoragePath"/> is associated.
//        /// </param>
//        /// <param name="path">
//        ///     The string on which this <see cref="StoragePath"/> instance is based.
//        /// </param>
//        /// <exception cref="ArgumentNullException">
//        ///     <paramref name="path"/> is <see langword="null"/>.
//        /// </exception>
//        /// <exception cref="ArgumentException">
//        ///     <paramref name="path"/> is an empty string.
//        /// </exception>
//        protected StoragePath(FileSystem fileSystem, string path)
//        {
//            _ = path ?? throw new ArgumentNullException(nameof(path));
//            _ = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

//            if (path.Length == 0)
//            {
//                throw new ArgumentException(ExceptionStrings.String.CannotBeEmpty(), nameof(path));
//            }

//            FileSystem = fileSystem;
//            UnderlyingString = path;
//            _pathWithoutTrailingDirectorySeparatorLazy = new Lazy<StoragePath>(() => TrimEndingDirectorySeparatorImpl());
//        }

//        /// <summary>
//        ///     Attempts to trim one trailing directory separator character from this path and
//        ///     return the resulting path.
//        /// </summary>
//        /// <param name="result">
//        ///     An <see langword="out"/> parameter which will, if the operation succeedes,
//        ///     hold the new <see cref="StoragePath"/> where one trailing directory separator has been
//        ///     trimmed.
//        ///     If this path doesn't have a trailing directory separator character, the same path
//        ///     instance is returned.
//        /// </param>
//        /// <returns>
//        ///     <see langword="true"/> if the operation succeeded; <see langword="false"/> if not.
//        /// </returns>
//        public bool TryTrimEndingDirectorySeparator([NotNullWhen(true)] out StoragePath? result)
//        {
//            if (!EndsWithDirectorySeparator)
//            {
//                result = this;
//                return true;
//            }

//            try
//            {
//                result = TrimEndingDirectorySeparator();
//                return true;
//            }
//            catch
//            {
//                result = null;
//                return false;
//            }
//        }

//        /// <summary>
//        ///     Trims one trailing directory separator character from this path and returns the
//        ///     resulting path.
//        ///     See remarks for details.
//        /// </summary>
//        /// <returns>
//        ///     A new <see cref="StoragePath"/> instance where one trailing directory separator has been
//        ///     trimmed.
//        ///     If this path doesn't have a trailing directory separator character, the same path
//        ///     instance is returned.
//        /// </returns>
//        /// <remarks>
//        ///     As described, the method trims exactly one trailing directory separator character
//        ///     from the path (if one exists - otherwise, the same path is returned).
//        ///     
//        ///     In comparison to .NET's <c>System.IO.Path.TrimEndingDirectorySeparator(string path)</c>
//        ///     method, <see cref="TrimEndingDirectorySeparator"/> also trims a directory separator
//        ///     character if the path is a root path.
//        ///     This can lead to trouble in certain scenarios, for example if the path is <c>"/"</c> on
//        ///     Unix. Trimming this character would result in an empty path <c>""</c> which is illegal.
//        ///     In such cases (and other scenarios where an invalid path is the result of trimming)
//        ///     this method throws an <see cref="InvalidOperationException"/>.
//        ///     If you are unsure whether trimming a path is possible, consider using
//        ///     <see cref="TryTrimEndingDirectorySeparator(out StoragePath?)"/>.
//        /// </remarks>
//        /// <exception cref="InvalidOperationException">
//        ///     Trimming the trailing directory separator character is not possible because the
//        ///     resulting path is invalid.
//        /// </exception>
//        public StoragePath TrimEndingDirectorySeparator() =>
//            _pathWithoutTrailingDirectorySeparatorLazy.Value;

//        private StoragePath TrimEndingDirectorySeparatorImpl()
//        {
//            if (!EndsWithDirectorySeparator)
//            {
//                return this;
//            }

//            if (Length == 1)
//            {
//                throw new InvalidOperationException(ExceptionStrings.StoragePath.TrimmingResultsInEmptyPath());
//            }

//            var trimmedPath = UnderlyingString.Substring(0, UnderlyingString.Length - 1);

//            try
//            {
//                return FileSystem.GetPath(trimmedPath);
//            }
//            catch (Exception ex)
//            {
//                throw new InvalidOperationException(ExceptionStrings.StoragePath.TrimmingResultsInInvalidPath(), ex);
//            }
//        }

//        /// <summary>
//        ///     Attempts to append the specified <paramref name="part"/> to the end of this path and
//        ///     return the resulting path.
//        /// </summary>
//        /// <param name="part">
//        ///     The part to be appended to the path.
//        /// </param>
//        /// <param name="result">
//        ///     An <see langword="out"/> parameter which will, if the operation succeedes,
//        ///     hold a new <see cref="StoragePath"/> instance which represents the path after appending the
//        ///     specified <paramref name="part"/>.
//        /// </param>
//        /// <returns>
//        ///     <see langword="true"/> if the operation succeeded; <see langword="false"/> if not.
//        /// </returns>
//        public bool TryAppend(string? part, [NotNullWhen(true)] out StoragePath? result)
//        {
//            if (part is null)
//            {
//                result = null;
//                return false;
//            }

//            try
//            {
//                result = Append(part);
//                return true;
//            }
//            catch
//            {
//                result = null;
//                return false;
//            }
//        }

//        /// <summary>
//        ///     Appends the specified <paramref name="part"/> to the end of this path.
//        /// </summary>
//        /// <param name="part">
//        ///     The part to be appended to the path.
//        /// </param>
//        /// <returns>
//        ///     A new <see cref="StoragePath"/> instance which represents the path after appending the
//        ///     specified <paramref name="part"/>.
//        /// </returns>
//        /// <exception cref="ArgumentNullException">
//        ///     <paramref name="part"/>
//        /// </exception>
//        /// <exception cref="ArgumentException">
//        ///     Appending <paramref name="part"/> would result in an invalid path.
//        /// </exception>
//        public StoragePath Append(string part)
//        {
//            _ = part ?? throw new ArgumentNullException(nameof(part));
//            if (part.Length == 0)
//            {
//                return this;
//            }

//            return FileSystem.GetPath(UnderlyingString + part);
//        }

//        /// <inheritdoc cref="TryCombine(string?, out StoragePath?)"/>
//        public bool TryCombine(StoragePath? other, [NotNullWhen(true)] out StoragePath? result) =>
//            TryCombine(other?.UnderlyingString, out result);

//        /// <summary>
//        ///     Attempts to concatenate the two paths while also ensuring that <i>at least one</i> directory separator
//        ///     character is inserted between them.
//        ///     
//        ///     If <paramref name="other"/> is rooted or starts with a directory separator character,
//        ///     this path is discarded and the resulting path will simply be <paramref name="other"/>.
//        ///     
//        ///     See remarks of <see cref="Combine(string)"/> for details and examples.
//        /// </summary>
//        /// <param name="other">
//        ///     Another path to be concatenated with this path.
//        /// </param>
//        /// <param name="result">
//        ///     An <see langword="out"/> parameter which will, if the operation succeedes,
//        ///     hold the resulting concatenated path.
//        /// </param>
//        /// <returns>
//        ///     <see langword="true"/> if the operation succeeded; <see langword="false"/> if not.
//        /// </returns>
//        public bool TryCombine(string? other, [NotNullWhen(true)] out StoragePath? result)
//        {
//            if (other is null)
//            {
//                result = null;
//                return false;
//            }

//            try
//            {
//                result = Combine(other);
//                return true;
//            }
//            catch
//            {
//                result = null;
//                return false;
//            }
//        }

//        /// <inheritdoc cref="Combine(string)"/>
//        public StoragePath Combine(StoragePath other) =>
//            Combine(other?.UnderlyingString!);

//        /// <summary>
//        ///     Concatenates the two paths while also ensuring that <i>at least one</i> directory separator
//        ///     character is present between them.
//        ///     
//        ///     If <paramref name="other"/> is rooted (i.e. it starts with a directory separator character),
//        ///     this path is discarded and the resulting path will simply be <paramref name="other"/>.
//        ///     
//        ///     See remarks for details and examples.
//        /// </summary>
//        /// <param name="other">
//        ///     Another path to be concatenated with this path.
//        /// </param>
//        /// <returns>The resulting concatenated path.</returns>
//        /// <remarks>
//        ///     This method behaves like .NET's <see cref="System.IO.Path.Combine(string, string)"/>
//        ///     method.
//        ///     
//        ///     In comparison to the alternatives (<see cref="Join(string)"/> and <see cref="Link(string)"/>),
//        ///     <see cref="Combine(string)"/> discards this path if <paramref name="other"/> is rooted or
//        ///     starts with a directory separator character.
//        ///     Out of the three methods, <see cref="Combine(string)"/> is the method that might
//        ///     remove the most information from the two specified paths.
//        ///     
//        ///     The following code demonstrates the behavior of <see cref="Combine(string)"/>:
//        ///     
//        ///     <code>
//        ///     // Note: The code assumes that / is the file system's directory separator.
//        ///     
//        ///     StoragePath first = fs.GetPath("firstPath");
//        ///     first.Combine("secondPath");    // Returns "firstPath/secondPath".
//        ///     first.Combine("/secondPath");   // Returns "/secondPath".
//        ///     first.Combine("///secondPath"); // Returns "///secondPath".
//        ///     
//        ///     first = fs.GetPath("firstPath/");
//        ///     first.Combine("secondPath");    // Returns "firstPath/secondPath".
//        ///     first.Combine("/secondPath");   // Returns "/secondPath".
//        ///     first.Combine("///secondPath"); // Returns "///secondPath".
//        ///     
//        ///     first = fs.GetPath("firstPath///");
//        ///     first.Combine("secondPath");    // Returns "firstPath///secondPath".
//        ///     first.Combine("/secondPath");   // Returns "/secondPath".
//        ///     first.Combine("///secondPath"); // Returns "///secondPath".
//        ///     
//        ///     first = fs.GetPath("firstPath");
//        ///     first.Join("");              // Returns "firstPath".
//        /// 
//        ///     first = fs.GetPath("/");
//        ///     first.Join("");              // Returns "/".
//        ///     first.Join("/");             // Returns "/".
//        ///     first.Join("//");            // Returns "//".
//        ///     
//        ///     first = fs.GetPath("//");
//        ///     first.Join("");              // Returns "//".
//        ///     first.Join("/");             // Returns "/".
//        ///     first.Join("//");            // Returns "//".
//        ///     </code>
//        /// </remarks>
//        /// <exception cref="ArgumentNullException">
//        ///     <paramref name="other"/> is <see langword="null"/>-
//        /// </exception>
//        /// <exception cref="ArgumentException">
//        ///     Concatenating the two paths results in a path with an invalid format.
//        /// </exception>
//        /// <seealso cref="Combine(StoragePath)"/>
//        /// <seealso cref="Join(string)"/>
//        /// <seealso cref="Join(StoragePath)"/>
//        /// <seealso cref="Link(string)"/>
//        /// <seealso cref="Link(StoragePath)"/>
//        public StoragePath Combine(string other)
//        {
//            _ = other ?? throw new ArgumentNullException(nameof(other));
//            if (other.Length == 0)
//            {
//                return this;
//            }

//            var otherStoragePath = FileSystem.GetPath(other);
//            if (otherStoragePath.Root is not null)
//            {
//                return otherStoragePath;
//            }

//            return Join(other);
//        }

//        /// <inheritdoc cref="TryJoin(string?, out StoragePath?)"/>
//        public bool TryJoin(StoragePath? other, [NotNullWhen(true)] out StoragePath? result) =>
//            TryJoin(other?.UnderlyingString, out result);

//        /// <summary>
//        ///     Attempts to concatenate the two paths while also ensuring that <i>at least one</i> directory separator
//        ///     character is inserted between them.
//        ///     
//        ///     All leading/trailing directory separator chars of <paramref name="other"/> and this path
//        ///     are preserved. Neither path is discarded.
//        ///     
//        ///     See remarks of <see cref="Join(string)"/> for details and examples.
//        /// </summary>
//        /// <param name="other">
//        ///     Another path to be concatenated with this path.
//        /// </param>
//        /// <param name="result">
//        ///     An <see langword="out"/> parameter which will, if the operation succeedes,
//        ///     hold the resulting concatenated path.
//        /// </param>
//        /// <returns>
//        ///     <see langword="true"/> if the operation succeeded; <see langword="false"/> if not.
//        /// </returns>
//        public bool TryJoin(string? other, [NotNullWhen(true)] out StoragePath? result)
//        {
//            if (other is null)
//            {
//                result = null;
//                return false;
//            }

//            try
//            {
//                result = Join(other);
//                return true;
//            }
//            catch
//            {
//                result = null;
//                return false;
//            }
//        }

//        /// <inheritdoc cref="Join(string)"/>
//        public StoragePath Join(StoragePath other) =>
//            Join(other?.UnderlyingString!);

//        /// <summary>
//        ///     Concatenates the two paths while also ensuring that <i>at least one</i> directory separator
//        ///     character is present between them.
//        ///     
//        ///     All leading/trailing directory separator chars of <paramref name="other"/> and this path
//        ///     are preserved. Neither path is discarded.
//        ///     
//        ///     See remarks for details and examples.
//        /// </summary>
//        /// <param name="other">
//        ///     Another path to be concatenated with this path.
//        /// </param>
//        /// <returns>The resulting concatenated path.</returns>
//        /// <remarks>
//        ///     This method behaves like .NET's
//        ///     <c>System.IO.Path.Join(ReadOnlySpan&lt;char&gt;, ReadOnlySpan&lt;char&gt;)</c> method.
//        ///     
//        ///     In comparison to the alternatives (<see cref="Combine(string)"/> and <see cref="Link(string)"/>),
//        ///     <see cref="Join(string)"/> preserves any leading/trailing directory separator chars of
//        ///     <paramref name="other"/> and this path.
//        ///     In comparison to <see cref="Combine(string)"/> specifically, neither path is discarded.
//        ///     Out of the three methods, <see cref="Join(string)"/> is the safest one as it does not
//        ///     remove any characters from either path.
//        ///     
//        ///     The following code demonstrates the behavior of <see cref="Join(string)"/>:
//        ///     
//        ///     <code>
//        ///     // Note: The code assumes that / is the file system's directory separator.
//        ///     
//        ///     StoragePath first = fs.GetPath("firstPath");
//        ///     first.Join("secondPath");    // Returns "firstPath/secondPath".
//        ///     first.Join("/secondPath");   // Returns "firstPath/secondPath".
//        ///     first.Join("///secondPath"); // Returns "firstPath///secondPath".
//        ///     
//        ///     first = fs.GetPath("firstPath/");
//        ///     first.Join("secondPath");    // Returns "firstPath/secondPath".
//        ///     first.Join("/secondPath");   // Returns "firstPath//secondPath".
//        ///     first.Join("///secondPath"); // Returns "firstPath////secondPath".
//        ///     
//        ///     first = fs.GetPath("firstPath///");
//        ///     first.Join("secondPath");    // Returns "firstPath///secondPath".
//        ///     first.Join("/secondPath");   // Returns "firstPath////secondPath".
//        ///     first.Join("///secondPath"); // Returns "firstPath//////secondPath".
//        ///     
//        ///     first = fs.GetPath("firstPath");
//        ///     first.Join("");              // Returns "firstPath".
//        /// 
//        ///     first = fs.GetPath("/");
//        ///     first.Join("");              // Returns "/".
//        ///     first.Join("/");             // Returns "//".
//        ///     first.Join("//");            // Returns "///".
//        ///     
//        ///     first = fs.GetPath("//");
//        ///     first.Join("");              // Returns "//".
//        ///     first.Join("/");             // Returns "///".
//        ///     first.Join("//");            // Returns "////".
//        ///     </code>
//        /// </remarks>
//        /// <exception cref="ArgumentNullException">
//        ///     <paramref name="other"/> is <see langword="null"/>-
//        /// </exception>
//        /// <exception cref="ArgumentException">
//        ///     Concatenating the two paths results in a path with an invalid format.
//        /// </exception>
//        /// <seealso cref="Combine(string)"/>
//        /// <seealso cref="Combine(StoragePath)"/>
//        /// <seealso cref="Join(StoragePath)"/>
//        /// <seealso cref="Link(string)"/>
//        /// <seealso cref="Link(StoragePath)"/>
//        public StoragePath Join(string other)
//        {
//            _ = other ?? throw new ArgumentNullException(nameof(other));
//            if (other.Length == 0)
//            {
//                return this;
//            }

//            var hasSeparator =
//                IsDirectorySeparator(UnderlyingString[UnderlyingString.Length - 1]) ||
//                IsDirectorySeparator(other[0]);

//            var joinedPath = hasSeparator
//                ? $"{UnderlyingString}{other}"
//                : $"{UnderlyingString}{FileSystem.PathInformation.DirectorySeparatorChar}{other}";

//            return FileSystem.GetPath(joinedPath);
//        }

//        /// <inheritdoc cref="TryLink(string?, out StoragePath?)"/>
//        public bool TryLink(StoragePath? other, [NotNullWhen(true)] out StoragePath? result) =>
//            TryLink(other?.UnderlyingString, out result);

//        /// <summary>
//        ///     Attempts to concatenate the two paths while also ensuring that <i>exactly one</i> directory separator
//        ///     character is inserted between them.
//        ///     
//        ///     Excess leading/trailing directory separators are removed from <paramref name="other"/>/this path
//        ///     in order to end up with exactly one separator between them. Neither path is discarded.
//        ///     
//        ///     See remarks of <see cref="Link(string)"/> for details and examples.
//        /// </summary>
//        /// <param name="other">
//        ///     Another path to be concatenated with this path.
//        /// </param>
//        /// <param name="result">
//        ///     An <see langword="out"/> parameter which will, if the operation succeedes,
//        ///     hold the resulting concatenated path.
//        /// </param>
//        /// <returns>
//        ///     <see langword="true"/> if the operation succeeded; <see langword="false"/> if not.
//        /// </returns>
//        public bool TryLink(string? other, [NotNullWhen(true)] out StoragePath? result)
//        {
//            if (other is null)
//            {
//                result = null;
//                return false;
//            }

//            try
//            {
//                result = Link(other);
//                return true;
//            }
//            catch
//            {
//                result = null;
//                return false;
//            }
//        }

//        /// <inheritdoc cref="Link(string)"/>
//        public StoragePath Link(StoragePath other) =>
//            Link(other?.UnderlyingString!);

//        /// <summary>
//        ///     Concatenates the two paths while also ensuring that <i>exactly one</i> directory separator
//        ///     character is present between them.
//        ///     
//        ///     Excess leading/trailing directory separators are removed from <paramref name="other"/>/this path
//        ///     in order to end up with exactly one separator between them. Neither path is discarded.
//        ///     
//        ///     See remarks for details and examples.
//        /// </summary>
//        /// <param name="other">
//        ///     Another path to be concatenated with this path.
//        /// </param>
//        /// <returns>The resulting concatenated path.</returns>
//        /// <remarks>
//        ///     This method currently doesn't have an equivalent in the .NET Framework.
//        ///     In essence, it behaves similarly to
//        ///     <c>System.IO.Path.Join(ReadOnlySpan&lt;char&gt;, ReadOnlySpan&lt;char&gt;)</c>,
//        ///     but with the difference that excess directory separators are removed between the two
//        ///     paths.
//        ///     
//        ///     In comparison to the alternatives (<see cref="Combine(string)"/> and <see cref="Join(string)"/>),
//        ///     <see cref="Link(string)"/> removes excess leading/trailing directory separator chars of
//        ///     <paramref name="other"/>/this path before concatenating them. This ensures that
//        ///     exactly one directory separator character is present between the two paths.
//        ///     In comparison to the alternatives, this method is the ideal when dealing
//        ///     with user input, as the result will, most likely, be a valid path without an
//        ///     excess number of directory separator characters.
//        ///     In comparison to <see cref="Join(string)"/> specifically, results like <c>firstPath//secondPath</c>
//        ///     are not possible with this method.
//        ///     
//        ///     Be aware that using this method can change the meaning/format of <paramref name="other"/> if it
//        ///     is a special path. If <paramref name="other"/> is, for example, a UNC path, trimming
//        ///     the two leading directory separator chars <c>//</c> will inevitably change the path's meaning.
//        ///     Then again, such a path (and absolute paths in general) should most likely not be
//        ///     concatenated with other paths in the first place.
//        ///     
//        ///     The following code demonstrates the behavior of <see cref="Link(string)"/>:
//        ///     
//        ///     <code>
//        ///     // Note: The code assumes that / is the file system's directory separator.
//        ///     
//        ///     StoragePath first = fs.GetPath("firstPath");
//        ///     first.Join("secondPath");    // Returns "firstPath/secondPath".
//        ///     first.Join("/secondPath");   // Returns "firstPath/secondPath".
//        ///     first.Join("///secondPath"); // Returns "firstPath/secondPath".
//        ///     
//        ///     first = fs.GetPath("firstPath/");
//        ///     first.Join("secondPath");    // Returns "firstPath/secondPath".
//        ///     first.Join("/secondPath");   // Returns "firstPath/secondPath".
//        ///     first.Join("///secondPath"); // Returns "firstPath/secondPath".
//        ///     
//        ///     first = fs.GetPath("firstPath///");
//        ///     first.Join("secondPath");    // Returns "firstPath/secondPath".
//        ///     first.Join("/secondPath");   // Returns "firstPath/secondPath".
//        ///     first.Join("///secondPath"); // Returns "firstPath/secondPath".
//        ///     
//        ///     first = fs.GetPath("firstPath");
//        ///     first.Join("");              // Returns "firstPath".
//        /// 
//        ///     first = fs.GetPath("/");
//        ///     first.Join("");              // Returns "/".
//        ///     first.Join("/");             // Returns "/".
//        ///     first.Join("//");            // Returns "/".
//        ///     
//        ///     first = fs.GetPath("//");
//        ///     first.Join("");              // Returns "//".
//        ///     first.Join("/");             // Returns "/".
//        ///     first.Join("//");            // Returns "/".
//        ///     </code>
//        /// </remarks>
//        /// <exception cref="ArgumentNullException">
//        ///     <paramref name="other"/> is <see langword="null"/>-
//        /// </exception>
//        /// <exception cref="ArgumentException">
//        ///     Concatenating the two paths results in a path with an invalid format.
//        /// </exception>
//        /// <seealso cref="Combine(string)"/>
//        /// <seealso cref="Combine(StoragePath)"/>
//        /// <seealso cref="Join(string)"/>
//        /// <seealso cref="Join(StoragePath)"/>
//        /// <seealso cref="Link(StoragePath)"/>
//        public StoragePath Link(string other)
//        {
//            _ = other ?? throw new ArgumentNullException(nameof(other));
//            if (other.Length == 0)
//            {
//                return this;
//            }

//            var part1 = UnderlyingString.TrimEnd(FileSystem.PathInformation.DirectorySeparatorChars.ToArray());
//            var part2 = other.TrimStart(FileSystem.PathInformation.DirectorySeparatorChars.ToArray());
//            return FileSystem.GetPath($"{part1}{FileSystem.PathInformation.DirectorySeparatorChar}{part2}");
//        }

//        /// <summary>
//        ///     Compares this path with another path based on the path strings.
//        ///     The comparison is done using the <see cref="PathInformation.DefaultStringComparison"/>
//        ///     of this path's file system.
//        /// </summary>
//        /// <param name="path">
//        ///     Another <see cref="StoragePath"/> or <see cref="string"/> to be compared with this path.
//        ///     
//        ///     If this is any other object or <see langword="null"/>, the return value is undefined.
//        /// </param>
//        /// <returns>
//        ///     A negative value if this path precedes the other <paramref name="path"/>.
//        ///     Zero if the two paths are considered equal.
//        ///     A positive value if this path follows the other <paramref name="path"/>.
//        /// </returns>
//        int IComparable.CompareTo(object? path) => path switch
//        {
//            StoragePath storagePath => CompareTo(storagePath),
//            string pathStr => CompareTo(pathStr),
//            null => CompareTo((string?)null),
//            _ => throw new ArgumentException(ExceptionStrings.Comparable.TypeIsNotSupported(path.GetType()), nameof(path)),
//        };

//        /// <inheritdoc cref="CompareTo(string?)"/>
//        public int CompareTo(StoragePath? path) =>
//            CompareTo(path, FileSystem.PathInformation.DefaultStringComparison);

//        /// <inheritdoc cref="CompareTo(string?, StringComparison)"/>
//        public int CompareTo(StoragePath? path, StringComparison stringComparison) =>
//            CompareTo(path?.UnderlyingString, stringComparison);

//        /// <summary>
//        ///     Compares this path with another path based on the path strings.
//        ///     The comparison is done using the <see cref="PathInformation.DefaultStringComparison"/>
//        ///     of this path's file system.
//        /// </summary>
//        /// <param name="path">Another path to be compared with this path.</param>
//        /// <returns>
//        ///     A negative value if this path precedes the other <paramref name="path"/>.
//        ///     Zero if the two paths are considered equal.
//        ///     A positive value if this path follows the other <paramref name="path"/>.
//        /// </returns>
//        public int CompareTo(string? path) =>
//            CompareTo(path, FileSystem.PathInformation.DefaultStringComparison);

//        /// <summary>
//        ///     Compares this path with another path based on the path strings.
//        ///     The comparison is done using the specified <paramref name="stringComparison"/> value.
//        /// </summary>
//        /// <param name="path">Another path to be compared with this path.</param>
//        /// <param name="stringComparison">
//        ///     The <see cref="StringComparison"/> to be used for comparing the two paths.
//        /// </param>
//        /// <returns>
//        ///     A negative value if this path precedes the other <paramref name="path"/>.
//        ///     Zero if the two paths are considered equal.
//        ///     A positive value if this path follows the other <paramref name="path"/>.
//        /// </returns>
//        public int CompareTo(string? path, StringComparison stringComparison) =>
//            string.Compare(UnderlyingString, path, stringComparison);

//        /// <summary>
//        ///     Compares this path with another path for string equality.
//        ///     The comparison is done using the <see cref="PathInformation.DefaultStringComparison"/>
//        ///     of this path's file system.
//        /// </summary>
//        /// <param name="path">
//        ///     Another <see cref="StoragePath"/> or <see cref="string"/> to be compared with this path.
//        ///     
//        ///     If this is any other object or <see langword="null"/>, this method returns <see langword="false"/>.
//        /// </param>
//        /// <returns>
//        ///     <see langword="true"/> if the two paths are considered equal;
//        ///     <see langword="false"/> if not.
//        /// </returns>
//        public sealed override bool Equals(object? path) => path switch
//        {
//            string pathStr => Equals(pathStr),
//            StoragePath storagePath => Equals(storagePath),
//            _ => false
//        };

//        /// <inheritdoc cref="Equals(string?)"/>
//        public bool Equals(StoragePath? path) =>
//            Equals(path?.UnderlyingString, FileSystem.PathInformation.DefaultStringComparison);

//        /// <inheritdoc cref="Equals(string?, StringComparison)"/>
//        public bool Equals(StoragePath? path, StringComparison stringComparison) =>
//            Equals(path?.UnderlyingString, stringComparison);

//        /// <summary>
//        ///     Compares this path with another path for string equality.
//        ///     The comparison is done using the <see cref="PathInformation.DefaultStringComparison"/>
//        ///     of this path's file system.
//        /// </summary>
//        /// <param name="path">Another path to be compared with this path.</param>
//        /// <returns>
//        ///     <see langword="true"/> if the two paths are considered equal;
//        ///     <see langword="false"/> if not.
//        /// </returns>
//        public bool Equals(string? path) =>
//            Equals(path, FileSystem.PathInformation.DefaultStringComparison);

//        /// <summary>
//        ///     Compares this path with another path for string equality.
//        ///     The comparison is done using the specified <paramref name="stringComparison"/> value.
//        /// </summary>
//        /// <param name="path">Another path to be compared with this path.</param>
//        /// <param name="stringComparison">
//        ///     The <see cref="StringComparison"/> to be used for comparing the two paths.
//        /// </param>
//        /// <returns>
//        ///     <see langword="true"/> if the two paths are considered equal;
//        ///     <see langword="false"/> if not.
//        /// </returns>
//        public bool Equals(string? path, StringComparison stringComparison) =>
//            ToString().Equals(path, stringComparison);

//        /// <summary>
//        ///     Returns a hash code for the path.
//        /// </summary>
//        /// <returns>
//        ///     A hash code for the path.
//        ///     This method returns the hash code of the underlying path string.
//        /// </returns>
//        [DebuggerStepThrough]
//        public sealed override int GetHashCode() =>
//            UnderlyingString.GetHashCode();

//        /// <summary>
//        ///     Returns the underlying path string with which this path has been initialized.
//        /// </summary>
//        /// <returns>
//        ///     The underlying path string with which this path has been initialized.
//        ///     This value is never <see langword="null"/> or empty.
//        /// </returns>
//        /// <seealso cref="UnderlyingString"/>
//        [DebuggerStepThrough]
//        public sealed override string ToString() =>
//            UnderlyingString;

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private bool IsDirectorySeparator(char c)
//        {
//            return c == FileSystem.PathInformation.DirectorySeparatorChar
//                || c == FileSystem.PathInformation.AltDirectorySeparatorChar;
//        }

//        /// <inheritdoc cref="operator /(StoragePath, string)"/>
//        public static StoragePath operator /(StoragePath path1, StoragePath path2)
//        {
//            // The called overload validates for null.
//            return path1 / path2?.UnderlyingString!;
//        }

//        /// <summary>
//        ///     Concatenates the two paths via the <see cref="Join(string)"/> method.
//        ///     Please see <see cref="Join(string)"/> for details on the specifics of the concatenation.
//        /// </summary>
//        /// <param name="path1">The first path.</param>
//        /// <param name="path2">The second path.</param>
//        /// <returns>The resulting concatenated path.</returns>
//        /// <exception cref="ArgumentNullException">
//        ///     <paramref name="path1"/> or <paramref name="path2"/> is <see langword="null"/>.
//        /// </exception>
//        /// <exception cref="ArgumentException">
//        ///     Concatenating the two paths results in a path with an invalid format.
//        /// </exception>
//        /// <seealso cref="Join(string)"/>
//        /// <seealso cref="Join(StoragePath)"/>
//        public static StoragePath operator /(StoragePath path1, string path2)
//        {
//            _ = path1 ?? throw new ArgumentNullException(nameof(path1));
//            _ = path2 ?? throw new ArgumentNullException(nameof(path2));
//            return path1.Join(path2);
//        }

//        /// <summary>
//        ///     Appends the specified <paramref name="part"/> to the end of the <paramref name="path"/>.
//        /// </summary>
//        /// <param name="path">
//        ///     The path to which the specified <paramref name="part"/> should be appended.
//        /// </param>
//        /// <param name="part">
//        ///     The part to be appended to the path.
//        /// </param>
//        /// <returns>
//        ///     A new <see cref="StoragePath"/> instance which represents the path after appending the
//        ///     specified <paramref name="part"/>.
//        /// </returns>
//        /// <exception cref="ArgumentNullException">
//        ///     <paramref name="part"/>
//        /// </exception>
//        /// <exception cref="ArgumentException">
//        ///     Appending <paramref name="part"/> would result in an invalid path.
//        /// </exception>
//        /// <seealso cref="Append(string)"/>
//        public static StoragePath operator +(StoragePath path, string part)
//        {
//            _ = path ?? throw new ArgumentNullException(nameof(path));
//            _ = part ?? throw new ArgumentNullException(nameof(part));
//            return path.Append(part);
//        }

//        /// <inheritdoc cref="operator ==(string?, StoragePath?)"/>
//        public static bool operator ==(StoragePath? path1, StoragePath? path2) =>
//            path1?.Equals(path2) ?? path2?.Equals(path1) ?? true;

//        /// <inheritdoc cref="operator !=(string?, StoragePath?)"/>
//        public static bool operator !=(StoragePath? path1, StoragePath? path2) =>
//            !(path1 == path2);

//        /// <inheritdoc cref="operator ==(string?, StoragePath?)"/>
//        public static bool operator ==(StoragePath? path1, string? path2) =>
//            path1?.Equals(path2) ?? path2 is null;

//        /// <inheritdoc cref="operator !=(string?, StoragePath?)"/>
//        public static bool operator !=(StoragePath? path1, string? path2) =>
//            !(path1 == path2);

//        /// <summary>
//        ///     Compares the two paths for string equality.
//        ///     The comparison is done using the <see cref="PathInformation.DefaultStringComparison"/>
//        ///     of this path's file system.
//        /// </summary>
//        /// <param name="path1">The first path.</param>
//        /// <param name="path2">The second path.</param>
//        /// <returns>
//        ///     <see langword="true"/> if the two paths are considered equal;
//        ///     <see langword="false"/> if not.
//        /// </returns>
//        public static bool operator ==(string? path1, StoragePath? path2) =>
//            path2?.Equals(path1) ?? path1 is null;

//        /// <summary>
//        ///     Compares the two paths for string inequality.
//        ///     The comparison is done using the <see cref="PathInformation.DefaultStringComparison"/>
//        ///     of this path's file system.
//        /// </summary>
//        /// <param name="path1">The first path.</param>
//        /// <param name="path2">The second path.</param>
//        /// <returns>
//        ///     <see langword="true"/> if the two paths are considered unequal;
//        ///     <see langword="false"/> if not.
//        /// </returns>
//        public static bool operator !=(string? path1, StoragePath? path2) =>
//            !(path1 == path2);

//        /// <summary>
//        ///     Implicitly converts the specified <paramref name="path"/> to its underlying path string.
//        /// </summary>
//        /// <param name="path">The path to be converted.</param>
//        /// <returns>
//        ///     The path's underlying path string or <see langword="null"/> if <paramref name="path"/>
//        ///     is <see langword="null"/>.
//        /// </returns>
//        [return: NotNullIfNotNull("path")]
//        public static implicit operator string?(StoragePath? path) =>
//            path?.ToString();
//    }
//#pragma warning restore CA1036
//}

