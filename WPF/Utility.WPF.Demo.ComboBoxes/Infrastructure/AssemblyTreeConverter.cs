using System;
using System.Globalization;
using System.Windows.Data;
using Utility.Trees.Abstractions;
using Utility.Trees;
using System.Reflection;
using Utility.ProjectStructure;
using System.Linq;
using System.Collections;

namespace Utility.WPF.Demo.ComboBoxes
{
    public class AssemblyTreeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sParameter = null;
            if (parameter is string str)
            {
                sParameter = str;
            }
            ITree tree = Utility.WPF.Controls.ComboBoxes.Ex.ToTree(new[] { typeof(Utility.WPF.Demo.Data.Model.Character).Assembly }, new Predicate<DictionaryEntry>(a =>
            {
                return a.Value.GetType().ToString().Contains(sParameter);

            }));
            return tree.Items;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static AssemblyTreeConverter Instance { get; } = new AssemblyTreeConverter();
    }


    public static class Ex
    {
        //public static ITree ToTree(Assembly[] assemblies, Predicate<Type>? predicate = null)
        //{
        //    Tree t_tree = new(default);

        //    foreach (var assembly in assemblies)
        //    {
        //        Tree tree = new(assembly);

        //        foreach (var resKeyValue in assembly.GetTypes())
        //        {
        //            if (predicate?.Invoke(resKeyValue) != false)
        //            {
        //                var _tree = new Tree(resKeyValue);
        //                _tree.Parent = tree;
        //                tree.Add(_tree);
        //                //foreach (var dicKeyValue in resKeyValue.ResourceDictionary.Cast<DictionaryEntry>().Select(a => new DictionaryEntryKeyValue(a)).ToArray())
        //                //{
        //                //    if (predicate?.Invoke(dicKeyValue) != false)
        //                //    {

        //                //        _tree.Children.Add(new Tree<KeyValue>(dicKeyValue) { Parent = _tree });
        //                //    }
        //                //}
        //            }
        //        }
        //        t_tree.Add(tree);
        //    }
        //    return t_tree;
        //}


        //public static ITree ToTree(Assembly[] assemblies, Predicate<DictionaryEntryKey>? predicate = null)
        //{
        //    Tree<KeyValue> t_tree = new(new KeyValue("root"));

        //    foreach (var assembly in assemblies)
        //    {
        //        Tree<KeyValue> tree = new(new AssemblyKeyValue(assembly));

        //        foreach (var resKeyValue in assembly.SelectResourceDictionaries(ignoreXamlReaderExceptions: true))
        //        {
        //            var _tree = new Tree<KeyValue>(resKeyValue);
        //            foreach (var dicKeyValue in resKeyValue.ResourceDictionary.Cast<DictionaryEntry>().Select(a => new DictionaryEntryKey(a)).ToArray())
        //            {
        //                if (predicate?.Invoke(dicKeyValue) != false)
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

    }
}
