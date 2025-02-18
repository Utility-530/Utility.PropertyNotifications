using BreadcrumbLib.Infrastructure;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BreadcrumbLib
{
    [TemplatePart(Type = typeof(ItemsControl), Name = PartNameView)]
    public class Breadcrumb : TreeView
    {
        protected const string PartNameView = "PART_View";
        private ItemsControl view;
        protected static readonly DependencyPropertyKey SelectedItemPropertyKey = DependencyProperty.RegisterReadOnly(
           "SelectedItem", typeof(object), typeof(Breadcrumb), new FrameworkPropertyMetadata(null,
              FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register("Buttons",
           typeof(ObservableCollection<ButtonBase>), typeof(Breadcrumb), new UIPropertyMetadata(new ObservableCollection<ButtonBase>()));

        static Breadcrumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Breadcrumb), new FrameworkPropertyMetadata(typeof(Breadcrumb)));
        }
        public Breadcrumb()
        { 
        }

        #region properties

        /// <remarks>
        /// TreeView's SelectedItem only has get accessor, hence need for new property
        /// </remarks>
        public new object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            private set { SetValue(SelectedItemPropertyKey, value); }
        }

        /// <summary>
        /// The buttons at the right end of the control
        /// </summary>
        public ObservableCollection<ButtonBase> Buttons
        {
            get { return (ObservableCollection<ButtonBase>)GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }

        #endregion properties

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            view = GetTemplateChild(PartNameView) as ItemsControl;
            var view2 = GetTemplateChild("PART_Buttons") as ItemsControl;

            view.PreviewMouseDown += (s, e) => Breadcrumb_MouseDown(VisualTreeHelper.HitTest(this, e.GetPosition(this)))?.Focus();

            //Mouse.AddMouseDownHandler(this.GetParentWindow(this), Handler2);

            if (!Items.IsEmpty)
            {
                var item = Items[0];
                Items.RemoveAt(0);
                Helper.GoTo(item, view.Items, view.ItemContainerGenerator);
            }

            this.Focus();

            Breadcrumb Breadcrumb_MouseDown(HitTestResult target)
            {
                switch (target?.VisualHit)
                {
                    case Border:
                    case Path:
                        return this;
                    default:
                        return null;
                }
            }
        }

        ///<summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items" /> property changes.
        ///</summary>
        ///<param name="e">Information about the change.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (view != null && view.Items.IsEmpty && !Items.IsEmpty)
                Helper.GoTo(Items[0], view.Items, view.ItemContainerGenerator);
            base.OnItemsChanged(e);
        }

        public void AddLast(object item)
        {
            AddTrail(view.Items[^1], item);
        }


        internal void AddTrail(object parent, object item)
        {
            if (Helper.AddTrail(parent, item, view.Items, view.ItemContainerGenerator) is BreadcrumbItem bitem)
                bitem.IsSelected = true;
        }


        internal void GoTo(object target)
        {
            Helper.GoTo(target, view.Items, view.ItemContainerGenerator);
        }


        class Helper
        {
            public static object AddTrail(object parent, object item, IList items, ItemContainerGenerator generator)
            {
                if (parent == item)
                    return null;

                int index = 0;

                if (parent != null)
                    index = GetIndex(parent, items);

                for (int i = items.Count - 1; i >= index + 1; i--)
                    RemoveItem(items, i, generator);

                return AddAndSelect(item, items);
            }

            public static void GoTo(object target, IList items, ItemContainerGenerator generator)
            {
                int index = GetIndex(target, items);

                for (int i = items.Count - 1; i >= index; i--)
                    RemoveItem(items, i, generator);

                AddAndSelect(target, items);
            }

            private static object AddAndSelect(object item, IList items)
            {

                if (items.Contains(item) == false)
                    items.Add(item);

                return item;
            }

            private static int GetIndex(object item, IList items)
            {

                for (int i = items.Count - 1; i >= 0; i--)
                {
                    if (items[i].Equals(item))
                    {
                        return i + 1;

                    }
                }
                return 0;
            }

            private static void RemoveItem(IList items, int i, ItemContainerGenerator generator)
            {
                BreadcrumbItem container = generator.ContainerFromIndex(i) as BreadcrumbItem;
                if (container != null)
                    container.IsSelected = false;
                items.RemoveAt(i);
            }
        }
    }

    internal class BreadcrumbView : CustomItemsControl<BreadcrumbItem>
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            object parent = null;
            if (Items.Count > 1)
                parent = Items[^2];
            if (parent is DependencyObject dependencyObject)
                parent = ItemContainerGenerator.ItemFromContainer(dependencyObject);
            return new BreadcrumbItem(parent);
        }
    }
}