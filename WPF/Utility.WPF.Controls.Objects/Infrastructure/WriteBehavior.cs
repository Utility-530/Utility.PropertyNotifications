using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using Newtonsoft.Json.Linq;

namespace Utility.WPF.Controls.Objects
{
    public class WriteBehavior : Behavior<JsonControl>
    {
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(WriteBehavior), new PropertyMetadata());

        private static void changed2(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ObjectBehavior @object && e.NewValue is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    //if (item is INotifyCollectionChanged inpc)
                    //{
                    //    inpc.CollectionChanged -= (s, ev) =>
                    //    {
                    //        @object._playSubject.OnNext(@object.AssociatedObject);
                    //    };
                    //    inpc.CollectionChanged += (s, ev) =>
                    //    {
                    //        Application.Current.Dispatcher.Invoke(() =>
                    //        {
                    //            @object._playSubject.OnNext(@object.AssociatedObject);
                    //        });

                    //    };
                    //}
                }
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == nameof(Collections))
            {
            }
            base.OnPropertyChanged(e);
        }

        static WriteBehavior()
        {
        }

        protected override void OnAttached()
        {
            AssociatedObject.ValueChanged += AssociatedObject_ValueChanged;
            base.OnAttached();
        }

        protected void AssociatedObject_ValueChanged(object sender, ValueChangedRoutedEventArgs e)
        {
            var token = AssociatedObject.ItemsSource.Cast<JToken>().First();
            if (token["$type"] is { } s && Type.GetType(s.ToString()) is Type type)
            {
                var parsed = token.ToString();
                var source = Source;
                Task.Run(() =>
            {
                System.IO.File.WriteAllText(source, parsed);
            });
            }
        }
    }
}