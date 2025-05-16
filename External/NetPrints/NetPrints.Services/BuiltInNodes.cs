using NetPrints.Core;
using NetPrints.Graph;
using NetPrints.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPrints.WPF.Demo
{
    public class BuiltInNodes : IBuiltInNodes
    {

        Lazy<Dictionary<GraphType, ITypeSpecifier[]>> dict = new(() => builtInNodes);

        public ITypeSpecifier[] this[GraphType graph]
        {
            get
            {
                if (dict.Value.TryGetValue(graph, out var nodes))
                {
                    return nodes;
                }

                return [];
            }
        }

        static readonly Dictionary<GraphType, ITypeSpecifier[]> builtInNodes = new()
        {
            [GraphType.Method] =
            [
                TypeSpecifier.FromType<ForLoopNode>(),
                TypeSpecifier.FromType<IfElseNode>(),
                TypeSpecifier.FromType<ConstructorNode>(),
                TypeSpecifier.FromType<TypeOfNode>(),
                TypeSpecifier.FromType<ExplicitCastNode>(),
                TypeSpecifier.FromType<ReturnNode>(),
                TypeSpecifier.FromType<MakeArrayNode>(),
                TypeSpecifier.FromType<LiteralNode>(),
                TypeSpecifier.FromType<TypeNode>(),
                TypeSpecifier.FromType<MakeArrayTypeNode>(),
                TypeSpecifier.FromType<ThrowNode>(),
                TypeSpecifier.FromType<AwaitNode>(),
                TypeSpecifier.FromType<TernaryNode>(),
                TypeSpecifier.FromType<DefaultNode>(),
            ],
            [GraphType.Constructor] =
            [
                TypeSpecifier.FromType<ForLoopNode>(),
                TypeSpecifier.FromType<IfElseNode>(),
                TypeSpecifier.FromType<ConstructorNode>(),
                TypeSpecifier.FromType<TypeOfNode>(),
                TypeSpecifier.FromType<ExplicitCastNode>(),
                TypeSpecifier.FromType<MakeArrayNode>(),
                TypeSpecifier.FromType<LiteralNode>(),
                TypeSpecifier.FromType<TypeNode>(),
                TypeSpecifier.FromType<MakeArrayTypeNode>(),
                TypeSpecifier.FromType<ThrowNode>(),
                TypeSpecifier.FromType<TernaryNode>(),
                TypeSpecifier.FromType<DefaultNode>(),
            ],
            [GraphType.Class] =
            [
                TypeSpecifier.FromType<TypeNode>(),
                TypeSpecifier.FromType<MakeArrayTypeNode>(),
            ],
        };

        public static BuiltInNodes Instance { get; } = new();
    }
}
