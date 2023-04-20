using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityFlow;

namespace CoffeeFlow.WPF.Infrastructure.Infrastructure
{
    public class NodeWrapperFactory
    {
        public static IEnumerable<NodeWrapper> GetTriggers()
        {
            NodeWrapper r = new()
            {
                TypeOfNode = NodeType.RootNode,
                NodeName = "GameStart"
            };
            yield return r;

            NodeWrapper r2 = new()
            {
                TypeOfNode = NodeType.RootNode,
                NodeName = "Window Close"
            };
            yield return r2;

            NodeWrapper con = new()
            {
                TypeOfNode = NodeType.ConditionNode,
                NodeName = "Condition",
                IsDeletable = false
            };
            yield return con;
        }

        public static IEnumerable<NodeWrapper> GetMethods(string filename, bool isClassNameOnly = false)
        {
            string className = Path.GetFileNameWithoutExtension(filename);
            SyntaxTree syntaxTree;
            using (var stream = File.OpenRead(filename))
            {
                syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(stream));
            }
            var root = syntaxTree.GetRoot();

            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var classNode in classes)
            {
                string cname = classNode.Identifier.ToString();

                //Skip this class entirely if it doesn't match the class name of the code file
                if (isClassNameOnly && cname != className)
                    continue;

                IEnumerable<MethodDeclarationSyntax> methods = classNode.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

                foreach (var method in methods)
                {
                    NodeWrapper node = new NodeWrapper();
                    node.NodeName = method.Identifier.ToString();
                    node.CallingClass = cname;

                    bool isPublic = false;
                    foreach (var mod in method.Modifiers)
                    {
                        if (mod.IsKind(SyntaxKind.PublicKeyword))
                        {
                            isPublic = true;
                        }
                    }

                    if (!isPublic)
                        continue;

                    ParameterListSyntax parameters = method.ParameterList;

                    foreach (var param in parameters.Parameters)
                    {
                        Argument a = new Argument();
                        a.Name = param.Identifier.ToString();

                        a.ArgTypeString = param.Type.ToString();
                        node.Arguments.Add(a);
                    }

                    node.TypeOfNode = NodeType.MethodNode;
                    yield return node;
                    //Methods.Add(node);
                }
            }
        }

        public static IEnumerable<NodeWrapper> GetVariables(string filename, bool isClassNameOnly = false)
        {
            string className = Path.GetFileNameWithoutExtension(filename);
            SyntaxTree syntaxTree;
            using (var stream = File.OpenRead(filename))
            {
                syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(stream));
            }
            var root = syntaxTree.GetRoot();

            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var classDef in classes)
            {
                string cname = classDef.Identifier.ToString();

                //Skip this class entirely if it doesn't match the class name of the code file
                if (isClassNameOnly && cname != className)
                    continue;

                IEnumerable<FieldDeclarationSyntax> variables = classDef.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();

                foreach (var variable in variables)
                {
                    FieldDeclarationSyntax field = variable;
                    VariableDeclarationSyntax var = field.Declaration;

                    NodeWrapper node = new NodeWrapper();
                    node.NodeName = var.Variables.First().Identifier.ToString();
                    //node.NodeName = variable.Variables.First().Identifier.Value.ToString();
                    node.CallingClass = cname;


                    bool isPublic = false;
                    foreach (var mod in field.Modifiers)
                    {
                        if (mod.IsKind(SyntaxKind.PublicKeyword))
                        {
                            isPublic = true;
                        }
                    }

                    if (!isPublic)
                        continue;

                    node.BaseAssemblyType = var.Type.ToString();
                    node.TypeOfNode = NodeType.VariableNode;
                    yield return node;
                }
            }
        }
    }
}
