using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.ProjectStructure;
using Utility.Trees;
using Utility.Trees.Abstractions;
using Utility.WPF.ResourceDictionarys;

namespace Utility.WPF.Controls.Trees
{
}
        //public static ITree ToTree(Assembly[] assemblies, Predicate<DictionaryEntry>? predicate = null)
        //{
        //    Tree<KeyValue> t_tree = new(new KeyValue("root"));

        //    foreach (var assembly in assemblies)
        //    {
        //        Tree<KeyValue> tree = new(new AssemblyKeyValue(assembly));

        //        foreach (var resKeyValue in assembly.SelectResourceDictionaries(ignoreXamlReaderExceptions: true))
        //        {
        //            var _tree = new Tree<KeyValue>(resKeyValue);
        //            foreach (var dicKeyValue in resKeyValue.ResourceDictionary.Cast<DictionaryEntry>().Select(a => new DictionaryEntryKeyValue(a)).ToArray())
        //            {
        //                if (predicate?.Invoke(dicKeyValue.Entry) != false)
        //                {

        //                    _tree.Add(new Tree<KeyValue>(dicKeyValue) { Parent = _tree });

        //                }
        //            }
        //            if (_tree.HasItems)
        //            {
        //                _tree.Parent = tree;
        //                tree.Add(_tree);
        //            }
        //        }
        //        t_tree.Add(tree);
        //    }
        //    return t_tree;
        //}

        //public static ITree ToTree(Assembly[] assemblies, Predicate<DictionaryEntry>? predicate = null)
        //{
        //    Tree t_tree = new("root") { Key = new StringKey("root")};

        //    foreach (var assembly in assemblies)
        //    {
        //        Tree tree = new(assembly) { Key = ass };

        //        foreach (var resKeyValue in assembly.SelectResourceDictionaries(ignoreXamlReaderExceptions: true))
        //        {
        //            var _tree = new Tree<KeyValue>(resKeyValue);
        //            foreach (var dicKeyValue in resKeyValue.ResourceDictionary.Cast<DictionaryEntry>().Select(a => new DictionaryEntryKeyValue(a)).ToArray())
        //            {
        //                if (predicate?.Invoke(dicKeyValue.Entry) != false)
        //                {

        //                    _tree.Add(new Tree<KeyValue>(dicKeyValue) { Parent = _tree });

        //                }
        //            }
        //            if (_tree.HasItems)
        //            {
        //                _tree.Parent = tree;
        //                tree.Add(_tree);
        //            }
        //        }
        //        t_tree.Add(tree);
        //    }
        //    return t_tree;
        //}





