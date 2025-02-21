# nullable enable
using NetFabric.Hyperlinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Tiny.Toolkits;
using Utility.Commands;
using Utility.Helpers;
using Utility.Models;
using Utility.WPF.Controls.Trees;
using static LambdaConverters.ValueConverter;

namespace Utility.WPF.Controls.Objects
{

    public static class Commands
    {
        public static readonly RoutedCommand FooCommand = new RoutedCommand("Foo", typeof(JsonControl));
        public static readonly RoutedCommand UnFooCommand = new RoutedCommand("UnFoo", typeof(JsonControl));
    }

    public class ValueChangedRoutedEventArgs : RoutedEventArgs
    {
        public ValueChangedRoutedEventArgs(JPropertyNewValue jPropertyNewValue, RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
            JPropertyNewValue = jPropertyNewValue;
        }

        public JPropertyNewValue JPropertyNewValue { get; }
    }

    public delegate void ValueChangedRoutedEventHandler(object sender, ValueChangedRoutedEventArgs e);

    /// <summary>
    /// <a href="https://github.com/catsgotmytongue/JsonControls-WPF">JSON controls</a>
    /// </summary>
    public partial class JsonControl : TreeView
    {

        public static readonly DependencyProperty JsonProperty = DependencyProperty.Register(nameof(Json), typeof(string), typeof(JsonControl), new PropertyMetadata(null, Change2));
        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register(nameof(Object), typeof(JToken), typeof(JsonControl), new PropertyMetadata(null, Change));
        public static readonly DependencyProperty ValidationSchemaProperty = DependencyProperty.Register(nameof(ValidationSchema), typeof(JSchema), typeof(JsonControl), new PropertyMetadata(null, Change));
        public static readonly DependencyProperty SchemaProperty = DependencyProperty.Register(nameof(Schema), typeof(Schema), typeof(JsonControl), new PropertyMetadata());
        public static readonly DependencyProperty ChangeValueCommandProperty = DependencyProperty.Register("ChangeValueCommand", typeof(ICommand), typeof(JsonControl), new PropertyMetadata());

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
    name: "ValueChanged",
    routingStrategy: RoutingStrategy.Bubble,
    handlerType: typeof(ValueChangedRoutedEventHandler),
    ownerType: typeof(CustomTreeViewItem));
        private static void Change2(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        private static void Change(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        
        }

        static JsonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(JsonControl), new FrameworkPropertyMetadata(typeof(JsonControl)));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(Commands.FooCommand, OnFoo, OnCanFoo));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(Commands.UnFooCommand, OnUnFoo, OnCanUnFoo));
        }

        public JsonControl()
        {
            ChangeValueCommand = new Command<JPropertyNewValue>((a) =>
            {
                RaiseEvent(new ValueChangedRoutedEventArgs(a, ValueChangedEvent, this));
            });
        }

        private static void OnCanUnFoo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private static void OnUnFoo(object sender, ExecutedRoutedEventArgs e)
        {
            if (((sender as Control).DataContext as JObject).Parent is JArray jArray)
            {
                jArray.Remove(((sender as Control).DataContext as JObject));
            }
            else if ((sender as Control).DataContext is JObject jObject)
            {
                clear(jObject);
            }

            e.Handled = true;
        }


        static void clear(JObject jObject)
        {
            foreach (var x in jObject.Properties())
            {
                switch (((x.Value) as JValue).Type)
                {
                    case JTokenType.String:
                        x.Value = string.Empty;
                        break;
                    case JTokenType.Integer:
                        x.Value = 0;
                        break;
                    case JTokenType.TimeSpan:
                        x.Value = TimeSpan.Zero;
                        break;
                }
            }
        }
        static void OnFoo(object sender, RoutedEventArgs e)
        {

            var jArray = (sender as Control).DataContext as JArray;

            if (((sender as TreeView).SelectedItem as TreeViewItem)?.DataContext is JObject jObject)
            {
                jArray.Add(jObject);
            }
            else
            {
                jArray.Add(jArray[0]);
            }

            clear((jArray.Last as JObject));
            e.Handled = true;
            // here I need to have the instance of MyCustomControl so that I can call myCustCtrl.Foo();
            // Foo(); // <--- problem! can't access this

            static object GetDefault(Type type)
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }
                return null;
            }
        }

        static void OnCanFoo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (sender as Control).DataContext is JArray { Count: > 0 };
            e.Handled = true;
        }

        public event ValueChangedRoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public string Json
        {
            get => (string)GetValue(JsonProperty);
            set => SetValue(JsonProperty, value);
        }

        public JToken Object
        {
            get => (JToken)GetValue(ObjectProperty);
            set => SetValue(ObjectProperty, value);
        }

        public JSchema ValidationSchema
        {
            get { return (JSchema)GetValue(ValidationSchemaProperty); }
            set { SetValue(ValidationSchemaProperty, value); }
        }

        public Schema Schema
        {
            get { return (Schema)GetValue(SchemaProperty); }
            set { SetValue(SchemaProperty, value); }
        }

        public ICommand ChangeValueCommand
        {
            get { return (ICommand)GetValue(ChangeValueCommandProperty); }
            set { SetValue(ChangeValueCommandProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            this.WhenAnyValue(a => a.Object)
                         .WhereNotNull()
                         .Merge(this.WhenAnyValue(a => a.Json).WhereNotNull().Select(convert))
                         .Subscribe(a =>
                         {
                             this.Load(a, this);
                         }, e =>
                         {
                             MessageBox.Show(e.Message);
                         });


            JToken convert(string json)
            {
                //return Newtonsoft.Json.JsonConvert.SerializeObject(e, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, Converters = converters });
                return JToken.Parse(json);
            }

            base.OnApplyTemplate();
        }

        private void Load(JToken jToken, TreeView jsonTreeView)
        {
            jsonTreeView.ItemsSource = null;
            jsonTreeView.Items.Clear();

            try
            {
                //var jToken = JToken.Parse(json);
                if (ValidationSchema != null)
                    if (jToken.IsValid(ValidationSchema) == false)
                    {
                        MessageBox.Show("Schema not valid!");
                    }

                jsonTreeView.ItemsSource = new[] { jToken };//.Children();
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
    }

    public class JsonObjectTypeTemplateSelector : DataTemplateSelector
    {
        Dictionary<string, SchemaProperty> properties = new();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var presenter = (FrameworkElement)container;

            var jsonControl = container.FindParent<JsonControl>();

            //if (presenter.FindParent<TreeView>() is JsonControl jsonControl)
            //{
            //    Schema = jsonControl.Schema;
            //}

            if (container is FrameworkElement frameworkElement)
            {
                if (item is JObject { Type: { } type } jObject)
                {
                    if (type == JTokenType.Object)
                        return frameworkElement.FindResource("ObjectPropertyTemplate") as DataTemplate;
                }
                else if (item is JArray { } jArray)
                {
                    return frameworkElement.FindResource("ArrayPropertyTemplate") as DataTemplate;
                }
                else if (item is JProperty { Value: { Type: { } _type } _value, Parent: var parent } property)
                {
                    if (property.Name == "$type")
                    {
                        return frameworkElement.FindResource("TypeTemplate") as DataTemplate;
                    }
                    if (jsonControl.Schema is not null)


                        if (findProperty(property.Name, parent, jsonControl.Schema, out SchemaProperty schemaProperty))
                        {
                            if (schemaProperty.EnumType != null)
                            {
                                //var etype = Type.GetType(schemaProperty.EnumType);

                                return frameworkElement.FindResource("EnumTemplate") as DataTemplate;
                            }
                            if (frameworkElement.FindResource(schemaProperty.Format) is DataTemplate dataTemplate)
                            {
                                return dataTemplate;
                            }
                        }


                    return frameworkElement.FindResource(Convert(_type)) as DataTemplate;
                }
            }

            if (item is JObject)
                return (DataTemplate)presenter.Resources["RootTemplate"];

            if (item is null)
                return MissingTemplate;

            return presenter.Resources[Extensions.ChooseString(ChooseType(presenter))] as DataTemplate;

            bool findProperty(string name, JContainer? parent, Schema schema, out SchemaProperty? property)
            {
                if (properties.TryGetValue(name, out SchemaProperty? prop))
                {
                    property = prop;
                    return true;

                }
                else if (schema?.Properties.Where(a => a.Name == name).ToArray() is SchemaProperty[] props)
                {
                    foreach (var x in props)
                    {
                        if (x.Type != null)
                            if (type() is string _type && x.Type != _type)
                            {
                                continue;
                            }
                        property = properties[x.Name] = x;
                        return true;
                    }
                }
                property = null;
                return false;

                string type()
                {
                    if (parent is JObject { First: JProperty { Value: { } value } })
                    {
                        return value.ToString();
                    }
                    return null;
                }
            }
        }
        private static string Convert(JTokenType _type)
        {
            return _type switch
            {
                //JTokenType.DateTime => "DateTimeTemplate",
                JTokenType.Date => "DateTemplate",
                JTokenType.TimeSpan => "TimeTemplate",
                //JTokenType.Enum => "EnumTemplate",
                JTokenType.String => "StringTemplate",
                JTokenType.Integer => "IntegerTemplate",
                //JTokenType.Number => "NumberTemplate",
                JTokenType.Boolean => "BooleanTemplate",
                JTokenType.Null => "NullTemplate",
                JTokenType.Array => "ArrayPropertyTemplate",
                JTokenType.Object => "ObjectPropertyTemplate",

                //Type.Null | Type.Enum => "EnumNullTemplate",
                //Type.Null | Type.Integer => "IntegerNullTemplate",
                _ => throw new Exception("Nsdf33 dd")
            };
        }

        private static Extensions.Type ChooseType(FrameworkElement presenter)
        {
            //if (schema.Type.HasFlag(JType.String) && schema.Format == JsonFormatStrings.DateTime) // TODO: What to do with date/time?
            return Extensions.Type.DateTime;
            //else if (schema.Type.HasFlag(JsonObjectType.String) && schema.Format == "date")
            //    return Type.Date;
            //else if (schema.Type.HasFlag(JsonObjectType.String) && schema.Format == "time")
            //    return Type.Time;
            //else if (schema.Type.HasFlag(JsonObjectType.String) && schema.Enumeration.Count > 0)
            //    return Type.Enum;
            //else if (schema.Type.HasFlag(JsonObjectType.String))
            //    return Type.String;
            //else if (schema.Type.HasFlag(JsonObjectType.Integer) && schema.Enumeration.Count > 0)
            //    return Type.Enum;
            //else if (schema.Type.HasFlag(JsonObjectType.Integer))
            //    return Type.Integer;
            //else if (schema.Type.HasFlag(JsonObjectType.Number))
            //    return Type.Number;
            //else if (schema.Type.HasFlag(JsonObjectType.Boolean))
            //    return Type.Boolean;
            //else if (schema.Type.HasFlag(JsonObjectType.Object))
            //    return Type.Object;
            //else if (schema.Type.HasFlag(JsonObjectType.Array))
            //    return Type.Array;
            //else if (schema.Type.HasFlag(JsonObjectType.None))
            //{
            //    var x = schema.OneOf.SingleOrDefault(a => a.Type != JsonObjectType.Null);
            //    var xnull = schema.OneOf.SingleOrDefault(a => a.Type == JsonObjectType.Null);
            //    if (x == null || xnull == null)
            //    {
            //        throw new Exception("Nsdf2 2233 dd");
            //    }
            //    else
            //    {
            //        return Type.Null | ChooseType(presenter, x.ActualSchema);
            //    }
            //}
            //else
            //    throw new Exception("Nsdf33 dd");
        }

        public DataTemplate MissingTemplate { get; set; }
        public JSchema Schema { get; private set; }
    }


    public class Extensions
    {
        public static string ChooseString(Type type)
        {
            return type switch
            {
                Type.DateTime => "DateTimeTemplate",
                Type.Date => "DateTemplate",
                Type.Time => "TimeTemplate",
                Type.Enum => "EnumTemplate",
                Type.String => "StringTemplate",
                Type.Integer => "IntegerTemplate",
                Type.Number => "NumberTemplate",
                Type.Boolean => "BooleanTemplate",
                Type.Object => "ObjectTemplate",
                Type.Array => "ArrayTemplate",
                Type.Null | Type.Enum => "EnumNullTemplate",
                Type.Null | Type.Integer => "IntegerNullTemplate",
                _ => throw new Exception("Nsdf33 dd")
            };
        }

        public static Type Choose(object instance)
        {
            if (instance is string)
                return Type.String;
            else if (instance is DateTime)
                return Type.DateTime;
            else if (instance is Enum)
                return Type.Enum;
            else if (instance is int)
                return Type.Integer;
            else if (instance is bool)
                return Type.Boolean;
            else if (instance is IEnumerable)
                return Type.Array;
            else if (instance is object)
                return Type.Object;
            else
                throw new Exception("Nsdf33 dd");
        }

        public enum Type
        {
            None = 0x0,
            Array = 0x1,
            Boolean = 0x2,
            Integer = 0x4,
            Null = 0x8,
            Number = 0x10,
            Object = 0x20,
            String = 0x40,
            File = 0x80,
            DateTime,
            Date,
            Time,
            Enum

        }
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

    #region converters


    public record JPropertyNewValue(object EventArgs, JProperty JProperty);


    public class EventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new JPropertyNewValue(value, parameter as JProperty);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
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

        public static ComplexPropertyMethodValueConverter Instance { get; } = new();
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
                }
                return new JEnumerable<JToken>();
            });

        }

        public static IValueConverter JArrayLengthConverter { get; } = Create<object, string>(jToken =>
        {
            if (jToken.Value is JToken { Type: JTokenType type } jtoken)
                return type switch
                {
                    JTokenType.Array => $"[{jtoken.Children().Count()}]",
                    JTokenType.Property => $"[ {jtoken.Children().FirstOrDefault()?.Children().Count()} ]",
                    _ => throw new ArgumentOutOfRangeException("Type should be JProperty or JArray"),
                };
            throw new Exception("fsdfdfsd");
        }
        , errorStrategy: LambdaConverters.ConverterErrorStrategy.DoNothing);

        public static IValueConverter JTokenConverter { get; } = Create<object, object>(jval =>
        jval.Value switch
        {
            JValue value when value.Type == JTokenType.Null => "null",
            JValue value => value?.Value,
            _ => jval.Value.ToString() ?? string.Empty
        },
        a =>
        {
            return new JValue(a.Value);
        });
    }

    public class JTokenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JValue { Value: var _value })
            {
                return _value.ToString();
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new JValue(value);
        }

        public static JTokenConverter Instance { get; } = new();
    }




    public class EnumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.SingleOrDefault(a => a is Schema) is Schema schema && values.SingleOrDefault(a => a is JProperty) is JProperty s)
            {
                return new EnumViewModel(schema, s);
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumViewModel : Utility.PropertyNotifications.NotifyPropertyClass
    {
        private readonly Schema schema;
        private readonly JProperty jProperty;
        private Enum value;

        public EnumViewModel(Schema schema, JProperty jProperty)
        {
            //var value = s.Value;
            if (schema.Properties.SingleOrDefault(a => a.Name == jProperty.Name) is SchemaProperty prop)
            {
                var type = Type.GetType(prop.EnumType);
                value = (Enum)Enum.Parse(type, jProperty.Value.Value<string>());
                this.RaisePropertyChanged(nameof(Value));
            }

            this.schema = schema;
            this.jProperty = jProperty;
        }

        public Enum Value
        {
            get => value; set
            {
                this.value = value;
                jProperty.Value = JContainer.FromObject(value.ToString());
            }
        }
    }


    internal class BooleanToNullConverter : IMultiValueConverter
    {
        //public Dictionary<string[], object[]> _value = new();
        private JsonSchema jsonSchema;
        private long value;

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Length == 2)
            {
                if (value[1] is JsonSchema { Type: var type } schema)
                {
                    //return _value[targetType] = value;
                    jsonSchema = schema;
                    if (value[0] != null)
                        this.value = (long)value[0];
                    return value[0] != null;
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            if (value is false)
            {
                return new[] { null, jsonSchema };
            }
            //return _value[targetType[0]];
            return new object[] { this.value, jsonSchema };

        }
    }

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return value is DateTime ? value : DateTime.Parse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("yyyy-MM-dd");
            }
            return null;
        }
    }

    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return value is DateTime ? value : DateTime.Parse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class DecimalUpDownRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return parameter.ToString() == "min" ? decimal.MinValue : decimal.MaxValue;
            return (decimal)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    internal class EnumToNumberConverter : IMultiValueConverter
    {
        private JsonSchema jsonSchema;



        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {

            //if (value is string str)
            //{
            //    return new object[] { jsonSchema.Enumeration.ElementAt(jsonSchema.EnumerationNames.IndexOf(str)), jsonSchema };
            //}


            return new[] { DependencyProperty.UnsetValue };
        }


    }


    public class IndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var list = (IList)values[1];
            if (list != null)
            {
                var index = list.IndexOf(values[0]) + 1;
                return targetType == typeof(string) ? index.ToString() : (object)index;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JValue { Value: var _value })
            {
                return int.Parse(_value.ToString());
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return new JValue(value);
            }
            catch
            {
                return default(int);
            }
        }
    }

    //public class IntegerUpDownRangeConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value == null)
    //            return parameter.ToString() == "min" ? int.MinValue : int.MaxValue;

    //        if (value is decimal)
    //            return (int)(decimal)value;
    //        if (value is double)
    //            return (int)(double)value;

    //        return (int)value;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value;
    //    }
    //}

    public class NullableToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IsNullable(value, value?.GetType()) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        static bool IsNullable<T>(T obj)
        {
            if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        static bool IsNullable(object obj, Type? type = null)
        {
            if (obj == null) return true; // obvious
            if (!(type ??= obj.GetType()).IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NumberConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? value.ToString() : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(decimal))
            {
                try
                {
                    return decimal.Parse(value.ToString());
                }
                catch
                {
                    return default(decimal);
                }
            }
            else
            {
                try
                {
                    return double.Parse(value.ToString());
                }
                catch
                {
                    return default(double);
                }
            }
        }
    }
     
    public class GuidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JValue { Value: var _value })
            {
                return Guid.Parse(_value.ToString());
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return new JValue(value);
            }
            catch
            {
                return default(int);
            }
        }
    }

    public class SchemaNullableToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JsonSchema { Description: "nullable" } schema)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SchemaNullableToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JsonSchema { Description: "nullable" } schema)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return value is TimeSpan ?
                DateTime.MinValue + (TimeSpan)value :
                DateTime.MinValue + TimeSpan.Parse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
                return ((DateTime)value).TimeOfDay;

            return value;
        }
    }


    #endregion converters
}
