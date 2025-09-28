using Newtonsoft.Json;
using ReactiveUI;
using System.Collections;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Trees.Extensions;
using Utility.Trees.Abstractions;
using Utility.WPF.Factorys;
using Utility.WPF.ResourceDictionarys;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.WPF.Controls.ComboBoxes
{
    public class DataTemplateTreeSelectorBehavior : AssemblyTreeSelectorBehavior
    {
        public static readonly DependencyProperty DataTemplateProperty =
            DependencyProperty.Register("DataTemplate", typeof(DataTemplate), typeof(DataTemplateTreeSelectorBehavior), new PropertyMetadata());

        public DataTemplateTreeSelectorBehavior()
        {
            FrameworkElementKind = nameof(DataTemplate);
            Type = typeof(DataTemplate);
        }

        public DataTemplate DataTemplate
        {
            get { return (DataTemplate)GetValue(DataTemplateProperty); }
            set { SetValue(DataTemplateProperty, value); }
        }

        protected override void Set(object data)
        {
            DataTemplate = (DataTemplate)data;
        }
    }

    public class ItemsPanelTemplateTreeSelectorBehavior : AssemblyTreeSelectorBehavior
    {
        public static readonly DependencyProperty ItemsPanelTemplateProperty = DependencyProperty.Register("ItemsPanelTemplate", typeof(ItemsPanelTemplate), typeof(ItemsPanelTemplateTreeSelectorBehavior), new PropertyMetadata());

        public ItemsPanelTemplateTreeSelectorBehavior()
        {
            FrameworkElementKind = nameof(ItemsPanelTemplate);
            Type = typeof(ItemsPanelTemplate);
        }

        public ItemsPanelTemplate ItemsPanelTemplate
        {
            get { return (ItemsPanelTemplate)GetValue(ItemsPanelTemplateProperty); }
            set { SetValue(ItemsPanelTemplateProperty, value); }
        }

        protected override void Set(object data)
        {
            ItemsPanelTemplate = (ItemsPanelTemplate)data;
        }
    }

    public class StyleTreeSelectorBehavior : AssemblyTreeSelectorBehavior
    {
        public Style SelectedStyle
        {
            get { return (Style)GetValue(SelectedStyleProperty); }
            set { SetValue(SelectedStyleProperty, value); }
        }


        public static readonly DependencyProperty SelectedStyleProperty =
            DependencyProperty.Register("SelectedStyle", typeof(Style), typeof(StyleTreeSelectorBehavior), new PropertyMetadata());

        public StyleTreeSelectorBehavior()
        {
            FrameworkElementKind = nameof(Style);
            Type = typeof(Style);
        }

        protected override void Set(object data)
        {
            SelectedStyle = (Style)data;
        }
    }



    public class AssemblyTreeSelectorBehavior : TreeSelectorBehavior
    {
        public static readonly DependencyProperty AssembliesProperty = DependencyProperty.Register("Assemblies", typeof(IEnumerable), typeof(AssemblyTreeSelectorBehavior), new PropertyMetadata(ChangedAS));
        public static readonly DependencyProperty FrameworkElementKindProperty = DependencyProperty.Register("FrameworkElementKind", typeof(string), typeof(AssemblyTreeSelectorBehavior), new PropertyMetadata(ChangedAS));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(AssemblyTreeSelectorBehavior), new PropertyMetadata(TypeChanged));
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(object), typeof(AssemblyTreeSelectorBehavior), new PropertyMetadata(Changed2));
        public static readonly DependencyProperty FullKeyProperty = DependencyProperty.Register("FullKey", typeof(string), typeof(AssemblyTreeSelectorBehavior), new PropertyMetadata(Changed3));


        protected override void OnAttached()
        {
            AssociatedObject.WhenAnyValue(a => a.SelectedItems)
                .Subscribe(a => SelectedItemsChanged(a));

            AssociatedObject.WhenAnyValue(a => a.SelectedNode)
                .Subscribe(a => SelectedNodeChanged(a));

            if (Assemblies is { } assemblies && FrameworkElementKind is { } kind)
                SetItemsSource(this, assemblies, kind);
            if (AssociatedObject.ItemsSource is IReadOnlyTree tree && Type is Type type)
                ChangeType(tree, type);

            AssociatedObject.ParentPath = "Parent";
            AssociatedObject.SelectedItemTemplateSelector = CustomItemTemplateSelector.Instance;

            base.OnAttached();
        }

        private static void Changed3(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AssemblyTreeSelectorBehavior { AssociatedObject.ItemsSource: IReadOnlyTree tree } typeSelector && e.NewValue is string type)
            {
                typeSelector.ChangeFullKey(tree, type);
            }
        }

        private void ChangeFullKey(IReadOnlyTree tree, string type)
        {
            var x = AssemblyTreeKey.FromString(type);
            //var y = new DictionaryEntry(x.Element, default);
            //var descendants = tree.MatchDescendants(a => a is IReadOnlyTree { Data: DictionaryEntry { Key: { } key } } && key.ToString()?.Equals(x.Element) == true).ToArray();
            //if (descendants.Length > 1)
            //{

            //}
            //else
            if (AssociatedObject.SelectedNode is IGetData { Data: DictionaryEntry { Key: { } key } })
            {
                if (key.Equals(x.Element))
                    return;
            }

            var descendants = tree.Descendants(a => a.tree is IGetData { Data: DictionaryEntry { Key: { } key } }).ToArray();
            if (descendants.SingleOrDefault(a => a
                is IGetData { Data: DictionaryEntry { Key: { } key } }
                && NewMethod(x.Element, key)
                && (a as IGetParent<IReadOnlyTree>).Parent is IGetData { Data: ResourceDictionaryKeyValue { } res } parent
                && res.Entry.Key.Equals(x.ResourceDictionary) == true
                && (a as IGetParent<IReadOnlyTree>).Parent is IGetData  { Data: Assembly assembly }
                && assembly.GetName().Name.Equals(x.Assembly)) is { } innerTree)
            {
                AssociatedObject.IsError = false;
                AssociatedObject.UpdateSelectedItems(innerTree);
                if (AssociatedObject.TreeView?.ItemContainerGenerator.ContainerFromItem(AssociatedObject.TreeView.SelectedItem) is TreeViewItem item)
                    item.IsSelected = true;
                AssociatedObject.SelectedNode = innerTree;
            }
            else
            {
                AssociatedObject.IsError = true;
            }
        }

        private static void TypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AssemblyTreeSelectorBehavior { AssociatedObject.ItemsSource: IReadOnlyTree tree } typeSelector && e.NewValue is Type type)
            {
                typeSelector.ChangeType(tree, type);
            }
        }

        void ChangeType(IReadOnlyTree tree, Type _type)
        {
            //if (tree.MatchDescendant(a => a.Data.GetType().Equals(_type)) is IReadOnlyTree { } innerTree)
            //{
            //    IsError = false;
            //    UpdateSelectedItems(innerTree);
            //    if (_treeView?.ItemContainerGenerator.ContainerFromItem(_treeView.SelectedItem) is TreeViewItem item)
            //        item.IsSelected = true;
            //    SelectedNode = innerTree;
            //}
            //else
            //{
            //    IsError = true;
            //}
        }


        private static void Changed2(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AssemblyTreeSelectorBehavior { AssociatedObject.ItemsSource: IReadOnlyTree tree } typeSelector && e.NewValue is { } type)
            {
                typeSelector.Change(tree, type);
            }
        }

        void Change(IReadOnlyTree tree, object _key)
        {
            if (AssociatedObject.SelectedNode is IGetData { Data: DictionaryEntry { Key: { } key } })
            {
                if (key.Equals(_key))
                    return;
            }

            var descendants = tree.Descendants(a => a.tree is IGetData { Data: DictionaryEntry { Key: { } key } }).ToArray();
            if (descendants.SingleOrDefault(a => a
                is IGetData { Data: DictionaryEntry { Key: { } key } }
                && NewMethod(_key, key)) is { } innerTree)
            {
                AssociatedObject.IsError = false;
                AssociatedObject.UpdateSelectedItems(innerTree);
                if (AssociatedObject.TreeView?.ItemContainerGenerator.ContainerFromItem(AssociatedObject.TreeView.SelectedItem) is TreeViewItem item)
                    item.IsSelected = true;
                AssociatedObject.SelectedNode = innerTree;
            }
            else
            {
                AssociatedObject.IsError = true;
            }
        }

        private static bool NewMethod(object _type, object key)
        {
            return key.Equals(_type);
        }




        private static void ChangedAS(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AssemblyTreeSelectorBehavior { FrameworkElementKind: string kind, Assemblies: IEnumerable enumerable } typeSelector)
            {
                SetItemsSource(typeSelector, enumerable, kind);

            }
        }

        private static void SetItemsSource(AssemblyTreeSelectorBehavior selector, IEnumerable enumerable, string filterType)
        {
            if (selector.AssociatedObject == null)
                return;
            selector.AssociatedObject.ItemsSource = Ex.ToTree(enumerable.Cast<Assembly>().ToArray(), filterType);

            if (selector.AssociatedObject.ItemsSource is IReadOnlyTree tree)
            {
                if (selector.FullKey is { } fullKey)
                {
                    selector.ChangeFullKey(tree, fullKey);
                }
                if (selector.Key is { } key)
                {
                    selector.Change(tree, key);
                }
                if (selector.Type is { } type)
                {
                    selector.ChangeType(tree, type);
                }
            }
        }

        public AssemblyTreeSelectorBehavior()
        {

        }

        protected void SelectedItemsChanged(object value)
        {
            AssemblyTreeKey assemblyTreeKey = new();

            if (value is IEnumerable enumerable)
            {
                foreach (var x in enumerable)
                {
                    if (x is IGetData { Data: { } data })
                    {
                        if (data is Assembly assembly)
                        {
                            assemblyTreeKey.Assembly = assembly.GetName().Name;
                        }
                        else if (data is ResourceDictionaryKeyValue res)
                        {
                            assemblyTreeKey.ResourceDictionary = res.Entry.Key.ToString();
                        }
                        else if (data is DictionaryEntry entry)
                        {
                            assemblyTreeKey.Element = entry.Key.ToString();

                        }
                    }
                }
            }

            FullKey = assemblyTreeKey.ToString();
        }

        protected void SelectedNodeChanged(object value)
        {
            if (value is IGetData { Data: DictionaryEntry { Key: { } key, Value: { } _value } } &&
                _value.GetType() == Type)
            {
                Key = key;
                Set(_value);
            }
        }
        protected virtual void Set(object value)
        {

        }

        public string FullKey
        {
            get { return (string)GetValue(FullKeyProperty); }
            set { SetValue(FullKeyProperty, value); }
        }

        public string FrameworkElementKind
        {
            get { return (string)GetValue(FrameworkElementKindProperty); }
            set { SetValue(FrameworkElementKindProperty, value); }
        }

        public IEnumerable Assemblies
        {
            get { return (IEnumerable)GetValue(AssembliesProperty); }
            set { SetValue(AssembliesProperty, value); }
        }

        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public object Key
        {
            get { return GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }


        class CustomItemTemplateSelector : DataTemplateSelector
        {
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (item is IGetData { Data: { } data } tree)
                {
                    return TemplateGenerator.CreateDataTemplate(() =>
                    {

                        var textBlock = new TextBlock { };

                        Binding binding = new() { /*Path = new PropertyPath(nameof(IReadOnlyTree.Data)),*/ Converter = KeyValueConverter.Instance };
                        textBlock.SetBinding(TextBlock.TextProperty, binding);
                        return textBlock;
                    });

                }
                throw new Exception("d ss!$sd");
            }

            public static CustomItemTemplateSelector Instance { get; } = new();
        }

    }


    public class AssemblyTreeKey
    {

        public string Assembly { get; set; }
        public string ResourceDictionary { get; set; }
        public string Element { get; set; }

        public override string ToString()
        {
            var x = JsonConvert.SerializeObject(this);
            return x;
        }

        public static AssemblyTreeKey FromString(string s)
        {
            var x = JsonConvert.DeserializeObject<AssemblyTreeKey>(s);
            return x;
        }
    }

}
