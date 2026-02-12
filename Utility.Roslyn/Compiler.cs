using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Basic.Reference.Assemblies;

namespace Utility.Roslyn
{
    public static class Compiler
    {
        public static CSharpCompilation Default() => Compile(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), ReferenceAssemblyKind.Net80);


        static CSharpCompilationOptions DefaultOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        public class CompilationException : Exception
        {
            public CompilationException(System.Collections.Immutable.ImmutableArray<Diagnostic> diagnostics)
            {
                Diagnostics = diagnostics;
            }

            public System.Collections.Immutable.ImmutableArray<Diagnostic> Diagnostics { get; }
        }

        public static IEnumerable<INamedTypeSymbol> AllTypes(
            string pathToDll)
        {
            var compilation = CSharpCompilation.Create(
                "C",
                references: new[] { MetadataReference.CreateFromFile(pathToDll) }
            );

            var assembly = compilation
                .References
                .Select(compilation.GetAssemblyOrModuleSymbol)
                .OfType<IAssemblySymbol>()
                .First();

            return assembly.GlobalNamespace.AllTypes();
        }

        public static CSharpCompilation Compile(IEnumerable<string> assemblyPaths, IEnumerable<string> sourcePaths, IEnumerable<string> sources)
        {
            var _assemblyReferences = assemblyPaths.Select(path =>
            {
                DocumentationProvider documentationProvider = DocumentationProvider.Default;

                return MetadataReference.CreateFromFile(path, documentation: documentationProvider);
            });

            var assemblyReferences = Basic.Reference.Assemblies.ReferenceAssemblies.Net80.Concat(_assemblyReferences);

            // Create syntax trees from sources
            sources = sources/*.Concat(sourcePaths.Select(path => File.ReadAllText(path)))*/.Distinct();
            var syntaxTrees = sources.Select(source => ParseSyntaxTree(source));

            var compilation = CSharpCompilation.Create("C", syntaxTrees, assemblyReferences, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Try to compile, on success create a new compilation that references the created assembly instead of the sources.
            // The compilation will fail eg. if the sources have references to the not-yet-compiled assembly.
            (EmitResult compilationResults, Stream stream) = CompileInMemory(compilation);
            //IEnumerable<INamedTypeSymbol> allTypes;
            if (compilationResults.Success)
            {
                assemblyReferences = assemblyReferences.Concat(new[] { MetadataReference.CreateFromStream(stream) });
                compilation = CSharpCompilation.Create("C", references: assemblyReferences);
                return compilation;
            }
            else
            {
                throw new Exception("DSF 3c 2a");
            }
        }

        public static CSharpCompilation Compile(
            IEnumerable<string> assemblyPaths = null,
            IEnumerable<string> sourcePaths = null,
            IEnumerable<string> sources = null,
            ReferenceAssemblyKind? kind = null,
            CSharpCompilationOptions? options = null)
        {
            List<PortableExecutableReference> references = new();
            if (assemblyPaths != null || sourcePaths != null || sources != null)
            {
                references = assemblyPaths?
                    .Select(a => MetadataReference.CreateFromFile(a))
                    .ToList();

                var syntaxTrees = combine(sources, sourcePaths?.Select(path => File.ReadAllText(path)))
                    .Distinct()
                    .Select(ParseSyntaxTree)
                    .ToList();

                var compilation = CSharpCompilation.Create(
                    "C",
                    syntaxTrees,
                    references,
                    options ?? DefaultOptions
                );

                if (kind.HasValue)
                    compilation = compilation.WithReferenceAssemblies(kind.Value);

                var (emitResult, stream) = CompileInMemory(compilation);

                if (!emitResult.Success)
                    throw new CompilationException(emitResult.Diagnostics);

                stream.Position = 0;
                references.Add(MetadataReference.CreateFromStream(stream));
            }

            var comp2 = CSharpCompilation.Create(
                "C",
                references: references,
                options: options ?? DefaultOptions
            );
            return kind.HasValue ? comp2.WithReferenceAssemblies(kind.Value) : comp2;
        }

        static IEnumerable<T> combine<T>(IEnumerable<T>? a = null, IEnumerable<T>? b = null)
        {
            if (a == null && b == null)
                return Array.Empty<T>();
            else if (a != null && b != null)
                return a.Concat(b);
            else if (a == null)
                return b;
            else if (b == null)
                return a;

            throw new Exception("sd ");
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

        public static (EmitResult, Stream) CompileInMemory(CSharpCompilation compilation)
        {
            Stream stream = new MemoryStream();
            var compilationResults = compilation.Emit(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return (compilationResults, stream);
        }
        public static IEnumerable<INamedTypeSymbol> GetValidTypes(this CSharpCompilation compilation)
        {
            return compilation.SourceModule.ReferencedAssemblySymbols.SelectMany(module => module.GlobalNamespace.GetNamespaceTypes())
                .Concat(compilation.GetSyntaxTreeTypes());
        }
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

        static IEnumerable<INamedTypeSymbol> AllTypes(this INamespaceSymbol ns)
        {
            foreach (var type in ns.GetTypeMembers())
                yield return type;

            foreach (var childNs in ns.GetNamespaceMembers())
                foreach (var type in AllTypes(childNs))
                    yield return type;
        }

        public static bool IsPublic(this ISymbol symbol)
        {
            return symbol.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public;
        }

        public static IEnumerable<IMethodSymbol> GetMethods(this ITypeSymbol symbol)
        {
            return symbol.GetAllMembers()
                    .Where(member => member.Kind == SymbolKind.Method)
                    .Cast<IMethodSymbol>()
                    .Where(method => method.MethodKind == MethodKind.Ordinary || method.MethodKind == MethodKind.BuiltinOperator || method.MethodKind == MethodKind.UserDefinedOperator);
        }


        private static readonly Dictionary<ITypeSymbol, List<ISymbol>> allMembersCache = new Dictionary<ITypeSymbol, List<ISymbol>>();


        public static IReadOnlyCollection<ISymbol> GetAllMembers(this ITypeSymbol symbol)
        {
            if (allMembersCache.TryGetValue(symbol, out var allMembers))
            {
                return allMembers;
            }

            var members = new List<ISymbol>();
            var overridenMethods = new HashSet<IMethodSymbol>();

            var startSymbol = symbol;

            while (symbol != null)
            {
                var symbolMembers = symbol.GetMembers();

                // Add symbols which weren't overriden yet
                List<ISymbol> newMembers = symbolMembers.Where(m => !(m is IMethodSymbol methodSymbol) || !overridenMethods.Contains(methodSymbol)).ToList();

                members.AddRange(newMembers);

                // Recursively add overriden methods
                List<IMethodSymbol> newOverridenMethods = symbolMembers.OfType<IMethodSymbol>().ToList();
                while (newOverridenMethods.Count > 0)
                {
                    newOverridenMethods.ForEach(m => overridenMethods.Add(m));
                    newOverridenMethods = newOverridenMethods
                        .Where(m => m.OverriddenMethod != null)
                        .Select(m => m.OverriddenMethod)
                        .ToList();
                }

                symbol = symbol.BaseType;
            }

            allMembersCache.Add(startSymbol, members.ToList());

            return members;
        }
    }
}
