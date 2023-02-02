using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Utility.Structs
{
    public interface IValidationResult<TIn>
    {
        TIn Input { get; }

        string Message { get; }

        bool Success { get; }
    }

    public class ValidationResult<TIn, TEnum> : IValidationResult<TIn> where TEnum : struct, Enum where TIn : notnull
    {
        public ValidationResult(TIn value, bool success, TEnum? code = null, string? message = null)
        {
            if (success == false && (message == null || code == null))
                throw new ArgumentNullException(nameof(message), "Constructing " + nameof(ValidationResult<TIn, TEnum>));
            Input = value;
            Success = success;
            Code = code;
            this.Message = message!;
        }

        public TIn Input { get; }

        public bool Success { get; }

        public TEnum? Code { get; }

        public string Message { get; }

        public override string ToString()
        {
            return Message + " where input is " + Input.ToString();
        }
    }

    public interface IValidater<TIn>
    {
        TIn Value { get; }

        IValidationResult<TIn> Validate();
    }

    public class Validater<TIn, TEnum> : IValidater<TIn> where TEnum : struct, Enum where TIn : notnull
    {
        private readonly Expression<Func<TIn, bool>> lambda;

        public Validater(TIn value, TEnum code, Expression<Func<TIn, bool>> lambda)
        {         
            Value = value;
            Code = code;
            this.lambda = lambda;
        }

        public TIn Value { get; }

        public TEnum Code { get; }

        public virtual IValidationResult<TIn> Validate()
        {
            return lambda.Compile().Invoke(Value) ? new ValidationResult<TIn, TEnum>(Value, true, default(TEnum), this.ToString()) : new ValidationResult<TIn, TEnum>(Value, false, Code, this.ToString());
        }

        public override string ToString()
        {
            return lambda.ToString();
        }
    }

    public class MultiValidationResult<T>
    {
        public MultiValidationResult(IReadOnlyList<IValidationResult<T>> passed, IReadOnlyList<IValidationResult<T>> failed)
        {
            Passed = passed;
            Failed = failed;
        }

        public bool IsValid => Failed.Count == 0;

        public IReadOnlyList<IValidationResult<T>> Passed { get; }

        public IReadOnlyList<IValidationResult<T>> Failed { get; }
    }

    public class MultiValidater<TIn>
    {
        public MultiValidater(TIn value, IReadOnlyList<IValidater<TIn>> validaters)
        {
            Value = value;
            Validaters = validaters;
        }

        public TIn Value { get; }

        public IReadOnlyList<IValidater<TIn>> Validaters { get; }

        public virtual MultiValidationResult<TIn> Validate()
        {
            List<IValidationResult<TIn>> passed = new List<IValidationResult<TIn>>();
            List<IValidationResult<TIn>> failed = new List<IValidationResult<TIn>>();

            foreach (var v in Validaters)
            {
                var output = v.Validate() switch
                {
                    { Success: { } success } validation when success => (passed, validation),
                    { Success: { } success } validation when success == false => (failed, validation),
                    _ => throw new ArgumentOutOfRangeException(v.ToString()),
                };
                output.Item1.Add(output.validation);
            }

            return new MultiValidationResult<TIn>(passed, failed);
        }
    }

    public class MultiValidationException<T> : Exception
    {
        public MultiValidationException(MultiValidationResult<T> result)
        {
            Result = result;
        }

        public MultiValidationResult<T> Result { get; }
    }
}
