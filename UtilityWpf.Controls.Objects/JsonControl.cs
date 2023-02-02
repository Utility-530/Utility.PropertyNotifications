# nullable enable

using Newtonsoft.Json.Linq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Utility.Helpers;
using static LambdaConverters.TemplateSelector;
using static LambdaConverters.ValueConverter;

namespace UtilityWpf.Controls.Objects
{
    public static class Commands
    {
        public static readonly RoutedCommand FooCommand = new RoutedCommand("Foo", typeof(JsonControl));
    }

    /// <summary>
    /// <a href="https://github.com/catsgotmytongue/JsonControls-WPF">JSON controls</a>
    /// </summary>
    public partial class JsonControl : TreeView
    {
        private readonly ReplaySubject<TreeView> subject = new(1);
        private readonly ReplaySubject<bool> toggleItems = new(1);
        private const GeneratorStatus Generated = GeneratorStatus.ContainersGenerated;

        public static readonly DependencyProperty JsonProperty = DependencyProperty.Register(nameof(Json), typeof(string), typeof(JsonControl), new PropertyMetadata(null, Change2));

        private static void Change2(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register(nameof(Object), typeof(object), typeof(JsonControl), new PropertyMetadata(null, Change));

        private static void Change(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        static JsonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(JsonControl), new FrameworkPropertyMetadata(typeof(JsonControl)));
        }

        public JsonControl()
        {
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(Commands.FooCommand, OnFoo, OnCanFoo));

            this.WhenAnyValue(a => a.Object)
                .WhereNotNull()
                .Select(e => Newtonsoft.Json.JsonConvert.SerializeObject(e))
                .Merge(this.WhenAnyValue(a => a.Json).WhereNotNull())
                .CombineLatest(subject)
                .Subscribe(a =>
                {
                    this.Load(a.First, a.Second);
                }, e =>
                {
                });

            toggleItems
                .CombineLatest(subject)
                .Subscribe(a =>
                {
                    ToggleItems(a.First, a.Second);
                });

            static void OnFoo(object sender, RoutedEventArgs e)
            {
                // here I need to have the instance of MyCustomControl so that I can call myCustCtrl.Foo();
                // Foo(); // <--- problem! can't access this
            }

            static void OnCanFoo(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = true;
                e.Handled = true;
            }
        }

        public string Json
        {
            get => (string)GetValue(JsonProperty);
            set => SetValue(JsonProperty, value);
        }

        public object Object
        {
            get => (object)GetValue(ObjectProperty);
            set => SetValue(ObjectProperty, value);
        }

        public override void OnApplyTemplate()
        {
            subject.OnNext(this);
            base.OnApplyTemplate();
        }

        private void Load(string json, TreeView jsonTreeView)
        {
            jsonTreeView.ItemsSource = null;
            jsonTreeView.Items.Clear();

            try
            {
                jsonTreeView.ItemsSource = JToken.Parse(json).Children();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open the JSON string:\r\n" + ex.Message);
            }
        }

        private void JValue_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && e.ClickCount == 2)
            {
                Clipboard.SetText(tb.Text);
            }
        }

        private void ExpandAll(object sender, RoutedEventArgs e)
        {
            toggleItems.OnNext(true);
        }

        private void CollapseAll(object sender, RoutedEventArgs e)
        {
            toggleItems.OnNext(false);
        }

        private void ToggleItems(bool isExpanded, TreeView jsonTreeView)
        {
            if (jsonTreeView.Items.IsEmpty)
                return;

            var prevCursor = Cursor;
            //System.Windows.Controls.DockPanel.Opacity = 0.2;
            //System.Windows.Controls.DockPanel.IsEnabled = false;
            Cursor = Cursors.Wait;

            var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Normal, (s, e) =>
            {
                ToggleItems(jsonTreeView, jsonTreeView.Items, isExpanded);
                //System.Windows.Controls.DockPanel.Opacity = 1.0;
                //System.Windows.Controls.DockPanel.IsEnabled = true;
                (s as DispatcherTimer)?.Stop();
                Cursor = prevCursor;
            }, Application.Current.Dispatcher);
            timer.Start();

            static void ToggleItems(ItemsControl parentContainer, ItemCollection items, bool isExpanded)
            {
                var itemGen = parentContainer.ItemContainerGenerator;
                if (itemGen.Status == Generated)
                {
                    Recurse(items, isExpanded, itemGen);
                }
                else
                {
                    itemGen.StatusChanged += delegate
                    {
                        Recurse(items, isExpanded, itemGen);
                    };
                }

                static void Recurse(ItemCollection items, bool isExpanded, ItemContainerGenerator itemGen)
                {
                    if (itemGen.Status != Generated)
                        return;

                    foreach (var item in items)
                    {
                        var tvi = itemGen.ContainerFromItem(item) as TreeViewItem;
                        tvi.IsExpanded = isExpanded;
                        ToggleItems(tvi, tvi.Items, isExpanded);
                    }
                }
            }
        }
    }

    public class ComplexPropertyMethodValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Converters.MethodToValueConverter.Convert(value, null, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static ComplexPropertyMethodValueConverter Instance { get; } = new ComplexPropertyMethodValueConverter();
    }

    internal static class Converters
    {
        private static readonly Lazy<Dictionary<int, Color>> NiceColors = new Lazy<Dictionary<int, Color>>(() =>
                                 ColorStore.Collection
                                 .Select((a, i) => Tuple.Create(i, (Color)ColorConverter.ConvertFromString(a.Value)))
                                 .ToDictionary(a => a.Item1, a => a.Item2));

        public static IValueConverter JTokenTypeToColorConverter { get; } = Create<JTokenType, Color>(a => NiceColors.Value[(byte)a.Value]);

        public static IValueConverter MethodToValueConverter { get; } = Create<object, JEnumerable<JToken>?, string>(a =>
                      {
                          if (a.Parameter != null && a.Value?.GetType().GetMethod(a.Parameter, Array.Empty<Type>()) is MethodInfo methodInfo)
                              return (JEnumerable<JToken>?)methodInfo.Invoke(a.Value, Array.Empty<object>());
                          return new JEnumerable<JToken>();
                      });

        public static IValueConverter JArrayConverter { get; } = GetJArrayConverter();
        public static IValueConverter GetJArrayConverter()
        {
            List<JToken> collection = new();
            return Create<object, IEnumerable<JToken>?>(a =>
            {
                if (a.Value is JArray jarray)
                {
                    collection.AddRange(jarray);
                    return collection;
                } return new JEnumerable<JToken>();
            });

        }

        //public static IValueConverter ComplexPropertyMethodToValueConverter => Create<object, JEnumerable<JToken>?, string>(args =>
        //(JEnumerable<JToken>)MethodToValueConverter.Convert(args.Value, null, args.Parameter, args.Culture));

        public static IValueConverter JArrayLengthConverter { get; } = Create<object, string>(jToken =>
                      {
                          if (jToken.Value is JToken { Type: JTokenType type } jtoken)
                              return type switch
                              {
                                  JTokenType.Array => $"[{jtoken.Children().Count()}]",
                                  JTokenType.Property => $"[ { jtoken.Children().FirstOrDefault()?.Children().Count()} ]",
                                  _ => throw new ArgumentOutOfRangeException("Type should be JProperty or JArray"),
                              };
                          throw new Exception("fsdfdfsd");
                      }
        , errorStrategy: LambdaConverters.ConverterErrorStrategy.DoNothing);

        public static IValueConverter JTokenConverter { get; } = Create<object, string>(jval => jval.Value switch
                      {
                          JValue value when value.Type == JTokenType.Null => "null",
                          JValue value => value?.Value?.ToString() ?? string.Empty,
                          _ => jval.Value.ToString() ?? string.Empty
                      });
    }

    internal static class TemplateSelector
    {
        private static readonly Dictionary<string, object> resource = new();

        public static DataTemplateSelector JPropertyDataTemplateSelector =
            Create<object>(
                e =>
                    (e.Item, e.Container) switch
                    {
                        (JProperty property, FrameworkElement frameworkElement) => property.Value.Type switch
                        {
                            JTokenType.Object => resource.GetValueOrNew("ObjectPropertyTemplate", frameworkElement.FindResource("ObjectPropertyTemplate")),
                            JTokenType.Array => resource.GetValueOrNew("ArrayPropertyTemplate", frameworkElement.FindResource("ArrayPropertyTemplate")),
                            _ => resource.GetValueOrNew("PrimitivePropertyTemplate", frameworkElement.FindResource("PrimitivePropertyTemplate")),
                        },
                        (JObject jObject, FrameworkElement frameworkElement) => resource.GetValueOrNew("ObjectPropertyTemplate", frameworkElement.FindResource("ObjectPropertyTemplate")),
                        (_, FrameworkElement frameworkElement) =>
                        frameworkElement.FindResource(new DataTemplateKey(e.Item.GetType())),
                        _ => null
                    } as DataTemplate);
    }

    internal static class ColorStore
    {
        public static readonly Dictionary<string, string> Collection = new()
        {
            { "navy", "#001F3F" },
            { "blue", "#0074D9" },
            { "aqua", "#7FDBFF" },
            { "teal", "#39CCCC" },
            { "olive", "#3D9970" },
            { "green", "#2ECC40" },
            { "d", "#d59aea" },
            { "yellow", "#FFDC00" },
            { "black", "#111111" },
            { "red", "#FF4136" },
            { "fuchsia", "#F012BE" },
            { "purple", "#B10DC9" },
            { "maroon", "#85144B" },
            { "gray", "#AAAAAA" },
            { "silver", "#DDDDDD" },
            { "orange", "#FF851B" },
            { "a", "#ff035c" },
            { "b", "#9eb4cc" },
            { "c", "#fbead3" },
        };
    }
}