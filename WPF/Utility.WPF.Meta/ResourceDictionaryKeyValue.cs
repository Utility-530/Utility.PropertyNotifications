using MoreLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Utility.Common;
using Utility.Helpers;
using Utility.WPF.Helpers;

namespace Utility.WPF.Meta
{
    public class ResourceDictionaryKeyValue : KeyValue
    {
        private readonly Lazy<MasterDetailGrid> lazy;

        public ResourceDictionaryKeyValue(string key, ResourceDictionary resourceDictionary) : base(key)
        {
            ResourceDictionary = resourceDictionary;
            lazy = new(() => new MasterDetailGrid(resourceDictionary.Cast<DictionaryEntry>().Select(a => new DataTemplateKeyValue(a)).ToArray()));
        }

        public ResourceDictionary ResourceDictionary { get; }

        public override FrameworkElement Value => lazy.Value;

        public static IEnumerable<ResourceDictionaryKeyValue> ResourceViewTypes(Assembly assembly) =>
            assembly
            .SelectResourceDictionaries(predicate: entry => Predicate(entry.Key.ToString()), ignoreXamlReaderExceptions: true)
       //.GroupBy(type =>
       //(type.Name.Contains("UserControl") ? type.Name?.ReplaceLast("UserControl", string.Empty) :
       //type.Name.Contains("View") ? type.Name?.ReplaceLast("View", string.Empty) :
       //type.Name)!)
       .OrderBy(a => a.entry.Key)
       //.ToDictionaryOnIndex()
       .Select(a => new ResourceDictionaryKeyValue(a.entry.Key.ToString().Split("/").Last().Remove(".baml"), a.resourceDictionary));

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
    }
}