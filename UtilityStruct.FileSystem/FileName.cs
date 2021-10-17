using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UtilityStruct.Common;
using UtilityStruct.Infrastructure;

namespace UtilityStruct.FileSystem
{
    public ref struct FileName
    {
        public enum ErrorCode
        {
            FileNameContainsInvalidCharacter
        }
        public FileName(string name) : this(name.AsSpan())
        {
            if (Validate(name) is { Failed: { Count: { } failures } failed } valids && failures > 0)
                throw new MultiValidationException<string>(valids);
        }

        public FileName(string name, string extension) : this(name.AsSpan(), extension.AsSpan())
{
            if (Validate(name) is { Failed: { Count: { } failures } failed } valids && failures > 0)
                throw new MultiValidationException<string>(valids);
        }

        public FileName(ReadOnlySpan<char> name)
        {
            Value = name;
            WithOutExtension = PathHelper.GetNameWithoutExtension(name);
            Extension = new Extension(System.IO.Path.GetExtension(name));
        }

        public FileName(ReadOnlySpan<char> name, ReadOnlySpan<char> extension)
        {
            Value = SpanHelper.Concat(name, ".", extension);
            WithOutExtension = name;
            Extension = new Extension(extension);
        }

        public ReadOnlySpan<char> Value { get; }

        public ReadOnlySpan<char> WithOutExtension { get; }

        public Extension Extension { get; }

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
                    new[] { new Validater<string, ErrorCode>(path, ErrorCode.FileNameContainsInvalidCharacter, a => PathHelper.IsFileNameCharactersValid(a)) });

        }

        public class Validator : Validater<string, ErrorCode>
        {
            public Validator(string value, ErrorCode code, Expression<Func<string, bool>> lambda) : base(value, code, lambda)
            {
            }
        }

        public static ReadOnlySpan<char> MakeValid(ReadOnlySpan<char> name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return SpanHelper.Replace(name, invalidRegStr, "_");
        }

        public FileName ChangeExtension(ReadOnlySpan<char> extension)
        {
            return new FileName(SpanHelper.Concat(this.WithOutExtension, ".", extension));
        }

        /// <summary>
        ///     Implicitly converts the specified <paramref name="fileName"/> to its underlying path string.
        /// </summary>
        /// <param name="fileName">The path to be converted.</param>
        /// <returns>
        ///     The path's underlying path string or <see langword="null"/> if <paramref name="fileName"/>
        ///     is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull("fileName")]
        public static implicit operator string(FileName fileName) => fileName.Value.ToString();

        /// <summary>
        ///     Implicitly converts the specified <paramref name="fileName"/> to its underlying path string.
        /// </summary>
        /// <param name="fileName">The path to be converted.</param>
        /// <returns>
        ///     The path's underlying path string or <see langword="null"/> if <paramref name="fileName"/>
        ///     is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull("fileName")]
        public static implicit operator FileName(string fileName) => new FileName(fileName);
    }
}
