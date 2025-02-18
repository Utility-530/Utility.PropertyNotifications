
using Splat;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Changes;
using Utility.Descriptors;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Models;
using Utility.Nodes;
using Utility.Nodes.Demo;
using Utility.Nodes.WPF;
using Utility.Reactives;
using Utility.Structs.Repos;
using Utility.Trees.Abstractions;
using Type = System.Type;

namespace Utility.WPF.Controls.PropertyTrees
{
    public class PropertyTree : TreeView
    {

        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register(nameof(Object), typeof(object), typeof(PropertyTree), new PropertyMetadata(null, changed));

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyTree tree && e.NewValue is { } value)
            {
                tree.ItemsSource = create(value).Items;
            }

            static Node create(object obj)
            {
                Locator.Current.GetService<INodeSource>().Reset();
                var root = DescriptorFactory.CreateRoot(obj, Guid.NewGuid(), "obj").Take(1).Wait();
                var reflectionNode = new Node(root) { IsExpanded = true };
  
                Locator.Current.GetService<INodeSource>().Add(reflectionNode);
                return reflectionNode;
            }
        }

        static PropertyTree()
        {

            Style dynamicStyle = new(typeof(TreeView)) { };
            dynamicStyle.Setters.Add(new Setter(TreeView.ItemTemplateSelectorProperty, Utility.Nodes.WPF.DataTemplateSelector.Instance));
            dynamicStyle.Setters.Add(new Setter(TreeView.ItemContainerStyleSelectorProperty, Utility.Nodes.WPF.StyleSelector.Instance));
            StyleProperty.OverrideMetadata(typeof(PropertyTree), new FrameworkPropertyMetadata(dynamicStyle));
        }


        public PropertyTree()
        {
            Locator.CurrentMutable.RegisterLazySingleton<ITreeRepository>(() => new InMemoryTreeRepository());
            Locator.CurrentMutable.RegisterLazySingleton<INodeSource>(() => new NodeSource());
            Locator.CurrentMutable.RegisterConstant<IContext>(new Context());
            Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            Locator.CurrentMutable.RegisterConstant<IExpander>(Utility.Nodes.WPF.Expander.Instance);
        }



        public object Object
        {
            get { return (object)GetValue(ObjectProperty); }
            set { SetValue(ObjectProperty, value); }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = base.GetContainerForItemOverride();
            //if(item is TreeViewItem tvi)
            //{
            //    tvi.IsExpanded = true;
            //}
            return item;
        }
    }


    public class NodeSource : INodeSource
    {
        ObservableCollection<INode> nodes = [];
        public string New { get; }

        public void Add(INode node)
        {
            if (Locator.Current.GetService<IFilter>().Filter(node) == false)
            {
                node.IsVisible = false;
            }
            else
            {
                Locator.Current.GetService<IContext>().UI.Post((a) => nodes.Add(node), null);
            }
        }

        public void Reset()
        {
            Locator.Current.GetService<ITreeRepository>().Reset();
        }


        public IObservable<INode> ChildrenByGuidAsync(Guid guid)
        {
            bool b = false;
            return Observable.Create<INode>(observer =>
            {
                CompositeDisposable composite = new();
                nodes
                .AndAdditions<INode>()
                .Subscribe(node =>
                {
                    if (node.Key == new GuidKey(guid))
                    {
                        if (node.Data is IChildren children )
                        {
                            if (b == false)
                            {
                                b = true;
                                children.Children.Subscribe(async child =>
                                {
                                    if (child is Change<IDescriptor> { Type: Changes.Type.Add } change)
                                    {
                                        var tree = await (node as Node).ToTree(change.Value);

                                        if (tree is INode _node)
                                        {
                                            _node.IsExpanded = true;
                                            observer.OnNext(_node);
                                            this.Add(_node);
                                        }
                                        else
                                            throw new Exception(" $43333");

                                    }
                                }, () => observer.OnCompleted())
                                .DisposeWith(composite);
                            }
                            else
                            {

             //                   children.Children.Subscribe(async child =>
             //                   {
             //                       if (child is Change<IDescriptor> { Type: Changes.Type.Add } change)
             //                       {
             //                           var tree = await (node as Node).ToTree(change.Value);

             //                           if (tree is INode _node)
             //                           {
             //                               _node.IsExpanded = true;
             //                               observer.OnNext(_node);
             //                               this.Add(_node);
             //                           }
             //                           else
             //                               throw new Exception(" $43333");

             //                       }
             //                   }, () => observer.OnCompleted())
             //.DisposeWith(composite);

                            }
                            return;
                        }

                        throw new Exception("sdfg 324333");
                    }
                }).DisposeWith(composite);
                return composite;
            });
        }

        public IObservable<Structs.Repos.Key?> Find(Guid parentGuid, string name, Guid? guid = null, Type? type = null, int? localIndex = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<DateValue> Get(Guid guid, string name)
        {
            throw new NotImplementedException();
        }

        public int? MaxIndex(Guid guid, string v)
        {
            throw new NotImplementedException();
        }

        public DateTime Remove(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void Remove(INode node)
        {
            throw new NotImplementedException();
        }


        public void Set(Guid guid, string name, object value, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public IObservable<INode?> Single(string v)
        {
            throw new NotImplementedException();
        }
    }
}
