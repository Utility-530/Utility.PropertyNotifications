using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Utility.Enums;
using Utility.Helpers;
using Utility.Meta;

namespace Utility.WPF.Meta
{
    public static class Helper
    {

        public const string DemoAppNameAppendage = "Demo";

        public static IEnumerable<FrameworkElementKeyValue> ViewTypes(this Assembly assembly) => assembly
            .GetTypes()
            .Where(a => typeof(FrameworkElement).IsAssignableFrom(a))
            .GroupBy(type =>
            (type.Name.Contains("UserControl") ? type.Name?.ReplaceLast("UserControl", string.Empty) :
            type.Name.Contains("View") ? type.Name?.ReplaceLast("View", string.Empty) : type.Name)!)
            .OrderBy(a => a.Key)
            .ToDictionaryOnIndex()
            .Select(a => new FrameworkElementKeyValue(a.Key, a.Value));

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

        public static Dictionary<string, T> ToDictionaryOnIndex<T>(this IEnumerable<IGrouping<string, T>> groupings)
            => groupings
                .SelectMany(grp => grp.Index().ToDictionary(kvp => kvp.Key > 0 ? grp.Key + kvp.Key : grp.Key, c => c.Value))
                .ToDictionary(a => a.Key, a => a.Value);

        public static IEnumerable<KeyValuePair<int, TSource>> Index<TSource>(this IEnumerable<TSource> source, int startIndex = 0)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Select((item, index) => new KeyValuePair<int, TSource>(startIndex + index, item));
        }

    }
}