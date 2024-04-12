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
    public static class Ex
    {
        public static ITree ToTree(Assembly[] assemblies)
        {
            ViewModelTree t_tree = new("root");

            foreach (var assembly in assemblies)
            {
                ViewModelTree tree = new(assembly.GetName().Name, assembly);

                foreach (var type in assembly.GetTypes())
                {
                    //if (predicate?.Invoke(resKeyValue) != false)
                    //{
                    var _tree = new ViewModelTree(type.Name, type)
                    {
                        Parent = tree
                    };
                    tree.Add(_tree);
                }
                t_tree.Add(tree);
            }
            return t_tree;
        }


        public static ITree ToTree(Assembly[] assemblies, string filterType)
        {
            return ToTree(assemblies, new Predicate<DictionaryEntry>(a =>
            {
                return a.Value.GetType().ToString().Contains(filterType);

            }));
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

        public static ITree ToTree(Assembly[] assemblies, Predicate<DictionaryEntry>? predicate = null)
        {
            Tree t_tree = new("root") { Key = new StringKey("root")};

            foreach (var assembly in assemblies)
            {
                Tree tree = new(assembly) { Key = new AssemblyKey(assembly) };

                foreach (var resKeyValue in assembly.SelectResourceDictionaries(ignoreXamlReaderExceptions: true))
                {
                    var _tree = new Tree(resKeyValue) { Key = resKeyValue.Key };
                    foreach (var dicKeyValue in resKeyValue.ResourceDictionary.Cast<DictionaryEntry>().ToArray())
                    {
                        if (predicate?.Invoke(dicKeyValue) != false)
                        {

                            _tree.Add(new Tree(dicKeyValue) { Parent = _tree, Key = dicKeyValue.Key.ToString() });

                        }
                    }
                    if (_tree.HasItems)
                    {
                        _tree.Parent = tree;
                        tree.Add(_tree);
                    }
                }
                t_tree.Add(tree);
            }
            return t_tree;
        }
    }


    public class ViewModelTree : Tree, IName
    {
        private bool isExpanded = false;
        private bool isSelected;

        public ViewModelTree(string name, object? data = null, params object[] items) : base(data, items)
        {
            Name = name;
        }

        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                isExpanded = value;
                //this.OnPropertyChanged();
            }
        }

        public bool IsSelected { get => isSelected; set => isSelected = value; }
        public string Name { get; }
    }
}
