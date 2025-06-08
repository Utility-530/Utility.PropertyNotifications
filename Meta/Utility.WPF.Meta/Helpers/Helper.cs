using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Utility.Enums;
using Utility.Helpers;
using Utility.Helpers.Reflection;
using Utility.Meta;

namespace Utility.WPF.Meta
{
    public static class Helper
    {

        public const string DemoAppNameAppendage = "Demo";

        public static IEnumerable<FrameworkElementKeyValue> ViewTypes(this Assembly assembly) =>
            from type in assembly.GetTypes()
            where typeof(FrameworkElement).IsAssignableFrom(type)
            let _name = type.Name
            let name = type.GetAttributePropertySafe<ViewAttribute, int?>(a => a.Index)?.ToString() + _name
            orderby name
            select new FrameworkElementKeyValue(_name, type);
      
        public static IEnumerable<TypeKeyValue> Types(this Assembly assembly) =>
            from type in assembly.GetTypes()
            where type.ContainsGenericParameters == false
            where type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Any()
            select new TypeKeyValue(type.Name, type);


        public static IEnumerable<TypeKeyValue> TypesOf<T>(this Assembly assembly) =>
            from type in assembly.GetTypes()
            where type.IsAssignableTo(typeof(T))
            where type.ContainsGenericParameters == false
            where type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Length != 0
            select new TypeKeyValue(type.Name, type);

        public static IEnumerable<(Assembly, AssemblyType)> FindAssemblies()
        {
            return from a in AssemblySingleton.Instance.Assemblies
                   let contains = a.GetName().Name?.Contains(DemoAppNameAppendage) ?? false ? AssemblyType.Application : default
                   let userControls = a.DefinedTypes.Any(a => a.IsAssignableTo(typeof(UserControl))) ? AssemblyType.UserControl : default
                   let controls = a.DefinedTypes.Any(a => a.IsAssignableTo(typeof(Control))) ? AssemblyType.Control : default
                   //let viewModels = a.DefinedTypes.Any(a => a.IsAssignableTo(typeof(ReactiveObject))) ? AssemblyType.ViewModel : default
                   let resNames = a.GetManifestResourceNames().Length > 0 ? AssemblyType.ResourceDictionary : default
                   select (a, Utility.Helpers.EnumHelper.CombineFlags(new[] { contains, userControls, controls, /*viewModels,*/ resNames }));
        }

        public static bool Predicate(string key)
        {
            var rKey = key.Remove(".baml");

            foreach (var ignore in new[] { "view", "usercontrol", "app" })
            {
                if (rKey.EndsWith(ignore))
                    return false;
            }
            return true;
        }

        public static IEnumerable<KeyValuePair<int, TSource>> Index<TSource>(this IEnumerable<TSource> source, int startIndex = 0)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Select((item, index) => new KeyValuePair<int, TSource>(startIndex + index, item));
        }

    }
}