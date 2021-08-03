using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using UtilityStruct.Helper;
using UtilityStruct.Infrastructure;

namespace UtilityStruct.FileSystem
{
    public ref struct FileNameWithoutExtension
    {
        public enum ErrorCode
        {
            FileNameContainsInvalidCharacter
        }

        public FileNameWithoutExtension(string name) : this(name.AsSpan())
        {
            if (Validate(name) is { Failed: { Count: { } failures } failed } valids && failures > 0)
                throw new MultiValidationException<string>(valids);
        }

        public FileNameWithoutExtension(ReadOnlySpan<char> name)
        {
            Value = name;
        }

        public ReadOnlySpan<char> Value { get; }

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
            return Helper.SpanHelper.Replace(name, invalidRegStr, "_");
        }

        public FileName AddExtension(ReadOnlySpan<char> extension)
        {
            return new FileName(SpanHelper.Concat(this.Value, ".", extension));
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
        public static implicit operator string(FileNameWithoutExtension fileName) => fileName.Value.ToString();

        /// <summary>
        ///     Implicitly converts the specified <paramref name="filename"/> to its underlying path string.
        /// </summary>
        /// <param name="filename">The path to be converted.</param>
        /// <returns>
        ///     The path's underlying path string or <see langword="null"/> if <paramref name="filename"/>
        ///     is <see langword="null"/>.
        /// </returns>
        [return: NotNullIfNotNull("filename")]
        public static implicit operator FileNameWithoutExtension(string filename) => new FileNameWithoutExtension(filename);
    }
}
