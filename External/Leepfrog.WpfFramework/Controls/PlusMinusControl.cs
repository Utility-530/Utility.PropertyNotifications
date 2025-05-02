using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Leepfrog.WpfFramework.Controls
{
    [ContentProperty("Content")]
    public class PlusMinusControl : Slider
    {
        static PlusMinusControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlusMinusControl), new FrameworkPropertyMetadata(typeof(PlusMinusControl)));
        }



        [System.ComponentModel.Bindable(true)]
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(PlusMinusControl), new PropertyMetadata(null));




        [System.ComponentModel.Bindable(true)]
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(PlusMinusControl), new PropertyMetadata(null));



        public bool IsWrapping
        {
            get { return (bool)GetValue(IsWrappingProperty); }
            set { SetValue(IsWrappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWrapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWrappingProperty =
            DependencyProperty.Register("IsWrapping", typeof(bool), typeof(PlusMinusControl), new PropertyMetadata(false));



        public PlusMinusControl()
        {
            this.Loaded += PlusMinusControl_Loaded;
        }

        private void PlusMinusControl_Loaded(object sender, RoutedEventArgs e)
        {
            // get the repeat buttons
            var minusButton = GetTemplateChild("PART_Minus") as RepeatButton;
            if (minusButton != null)
            {
                minusButton.Click += Minus_Click;
            }
            var plusButton = GetTemplateChild("PART_Plus") as RepeatButton;
            if (plusButton != null)
            {
                plusButton.Click += Plus_Click;
                this.Loaded -= PlusMinusControl_Loaded;
            }
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            var newValue = Value - SmallChange;
            if (newValue < Minimum)
            {
                newValue = (IsWrapping ? Maximum : Minimum);
            }
            Value = newValue;
        }
        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            var newValue = Value + SmallChange;
            if (newValue > Maximum)
            {
                if (IsWrapping)
                {
                    newValue = Minimum;
                }
                else
                { 
                    newValue = Maximum;
                    OverLimit?.Invoke(this,new EventArgs());
                }
            }
            Value = newValue;
        }

        public event EventHandler OverLimit;

    }
}
