using System.Collections.Generic;

namespace Utility.Nodify.Operations
{

    //public enum OperationType
    //{
    //    Normal,
    //    Expando,
    //    Expression,
    //    Calculator,
    //    //Group,
    //    Graph
    //}

    //public class OperationInfo
    //{
    //    public string Title { get; set; }
    //    //public OperationType Type { get; set; }
    //    public IOperation? Operation { get; set; }
    //    public List<Input> Inputs { get; } = new List<Input>();
    //    public uint MinInput { get; set; }
    //    public uint MaxInput { get; set; }
    //}

    public class FilterInfo
    {
        public string Title { get; set; }

        public IFilter? Filter { get; set; }
    }

    public record Input(string Name, object DefaultValue);
}
