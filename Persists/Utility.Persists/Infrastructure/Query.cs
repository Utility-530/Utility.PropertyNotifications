using Utility.Enums;
using Utility.Interfaces.NonGeneric.Data;

namespace Utility.Persists
{
    public class MaxRowId : IQuery
    {
    }

    public class MaxRowIdResult : IQueryResult
    {
        public MaxRowIdResult(int id, bool isSuccess)
        {
            Id = id;
            IsSuccess = isSuccess;
        }

        public int Id { get; }

        public bool IsSuccess { get; }
    }

    public class FirstQuery : IQuery
    {
    }

    public class FirstOrDefaultQuery : IQuery
    {
    }

    public class AllQuery : IQuery
    {
    }

    public class ContainsStringQuery : IQuery
    {
        public ContainsStringQuery(string text, string property)
        {
            Text = text;
            Property = property;
        }

        public string Text { get; }
        public string Property { get; }
    }

    public class MatchesStringQuery : IQuery
    {
        public MatchesStringQuery(string text, string property, AbsoluteOrder absoluteOrder)
        {
            Text = text;
            Property = property;
            AbsoluteOrder = absoluteOrder;
        }

        public string Text { get; }
        public string Property { get; }

        public AbsoluteOrder AbsoluteOrder { get; }
    }

    public class MatchesStringOrderQuery : IQuery
    {
        public MatchesStringOrderQuery(string text, string property, AbsoluteOrder absoluteOrder)
        {
            Text = text;
            Property = property;
            AbsoluteOrder = absoluteOrder;
        }

        public string Text { get; }
        public string Property { get; }

        public AbsoluteOrder AbsoluteOrder { get; }
    }

    public class CountQuery : IQuery
    {
    }
}