using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Utility.Structs.Infrastructure;

namespace Utility.Structs.FileSystem
{
    public ref struct File
    {
        public enum ErrorCode
        {
            FileNameContainsInvalidCharacter
        }

        public File(string path) : this(path.AsSpan())
        {
            if (Validate(path) is { Failed: { Count: { } failures } failed } valids && failures > 0)
                throw new MultiValidationException<string>(valids);
        }

        public File(Directory directory, FileName fileName)
        {
            Value = directory / fileName;
            Name = fileName;
            Parent = directory;
        }

        public File(ReadOnlySpan<char> path)
        {
            Value = new Path(path);
            Name = new FileName(path);
            Parent = new Directory(System.IO.Path.GetDirectoryName(path));
        }

        public Path Value { get; }

        public Directory Parent { get; }

        public FileName Name { get; }


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
                Path.GetValidator(path).Validaters.Concat(
                    new[] { new Validater<string, ErrorCode>(path, ErrorCode.FileNameContainsInvalidCharacter, a => PathHelper.IsFileNameCharactersValid(a)) })
                .ToArray());
        }

        public class Validator : Validater<string, ErrorCode>
        {
            public Validator(string value, ErrorCode code, Expression<Func<string, bool>> lambda) : base(value, code, lambda)
            {
            }
        }


        /// <summary>
        ///     Implicitly converts the specified <paramref name="file"/> to its underlying path string.
        /// </summary>
        /// <param name="file">The path to be converted.</param>
        /// <returns>
        ///     The path's underlying path string or <see langword="null"/> if <paramref name="file"/>
        ///     is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull("file")]
        public static implicit operator File(string file) => new File(file);

        /// <summary>
        ///     Implicitly converts the specified <paramref name="file"/> to its underlying path string.
        /// </summary>
        /// <param name="file">The path to be converted.</param>
        /// <returns>
        ///     The path's underlying path string or <see langword="null"/> if <paramref name="file"/>
        ///     is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull("file")]
        public static implicit operator string(File file) => file.Value.Value.ToString();
    }
}
