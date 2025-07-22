using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Utility.Helpers;

namespace Utility.WPF.ResourceDictionarys
{
    public class AutoResourceDictionary : SharedResourceDictionary
    {
        private Type? type;

        public string Exclude { get; set; }

        public Type Type
        {
            set
            {
                if (type == value)
                    return;
                type = value;
                string[] exclusions = null;
                if (Exclude != null)
                {
                    exclusions = [.. this.Exclude.Split(',').Select(a => a)];
                }

                foreach (var (_, resourceDictionary) in type.Assembly.SelectResourceDictionaries(a => !predicate(a, exclusions)))
                {
                    AddToMergedDictionaries(resourceDictionary);
                }
            }
        }

        private static bool predicate(System.Collections.DictionaryEntry a, string[] exclusions)
        {
            if (exclusions is { } ex &&  a.Key is string key)
                return StringHelper.Contains(ex, key, StringComparison.InvariantCultureIgnoreCase);
            return false;
        }
    }
}

//foreach (var item in keys)
//{
//    try
//    {

//        MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(item, UriKind.Relative) });
//    }
//    catch(IOException ex) when (ex.Message.Contains("Cannot locate resource"))
//    {

//    }
//}