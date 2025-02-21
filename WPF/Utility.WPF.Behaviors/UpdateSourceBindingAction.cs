using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            (Source ?? AssociatedObject).GetBindingExpression(Property).UpdateSource();
        }

    }
}
