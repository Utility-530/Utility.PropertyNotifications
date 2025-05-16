using NetPrints.Core;
using NetPrints.Enums;
using NetPrints.Graph;
using NetPrints.Interfaces;
using System;
using System.Linq;

namespace NetPrints.Services
{
    public class OverrideService
    {
        /// <summary>
        /// Adds an override method for the given method specifier to
        /// the given class.
        /// </summary>
        /// <param name="cls">Class to add the method to.</param>
        /// <param name="methodSpecifier">Method specifier for the method to override.</param>
        /// <returns>Method in the class that represents the overriding method.</returns>
        public static IMethodGraph AddOverrideMethod(IClassGraph cls, IMethodSpecifier methodSpecifier)
        {
            if (cls.Methods.Any(m => m.Name == methodSpecifier.Name)
                || !(methodSpecifier.Modifiers.HasFlag(MethodModifiers.Virtual)
                || methodSpecifier.Modifiers.HasFlag(MethodModifiers.Override)
                || methodSpecifier.Modifiers.HasFlag(MethodModifiers.Abstract)))
            {
                return null;
            }

            // Remove virtual & abstract flag and add override flag
            MethodModifiers modifiers = methodSpecifier.Modifiers;
            modifiers &= ~(MethodModifiers.Virtual | MethodModifiers.Abstract);
            modifiers |= MethodModifiers.Override;

            // Create method
            IMethodGraph newMethod = new MethodGraph(methodSpecifier.Name)
            {
                Class = cls,
                Modifiers = modifiers,
                Visibility = methodSpecifier.Visibility
            };

            // Set position of entry and return node
            newMethod.EntryNode.PositionX = 560;
            newMethod.EntryNode.PositionY = 504;
            newMethod.ReturnNodes.First().PositionX = newMethod.EntryNode.PositionX + 672;
            newMethod.ReturnNodes.First().PositionY = newMethod.EntryNode.PositionY;

            // Connect entry and return node execution pins
            GraphUtil.ConnectExecPins(newMethod.EntryNode.InitialExecutionPin, newMethod.MainReturnNode.ReturnPin);

            // Add generic arguments
            for (var i = 0; i < methodSpecifier.GenericArguments.Count; i++)
            {
                newMethod.MethodEntryNode.AddGenericArgument();
            }

            const int offsetX = -308;
            const int offsetY = -112;

            // Add argument pins, their type nodes and connect them
            for (int i = 0; i < methodSpecifier.Parameters.Count; i++)
            {
                IBaseType argType = methodSpecifier.Parameters[i].Value;
                TypeNode argTypeNode = GraphUtil.CreateNestedTypeNode(newMethod, argType, newMethod.EntryNode.PositionX + offsetX, newMethod.EntryNode.PositionY + offsetY * (i + 1));

                newMethod.MethodEntryNode.AddArgument();

                GraphUtil.ConnectTypePins(argTypeNode.OutputTypePins[0], newMethod.EntryNode.InputTypePins[i]);
            }

            // Add return types, their type nodes and connect them
            for (int i = 0; i < methodSpecifier.ReturnTypes.Count; i++)
            {
                IBaseType returnType = methodSpecifier.ReturnTypes[i];
                TypeNode returnTypeNode = GraphUtil.CreateNestedTypeNode(newMethod, returnType, newMethod.MainReturnNode.PositionX + offsetX, newMethod.MainReturnNode.PositionY + offsetY * (i + 1));

                newMethod.MainReturnNode.AddReturnType();

                GraphUtil.ConnectTypePins(returnTypeNode.OutputTypePins[0], newMethod.MainReturnNode.InputTypePins[i]);
            }

            cls.Methods.Add(newMethod);

            return newMethod;
        }
    }
}
