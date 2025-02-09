using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes.Demo.Styles
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/8715315/how-to-trigger-datatemplateselector-when-property-changes"></a>
    /// </summary>
    public class UpdateContentControlBehavior : Behavior<ContentControl>
    {

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(UpdateContentControlBehavior), new FrameworkPropertyMetadata(null, OnValueChanged));
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UpdateContentControlBehavior behavior /*&& e.NewValue.GetType() != behavior.Type*/)
                behavior.Update();
        }

        public UpdateContentControlBehavior() : base() { }

        protected override void OnAttached()
        {
            base.OnAttached();
            Update();
        }

        void Update()
        {
            if (Value != null && AssociatedObject != null)
            {
                var binding = BindingOperations.GetBinding(AssociatedObject, ContentPresenter.ContentProperty);
                BindingOperations.ClearBinding(AssociatedObject, ContentPresenter.ContentProperty);
                AssociatedObject.Content = null;
                BindingOperations.SetBinding(AssociatedObject, ContentPresenter.ContentProperty, binding);
            }
        }
    }
}
