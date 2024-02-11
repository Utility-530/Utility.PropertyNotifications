using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.WPF.Controls.Trees
{
    static class Ex
    {
        public static ITree ToTree(Assembly[] assemblies)
        {
            ViewModelTree t_tree = new();

            foreach (var assembly in assemblies)
            {
                ViewModelTree tree = new(assembly);

                foreach (var resKeyValue in assembly.GetTypes())
                {
                    //if (predicate?.Invoke(resKeyValue) != false)
                    //{
                    var _tree = new ViewModelTree(resKeyValue)
                    {
                        Parent = tree
                    };
                    tree.Add(_tree);
                    //foreach (var dicKeyValue in resKeyValue.ResourceDictionary.Cast<DictionaryEntry>().Select(a => new DictionaryEntryKeyValue(a)).ToArray())
                    //{
                    //    if (predicate?.Invoke(dicKeyValue) != false)
                    //    {

                    //        _tree.Children.Add(new Tree<KeyValue>(dicKeyValue) { Parent = _tree });
                    //    }
                    //}
                    // }
                }
                t_tree.Add(tree);
            }
            return t_tree;
        }
    }

    public class ViewModelTree : Tree
    {
        private bool isExpanded = false;
        private bool isSelected;

        public ViewModelTree(object? data = null, params object[] items) : base(data, items)
        {
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
    }
}
