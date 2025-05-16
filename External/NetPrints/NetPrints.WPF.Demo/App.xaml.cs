
using NetPrints.Compilation;
using NetPrints.Core;
using NetPrints.Enums;
using NetPrints.Interfaces;
using NetPrints.Reflection;
using NetPrints.WPF.Views;
using NetPrintsEditor.Converters;
using NetPrintsEditor.Reflection;
using Splat;
using System.Configuration;
using System.Data;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Data;

namespace NetPrints.WPF.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Locator.CurrentMutable.RegisterLazySingleton<IReflectionProvider>(() => ReflectionProvider.From(Project.CreateNew("MyProject", "MyNamespace")));
            Locator.CurrentMutable.RegisterLazySingleton(() => new CodeCompiler());
            Locator.CurrentMutable.RegisterLazySingleton<IValueConverter>(() => new SuggestionListConverter());
            Locator.CurrentMutable.RegisterLazySingleton<IBuiltInNodes>(() => BuiltInNodes.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<ISpecifierConverter>(() => new SpecifierConverter());
            //Locator.CurrentMutable.RegisterLazySingleton<ITypesEnumerable>(() => new TypesEnumerable());
            Locator.CurrentMutable.RegisterLazySingleton<ITypesEnumerable>(() => new VariableTypesEnumerable());
            Locator.CurrentMutable.RegisterLazySingleton<IAssemblyReferences>(() => new DefaultReferences());
        }
    }

    public class TypesEnumerable : ITypesEnumerable
    {
        Lazy<IEnumerable<ITypesProvider>> types = new(() =>
        {
            var methodQuery = Helper.CreateMethodQuery()
                        .WithArgumentType(TypeSpecifier.FromType<int>())
                        //.WithVisibleFrom(TypeSpecifier.FromType<string>())
                        .WithReturnType(TypeSpecifier.FromType<string>())
                        .WithStatic(true)
                        .AndName("Static Methods");

            return [methodQuery];
        });

        public IEnumerable<ITypesProvider> Types => types.Value;
    }
    
    public class VariableTypesEnumerable : ITypesEnumerable
    {
        Lazy<IEnumerable<ITypesProvider>> types = new(() =>
        {
            var methodQuery = Helper.CreateVariableQuery()
                        //.WithArgumentType(TypeSpecifier.FromType<int>())
                        //.WithVisibleFrom(TypeSpecifier.FromType<string>())
                        //.WithReturnType(TypeSpecifier.FromType<string>())
                        //.WithStatic(true)
                        .WithType(TypeSpecifier.FromType<NetPrints.Core.BaseType>())
                        .AndName("Static Methods");

            return [methodQuery];
        });

        public IEnumerable<ITypesProvider> Types => types.Value;
    }

    public class DefaultReferences : IAssemblyReferences
    {
        private static readonly IEnumerable<IAssemblyReference> references = new IAssemblyReference[]
        {
            new FrameworkAssemblyReference(".NETFramework/v4.5/System.dll"),
            new FrameworkAssemblyReference(".NETFramework/v4.5/System.Core.dll"),
            new FrameworkAssemblyReference(".NETFramework/v4.5/mscorlib.dll"),
            new AssemblyReference(@"O:\Users\rytal\source\repos\Utility\External\NetPrints\NetPrints.WPF.Demo\bin\Debug\net8.0-windows\\NetPrints.Core.dll")
        };

        public IEnumerable<IAssemblyReference> References => references;

    }


}