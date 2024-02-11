using System;
using System.Globalization;
using System.Windows.Data;
using Utility.Trees.Abstractions;
using Utility.Trees;
using System.Collections.Generic;
using System.Reflection;

namespace Utility.WPF.Demo.Trees
{
    internal class AssemblyTreeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sParameter = null;
            if (parameter is string str)
            {
                sParameter = str;
            }
            ITree tree = Ex.ToTree(new[] { typeof(AssemblyTreeConverter).Assembly, typeof(IDynamicTree).Assembly }, new Predicate<Type>(a =>
            {
                //if (a is DictionaryEntryKeyValue kv)
                //{
                //    return kv.Value.GetType().Name.Contains(sParameter);
                //}
                return true;
            }));
            return tree.Items;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static AssemblyTreeConverter Instance { get; } = new AssemblyTreeConverter();
    }


    static class Ex
    {
        public static ITree ToTree(Assembly[] assemblies, Predicate<Type>? predicate = null)
        {
            Tree t_tree = new(default);

            foreach (var assembly in assemblies)
            {
                Tree tree = new(assembly);

                foreach (var resKeyValue in assembly.GetTypes())
                {
                    if (predicate?.Invoke(resKeyValue) != false)
                    {
                        var _tree = new Tree(resKeyValue);
                        _tree.Parent = tree;
                        tree.Add(_tree);
                        //foreach (var dicKeyValue in resKeyValue.ResourceDictionary.Cast<DictionaryEntry>().Select(a => new DictionaryEntryKeyValue(a)).ToArray())
                        //{
                        //    if (predicate?.Invoke(dicKeyValue) != false)
                        //    {

                        //        _tree.Children.Add(new Tree<KeyValue>(dicKeyValue) { Parent = _tree });
                        //    }
                        //}
                    }
                }
                t_tree.Add(tree);
            }
            return t_tree;
        }


    }
}
