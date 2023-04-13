using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls
{
    public class TypeControl : HeaderedContentControl
    {
        public delegate void ChangedEventHandler(object sender, ChangedEventArgs e);

        public class ChangedEventArgs : RoutedEventArgs
        {
            public ChangedEventArgs(Type type, string property, string? value)
            {
                Type = type;
                Property = property;
                Value = value;
            }

            public ChangedEventArgs(RoutedEvent routedEvent, Type type, string property, string? value) : base(routedEvent)
            {
                Type = type;
                Property = property;
                Value = value;
            }

            public ChangedEventArgs(RoutedEvent routedEvent, object source, Type type, string property, string? value) : base(routedEvent, source)
            {
                Type = type;
                Property = property;
                Value = value;
            }

            public string? Value { get; }
            public string Property { get; }
            public Type Type { get; }
        }

        private ComboBox comboBox;
        private Button buttonClear;
        private TextBox textBox = new();
        public static readonly DependencyProperty ShowValueProperty = DependencyProperty.Register("ShowValue", typeof(bool), typeof(TypeControl), new PropertyMetadata(true));
        public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register("Property", typeof(string), typeof(TypeControl), new PropertyMetadata(null));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(TypeControl), new PropertyMetadata(null));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(TypeControl), new PropertyMetadata(null));
        public static readonly RoutedEvent ChangedEvent = EventManager.RegisterRoutedEvent("Changed", RoutingStrategy.Bubble, typeof(ChangedEventHandler), typeof(TypeControl));

        static TypeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TypeControl), new FrameworkPropertyMetadata(typeof(TypeControl)));
        }

        public TypeControl()
        {
        }

        #region properties

        public bool ShowValue
        {
            get { return (bool)GetValue(ShowValueProperty); }
            set { SetValue(ShowValueProperty, value); }
        }

        public string Property
        {
            get { return (string)GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public event ChangedEventHandler Changed
        {
            add { AddHandler(ChangedEvent, value); }
            remove { RemoveHandler(ChangedEvent, value); }
        }

        #endregion properties

        public override void OnApplyTemplate()
        {
            comboBox = this.GetTemplateChild("cmbProperty") as ComboBox ?? throw new Exception("4fgd55  dd");
            //buttonFilter = this.GetTemplateChild("buttonAppy") as Button;
            buttonClear = this.GetTemplateChild("btnClear") as Button ?? throw new Exception("4e efgd55  dd"); ;
            textBox = this.GetTemplateChild("textBox") as TextBox ?? throw new Exception("yyy4fgd55  dd"); ;
            if (ShowValue == false)
                textBox.Visibility = Visibility.Collapsed;
            textBox.SelectionChanged += (a, e) => { Value = textBox.Text; RaiseChangedEvent(); };
            comboBox.SelectionChanged += (a, e) => { Property = e.AddedItems.Cast<string>().First(); RaiseChangedEvent(); };
            //buttonFilter.Click += ButtonFilter_Click;
            buttonClear.Click += ButtonClear_Click;
            comboBox.ItemsSource = Type.GetProperties().Select(a => a.Name);
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            Value = textBox.Text = string.Empty;
            RaiseChangedEvent();
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            RaiseChangedEvent();
        }

        private void RaiseChangedEvent()
        {
            RoutedEventArgs newEventArgs = new ChangedEventArgs(ChangedEvent, this, Type, Property, Value);
            RaiseEvent(newEventArgs);
        }
    }
}