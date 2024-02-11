using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Utility.Common;
using Utility.Helpers.NonGeneric;
using Utility.WPF.Controls.Base;

namespace Utility.WPF.Controls
{
    using Utility.WPF.Reactives;
    using static DependencyPropertyFactory<SliderItemsControl>;

    public class SliderItemsControl : Controlx
    {
        private ItemsControl ItemsControl;

        private StackPanel KeyValuePanel;
        private TextBlock at;
        private TextBlock bt;
        private readonly Subject<object> subject = new Subject<object>();

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SliderItemsControl));

        public static readonly DependencyProperty ShowKeyValuePanelProperty = Register(nameof(ShowKeyValuePanel), true);
        public static readonly DependencyProperty KeyValuePairProperty = Register(nameof(KeyValuePair));
        public static readonly DependencyProperty DictionaryProperty = Register(nameof(Dictionary));
        public static readonly DependencyProperty DataProperty = Register(nameof(Data));
        public static readonly DependencyProperty ValueProperty = Register(nameof(Value));
        public static readonly DependencyProperty KeyProperty = Register(nameof(Key));
        public static readonly DependencyProperty MinProperty = Register(nameof(Min));
        public static readonly DependencyProperty MaxProperty = Register(nameof(Max));

        static SliderItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliderItemsControl), new FrameworkPropertyMetadata(typeof(SliderItemsControl)));
        }

        public SliderItemsControl()
        {
            this.Observable(nameof(Data))
        .CombineLatest(this.Observable(nameof(Key)),
          this.Observable(nameof(Value)),
         this.Observable(nameof(Min)).StartWith(new object[] { null }),
     this.Observable(nameof(Max)).StartWith(new object[] { null }),
   (data, key, value, min, max) => new { data, key, value, min, max })
        .Subscribe(async _ =>
        {
            await (GetItems((IEnumerable)_.data, (string)_.key, (string)_.value, (string)_.min, (string)_.max)
            .ContinueWith(dp =>
           this.Dispatcher.InvokeAsync(async () =>
           {
               var xxx = (await dp).ToArray();
               Dictionary = xxx.ToDictionary(_a => _a.Key, _a => _a.Value);
               this.Dispatcher.Invoke(() =>
               {
                   KeyRangeCollection.Clear();
                   foreach (var xx in xxx)
                   {
                       xx.PropertyChanged += Xx_PropertyChanged;
                       KeyRangeCollection.Add(xx);
                   }
               });
           }, DispatcherPriority.Background, default)));
        });

            this.Observable(nameof(ShowKeyValuePanel))
                .Subscribe(b =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        KeyValuePanel.Visibility = ((bool)b) ? Visibility.Visible : Visibility.Collapsed;
                    });
                });
        }

        public ObservableCollection<KeyRange> KeyRangeCollection { get; } = new ObservableCollection<KeyRange>();

        #region properties

        public bool ShowKeyValuePanel
        {
            get { return (bool)GetValue(ShowKeyValuePanelProperty); }
            set { SetValue(ShowKeyValuePanelProperty, value); }
        }

        public object KeyValuePair
        {
            get { return (object)GetValue(KeyValuePairProperty); }
            set { SetValue(KeyValuePairProperty, value); }
        }

        public object Dictionary
        {
            get { return (object)GetValue(DictionaryProperty); }
            set { SetValue(DictionaryProperty, value); }
        }

        public IEnumerable Data
        {
            get { return (IEnumerable)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public string Min
        {
            get { return (string)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public string Max
        {
            get { return (string)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        #endregion properties

        public override void OnApplyTemplate()
        {
            ItemsControl = this.GetTemplateChild("ItemsControl") as ItemsControl;
            ItemsControl.ItemsSource = KeyRangeCollection;
            subject.OnNext(ItemsControl);
            KeyValuePanel = this.GetTemplateChild("KeyValuePanel") as StackPanel;
            at = KeyValuePanel.Children.First() as TextBlock;
            bt = KeyValuePanel.Children.OfType<TextBlock>().Last() as TextBlock;
            ValueChanged += SliderItemsControl_ValueChanged;
        }

        private void SliderItemsControl_ValueChanged(object sender, RoutedEventArgs e)
        {
            KeyValuePairRoutedEventArgs keyValuePairRoutedEventArgs = e as KeyValuePairRoutedEventArgs;
            this.Dispatcher.InvokeAsync(() =>
            {
                at.Text = keyValuePairRoutedEventArgs.KeyValuePair.Key;
                bt.Text = keyValuePairRoutedEventArgs.KeyValuePair.Value.ToString("00.###");
            });
        }

        private Task<IEnumerable<KeyRange>> GetItems(IEnumerable data, string key, string value, string? min, string? max) => Task.Run(() =>
             {
                 var type = data.First().GetType().GetProperties().ToDictionary(a => a.Name, a => a);
                 var keys = Utility.Helpers.PropertyHelper.GetPropertyValues<string>(data, type[key]);
                 var values = data.Cast<object>().Select(_ =>
                 type[value].GetValue(_)).ToList().Select(Convert.ToDouble);
                 var mins = min != null ? (type.TryGetValue(min, out PropertyInfo? outmin) ?
                data.Cast<object>().Select(_ =>
                 type[min].GetValue(_)).ToList().Select(Convert.ToDouble) : null) : null;
                 var maxs = max != null ? (type.TryGetValue(min, out PropertyInfo? outmax)) ?
                   data.Cast<object>().Select(_ =>
                 type[max].GetValue(_)).ToList().Select(Convert.ToDouble) : null : null;

                 var xxx = Factory.Create(keys, values, mins, maxs);

                 return xxx;
             });

        private void Xx_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                var keyValuePair = new KeyValuePair<string, double>((sender as KeyRange).Key, (sender as KeyRange).Value);
                KeyValuePair = keyValuePair;
                Dictionary = ItemsControl.ItemsSource.OfType<KeyRange>().ToDictionary(a => a.Key, a => a.Value);
                RaiseValueChangedEvent(keyValuePair);
            }, DispatcherPriority.Background);
        }

        protected void RaiseValueChangedEvent(KeyValuePair<string, double> KeyValuePair)
        {
            KeyValuePairRoutedEventArgs newEventArgs = new KeyValuePairRoutedEventArgs(SliderItemsControl.ValueChangedEvent) { KeyValuePair = KeyValuePair };
            RaiseEvent(newEventArgs);
        }

        public class KeyValuePairRoutedEventArgs : RoutedEventArgs
        {
            public KeyValuePair<string, double> KeyValuePair { get; set; }

            public KeyValuePairRoutedEventArgs(RoutedEvent @event) : base(@event)
            {
            }
        }

        private class Factory
        {
            public static IEnumerable<KeyRange> Create(IEnumerable<string> keys, IEnumerable<double> values, IEnumerable<double> mins, IEnumerable<double> maxs)
            {
                using (var b = keys.GetEnumerator())
                using (var c = values.GetEnumerator())
                using (var d = mins?.GetEnumerator())
                using (var e = maxs?.GetEnumerator())
                {
                    while (b.MoveNext() && c.MoveNext() && (bool)(d?.MoveNext() ?? true) && (bool)(e?.MoveNext() ?? true))
                    {
                        var ff = new KeyRange(b.Current, c.Current, d?.Current, e?.Current);
                        yield return ff;
                    }
                }
            }
        }
    }

    public class ValueChangedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        protected event Action<KeyValuePair<string, double>> Event;

        public void Execute(object parameter)
        {
            if (parameter == null)
            {
                return;
            }
            var kvp = (parameter as SliderItemsControl.KeyValuePairRoutedEventArgs).KeyValuePair;

            Event.Invoke(kvp);
        }
    }
}