using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Utility.PropertyTrees.Abstractions;
using static Evan.Wpf.DependencyHelper;
using Utility.WPF.Controls.Trees;

namespace Utility.PropertyTrees.WPF
{
    public class PropertyTree : ContentControl
    {
        public static readonly DependencyProperty

            SelectedObjectProperty =
            DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyTree),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, Infrastructure.Helper.SelectedObjectPropertyChanged)),

            EngineProperty =
            DependencyProperty.Register("Engine", typeof(IPropertyGridEngine), typeof(PropertyTree),
               new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, EnginePropertyChanged)),
            TemplateSelectorProperty = Register(),
            SelectionChangedProperty = Register(),
            SourceProperty = Register();

        private static void EnginePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyTree propertyGrid && e.NewValue is IPropertyGridEngine _)
            {
                propertyGrid.RefreshSelectedObject();
            }
        }

        public static RoutedCommand BrowseCommand = new(), NavigateCommand = new(), RefreshCommand = new();

        private IPropertyGridEngine engine;
        public SynchronizationContext context;
        TreeListView treeListView = new();

        public PropertyTree()
        {
            context = SynchronizationContext.Current ?? throw new Exception("4g4e&&&&&");
            treeListView = new PropertyTreeList();            
        }

        public virtual string DefaultCategoryName { get; set; } = CategoryAttribute.Default.Category;

        public virtual async void RefreshSelectedObject()
        {
            engine = Engine;
            if (engine == null || SelectedObject == null)
            {
                return;
            }

            Source = await engine.Convert(SelectedObject);
            this.Content = treeListView;
            treeListView.ItemsSource = new[] { Source };
        }

        #region DependencyProperties

        public IPropertyGridEngine Engine
        {
            get { return (IPropertyGridEngine)GetValue(EngineProperty); }
            set { SetValue(EngineProperty, value); }
        }

        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        public DataTemplateSelector TemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(TemplateSelectorProperty); }
            set { SetValue(TemplateSelectorProperty, value); }
        }

        public ICommand SelectionChanged
        {
            get { return (ICommand)GetValue(SelectionChangedProperty); }
            set { SetValue(SelectionChangedProperty, value); }
        }

        public IPropertyNode Source
        {
            get { return (IPropertyNode)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        #endregion DependencyProperties
    }
}
