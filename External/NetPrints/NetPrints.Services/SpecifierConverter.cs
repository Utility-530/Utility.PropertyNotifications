using NetPrints.Core;
using NetPrints.Core.Converters;
using NetPrints.Graph;
using NetPrintsEditor.Dialogs;
using System;

namespace NetPrintsEditor.Converters
{
    public class SpecifierConverter: ISpecifierConverter
    {
        public (string, string) Convert(ISpecifier value)
        {
            if (value is MethodSpecifier methodSpecifier)
            {
                return (MethodSpecifierConverter.Convert(methodSpecifier), OperatorUtil.IsOperator(methodSpecifier) ? "Operator_16x.png" : "Method_16x.png");
            }
            else if (value is VariableSpecifier variableSpecifier)
            {
                return ($"{variableSpecifier.Type} {variableSpecifier.Name} : {variableSpecifier.Type}", "Property_16x.png");
            }
            else if (value is DelegateSpecifier makeDelegateTypeInfo)
            {
                return ($"Make Delegate For A Method Of {makeDelegateTypeInfo.Type?.ShortName}", "Delegate_16x.png");
            }
            else if (value is TypeSpecifier t)
            {
                if (t == TypeSpecifier.FromType<ForLoopNode>())
                {
                    return ("For Loop", "Loop_16x.png");
                }
                else if (t == TypeSpecifier.FromType<IfElseNode>())
                {
                    return ("If Else", "If_16x.png");
                }
                else if (t == TypeSpecifier.FromType<ConstructorNode>())
                {
                    return ("Construct New Object", "Create_16x.png");
                }
                else if (t == TypeSpecifier.FromType<TypeOfNode>())
                {
                    return ("Type Of", "Type_16x.png");
                }
                else if (t == TypeSpecifier.FromType<ExplicitCastNode>())
                {
                    return ("Explicit Cast", "Convert_16x.png");
                }
                else if (t == TypeSpecifier.FromType<ReturnNode>())
                {
                    return ("Return", "Return_16x.png");
                }
                else if (t == TypeSpecifier.FromType<MakeArrayNode>())
                {
                    return ("Make Array", "ListView_16x.png");
                }
                else if (t == TypeSpecifier.FromType<LiteralNode>())
                {
                    return ("Literal", "Literal_16x.png");
                }
                else if (t == TypeSpecifier.FromType<TypeNode>())
                {
                    return ("Type", "Type_16x.png");
                }
                else if (t == TypeSpecifier.FromType<MakeArrayTypeNode>())
                {
                    return ("Make Array Type", "Type_16x.png");
                }
                else if (t == TypeSpecifier.FromType<ThrowNode>())
                {
                    return ("Throw", "Throw_16x.png");
                }
                else if (t == TypeSpecifier.FromType<AwaitNode>())
                {
                    return ("Await", "Task_16x.png");
                }
                else if (t == TypeSpecifier.FromType<TernaryNode>())
                {
                    return ("Ternary", "ConditionalRule_16x.png");
                }
                else if (t == TypeSpecifier.FromType<DefaultNode>())
                {
                    return ("Default", "None_16x.png");

                }
                else
                {
                    return (t.FullCodeName, "Type_16x.png");
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
