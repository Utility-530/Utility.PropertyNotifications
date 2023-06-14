using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Utility.WPF.Attached;
using Utility.WPF.Helpers;

namespace Utility.WPF.Controls.Dragablz
{
    public class GroupsControl : DragablzVerticalItemsControl
    {
        public static readonly DependencyProperty IsReadOnlyPathProperty = DependencyProperty.Register("IsReadOnlyPath", typeof(string), typeof(GroupsControl), new PropertyMetadata(null));

        static GroupsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupsControl), new FrameworkPropertyMetadata(typeof(GroupsControl)));
        }

        public string IsReadOnlyPath
        {
            get => (string)GetValue(IsReadOnlyPathProperty);
            set => SetValue(IsReadOnlyPathProperty, value);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is Control control)
            {
                SetBinding(control, item);
                SetIsReadOnlyBinding(control, item);
            }
            base.PrepareContainerForItemOverride(element, item);
        }

        protected virtual void SetBinding(Control element, object dataContext)
        {
            if (string.IsNullOrEmpty(DisplayMemberPath))
                return;
            _ = element.ApplyTemplate();
            if (element.ChildOfType<TextBlock>() is not TextBlock textBlock)
            {
                if (element.FindChild<Thumb>("PART_Thumb") is Thumb thumb)
                {
                    _ = thumb.ApplyTemplate();
                    if (thumb.ChildOfType<TextBlock>() is not TextBlock textBlock2)
                    {
                        return;
                    }
                    textBlock = textBlock2;
                }
                else
                    return;
            }

            this.GetTemplateChild("PART_Text");
            Binding myBinding = new()
            {
                Source = dataContext,
                Path = new PropertyPath(DisplayMemberPath),
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, myBinding);
        }

        private void SetIsReadOnlyBinding(Control element, object dataContext)
        {
            if (string.IsNullOrEmpty(IsReadOnlyPath))
                return;

            Binding myBinding = new Binding
            {
                Source = dataContext,
                Path = new PropertyPath(IsReadOnlyPath),
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding(element, Ex.IsReadOnlyProperty, myBinding);
        }
    }
}