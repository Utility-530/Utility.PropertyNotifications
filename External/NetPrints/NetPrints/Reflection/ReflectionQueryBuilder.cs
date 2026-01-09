using NetPrints.Core;
using NetPrints.Interfaces;
using NetPrints.WPF.Demo;
using NetPrintsEditor.Reflection;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPrints.Reflection
{

    public static class Helper
    {
        public static TypesProvider AndName(this IReflectionProviderQuery items, string name)
        {
            if (items is IReflectionProviderMethodQuery m)
                return new MethodTypes(m, name);
            else if (items is IReflectionProviderVariableQuery v)
                return new VariableTypes(v, name);

            throw new NotImplementedException();
        }

        public static NonStaticTypes NonStaticTypes(string name)
        {
            return new NonStaticTypes(name);
        }


        public static IReflectionProviderVariableQuery CreateVariableQuery()
        {
            return new ReflectionProviderVariableQuery();

        }
        public static IReflectionProviderMethodQuery CreateMethodQuery()
        {
            return new ReflectionProviderMethodQuery();

        }
    }

    public abstract class TypesProvider(string name) : ITypesProvider
    {
        public abstract IEnumerable<ISpecifier> types();

        public string Name => name;
        //protected virtual IEnumerable<SearchableComboBoxItem> Get() => types().Select(a => new SearchableComboBoxItem(Name, a));
    }

    public class MethodTypes(IReflectionProviderMethodQuery Query, string Name) : TypesProvider(Name)
    {
        static Lazy<IReflectionProvider> reflectionProvider = new(() => Locator.Current.GetService<IReflectionProvider>());
        public override IEnumerable<ISpecifier> types() => reflectionProvider.Value.GetMethods(Query);
    }

    public class VariableTypes(IReflectionProviderVariableQuery Query, string Name) : TypesProvider(Name)
    {
        public override IEnumerable<ISpecifier> types() => Locator.Current.GetService<IReflectionProvider>().GetVariables(Query);
    }


    public class Basic(string Name, params ISpecifier[] Query) : TypesProvider(Name)
    {
        public override IEnumerable<ISpecifier> types() => Query;
    }

    public class NonStaticTypes(string Name, params object[] Query) : TypesProvider(Name)
    {
        static Lazy<IReflectionProvider> reflectionProvider = new(() => Locator.Current.GetService<IReflectionProvider>());

        public override IEnumerable<ISpecifier> types() => reflectionProvider.Value.GetTypes();
    }

    public class NonStaticGenericTypes(string Name, params object[] Query) : TypesProvider(Name)
    {
        static Lazy<IReflectionProvider> reflectionProvider = new(() => Locator.Current.GetService<IReflectionProvider>());

        public override IEnumerable<ISpecifier> types() => reflectionProvider.Value.GetTypes().Where(t => t.GenericArguments.Any());
    }
}
