using BreadcrumbLib.Infrastructure;
using SniffCore.Buttons;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BreadcrumbLib
{
    public class SelectableHeaderedItemsControl : HeaderedItemsControl
    {

        private SplitButton button;
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SelectableHeaderedItemsControl), new UIPropertyMetadata(false));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SelectableHeaderedItemsControl), new UIPropertyMetadata(false));

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(object), typeof(SelectableHeaderedItemsControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ParentItemProperty =
            DependencyProperty.Register("ParentItem", typeof(object), typeof(SelectableHeaderedItemsControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty IsFirstProperty = 
            DependencyProperty.Register("IsFirst", typeof(bool), typeof(SelectableHeaderedItemsControl), new UIPropertyMetadata(false));

        //internal static readonly DependencyProperty ViewProperty =
        //    DependencyProperty.Register("View", typeof(BreadcrumbView), typeof(BreadcrumbItem), new UIPropertyMetadata(null));



        static SelectableHeaderedItemsControl()
        {
            
        }

        public SelectableHeaderedItemsControl()
            : this(null)
        { }

        internal SelectableHeaderedItemsControl(object parent)
        {
            //View = view;
            ParentItem = parent;
            IsFirst = ParentItem == null;
   
        }

        #region properties
        //internal BreadcrumbView View
        //{
        //    get => (BreadcrumbView)GetValue(ViewProperty);
        //    set => SetValue(ViewProperty, value);
        //}

        public bool IsFirst
        {
            get => (bool)GetValue(IsFirstProperty);
            set => SetValue(IsFirstProperty, value);
        }

        public object ParentItem
        {
            get => (object)GetValue(ParentItemProperty);
            set => SetValue(ParentItemProperty, value);
        }

        public object Image
        {
            get => GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        #endregion properties






    }
}