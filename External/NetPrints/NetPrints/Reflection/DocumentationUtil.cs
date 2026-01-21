using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NetPrints.Core;
using NetPrints.Interfaces;

namespace NetPrints.Reflection
{
    public static partial class ReflectionHelper
    {
        private static readonly Dictionary<string, XmlDocument> cachedDocuments =
            new Dictionary<string, XmlDocument>();

        private static readonly Dictionary<string, string> cachedMethodSummaries =
            new Dictionary<string, string>();

        private static readonly Dictionary<Tuple<string, string>, string> cachedMethodParameterInfos =
            new Dictionary<Tuple<string, string>, string>();

        private static readonly Dictionary<string, string> cachedMethodReturnInfo =
            new Dictionary<string, string>();

        //private readonly Microsoft.CodeAnalysis.Compilation compilation;

        public static string GetMethodDocumentation(this CSharpCompilation compilation, IMethodSpecifier methodSpecifier)
        {
            IMethodSymbol methodInfo = (IMethodSymbol)compilation.GetSymbolFromSpecifier(methodSpecifier);

            if (methodInfo == null)
            {
                return null;
            }

            return compilation.GetMethodSummary(methodInfo);
        }

        public static string GetMethodParameterDocumentation(this CSharpCompilation compilation, IMethodSpecifier methodSpecifier, int parameterIndex)
        {
            IMethodSymbol methodInfo = (IMethodSymbol)compilation.GetSymbolFromSpecifier(methodSpecifier);

            if (methodInfo == null)
            {
                return null;
            }

            return compilation.GetMethodParameterInfo(methodInfo.Parameters[parameterIndex]);
        }

        public static string GetMethodReturnDocumentation(this CSharpCompilation compilation, IMethodSpecifier methodSpecifier, int returnIndex)
        {
            IMethodSymbol methodInfo = (IMethodSymbol)compilation.GetSymbolFromSpecifier(methodSpecifier);

            if (methodInfo == null)
            {
                return null;
            }

            return compilation.GetMethodReturnInfo(methodInfo);
        }


        private static string GetAssemblyPath(this CSharpCompilation compilation, IAssemblySymbol assembly)
        {
            MetadataReference reference = compilation.GetMetadataReference(assembly);
            if (reference is PortableExecutableReference peReference)
            {
                return peReference.FilePath;
            }
            return null;
        }

        private static string GetMethodInfoKey(this IMethodSymbol methodInfo)
        {
            string key = $"M:{methodInfo.ContainingType.GetFullName()}.{methodInfo.Name}";

            if (methodInfo.Parameters.Length > 0)
            {
                key += "(";
                key += string.Join(",", methodInfo.Parameters.Select(p => p.Type.GetFullName()));
                key += ")";
            }

            return key;
        }

        private static string GetAssemblyDocumentationPath(this CSharpCompilation compilation, IAssemblySymbol assembly)
        {
            string assemblyPath = compilation.GetAssemblyPath(assembly);

            if (assemblyPath != null)
            {
                // Try to find the documentation in the framework doc path
                string docPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                        "Reference Assemblies/Microsoft/Framework/.NETFramework/v4.X",
                        $"{Path.GetFileNameWithoutExtension(assemblyPath)}.xml");

                // Try to find the documentation in the assembly's path
                if (!File.Exists(docPath))
                {
                    docPath = Path.ChangeExtension(assemblyPath, ".xml");
                }

                // Try to find the documentation in the current path
                if (!File.Exists(docPath))
                {
                    docPath = $"{Path.GetFileNameWithoutExtension(assemblyPath)}.xml";
                }

                return docPath;
            }

            return null;
        }

        private static XmlDocument GetAssemblyDocumentationDocument(this CSharpCompilation compilation, IAssemblySymbol assembly)
        {
            string assemblyPath = compilation.GetAssemblyPath(assembly);
            if (assemblyPath != null)
            {
                string key = Path.GetFileNameWithoutExtension(assemblyPath);

                if (cachedDocuments.ContainsKey(key))
                {
                    return cachedDocuments[key];
                }

                try
                {
                    string docPath = compilation.GetAssemblyDocumentationPath(assembly);
                    if (docPath != null)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(docPath));

                        cachedDocuments.Add(key, doc);

                        return doc;
                    }
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Gets the summary text for a method.
        /// </summary>
        /// <param name="methodInfo">Method to get summary text for.</param>
        /// <returns>Summary text for a method.</returns>
        public static string GetMethodSummary(this CSharpCompilation compilation, IMethodSymbol methodInfo)
        {
            string methodKey = GetMethodInfoKey(methodInfo);

            if (cachedMethodSummaries.ContainsKey(methodKey))
            {
                return cachedMethodSummaries[methodKey];
            }

            string documentation = null;

            XmlDocument doc = compilation.GetAssemblyDocumentationDocument(methodInfo.ContainingAssembly);
            if (doc != null)
            {
                XmlNodeList nodes = doc.SelectNodes($"doc/members/member[@name='{methodKey}']/summary");

                if (nodes.Count > 0)
                {
                    documentation = nodes.Item(0).InnerText;
                }

                cachedMethodSummaries.Add(methodKey, documentation);
            }

            return documentation;
        }

        /// <summary>
        /// Gets the summary text of a method's parameter.
        /// </summary>
        /// <param name="parameterSymbol">Parameter to get the summary text for.</param>
        /// <returns>Summary text of a method's parameter.</returns>
        public static string GetMethodParameterInfo(this CSharpCompilation compilation, IParameterSymbol parameterSymbol)
        {
            IMethodSymbol methodSymbol = (IMethodSymbol)parameterSymbol.ContainingSymbol;
            string methodKey = GetMethodInfoKey(methodSymbol);
            Tuple<string, string> cacheKey = new Tuple<string, string>(methodKey, parameterSymbol.Name);
            if (cachedMethodParameterInfos.ContainsKey(cacheKey))
            {
                return cachedMethodParameterInfos[cacheKey];
            }

            string documentation = null;

            XmlDocument doc = compilation.GetAssemblyDocumentationDocument(methodSymbol.ContainingAssembly);
            if (doc != null)
            {
                string searchName = $"M:{methodSymbol.ContainingType.GetFullName()}.{methodSymbol.Name}";
                if (methodSymbol.Parameters.Length > 0)
                {
                    searchName += "(";
                    searchName += string.Join(",", methodSymbol.Parameters.Select(p => p.Type.GetFullName()));
                    searchName += ")";
                }

                XmlNodeList nodes = doc.SelectNodes($"doc/members/member[@name='{searchName}']/param[@name='{parameterSymbol.Name}']");

                if (nodes.Count > 0)
                {
                    documentation = nodes.Item(0).InnerText;
                }

                cachedMethodParameterInfos.Add(cacheKey, documentation);
            }

            return documentation;
        }

        /// <summary>
        /// Gets a method's return information.
        /// </summary>
        /// <param name="methodSymbol">Method to get return information for.</param>
        /// <returns>Return information for the method.</returns>
        public static string GetMethodReturnInfo(this CSharpCompilation compilation, IMethodSymbol methodSymbol)
        {
            string methodKey = GetMethodInfoKey(methodSymbol);

            if (cachedMethodReturnInfo.ContainsKey(methodKey))
            {
                return cachedMethodReturnInfo[methodKey];
            }

            string documentation = null;

            XmlDocument doc = compilation.GetAssemblyDocumentationDocument(methodSymbol.ContainingType.ContainingAssembly);

            if (doc != null)
            {
                string searchName = $"M:{methodSymbol.ContainingType.GetFullName()}.{methodSymbol.Name}";
                if (methodSymbol.Parameters.Length > 0)
                {
                    searchName += "(";
                    searchName += string.Join(",", methodSymbol.Parameters.Select(p => p.Type.GetFullName()));
                    searchName += ")";
                }

                XmlNodeList nodes = doc.SelectNodes($"doc/members/member[@name='{searchName}']/returns");

                if (nodes.Count > 0)
                {
                    documentation = nodes.Item(0).InnerText;
                }

                cachedMethodReturnInfo.Add(methodKey, documentation);
            }

            return documentation;
        }
    }
}
