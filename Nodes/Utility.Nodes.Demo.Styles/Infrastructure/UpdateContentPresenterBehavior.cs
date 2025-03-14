using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Utility.Nodes.Demo.Styles
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/8715315/how-to-trigger-datatemplateselector-when-property-changes"></a>
    /// </summary>
    public class UpdateContentPresenterBehavior : Behavior<ContentPresenter>
    {

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(UpdateContentPresenterBehavior), new FrameworkPropertyMetadata(null, OnValueChanged));

        public static readonly DependencyProperty UpdateOnNullProperty = DependencyProperty.Register("UpdateOnNull", typeof(bool), typeof(UpdateContentPresenterBehavior), new PropertyMetadata(false));


        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public bool UpdateOnNull
        {
            get { return (bool)GetValue(UpdateOnNullProperty); }
            set { SetValue(UpdateOnNullProperty, value); }
        }

        static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UpdateContentPresenterBehavior behavior /*&& e.NewValue.GetType() != behavior.Type*/)
                behavior.Update();
        }

        public UpdateContentPresenterBehavior() : base() { }

        protected override void OnAttached()
        {
            base.OnAttached();
            Update();
        }

        void Update()
        {
            if (AssociatedObject != null && (UpdateOnNull || Value != null))
            {
                var binding = BindingOperations.GetBinding(AssociatedObject, ContentPresenter.ContentProperty);
                BindingOperations.ClearBinding(AssociatedObject, ContentPresenter.ContentProperty);
                AssociatedObject.Content = null;
                BindingOperations.SetBinding(AssociatedObject, ContentPresenter.ContentProperty, binding);
            }
        }
    }  
}
