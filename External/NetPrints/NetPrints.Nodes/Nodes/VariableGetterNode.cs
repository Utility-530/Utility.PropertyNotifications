using NetPrints.Core;
using NetPrints.Interfaces;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{
    /// <summary>
    /// Node that gets the value of a variable.
    /// </summary>
    [DataContract]
    public class VariableGetterNode : VariableNode
    {
        public VariableGetterNode(INodeGraph graph, VariableSpecifier variable)
            : base(graph, variable)
        {
        }

        public override string ToString()
        {
            string staticText = IsStatic ? $"{TargetType.ShortName}." : "";
            return $"Get {staticText}{VariableName}";
        }
    }
}
