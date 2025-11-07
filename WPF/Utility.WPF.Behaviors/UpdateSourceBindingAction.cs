using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Behaviors
{
    public class UpdateSourceBindingAction : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(FrameworkElement), typeof(UpdateSourceBindingAction), new PropertyMetadata());

        public FrameworkElement Source
        {
            get { return (FrameworkElement)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public DependencyProperty Property { get; set; }

        protected override void Invoke(object parameter)
        {
            (Source ?? AssociatedObject).GetBindingExpression(Property)?.UpdateSource();
        }
    }
}