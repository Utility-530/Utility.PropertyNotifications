using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using NetPrints.Core;
using NetPrints.Enums;
using NetPrints.Interfaces;

namespace NetPrints.Reflection
{

    public static class ReflectionProvider 
    {
        //private readonly Lazy<ReflectionHelper> reflectionHelper;
        private static IList<IMethodSymbol> extensionMethods;
        //private readonly Lazy<IList<INamedTypeSymbol>> types;


        /// <summary>
        /// Creates a ReflectionProvider given paths to assemblies and source files.
        /// </summary>
        /// <param name="assemblyPaths">Paths to assemblies.</param>
        /// <param name="sourcePaths">Paths to source files.</param>
        public static CSharpCompilation Compile(IEnumerable<string> assemblyPaths, IEnumerable<string> sourcePaths, IEnumerable<string> sources)
        {
            string targetFramework = Helpers.GetTargetFrameworkFromEntryAssembly();


            var _assemblyReferences = assemblyPaths.Select(path =>
            {
                DocumentationProvider documentationProvider = DocumentationProvider.Default;

                return MetadataReference.CreateFromFile(path, documentation: documentationProvider);
            });


            var assemblyReferences = Basic.Reference.Assemblies.ReferenceAssemblies.Net80.Concat(_assemblyReferences);


            // Create syntax trees from sources
            sources = sources/*.Concat(sourcePaths.Select(path => File.ReadAllText(path)))*/.Distinct();
            var syntaxTrees = sources.Select(source => Helpers.ParseSyntaxTree(source));

            var compilation = CSharpCompilation.Create("C", syntaxTrees, assemblyReferences, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Try to compile, on success create a new compilation that references the created assembly instead of the sources.
            // The compilation will fail eg. if the sources have references to the not-yet-compiled assembly.
            (EmitResult compilationResults, Stream stream) = compilation.CompileInMemory();
            //IEnumerable<INamedTypeSymbol> allTypes;
            if (compilationResults.Success)
            {
                assemblyReferences = assemblyReferences.Concat(new[] { MetadataReference.CreateFromStream(stream) });
                compilation = CSharpCompilation.Create("C", references: assemblyReferences);
                return compilation;
                // Get all types from all assemblies
                //allTypes = compilation.References
                //    .Select(r => compilation.GetAssemblyOrModuleSymbol(r) as IAssemblySymbol)
                //    .Where(a => a != null)
                //    .SelectMany(a => getAllTypes(a.GlobalNamespace));
            }
            else
            {
                throw new Exception("DSF 3c 2a");
                // Get types from the compilation even if it failed
                //allTypes = getAllTypes(compilation.Assembly.GlobalNamespace);
            }

            //types = new(() => allTypes.ToList());


            //reflectionHelper = new(() => new ReflectionHelper(compilation));
            //extensionMethods = new(() =>
            //{

            //    return reflectionHelper.Value.GetValidTypes().SelectMany(t => t.GetMethods().Where(m => m.IsExtensionMethod)).ToArray();

            //});
        }


        //public IReflectionHelper ReflectionHelper => reflectionHelper.Value;


        // Helper method to recursively get all types
        public static IEnumerable<INamedTypeSymbol> GetAllTypes(this CSharpCompilation compilation, INamespaceSymbol namespaceSymbol)
        {
            foreach (var type in namespaceSymbol.GetTypeMembers())
            {
                yield return type;

                // Get nested types
                foreach (var nestedType in getNestedTypes(type))
                {
                    yield return nestedType;
                }
            }

            foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
            {
                foreach (var type in compilation.GetAllTypes(childNamespace))
                {
                    yield return type;
                }
            }

            static IEnumerable<INamedTypeSymbol> getNestedTypes( INamedTypeSymbol type)
            {
                foreach (var nestedType in type.GetTypeMembers())
                {
                    yield return nestedType;
                    foreach (var deeplyNested in getNestedTypes(nestedType))
                    {
                        yield return deeplyNested;
                    }
                }
            }

        }


        public static IEnumerable<IMethodSpecifier> GetMethods(this CSharpCompilation compilation, IReflectionProviderMethodQuery? query)
        {

            IEnumerable<IMethodSymbol> methodSymbols;

            // Check if type is set (no type => get all methods)
            if (query.Type is not null)
            {
                // Get all methods of the type
                ITypeSymbol type = (ITypeSymbol)compilation.GetSymbolFromSpecifier(query.Type);

                if (type == null)
                {
                    return new MethodSpecifier[0];
                }

                methodSymbols = type.GetMethods();

                var extensions = (extensionMethods ??= compilation.GetValidTypes().SelectMany(t => t.GetMethods().Where(m => m.IsExtensionMethod)).ToArray())
                    .Where(m => type.IsSubclassOf(m.Parameters[0].Type))
                    .ToList();

                // Add applicable extension methods
                methodSymbols = methodSymbols.Concat(extensions);
            }
            else
            {
                // Get all methods of all public types
                methodSymbols = compilation.GetValidTypes()
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
                    compilation.TypeSpecifierIsSubclassOf));
            }

            // Check argument type
            if (query.ArgumentType is not null)
            {
                var searchType = compilation.GetSymbolFromSpecifier(query.ArgumentType);

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
                var searchType = compilation.GetSymbolFromSpecifier(query.ReturnType);
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


        public static IEnumerable<IVariableSpecifier> GetVariables(this CSharpCompilation compilation, IReflectionProviderVariableQuery query)
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
                    ITypeSymbol type = (ITypeSymbol)compilation.GetSymbolFromSpecifier(query.Type);

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
                    propertySymbols = compilation.GetValidTypes()
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
                        compilation.TypeSpecifierIsSubclassOf));
                }

                // Check property type
                if (query.VariableType is not null)
                {
                    var searchType = (ITypeSymbol)compilation.GetSymbolFromSpecifier(query.VariableType);

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

        public static IEnumerable<ITypeSpecifier> GetTypes(this CSharpCompilation compilation)
        {
            return compilation
                .GetValidTypes()
                .Where(t => t.IsPublic() && !(t.IsAbstract && t.IsSealed))
                .OrderBy(t => t.ContainingNamespace?.Name)
                .ThenBy(t => t.Name)
                .Select(t => ReflectionConverter.TypeSpecifierFromSymbol(t));
        }


        public static CSharpCompilation From(IProject project)
        {

            var references = project.References;

            // Add referenced assemblies
            var assemblyPaths = references.OfType<AssemblyReference>().Select(assemblyRef => assemblyRef.AssemblyPath);

            // Add source files
            var sourcePaths = references.OfType<SourceDirectoryReference>().SelectMany(directoryRef => directoryRef.SourceFilePaths);

            // Add our own sources
            var sources = project.GenerateClassSources();

            //App.ReloadReflectionProvider(assemblyPaths, sourcePaths, sources);
            return Compile(assemblyPaths, sourcePaths, sources);

        }
        public static CSharpCompilation Empty()
        {
            //App.ReloadReflectionProvider(assemblyPaths, sourcePaths, sources);
            return Compile([], [], []);

        }
    }


    public static partial class ReflectionHelper
    {
        //private readonly CSharpCompilation compilation;


        private static readonly Dictionary<ISpecifier, ISymbol> cachedSpecifierSymbols = new Dictionary<ISpecifier, ISymbol>();


        //public ReflectionHelper(CSharpCompilation compilation)
        //{
        //    this.compilation = compilation;
        //}


        public static IEnumerable<IMethodSpecifier> GetPublicMethodOverloads(this CSharpCompilation compilation, IMethodSpecifier methodSpecifier)
        {
            ITypeSymbol type = (ITypeSymbol)compilation.GetSymbolFromSpecifier(methodSpecifier.DeclaringType);

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

        public static IEnumerable<IConstructorSpecifier> GetConstructors(this CSharpCompilation compilation, ITypeSpecifier typeSpecifier)
        {
            var symbol = compilation.GetSymbolFromSpecifier<INamedTypeSymbol>(typeSpecifier);

            if (symbol != null)
            {
                return symbol.Constructors.Select(c => ReflectionConverter.ConstructorSpecifierFromSymbol(c));
            }

            return new ConstructorSpecifier[0];
        }

        public static IEnumerable<string> GetEnumNames(this CSharpCompilation compilation, ITypeSpecifier typeSpecifier)
        {
            var symbol = (ITypeSymbol)compilation.GetSymbolFromSpecifier(typeSpecifier);

            if (symbol != null)
            {
                return symbol.GetAllMembers()
                    .Where(member => member.Kind == SymbolKind.Field)
                    .Select(member => member.Name);
            }

            return new string[0];
        }

        public static bool TypeSpecifierIsSubclassOf(this CSharpCompilation compilation, ITypeSpecifier a, ITypeSpecifier b)
        {
            ITypeSymbol typeA = (ITypeSymbol)compilation.GetSymbolFromSpecifier(a);
            ITypeSymbol typeB = (ITypeSymbol)compilation.GetSymbolFromSpecifier(b);

            return typeA != null && typeB != null && typeA.IsSubclassOf(typeB);
        }

        public static T GetSymbolFromSpecifier<T>(this CSharpCompilation compilation, ITypeSpecifier specifier)
        {
            return (T)compilation.GetSymbolFromSpecifier(specifier);
        }

        public static ISymbol GetSymbolFromSpecifier(this CSharpCompilation compilation, ISpecifier specifier)
        {
            if (cachedSpecifierSymbols.TryGetValue(specifier, out var symbol))
            {
                return symbol;
            }

            if (specifier is IMethodSpecifier methodSpecifier)
            {
                INamedTypeSymbol declaringType = compilation.GetSymbolFromSpecifier<INamedTypeSymbol>(methodSpecifier.DeclaringType);


                var methodSymbol = declaringType?.GetMethods().FirstOrDefault(
                        m => m.Name == methodSpecifier.Name
                        && m.Parameters.Select(p => ReflectionConverter.BaseTypeSpecifierFromSymbol(p.Type)).SequenceEqual(methodSpecifier.ArgumentTypes));
                cachedSpecifierSymbols.Add(methodSpecifier, methodSymbol);
                return methodSymbol;
            }

            if (specifier is not ITypeSpecifier typeSpecifier)
            {
                throw new Exception("R f4effds");
            }

            // Find array ranks and remove them from the lookup name.
            // Example: int[][,] -> arrayRanks: { 1, 2 }, lookupName: int
            Stack<int> arrayRanks = new Stack<int>();

            ITypeSymbol foundType = null;

            var x = namedTypeSymbols().GetEnumerator();
            while (foundType == null && x.MoveNext())
            {
                if (x.Current is INamedTypeSymbol _symbol)
                    foundType = convert(_symbol);
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

            cachedSpecifierSymbols.Add(typeSpecifier, foundType);

            return foundType;

            INamedTypeSymbol convert(INamedTypeSymbol t)
            {
                if (typeSpecifier.GenericArguments.Count > 0)
                {
                    var typeArguments = typeSpecifier.GenericArguments
                        .Select(baseType => baseType is TypeSpecifier typeSpec ?
                            (ITypeSymbol)compilation.GetSymbolFromSpecifier(typeSpec) :
                            t.TypeArguments[typeSpecifier.GenericArguments.IndexOf(baseType)])
                        .ToArray();
                    return t.Construct(typeArguments);
                }
                else
                {
                    return t;
                }
            }

            IEnumerable<INamedTypeSymbol> namedTypeSymbols()
            {
                string lookupName = typeSpecifier.Name;
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

                if (typeSpecifier.GenericArguments.Count > 0)
                    lookupName += $"`{typeSpecifier.GenericArguments.Count}";

                return compilation.GetValidTypes(lookupName);
            }

        }



        public static bool HasImplicitCast(this CSharpCompilation compilation, ITypeSpecifier fromType, ITypeSpecifier toType)
        {
            // Check if there exists a conversion that is implicit between the types.

            ITypeSymbol fromSymbol = (ITypeSymbol)compilation.GetSymbolFromSpecifier(fromType);
            ITypeSymbol toSymbol = (ITypeSymbol)compilation.GetSymbolFromSpecifier(toType);

            return fromSymbol != null && toSymbol != null
                && compilation.ClassifyConversion(fromSymbol, toSymbol).IsImplicit;
        }

        /// <summary>
        /// Gets all classes declared in the compilation's syntax trees.
        /// Useful for when they can not be compiled into assemblies because
        /// of errors and we still want their symbols.
        /// </summary>
        public static IEnumerable<INamedTypeSymbol> GetSyntaxTreeTypes(this CSharpCompilation compilation)
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

        public static IEnumerable<INamedTypeSymbol> GetValidTypes(this CSharpCompilation compilation)
        {
            return compilation.SourceModule.ReferencedAssemblySymbols.SelectMany(module => module.GlobalNamespace.GetNamespaceTypes())
                .Concat(compilation.GetSyntaxTreeTypes());
        }

        public static IEnumerable<INamedTypeSymbol> GetValidTypes(this CSharpCompilation compilation, string name)
        {
            return compilation.SourceModule.ReferencedAssemblySymbols.Select(module =>
            {
                try { return module.GetTypeByMetadataName(name); }
                catch { return null; }
            })
            .Where(t => t != null)
            .Concat(compilation.GetSyntaxTreeTypes().Where(t => t.GetFullName() == name)); // TODO: Correct full name
        }



        public static IEnumerable<IMethodSpecifier> GetOverridableMethods(this CSharpCompilation compilation, ITypeSpecifier typeSpecifier)
        {
            if (compilation.GetSymbolFromSpecifier(typeSpecifier) is ITypeSymbol typeSymbol)
            {
                // Get all overridable methods, ignore special ones (properties / events)

                return typeSymbol.GetOverridableMethods();
            }
            else
            {
                return new IMethodSpecifier[0];
            }
        }

        public static (EmitResult, Stream) CompileInMemory(this CSharpCompilation compilation)
        {
            Stream stream = new MemoryStream();
            var compilationResults = compilation.Emit(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return (compilationResults, stream);
        }
    }




    static class Helpers
    {
        //public static ISymbol ToSymbol(this ISpecifier specifier)
        //{
        //    return 
        //}

        public static IEnumerable<INamedTypeSymbol> GetTypeNestedTypes(this INamedTypeSymbol typeSymbol)
        {
            var typeMembers = typeSymbol.GetTypeMembers();
            return typeMembers.Concat(typeMembers.SelectMany(t => GetTypeNestedTypes(t)));
        }

        public static IEnumerable<INamedTypeSymbol> GetNamespaceTypes(this INamespaceSymbol namespaceSymbol)
        {
            IEnumerable<INamedTypeSymbol> types = namespaceSymbol.GetTypeMembers();
            types = types.Concat(types.SelectMany(t => GetTypeNestedTypes(t)));
            return types.Concat(namespaceSymbol.GetNamespaceMembers().SelectMany(ns => GetNamespaceTypes(ns)));
        }

        public static IEnumerable<IMethodSpecifier> GetOverridableMethods(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.GetMethods()
                  .Where(m => (m.IsVirtual || m.IsOverride || m.IsAbstract) && m.MethodKind == MethodKind.Ordinary)
                  .OrderBy(m => m.ContainingNamespace?.Name)
                  .ThenBy(m => m.ContainingType?.Name)
                  .ThenBy(m => m.Name)
                  .Select(m => ReflectionConverter.MethodSpecifierFromSymbol(m));
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



