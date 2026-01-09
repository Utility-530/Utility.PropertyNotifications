using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using NetPrints.Core;
using NetPrints.Enums;
using NetPrints.Interfaces;
using NetPrints.Reflection;

namespace NetPrintsEditor.Reflection
{



    public class ReflectionProvider : IReflectionProvider
    {
        private readonly Lazy<ReflectionHelpers> reflectionHelpers;
        private readonly List<IMethodSymbol> extensionMethods;

        /// <summary>
        /// Creates a ReflectionProvider given paths to assemblies and source files.
        /// </summary>
        /// <param name="assemblyPaths">Paths to assemblies.</param>
        /// <param name="sourcePaths">Paths to source files.</param>
        public ReflectionProvider(IEnumerable<string> assemblyPaths, IEnumerable<string> sourcePaths, IEnumerable<string> sources)
        {
            string targetFramework = Helpers.GetTargetFrameworkFromEntryAssembly();


            var _assemblyReferences = assemblyPaths.Select(path =>
            {
                DocumentationProvider documentationProvider = DocumentationProvider.Default;

                return MetadataReference.CreateFromFile(path, documentation: documentationProvider);
            });


            var assemblyReferences = Basic.Reference.Assemblies.ReferenceAssemblies.Net80.Concat(_assemblyReferences);


            // Create syntax trees from sources
            sources = sources.Concat(sourcePaths.Select(path => File.ReadAllText(path))).Distinct();
            var syntaxTrees = sources.Select(source => Helpers.ParseSyntaxTree(source));

            var compilation = CSharpCompilation.Create("C", syntaxTrees, assemblyReferences, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Try to compile, on success create a new compilation that references the created assembly instead of the sources.
            // The compilation will fail eg. if the sources have references to the not-yet-compiled assembly.
            (EmitResult compilationResults, Stream stream) = Helpers.CompileInMemory(compilation);

            if (compilationResults.Success)
            {
                assemblyReferences = assemblyReferences.Concat(new[] { MetadataReference.CreateFromStream(stream) });
                compilation = CSharpCompilation.Create("C", references: assemblyReferences);
            }
            var documentationUtil = new DocumentationUtil(compilation);
            reflectionHelpers = new(() => new ReflectionHelpers(compilation, documentationUtil));

            extensionMethods = new List<IMethodSymbol>(reflectionHelpers.Value.GetValidTypes().SelectMany(t => t.GetMethods().Where(m => m.IsExtensionMethod)));

            documentationUtil = new DocumentationUtil(compilation);
        }



        public IEnumerable<IMethodSpecifier> GetMethods(IReflectionProviderMethodQuery? query)
        {

            IEnumerable<IMethodSymbol> methodSymbols;

            // Check if type is set (no type => get all methods)
            if (query.Type is not null)
            {
                // Get all methods of the type
                ITypeSymbol type = reflectionHelpers.Value.GetTypeFromSpecifier(query.Type);

                if (type == null)
                {
                    return new MethodSpecifier[0];
                }

                methodSymbols = type.GetMethods();

                var extensions = extensionMethods.Where(m => type.IsSubclassOf(m.Parameters[0].Type)).ToList();

                // Add applicable extension methods
                methodSymbols = methodSymbols.Concat(extensions);
            }
            else
            {
                // Get all methods of all public types
                methodSymbols = reflectionHelpers.Value.GetValidTypes()
                                .Where(t => t.IsPublic())
                                .SelectMany(t => t.GetMethods());
            }

            if (query.NameLike != null)
            {
                var regex = new Regex(query.NameLike);
                methodSymbols = methodSymbols.Where(m => regex.IsMatch(m.Name));
            }

            // Check static
            if (query.Static.HasValue)
            {
                methodSymbols = methodSymbols.Where(m => m.IsStatic == query.Static.Value);
            }

            // Check has generic arguments
            if (query.HasGenericArguments.HasValue)
            {
                methodSymbols = methodSymbols.Where(m => query.HasGenericArguments.Value ?
                    m.TypeParameters.Any() :
                    !m.TypeParameters.Any());
            }

            // Check visibility
            if (query.VisibleFrom is not null)
            {
                methodSymbols = methodSymbols.Where(m => NetPrintsUtil.IsVisible(query.VisibleFrom,
                    ReflectionConverter.TypeSpecifierFromSymbol(m.ContainingType),
                    ReflectionConverter.VisibilityFromAccessibility(m.DeclaredAccessibility),
                    reflectionHelpers.Value.TypeSpecifierIsSubclassOf));
            }

            // Check argument type
            if (query.ArgumentType is not null)
            {
                var searchType = reflectionHelpers.Value.GetTypeFromSpecifier(query.ArgumentType);

                if (searchType != null)
                    methodSymbols = methodSymbols
                    .Where(m => m.Parameters
                        .Select(p => p.Type)
                        .Any(t =>
                        {
                            var x = m.Name;

                            var b = SymbolEqualityComparer.Default.Equals(t, searchType);
                            //|| searchType.IsSubclassOf(t)
                            //|| t.TypeKind == TypeKind.TypeParameter;
                            return b;
                        }));
            }

            // Check return type
            if (query.ReturnType is not null)
            {
                var searchType = reflectionHelpers.Value.GetTypeFromSpecifier(query.ReturnType);
                if (searchType != null)
                    methodSymbols = methodSymbols
                        .Where(m =>
                        {
                            var b = SymbolEqualityComparer.Default.Equals(m.ReturnType, searchType);
                            //|| m.ReturnType.IsSubclassOf(searchType)
                            //|| m.ReturnType.TypeKind == TypeKind.TypeParameter;
                            return b;
                        });
            }

            var methodSpecifiers = methodSymbols
                .OrderBy(m => m.ContainingNamespace?.Name)
                .ThenBy(m => m.ContainingType?.Name)
                .ThenBy(m => m.Name)
                .Select(m => ReflectionConverter.MethodSpecifierFromSymbol(m));

            // HACK: Add default operators which we can not find by
            //       reflection at this time.
            if (query.HasGenericArguments != true && query.Static != false)
            {
                var defaultOperatorSpecifiers = DefaultOperatorSpecifiers.All;

                if (query.Type is not null)
                {
                    defaultOperatorSpecifiers = defaultOperatorSpecifiers.Where(t => t.DeclaringType == query.Type);
                }

                if (query.ReturnType is not null)
                {
                    defaultOperatorSpecifiers = defaultOperatorSpecifiers.Where(t => t.ReturnTypes.Any(rt => rt == query.ReturnType));
                }

                if (query.ArgumentType is not null)
                {
                    defaultOperatorSpecifiers = defaultOperatorSpecifiers.Where(t => t.ArgumentTypes.Any(at => at == query.ArgumentType));
                }

                methodSpecifiers = defaultOperatorSpecifiers.Concat(methodSpecifiers);
            }

            return methodSpecifiers;
        }


        public IEnumerable<IVariableSpecifier> GetVariables(IReflectionProviderVariableQuery query)
        {
            // Note: Currently we handle fields and properties in this function
            //       so there is some extra logic for handling the fields.
            //       This should be unified or seperated later.

            return symbols()
                .OrderBy(p => p.ContainingNamespace?.Name)
                .ThenBy(p => p.ContainingType?.Name)
                .ThenBy(p => p.Name)
                .Select(p => p is IPropertySymbol propertySymbol ? ReflectionConverter.VariableSpecifierFromSymbol(propertySymbol) : ReflectionConverter.VariableSpecifierFromField((IFieldSymbol)p));



            IEnumerable<ISymbol> symbols()
            {
                IEnumerable<ISymbol> propertySymbols;
                // Check if type is set (no type => get all methods)
                if (query.Type is not null)
                {
                    // Get all properties of the type
                    ITypeSymbol type = reflectionHelpers.Value.GetTypeFromSpecifier(query.Type);

                    if (type == null)
                    {
                        return Array.Empty<ISymbol>();
                    }

                    propertySymbols = type.GetAllMembers()
                        .Where(m => m.Kind == SymbolKind.Property || m.Kind == SymbolKind.Field);
                }
                else
                {
                    // Get all properties of all public types
                    propertySymbols = reflectionHelpers.Value.GetValidTypes()
                        .SelectMany(t => t.GetAllMembers()
                            .Where(m => m.Kind == SymbolKind.Property || m.Kind == SymbolKind.Field));
                }

                // Check static
                if (query.Static.HasValue)
                {
                    propertySymbols = propertySymbols.Where(m => m.IsStatic == query.Static.Value);
                }

                // Check visibility
                if (query.VisibleFrom is not null)
                {
                    propertySymbols = propertySymbols.Where(p => NetPrintsUtil.IsVisible(query.VisibleFrom,
                        ReflectionConverter.TypeSpecifierFromSymbol(p.ContainingType),
                        ReflectionConverter.VisibilityFromAccessibility(p.DeclaredAccessibility),
                        reflectionHelpers.Value.TypeSpecifierIsSubclassOf));
                }

                // Check property type
                if (query.VariableType is not null)
                {
                    var searchType = reflectionHelpers.Value.GetTypeFromSpecifier(query.VariableType);

                    propertySymbols = propertySymbols.Where(p => query.VariableTypeDerivesFrom ?
                        TypeSymbolFromFieldOrProperty(p).IsSubclassOf(searchType) :
                        searchType.IsSubclassOf(TypeSymbolFromFieldOrProperty(p)));
                }

                return propertySymbols;

                static ITypeSymbol TypeSymbolFromFieldOrProperty(ISymbol symbol)
                {
                    if (symbol is IFieldSymbol fieldSymbol)
                    {
                        return fieldSymbol.Type;
                    }
                    else if (symbol is IPropertySymbol propertySymbol)
                    {
                        return propertySymbol.Type;
                    }

                    throw new ArgumentException("symbol not a property nor field symbol.");
                }
            }
        }

        public IEnumerable<ITypeSpecifier> GetTypes()
        {
            return reflectionHelpers.Value.GetValidTypes().Where(
                    t => t.IsPublic() && !(t.IsAbstract && t.IsSealed))
                .OrderBy(t => t.ContainingNamespace?.Name)
                .ThenBy(t => t.Name)
                .Select(t => ReflectionConverter.TypeSpecifierFromSymbol(t));
        }


        public static ReflectionProvider From(IProject project) => Helpers.From(project);
        public static ReflectionProvider Empty() => Helpers.Empty();
    }


    class ReflectionHelpers
    {
        private readonly CSharpCompilation compilation;


        private readonly Dictionary<ITypeSpecifier, ITypeSymbol> cachedTypeSpecifierSymbols = new Dictionary<ITypeSpecifier, ITypeSymbol>();


        public ReflectionHelpers(CSharpCompilation compilation, DocumentationUtil documentationUtil)
        {
            this.compilation = compilation;
        }


        public IEnumerable<IMethodSpecifier> GetPublicMethodOverloads(IMethodSpecifier methodSpecifier)
        {
            ITypeSymbol type = GetTypeFromSpecifier(methodSpecifier.DeclaringType);

            // TODO: Get a better way to determine is a method specifier is an operator.
            bool isOperator = methodSpecifier.Name.StartsWith("op_");

            if (type != null)
            {
                return type.GetMethods()
                        .Where(m =>
                            m.Name == methodSpecifier.Name
                            && m.IsPublic()
                            && m.IsStatic == methodSpecifier.Modifiers.HasFlag(MethodModifiers.Static)
                            && (isOperator ?
                                (m.MethodKind == MethodKind.BuiltinOperator || m.MethodKind == MethodKind.UserDefinedOperator) :
                                m.MethodKind == MethodKind.Ordinary))
                        .OrderBy(m => m.ContainingNamespace?.Name)
                        .ThenBy(m => m.ContainingType?.Name)
                        .ThenBy(m => m.Name)
                        .Select(m => ReflectionConverter.MethodSpecifierFromSymbol(m));
            }
            else
            {
                return new MethodSpecifier[0];
            }
        }

        public IEnumerable<IConstructorSpecifier> GetConstructors(ITypeSpecifier typeSpecifier)
        {
            var symbol = GetTypeFromSpecifier<INamedTypeSymbol>(typeSpecifier);

            if (symbol != null)
            {
                return symbol.Constructors.Select(c => ReflectionConverter.ConstructorSpecifierFromSymbol(c));
            }

            return new ConstructorSpecifier[0];
        }

        public IEnumerable<string> GetEnumNames(ITypeSpecifier typeSpecifier)
        {
            var symbol = GetTypeFromSpecifier(typeSpecifier);

            if (symbol != null)
            {
                return symbol.GetAllMembers()
                    .Where(member => member.Kind == SymbolKind.Field)
                    .Select(member => member.Name);
            }

            return new string[0];
        }

        public bool TypeSpecifierIsSubclassOf(ITypeSpecifier a, ITypeSpecifier b)
        {
            ITypeSymbol typeA = GetTypeFromSpecifier(a);
            ITypeSymbol typeB = GetTypeFromSpecifier(b);

            return typeA != null && typeB != null && typeA.IsSubclassOf(typeB);
        }

        public T GetTypeFromSpecifier<T>(ITypeSpecifier specifier)
        {
            return (T)GetTypeFromSpecifier(specifier);
        }

        public IMethodSymbol GetMethodInfoFromSpecifier(IMethodSpecifier specifier)
        {
            INamedTypeSymbol declaringType = GetTypeFromSpecifier<INamedTypeSymbol>(specifier.DeclaringType);
            return declaringType?.GetMethods().FirstOrDefault(
                    m => m.Name == specifier.Name
                    && m.Parameters.Select(p => ReflectionConverter.BaseTypeSpecifierFromSymbol(p.Type)).SequenceEqual(specifier.ArgumentTypes));
        }

        public ITypeSymbol GetTypeFromSpecifier(ITypeSpecifier specifier)
        {
            if (cachedTypeSpecifierSymbols.TryGetValue(specifier, out var symbol))
            {
                return symbol;
            }

            string lookupName = specifier.Name;

            // Find array ranks and remove them from the lookup name.
            // Example: int[][,] -> arrayRanks: { 1, 2 }, lookupName: int
            Stack<int> arrayRanks = new Stack<int>();
            while (lookupName.EndsWith("]"))
            {
                lookupName = lookupName.Remove(lookupName.Length - 1);
                int arrayRank = 1;
                while (lookupName.EndsWith(","))
                {
                    arrayRank++;
                    lookupName = lookupName.Remove(lookupName.Length - 1);
                }
                arrayRanks.Push(arrayRank);

                if (lookupName.Last() != '[')
                {
                    throw new Exception("Expected [ in lookupName");
                }

                lookupName = lookupName.Remove(lookupName.Length - 1);
            }

            if (specifier.GenericArguments.Count > 0)
                lookupName += $"`{specifier.GenericArguments.Count}";

            IEnumerable<INamedTypeSymbol> types = GetValidTypes(lookupName);

            ITypeSymbol foundType = null;

            foreach (INamedTypeSymbol t in types)
            {
                if (t != null)
                {
                    if (specifier.GenericArguments.Count > 0)
                    {
                        var typeArguments = specifier.GenericArguments
                            .Select(baseType => baseType is TypeSpecifier typeSpec ?
                                GetTypeFromSpecifier(typeSpec) :
                                t.TypeArguments[specifier.GenericArguments.IndexOf(baseType)])
                            .ToArray();
                        foundType = t.Construct(typeArguments);
                    }
                    else
                    {
                        foundType = t;
                    }

                    break;
                }
            }

            if (foundType != null)
            {
                // Make array
                //while (arrayRanks.TryPop(out int arrayRank))
                while (arrayRanks.Count > 0)
                {
                    int arrayRank = arrayRanks.Pop();
                    foundType = compilation.CreateArrayTypeSymbol(foundType, arrayRank);
                }
            }

            cachedTypeSpecifierSymbols.Add(specifier, foundType);

            return foundType;
        }



        public bool HasImplicitCast(ITypeSpecifier fromType, ITypeSpecifier toType)
        {
            // Check if there exists a conversion that is implicit between the types.

            ITypeSymbol fromSymbol = GetTypeFromSpecifier(fromType);
            ITypeSymbol toSymbol = GetTypeFromSpecifier(toType);

            return fromSymbol != null && toSymbol != null
                && compilation.ClassifyConversion(fromSymbol, toSymbol).IsImplicit;
        }

        /// <summary>
        /// Gets all classes declared in the compilation's syntax trees.
        /// Useful for when they can not be compiled into assemblies because
        /// of errors and we still want their symbols.
        /// </summary>
        public IEnumerable<INamedTypeSymbol> GetSyntaxTreeTypes()
        {
            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(syntaxTree, true);
                var classSyntaxes = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
                var classes = classSyntaxes.Select(syntax => model.GetDeclaredSymbol(syntax));
                foreach (var cls in classes)
                {
                    yield return cls;
                }
            }
        }

        public IEnumerable<INamedTypeSymbol> GetTypeNestedTypes(INamedTypeSymbol typeSymbol)
        {
            var typeMembers = typeSymbol.GetTypeMembers();
            return typeMembers.Concat(typeMembers.SelectMany(t => GetTypeNestedTypes(t)));
        }

        public IEnumerable<INamedTypeSymbol> GetNamespaceTypes(INamespaceSymbol namespaceSymbol)
        {
            IEnumerable<INamedTypeSymbol> types = namespaceSymbol.GetTypeMembers();
            types = types.Concat(types.SelectMany(t => GetTypeNestedTypes(t)));
            return types.Concat(namespaceSymbol.GetNamespaceMembers().SelectMany(ns => GetNamespaceTypes(ns)));
        }

        public IEnumerable<INamedTypeSymbol> GetValidTypes()
        {
            return compilation.SourceModule.ReferencedAssemblySymbols.SelectMany(module => GetNamespaceTypes(module.GlobalNamespace))
                .Concat(GetSyntaxTreeTypes());
        }

        public IEnumerable<INamedTypeSymbol> GetValidTypes(string name)
        {
            return compilation.SourceModule.ReferencedAssemblySymbols.Select(module =>
            {
                try { return module.GetTypeByMetadataName(name); }
                catch { return null; }
            })
            .Where(t => t != null)
            .Concat(GetSyntaxTreeTypes().Where(t => t.GetFullName() == name)); // TODO: Correct full name
        }



        public IEnumerable<IMethodSpecifier> GetOverridableMethodsForType(ITypeSpecifier typeSpecifier)
        {
            ITypeSymbol type = GetTypeFromSpecifier(typeSpecifier);

            if (type != null)
            {
                // Get all overridable methods, ignore special ones (properties / events)

                return type.GetMethods()
                    .Where(m =>
                        (m.IsVirtual || m.IsOverride || m.IsAbstract)
                        && m.MethodKind == MethodKind.Ordinary)
                    .OrderBy(m => m.ContainingNamespace?.Name)
                    .ThenBy(m => m.ContainingType?.Name)
                    .ThenBy(m => m.Name)
                    .Select(m => ReflectionConverter.MethodSpecifierFromSymbol(m));
            }
            else
            {
                return new MethodSpecifier[0];
            }
        }
    }


    class ZZ
    {
        private DocumentationUtil documentationUtil;
        private ReflectionHelpers z;

        public ZZ(DocumentationUtil documentationUtil, ReflectionHelpers z)
        {
            this.documentationUtil = documentationUtil;
            this.z = z;
        }

        public string GetMethodDocumentation(IMethodSpecifier methodSpecifier)
        {
            IMethodSymbol methodInfo = z.GetMethodInfoFromSpecifier(methodSpecifier);

            if (methodInfo == null)
            {
                return null;
            }

            return documentationUtil.GetMethodSummary(methodInfo);
        }

        public string GetMethodParameterDocumentation(IMethodSpecifier methodSpecifier, int parameterIndex)
        {
            IMethodSymbol methodInfo = z.GetMethodInfoFromSpecifier(methodSpecifier);

            if (methodInfo == null)
            {
                return null;
            }

            return documentationUtil.GetMethodParameterInfo(methodInfo.Parameters[parameterIndex]);
        }

        public string GetMethodReturnDocumentation(IMethodSpecifier methodSpecifier, int returnIndex)
        {
            IMethodSymbol methodInfo = z.GetMethodInfoFromSpecifier(methodSpecifier);

            if (methodInfo == null)
            {
                return null;
            }

            return documentationUtil.GetMethodReturnInfo(methodInfo);
        }

    }



    static class Helpers
    {
        public static ReflectionProvider From(this IProject project)
        {

            var references = project.References;

            // Add referenced assemblies
            var assemblyPaths = references.OfType<AssemblyReference>().Select(assemblyRef => assemblyRef.AssemblyPath);

            // Add source files
            var sourcePaths = references.OfType<SourceDirectoryReference>().SelectMany(directoryRef => directoryRef.SourceFilePaths);

            // Add our own sources
            var sources = project.GenerateClassSources();

            //App.ReloadReflectionProvider(assemblyPaths, sourcePaths, sources);
            return new ReflectionProvider(assemblyPaths, sourcePaths, sources);

        }
        public static ReflectionProvider Empty()
        {

            //App.ReloadReflectionProvider(assemblyPaths, sourcePaths, sources);
            return new ReflectionProvider([], [], []);

        }


        public static (EmitResult, Stream) CompileInMemory(this CSharpCompilation compilation)
        {
            Stream stream = new MemoryStream();
            var compilationResults = compilation.Emit(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return (compilationResults, stream);
        }


        public static SyntaxTree ParseSyntaxTree(string source)
        {
            // LanguageVersion.Preview is not defined in the Roslyn version used
            // at the time of writing. However MaxValue - 1 (as defined in the newer versions
            // see https://github.com/dotnet/roslyn/blob/472276accaf70a8356747dc7111cfb6231871077/src/Compilers/CSharp/Portable/LanguageVersion.cs#L135
            // seems to work.
            const LanguageVersion previewVersion = (LanguageVersion)(int.MaxValue - 1);

            // Return a syntax tree of our source code
            return CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(languageVersion: previewVersion));
        }




        public static string GetTargetFrameworkFromEntryAssembly()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                throw new Exception("Entry assembly is null.");
            }

            var targetFrameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();

            if (targetFrameworkAttribute != null)
            {
                // Returns something like ".NETCoreApp,Version=v6.0" or ".NETFramework,Version=v4.5"
                return ParseTargetFrameworkMoniker(targetFrameworkAttribute.FrameworkName);
            }

            throw new Exception("TargetFrameworkAttribute not found on entry assembly.");
        }

        private static string ParseTargetFrameworkMoniker(string frameworkName)
        {
            // Example inputs:
            // ".NETCoreApp,Version=v6.0" -> "net6.0"
            // ".NETCoreApp,Version=v8.0" -> "net8.0"
            // ".NETStandard,Version=v2.0" -> "netstandard2.0"
            // ".NETFramework,Version=v4.7.2" -> "net472"

            var match = Regex.Match(frameworkName, @"Version=v?(\d+(?:\.\d+)*)");
            if (!match.Success)
            {
                return "net6.0";
            }

            string version = match.Groups[1].Value;

            if (frameworkName.Contains("NETCoreApp") || frameworkName.Contains(".NET,"))
            {
                // .NET Core/5+ uses "net6.0" format
                return $"net{version}";
            }
            else if (frameworkName.Contains("NETStandard"))
            {
                // .NET Standard uses "netstandard2.0" format
                return $"netstandard{version}";
            }
            else if (frameworkName.Contains("NETFramework"))
            {
                // .NET Framework uses "net472" format (no dots)
                return $"net{version.Replace(".", "")}";
            }

            return $"net{version}";
        }
    }
}



