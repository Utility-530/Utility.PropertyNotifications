
using Utility.Enums;

namespace Utility.Models.Trees
{    public enum CustomStringComparison
    {
        EqualTo, NotEqualTo, Contains, DoesNotContain, StartsWith, EndsWith, IsNull, IsNotNull, IsNotNullOrWhiteSpace
    }

    public enum NumberComparison
    {
        EqualTo, NotEqualTo, GreaterThan, GreaterThanOrEqualTo, LessThan, LessThanOrEqualTo,
    }

    public enum BooleanComparison
    {
        EqualTo, NotEqualTo
    }

    public enum TypeComparison
    {
        EqualTo, NotEqualTo, AssignableTo, AssignableFrom
    }

    public enum ComparisonType
    {
        Default, String, Number, Boolean, Type,
    }


    public interface IAndOr
    {
        AndOr Value { get; }
    }

    public interface IResolvable
    {
        bool IsEqual(object _value);
    }

    public interface ISelectable
    {

    }
}
