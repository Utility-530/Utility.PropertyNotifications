using System.Collections;
using System.Reflection;
using Utility.Keys;
using Utility.ProjectStructure;
using Utility.Trees;
using Utility.Trees.Abstractions;
using Utility.WPF.ResourceDictionarys;

namespace Utility.WPF.Controls.ComboBoxes
{
    public static class Ex
    {
        public static ITree ToTree(Assembly[] assemblies, string filterType)
        {
            return ToTree(assemblies, new Predicate<DictionaryEntry>(a =>
            {
                return a.Value.GetType().ToString().Contains(filterType);

            }));
        }

        public static ITree ToTree(Assembly[] assemblies, Predicate<DictionaryEntry>? predicate = null)
        {
            Tree t_tree = new("root") { Key = new StringKey("root") };

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
                    if (_tree.HasChildren)
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

}
