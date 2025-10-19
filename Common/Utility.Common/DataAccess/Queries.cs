//using Utility.Enums;
//using Utility.Interfaces.NonGeneric.Data;

//namespace Utility.Common
//{
//    public class FirstQuery : IQuery
//    {
//    }

//    public class FirstOrDefaultQuery : IQuery
//    {
//    }

//    public class AllQuery : IQuery
//    {
//    }

//    public class ContainsStringQuery : IQuery
//    {
//        public ContainsStringQuery(string text, string property)
//        {
//            Text = text;
//            Property = property;
//        }

//        public string Text { get; }
//        public string Property { get; }
//    }

//    public class MatchesStringQuery : IQuery
//    {
//        public MatchesStringQuery(string text, string property, AbsoluteOrder absoluteOrder)
//        {
//            Text = text;
//            Property = property;
//            AbsoluteOrder = absoluteOrder;
//        }

//        public string Text { get; }
//        public string Property { get; }

//        public AbsoluteOrder AbsoluteOrder { get; }
//    }

//    public class MatchesStringOrderQuery : IQuery
//    {
//        public MatchesStringOrderQuery(string text, string property, AbsoluteOrder absoluteOrder)
//        {
//            Text = text;
//            Property = property;
//            AbsoluteOrder = absoluteOrder;
//        }

//        public string Text { get; }
//        public string Property { get; }

//        public AbsoluteOrder AbsoluteOrder { get; }
//    }

//    public class CountQuery : IQuery
//    {
//    }
//}