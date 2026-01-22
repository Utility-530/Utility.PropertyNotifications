using System.Collections;
using System.Globalization;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using LanguageExt.TypeClasses;
using Splat;
using Tiny.Toolkits;
using Utility;
using Utility.Helpers;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Nodes;
using Utility.PropertyDescriptors;
using Utility.Reactives;
using Utility.Trees;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions.Async;
using Utility.WPF.Trees;

namespace Utility.WPF.Trees
{
    public class ChildrenConverter : IValueConverter
    {
        public INodeRoot? source;
        public Dictionary<string, (INodeViewModel, string)> parentDict = new();
        private bool initialised;

        public ChildrenConverter()
        {
            source = Locator.Current.GetService<INodeRoot>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (initialised == false)
                initialise();
            return load(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        static readonly string guid = "0e71b2a1-7f70-496a-8e1a-f6e04a55d6f1";
        void initialise()
        {


        }

        IEnumerable load(object value)
        {
            if (value is NodeViewModel tree)
            {
                tree.SuppressExceptions = true;
                parentDict[tree.Type.Name] = ((INodeViewModel)tree.Parent(), tree.Name());
                tree.SetName(tree.Type.Name);
                tree.SetKey(tree.Type.GUID.ToString());
                tree.SetParent(tree.Parent());
                tree.SuppressExceptions = false;

                var _source = Locator.Current.GetService<Interfaces.Exs.INodeSource>("temp");
                if (_source.Nodes is IList<INodeViewModel> nodes)
                    nodes.Clear();
                tree.IsExpanded = true;
                source
                    .Create(tree)
                    .Subscribe(a =>
                    {                    });
                return tree.Children;
            }    
            else
                throw new Exception("£34vfgdf 32");
        }

        public static ChildrenConverter Instance { get; } = new ChildrenConverter();
    }
}