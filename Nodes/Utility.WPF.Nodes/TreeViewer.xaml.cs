using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Nodes;
using Utility.Interfaces.NonGeneric;
using Utility.WPF.Nodes;
using Utility.Trees.Abstractions;
using VisualJsonEditor.Test;
using Utility.Extensions;
using Utility.Entities.Comms;

namespace Views.Trees
{
    public partial class TreeViewer : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(object), typeof(TreeViewer), new PropertyMetadata(Changed));
        public static readonly DependencyProperty TreeViewItemFactoryProperty = DependencyProperty.Register("TreeViewItemFactory", typeof(ITreeViewItemFactory), typeof(TreeViewer), new PropertyMetadata(Changed));
        public static readonly DependencyProperty PanelsConverterProperty = DependencyProperty.Register("PanelsConverter", typeof(IValueConverter), typeof(TreeViewer), new PropertyMetadata(Changed));
        public static readonly DependencyProperty DataTemplateSelectorProperty = DependencyProperty.Register("DataTemplateSelector", typeof(DataTemplateSelector), typeof(TreeViewer), new PropertyMetadata(Changed));
        public static readonly DependencyProperty TreeViewBuilderProperty = DependencyProperty.Register("TreeViewBuilder", typeof(ITreeViewBuilder), typeof(TreeViewer), new PropertyMetadata());
        public static readonly DependencyProperty TreeViewFilterProperty = DependencyProperty.Register("TreeViewFilter", typeof(ITreeViewFilter), typeof(TreeViewer), new PropertyMetadata());
        public static readonly DependencyProperty StyleSelectorProperty = DependencyProperty.Register("StyleSelector", typeof(StyleSelector), typeof(TreeViewer), new PropertyMetadata());
        public static readonly DependencyProperty EventListenerProperty = DependencyProperty.Register("EventListener", typeof(IEventListener), typeof(TreeViewer), new PropertyMetadata());

        private IDisposable disposable;
        private TreeView treeView;

        public TreeViewer()
        {
            InitializeComponent();
            treeView = new TreeView();          
            this.Content = treeView;
            Initialise(treeView);
            this.Loaded += Viewer_Loaded;

            void Viewer_Loaded(object sender, RoutedEventArgs e)
            {
                if (ViewModel != null)
                {
                    if (ViewModel is ILoad load)
                        load.Load();
                    disposable?.Dispose();
                    disposable = TreeViewBuilder.Build(treeView, ViewModel, TreeViewItemFactory, PanelsConverter, StyleSelector, DataTemplateSelector, TreeViewFilter);
                }
            }
        }

        void Initialise(TreeView treeView)
        {
            treeView
                .MouseDoubleClicks()
                .Subscribe(a =>
                {
                    if (a is { Header: ITree { } node })
                    {
                        EventListener?.Send(new DoubleClickChange(a, node));
                    }                  
                });

            this.AddHandler(CustomTreeViewItem.ConditionalClickEvent, new RoutedEventHandler(ItemLoaded));

            treeView
                .MouseSingleClicks()
                .Subscribe(a =>
                {
                    if (a is { Header: ITree { } node })
                    {
                        EventListener?.Send(new ClickChange(a, node));
                    }
                });

            treeView
                .MouseMoves()
                .Subscribe(a =>
                {
                    if (a.item is { Header: ITree { } node })
                    {
                        Utility.Structs.Point sPoint = new(a.point.X, a.point.Y);
                        EventListener?.Send(new OnHoverChange(a.item, node, true, sPoint));
                    }
                });

            treeView
                .MouseHoverLeaves()
                .Subscribe(a =>
                {
                    if (a is { Header: ITree { } node })
                    {
                        EventListener?.Send(new OnHoverChange(a, node, false, default));
                    }
                });
        }

        private void ItemLoaded(object sender, RoutedEventArgs e)
        {
            if(e.OriginalSource is TreeViewItem { Header:IReadOnlyTree tree})
            {
                EventListener?.Send(new OnLoadedChange(e.OriginalSource, tree));
            }
        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ILoad viewModel)
            {
                viewModel.Load();
            }
        }

        #region properties

        public IEventListener EventListener
        {
            get { return (IEventListener)GetValue(EventListenerProperty); }
            set { SetValue(EventListenerProperty, value); }
        }

        public object ViewModel
        {
            get { return (object)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public ITreeViewItemFactory TreeViewItemFactory
        {
            get { return (ITreeViewItemFactory)GetValue(TreeViewItemFactoryProperty); }
            set { SetValue(TreeViewItemFactoryProperty, value); }
        }

        public IValueConverter PanelsConverter
        {
            get { return (IValueConverter)GetValue(PanelsConverterProperty); }
            set { SetValue(PanelsConverterProperty, value); }
        }

        public DataTemplateSelector DataTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DataTemplateSelectorProperty); }
            set { SetValue(DataTemplateSelectorProperty, value); }
        }

        public ITreeViewBuilder TreeViewBuilder
        {
            get { return (ITreeViewBuilder)GetValue(TreeViewBuilderProperty); }
            set { SetValue(TreeViewBuilderProperty, value); }
        }

        public StyleSelector StyleSelector
        {
            get { return (StyleSelector)GetValue(StyleSelectorProperty); }
            set { SetValue(StyleSelectorProperty, value); }
        }

        public ITreeViewFilter TreeViewFilter
        {
            get { return (ITreeViewFilter)GetValue(TreeViewFilterProperty); }
            set { SetValue(TreeViewFilterProperty, value); }
        }

        #endregion

        public void Reload()
        {
            TreeExtensions.Visit(treeView as ItemsControl, 
            a =>
            {
                return a.Items.Cast<TreeViewItem>();
            }, 
            a =>
            {
                if (a is TreeView { DataContext: var datacontext })
                { }
                else if (a is TreeViewItem { Header: ISave viewModel })
                    viewModel.Save(null);
                else
                    throw new Exception("sdf 3l8hjhg");
            });

            treeView.Items.Clear();
            //root.Load(); 
            disposable.Dispose();
            disposable = TreeViewBuilder.Build(treeView, ViewModel, TreeViewItemFactory, PanelsConverter, StyleSelector, DataTemplateSelector, TreeViewFilter);
        }

        public object Add()
        {
            //Guid guid = default;
            object obj = default;
            TreeExtensions.Visit(treeView as ItemsControl, a =>
            {

                var items = a.Items.Cast<TreeViewItem>();
                return items;
            }, a =>
            {

                if (a is TreeViewItem { Header: IIsSelected { IsSelected: true } viewModel })
                    obj = viewModel;
                else
                {

                }
                //else
                //    throw new Exception("sdf 3l8hjhg");
            });
            return obj;
        }

        public void Remove()
        {
            TreeExtensions.Visit(treeView as ItemsControl, a =>
            {

                var items = a.Items.Cast<TreeViewItem>();
                return items;
            }, a =>
            {

                if (a is TreeViewItem { Header: IIsSelected { IsSelected: true } viewModel })
                {
                    if (viewModel is IDelete delete)
                        delete.Delete(null);
                }
                else
                {

                }
            });
        }
    }
}
