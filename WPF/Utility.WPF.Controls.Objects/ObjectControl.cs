using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Endless.Functional;
using Humanizer;
using MoreLinq;
using Splat;
using Utility.Helpers.Reflection;

namespace Utility.WPF.Controls.Objects
{
    using Utility.Helpers;
    using Utility.WPF.Converters;

    public class ObjectControl : ContentControl, IEnableLogger
    {
        private static readonly Brush AccentBrushConstant = (Brush)Application.Current.TryFindResource("PrimaryHueMidBrush") ?? Brushes.CornflowerBlue;

        private readonly ISubject<IValueConverter> converterChanges = new Subject<IValueConverter>();
        private readonly ISubject<IValueConverter> descriptionConverterChanges = new Subject<IValueConverter>();
        private readonly ISubject<IValueConverter> filterChanges = new Subject<IValueConverter>();
        private readonly ISubject<IComparer<string>> comparerChanges = new Subject<IComparer<string>>();
        private readonly ISubject<TextBlock> textBlockChanges = new Subject<TextBlock>();
        private readonly ISubject<Border> borderChanges = new Subject<Border>();
        private readonly ISubject<object> objectChanges = new Subject<object>();
        private readonly ISubject<bool> isTitleColoursInvertedChanges = new Subject<bool>();

        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register("Object", typeof(object), typeof(ObjectControl), new PropertyMetadata(null, ObjectChanged));
        public static readonly DependencyProperty ConverterProperty = DependencyProperty.Register("Converter", typeof(IValueConverter), typeof(ObjectControl), new PropertyMetadata(new DefaultConverter(), ConverterChanged));
        public static readonly DependencyProperty ComparerProperty = DependencyProperty.Register("Comparer", typeof(IComparer<string>), typeof(ObjectControl), new PropertyMetadata(null, ComparerChanged));
        public static readonly DependencyProperty DescriptionConverterProperty = DependencyProperty.Register("DescriptionConverter", typeof(IValueConverter), typeof(ObjectControl), new PropertyMetadata(new DescriptionConverter(), DescriptionConverterChanged));
        public static readonly DependencyProperty AccentBrushProperty = DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(ObjectControl), new PropertyMetadata(AccentBrushConstant));
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(IValueConverter), typeof(ObjectControl), new PropertyMetadata(new DefaultFilter(), FilterChanged));
        public static readonly DependencyProperty IsTitleColoursInvertedProperty = DependencyProperty.Register("IsTitleColoursInverted", typeof(bool), typeof(ObjectControl), new PropertyMetadata(true, IsTitleColoursInvertedChanged));

        static ObjectControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ObjectControl), new FrameworkPropertyMetadata(typeof(ObjectControl)));
        }

        public ObjectControl()
        {
            converterChanges
                .CombineLatest(objectChanges, comparerChanges, textBlockChanges, filterChanges, (valueConverter, value, comparer, textBlock, filter) => (valueConverter, value, comparer, textBlock, filter))
                .Subscribe(valueTuple =>
                {
                    var (valueConverter, value, comparer, textBlock, filter) = valueTuple;

                    Task.Run(() =>
                        value switch
                        {
                            string _ => (Visibility.Collapsed, value),
                            Version _ => (Visibility.Collapsed, value),
                            { } x when x.GetType().IsClass == false => (Visibility.Collapsed, value),
                            null => (Visibility.Collapsed, value),
                            IEnumerable<string> _ => (Visibility.Collapsed, value),
                            IEnumerable enumerable when enumerable.NotOfClassType() => (Visibility.Collapsed, enumerable),
                            IEnumerable enumerable when enumerable.OfSameType() => (Visibility.Collapsed, enumerable),
                            IEnumerable enumerable => DictionaryConverter.ConvertMany(enumerable, valueConverter, filter, comparer)
                                .Pipe(a => (a.Keys.Cast<object>().Any() ? Visibility.Visible : Visibility.Collapsed, a)),
                            _ => DictionaryConverter.Convert(value, valueConverter, filter, comparer)
                                .Pipe(a => (a.Keys.Cast<object>().Any() ? Visibility.Visible : Visibility.Collapsed, a))
                        })
                    .ToObservable()
                    .ObserveOnDispatcher()
                    .Subscribe(a =>
                    {
                        var (visibility, content) = a;
                        textBlock.SetValue(VisibilityProperty, visibility);
                        this.SetValue(ContentProperty, content);
                    }, e =>
                     {
                         SetValue(ContentProperty, new OrderedDictionary(1) { { "Error", e.Message } });
                         this.Log().Write(e, $"Error in {nameof(ObjectControl)} creating content", typeof(ObjectControl), LogLevel.Error);
                     },
                    () => { });
                });

            isTitleColoursInvertedChanges
                .CombineLatest(textBlockChanges, borderChanges, (b, t, br) => (b, t, br))
                .Subscribe(c =>
                {
                    var (b, textBlock, border) = c;
                    textBlock.Foreground = b ? Brushes.White : AccentBrush;
                    border.Background = b ? AccentBrush : Brushes.Transparent;
                });

            descriptionConverterChanges
                .CombineLatest(textBlockChanges, (a, b) => (a, b))
                .Subscribe(d =>
                {
                    var (valueConverter, textBlock) = d;
                    Binding binding = new Binding(nameof(Object))
                    {
                        RelativeSource = RelativeSource.TemplatedParent,
                        Converter = valueConverter
                    };
                    textBlock.SetBinding(TextBlock.TextProperty, binding);
                });
        }

        public override void OnApplyTemplate()
        {
            const string textBlock1 = "MainTextBlock";
            const string border1 = "MainBorder";

            if (GetTemplateChild(textBlock1) is TextBlock textBlock)
                textBlockChanges.OnNext(textBlock);
            else
                throw new Exception("Could not find " + textBlock1);

            if (GetTemplateChild(border1) is Border border)
                borderChanges.OnNext(border);
            else
                throw new Exception("Could not find " + border1);

            if (Object != null)
                objectChanges.OnNext(Object);
            if (Converter != null)
                converterChanges.OnNext(Converter);
            descriptionConverterChanges.OnNext(DescriptionConverter);
            comparerChanges.OnNext(Comparer);
            filterChanges.OnNext(Filter);
            isTitleColoursInvertedChanges.OnNext(IsTitleColoursInverted);
            base.OnApplyTemplate();
        }

        #region properties

        public object Object
        {
            get => GetValue(ObjectProperty);
            set => SetValue(ObjectProperty, value);
        }

        public IValueConverter Converter
        {
            get => (IValueConverter)GetValue(ConverterProperty);
            set => SetValue(ConverterProperty, value);
        }

        public IComparer<string> Comparer
        {
            get => (IComparer<string>)GetValue(ComparerProperty);
            set => SetValue(ComparerProperty, value);
        }

        public Brush AccentBrush
        {
            get => (Brush)GetValue(AccentBrushProperty);
            set => SetValue(AccentBrushProperty, value);
        }

        public IValueConverter DescriptionConverter
        {
            get => (IValueConverter)GetValue(DescriptionConverterProperty);
            set => SetValue(DescriptionConverterProperty, value);
        }

        public IValueConverter Filter
        {
            get => (IValueConverter)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        public bool IsTitleColoursInverted
        {
            get => (bool)GetValue(IsTitleColoursInvertedProperty);
            set => SetValue(IsTitleColoursInvertedProperty, value);
        }

        #endregion properties

        private static void ConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ObjectControl control && e.NewValue is IValueConverter converter)
                control.converterChanges.OnNext(converter);
        }

        private static void DescriptionConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ObjectControl control && e.NewValue is IValueConverter converter)
                control.descriptionConverterChanges.OnNext(converter);
        }

        private static void ObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ObjectControl control)
                control.objectChanges.OnNext(e.NewValue);
        }

        private static void ComparerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ObjectControl control && e.NewValue is IComparer<string> comparer)
                control.comparerChanges.OnNext(comparer);
        }

        private static void FilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ObjectControl control && e.NewValue is IValueConverter converter)
                control.filterChanges.OnNext(converter);
        }

        private static void IsTitleColoursInvertedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ObjectControl control && e.NewValue is bool b)
                control.isTitleColoursInvertedChanges.OnNext(b);
        }

        private class DefaultConverter : IValueConverter
        {
            public DefaultConverter()
            {
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value switch
                {
                    PropertyInfo propertyInfo => propertyInfo?.GetValue(parameter) ?? DependencyProperty.UnsetValue,
                    FieldInfo fieldInfo => fieldInfo.GetValue(parameter) ?? DependencyProperty.UnsetValue,
                    _ => DependencyProperty.UnsetValue
                };
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private class DefaultFilter : IValueConverter
        {
            public DefaultFilter()
            {
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return true;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public class DefaultMemberComparer : IComparer<string>
        {
            public int Compare(string? x, string? y)
            {
                return x != null ? y != null ? string.Compare(x, y, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) : -1 : 1;
            }
        }
    }

    public class ObjectControlDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container) => item switch
            {
                string _ => TextTemplate,
                Version _ => TextTemplate,
                { } x when x.GetType().IsClass == false => TextTemplate,
                null => TextTemplate,
                OrderedDictionary _ => OtherTemplate,
                IEnumerable<string> _ => EnumerableTextTemplate,
                IEnumerable enumerable when enumerable.NotOfClassType() => EnumerableTextTemplate,
                IEnumerable _ => EnumerableObjectTemplate,
                _ => OtherTemplate
            } ?? OtherTemplate ?? new DataTemplate();

        public DataTemplate? TextTemplate { get; set; }

        public DataTemplate? OtherTemplate { get; set; }

        public DataTemplate? EnumerableTextTemplate { get; set; }

        public DataTemplate? EnumerableObjectTemplate { get; set; }

        public static ObjectControlDataTemplateSelector Instance => new ObjectControlDataTemplateSelector();
    }

    public static class DictionaryConverter
    {
        private const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

        public static OrderedDictionary Convert(object value, IValueConverter valueConverter, IValueConverter filterConverter, IComparer<string>? comparer = null)
        {
            var arr = SelectMembers(value.GetType(), comparer).Where(m => (bool)filterConverter.Convert(m, null, value, CultureInfo.CurrentCulture)).ToArray();
            var dict = DictionaryHelper.ToOrderedDictionary(
                arr,
                a => a.Name.Humanize(LetterCasing.Title),
                a => valueConverter.Convert(a, value));

            return dict;
        }

        public static OrderedDictionary ConvertMany(IEnumerable value, IValueConverter valueConverter, IValueConverter filterConverter, IComparer<string>? comparer = null)
        {
            var dict = DictionaryHelper.ToOrderedDictionary(
                value.Cast<object>().SelectMany(obj =>
                {
                    var arr = SelectMembers(obj.GetType(), comparer).Where(m => (bool)filterConverter.Convert(m, null, value, CultureInfo.CurrentCulture)).ToArray();
                    return arr.Select(propertyInfo => (propertyInfo, obj));
                }),
                a => a.propertyInfo.Name.Humanize(LetterCasing.Title),
                a => valueConverter.Convert(a.propertyInfo, a.obj)?.ToString() ?? throw new NullReferenceException("object is null"));

            return dict;
        }

        public static IEnumerable<KeyValuePair<string, OrderedDictionary>> Convert(IEnumerable value, IValueConverter valueConverter, IValueConverter filterConverter)
        {
            foreach (var obj in value.Cast<object>().ToArray())
            {
                var dict = DictionaryHelper.ToOrderedDictionary(
                    SelectMembers(obj.GetType()).Where(m => (bool)filterConverter.Convert(m, null, obj, CultureInfo.CurrentCulture)),
                    a => a.Name.Humanize(LetterCasing.Title),
                    a => valueConverter.Convert(a, obj)?.ToString() ?? throw new NullReferenceException("object is null"));

                yield return KeyValuePair.Create(obj.GetType().Name.Humanize(LetterCasing.Title), dict);
            }
        }

        public static object Convert(this IValueConverter valueConverter, object value, object parameter)
        {
            return valueConverter.Convert(value, default, parameter, default);
        }

        private static MemberInfo[] SelectMembers(IReflect type, IComparer<string>? comparer = null)
        {
            MemberInfo[] members = type.GetFields(bindingFlags).Cast<MemberInfo>()
                .Concat(type.GetProperties(bindingFlags)).ToArray();
            return comparer != null ? members
                       .OrderBy(a => a.Name, comparer).ToArray() :
                members;
        }
    }
}