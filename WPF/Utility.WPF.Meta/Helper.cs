using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Utility.Helpers;
using Utility.WPF.Helpers;

namespace Utility.WPF.Meta
{
    public static class Helper
    {
        public static IEnumerable<FrameworkElementKeyValue> ViewTypes(this Assembly assembly) => assembly
            .GetTypes()
            .Where(a => typeof(UserControl).IsAssignableFrom(a))
            .GroupBy(type =>
            (type.Name.Contains("UserControl") ? type.Name?.ReplaceLast("UserControl", string.Empty) :
            type.Name.Contains("View") ? type.Name?.ReplaceLast("View", string.Empty) : type.Name)!)
            .OrderBy(a => a.Key)
            .ToDictionaryOnIndex()
            .Select(a => new FrameworkElementKeyValue(a.Key, a.Value));


        public static IEnumerable<ResourceDictionaryKeyValue> ResourceViewTypes(this Assembly assembly) => assembly
        .SelectResourceDictionaries(predicate: entry => Predicate(entry.Key.ToString()), ignoreXamlReaderExceptions: true)
        .OrderBy(a => a.entry.Key)
        .Select(a => new ResourceDictionaryKeyValue(a.entry.Key.ToString(), a.resourceDictionary));


        private static bool Predicate(string key)
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


    }
}