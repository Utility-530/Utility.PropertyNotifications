using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Interfaces.NonGeneric;
using Utility.WPF.Nodes;
using Utility.Trees.Abstractions;
using Utility.Trees.WPF.Abstractions;
using System.Collections.ObjectModel;
using Utility.Entities.Comms;
using Utility.Trees.Extensions;

namespace Utility.Trees.WPF
{
    public class TreeViewer : ItemsControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(object), typeof(TreeViewer), new PropertyMetadata(Changed));
        public static readonly DependencyProperty TreeViewItemFactoryProperty = DependencyProperty.Register("TreeViewItemFactory", typeof(ITreeViewItemFactory), typeof(TreeViewer), new PropertyMetadata(Changed));
        public static readonly DependencyProperty PanelsConverterProperty = DependencyProperty.Register("PanelsConverter", typeof(IValueConverter), typeof(TreeViewer), new PropertyMetadata(Changed));
        public static readonly DependencyProperty DataTemplateSelectorProperty = DependencyProperty.Register("DataTemplateSelector", typeof(DataTemplateSelector), typeof(TreeViewer), new PropertyMetadata(Changed));
        public static readonly DependencyProperty TreeViewBuilderProperty = DependencyProperty.Register("TreeViewBuilder", typeof(ITreeViewBuilder), typeof(TreeViewer), new PropertyMetadata());
        public static readonly DependencyProperty TreeViewFilterProperty = DependencyProperty.Register("TreeViewFilter", typeof(IFilter), typeof(TreeViewer), new PropertyMetadata());
        public static readonly DependencyProperty StyleSelectorProperty = DependencyProperty.Register("StyleSelector", typeof(StyleSelector), typeof(TreeViewer), new PropertyMetadata());
        public static readonly DependencyProperty EventListenerProperty = DependencyProperty.Register("EventListener", typeof(IEventListener), typeof(TreeViewer), new PropertyMetadata());

        private IDisposable disposable;

        public TreeViewer()
        {
            Initialise(this);
            this.Loaded += Viewer_Loaded;

            void Viewer_Loaded(object sender, RoutedEventArgs e)
            {
                if (ViewModel != null && IsInitialised == null)
                {
                    IsInitialised = ViewModel;
                    if (ViewModel is ILoad load)
                        load.Load();
                    disposable?.Dispose();
                    if (ViewModel is IChildren items)
                    {
                        this.ItemContainerStyleSelector = StyleSelector;
                        this.ItemTemplateSelector = DataTemplateSelector;         
                        disposable = TreeViewBuilder.Build(this, items, TreeViewItemFactory, PanelsConverter, StyleSelector, DataTemplateSelector, TreeViewFilter);
                    }
                    else
                        throw new Exception("ds 38787");
                }
            }
        }

        public object IsInitialised { get; set; }


        void Initialise(ItemsControl treeView)
        {
            this.ItemsSource = new ObservableCollection<ItemsControl>();

            if (treeView is TreeView _treeView)
            {
                _treeView.SelectedItemChanges().Subscribe(a =>
                {
                    if (a is HeaderedItemsControl { Header: ITree { } node })
                    {
                        EventListener?.Send(new SelectedItemChange(a, node));
                    }
                });
            }

            //ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Visible);
            //ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Visible);

            treeView
                .MouseDoubleClicks()
                .Subscribe(a =>
                {
                    if (a is { Header: ITree { } node })
                    {
                        EventListener?.Send(new DoubleClickChange(a, node));
                    }
                });

            this.AddHandler(LoadedEvent, new RoutedEventHandler(ItemLoaded));

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
            if (e.OriginalSource is TreeViewItem { Header: IReadOnlyTree tree })
            {
                EventListener?.Send(new OnLoadedChange(e.OriginalSource, tree));
            }
        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ViewModelProperty && e.NewValue!=e.OldValue)
            {
                if (e.NewValue is IChildren items && d is TreeViewer treeViewer && e.NewValue != treeViewer.IsInitialised && treeViewer.TreeViewBuilder != null)
                {
                    treeViewer.ItemContainerStyleSelector = treeViewer.StyleSelector;
                    treeViewer.ItemTemplateSelector = treeViewer.DataTemplateSelector;
                    treeViewer.disposable = treeViewer.TreeViewBuilder.Build(treeViewer, items, treeViewer.TreeViewItemFactory, treeViewer.PanelsConverter, treeViewer.StyleSelector, treeViewer.DataTemplateSelector, treeViewer.TreeViewFilter);
                }
            }
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

        public IFilter TreeViewFilter
        {
            get { return (IFilter)GetValue(TreeViewFilterProperty); }
            set { SetValue(TreeViewFilterProperty, value); }
        }

        #endregion

        public void Reload()
        {
            this.Visit<ItemsControl>(
            a =>
            {
                return a.Items.Cast<HeaderedItemsControl>();
            },
            a =>
            {
                if (a is ItemsControl { DataContext: var datacontext })
                { }
                else if (a is HeaderedItemsControl { Header: ISave viewModel })
                    viewModel.Save(null);
                else
                    throw new Exception("sdf 3l8hjhg");
            });

            this.Items.Clear();
            //root.Load(); 
            disposable.Dispose();

            if (ViewModel is IChildren items)
                disposable = TreeViewBuilder.Build(this, items, TreeViewItemFactory, PanelsConverter, StyleSelector, DataTemplateSelector, TreeViewFilter);
            else
                throw new Exception("ds 38787");
        }

        public object Add()
        {
            //Guid guid = default;
            object? obj = default;
            (this as ItemsControl).Visit( a =>
            {

                var items = a.Items.Cast<HeaderedItemsControl>();
                return items;
            }, a =>
            {

                if (a is HeaderedItemsControl { Header: IGetIsSelected { IsSelected: true } viewModel })
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
            (this as ItemsControl).Visit(a =>
            {

                var items = a.Items.Cast<HeaderedItemsControl>();
                return items;
            }, a =>
            {

                if (a is HeaderedItemsControl { Header: IGetIsSelected { IsSelected: true } viewModel })
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
