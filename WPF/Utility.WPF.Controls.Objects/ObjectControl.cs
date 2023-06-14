using Endless.Functional;
using Humanizer;
using MoreLinq;
using Splat;
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

namespace Utility.WPF.Controls.Objects
{
    using global::Utility.WPF.Converters;
    using Utility;

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
                    PropertyInfo propertyInfo => propertyInfo.GetValue(parameter) ?? DependencyProperty.UnsetValue,
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

    namespace Utility
    {
        public static class TypeHelper
        {
            public static bool IsDerivedFrom<T>(this Type type)
            {
                return typeof(T).IsAssignableFrom(type);
            }

            public static IEnumerable<Type> GetTypesFromTheSameAssembly(this IEnumerable<Type> types, Predicate<Type>? predicate = default)
            {
                return types.SelectMany(a => a.Assembly.GetTypes()).Where(a => predicate?.Invoke(a) ?? true);
            }

            public static bool IsNumericType(this Type o)
            {
                return Type.GetTypeCode(o) switch
                {
                    TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Int16 or
                    TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
                    _ => false,
                };
            }

            /// <summary>
            /// Checks whether all types in an enumerable are the same.
            /// </summary>
            /// <param name="enumerable"></param>
            /// <returns></returns>
            public static bool OfSameType(this IEnumerable enumerable) => OfSameType(enumerable, out Type _);

            /// <summary>
            /// Checks whether all types in an enumerable are the same.
            /// </summary>
            /// <param name="enumerable"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public static bool OfSameType(this IEnumerable enumerable, out Type type)
            {
                var (t, sameType) =
                    enumerable
                        .Cast<object>()
                        .Select(a => a.GetType())
                        .AggregateUntil(
                            (type: default(Type), sameType: true),
                            (a, b) => (b, a.type == null || a.type == b),
                            a => !a.sameType);
                type = t!;
                return sameType;
            }

            public static bool OfClassType(this IEnumerable enumerable) =>
                enumerable
                    .Cast<object>()
                    .Select(a => a.GetType())
                    .All(a => a.IsClass);

            public static bool NotOfClassType(this IEnumerable enumerable) =>
                enumerable
                    .Cast<object>()
                    .Select(a => a.GetType())
                    .All(a => a.IsClass == false);

            public static bool IsDerivedFromGenericType(this Type type, Type interfaceType)
            {
                return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
            }
        }

        public static class DictionaryHelper
        {
            /// <summary>
            /// Get a the value for a key. If the key does not exist, creates a new one
            /// </summary>
            /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
            /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
            /// <param name="dic">The dictionary to call this method on.</param>
            /// <param name="key">The key to look up.</param>
            /// <returns>The key value. null if this key is not in the dictionary.</returns>
            public static TValue GetValueOrNew<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key) where TValue : new()
            {
                return dic[key] = dic.TryGetValue(key, out TValue? result) ? result : new TValue();
            }

            /// <summary>
            /// Get a the value for a key. If the key does not exist, creates a new one
            /// </summary>
            /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
            /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
            /// <param name="dic">The dictionary to call this method on.</param>
            /// <param name="key">The key to look up.</param>
            /// <returns>The key value. null if this key is not in the dictionary.</returns>
            public static TValue GetValueOrNew<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value) where TKey : notnull
            {
                return dic[key] = dic.TryGetValue(key, out TValue? result) ? result : value;
            }

            public static IDictionary<K, V> AddRange<K, V>(this IDictionary<K, V> me, params IDictionary<K, V>[] others)
            {
                foreach (IDictionary<K, V> src in others)
                {
                    foreach (KeyValuePair<K, V> p in src)
                    {
                        //if(typeof(IDictionary).IsAssignableFrom(typeof(V)e()))
                        me[p.Key] = p.Value;
                    }
                }
                return me;
            }

            /// <summary>
            /// Unionise two dictionaries of generic types.
            /// Duplicates take their value from the leftmost dictionary.
            /// </summary>
            /// <typeparam name="T1">Generic key type</typeparam>
            /// <typeparam name="T2">Generic value type</typeparam>
            /// <param name="D1">System.Collections.Generic.Dictionary 1</param>
            /// <param name="D2">System.Collections.Generic.Dictionary 2</param>
            /// <returns>The combined dictionaries.</returns>
            public static Dictionary<T1, T2> UnionDictionaries<T1, T2>(Dictionary<T1, T2> D1, Dictionary<T1, T2> D2) where T1 : notnull
            {
                Dictionary<T1, T2> rd = new Dictionary<T1, T2>(D1);
                foreach (var key in D2.Keys)
                {
                    if (!rd.ContainsKey(key))
                        rd.Add(key, D2[key]);
                    else if (rd[key]!.GetType().IsGenericType)
                    {
                        if (rd[key]!.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            MethodInfo? info = MethodBase.GetCurrentMethod() is MethodInfo info1 ? info1 : typeof(DictionaryHelper).GetMethod("UnionDictionaries", BindingFlags.Public | BindingFlags.Static);
                            var genericMethod = info?.MakeGenericMethod(rd[key]!.GetType().GetGenericArguments()[0], rd[key]!.GetType().GetGenericArguments()[1]);
                            var invocationResult = genericMethod?.Invoke(null, new object[] { rd[key]!, D2[key]! });
                            rd[key] = (T2)(invocationResult ?? throw new NullReferenceException("Invovation result in UnionDictionaries is null"));
                        }
                    }
                }
                return rd;
            }

            public static Dictionary<T, R> ToDictionary<T, R>(this IEnumerable<KeyValuePair<T, R>> kvps) where T : notnull
            {
                return kvps.ToDictionary(a => a.Key, a => a.Value);
            }

            public static Dictionary<T, R> ToDictionary<T, R>(this IEnumerable<Tuple<T, R>> kvps) where T : notnull
            {
                return kvps.ToDictionary(a => a.Item1, a => a.Item2);
            }

            public static IEnumerable<dynamic> ToDynamics<T>(this IList<Dictionary<string, T>> dics) where T : notnull
            {
                return dics.Select(_ =>
                {
                    return _.ToDynamic();
                });
            }

            public static dynamic ToDynamic<T>(this Dictionary<string, T> dict) where T : notnull
            {
                IDictionary<string, object> eo = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
                foreach (KeyValuePair<string, T> kvp in dict)
                {
                    eo.Add(new KeyValuePair<string, object>(kvp.Key, kvp.Value));
                }
                return eo;
            }

            public static OrderedDictionary ToOrderedDictionary<TSource>(IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, object> elementSelector)
            {
                var d = new OrderedDictionary();

                foreach (TSource element in source)
                {
                    var val = elementSelector(element);
                    if (val != null)
                    {
                        if (d.Contains(keySelector(element)))
                        {
                            var key = keySelector(element);
                            int i = 0;
                            while (d.Contains(key))
                            {
                                key = keySelector(element) + i;
                            }
                            d.Add(key, val);
                        }
                        else
                            d.Add(keySelector(element), val);
                    }
                }

                return d;
            }
        }
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

    public static class LinqExtension
    {
        public static IEnumerable<(T, T)> LeftOuterJoin<T, R>(this IEnumerable<T> firsts, IEnumerable<T> seconds, Func<T, R> equality)
            => from first in firsts
               join second in seconds on equality(first) equals equality(second) into temp
               from a in temp.DefaultIfEmpty()
               select (first, a);

        public static IEnumerable<(T, R)> LeftOuterJoin<T, R, S>(this IEnumerable<T> firsts, IEnumerable<R> seconds, Func<T, S> equalityOne, Func<R, S> equalityTwo)
            => from first in firsts
               join second in seconds on equalityOne(first) equals equalityTwo(second) into temp
               from a in temp.DefaultIfEmpty()
               select (first, a);

        public static IEnumerable<(T, T)> RightOuterJoin<T, R>(this IEnumerable<T> firsts, IEnumerable<T> seconds, Func<T, R> equality)
            => from second in seconds
               join first in firsts on equality(second) equals equality(first) into temp
               from a in temp.DefaultIfEmpty()
               select (a, second);

        public static IEnumerable<(T, R)> RightOuterJoin<T, R, S>(this IEnumerable<T> firsts, IEnumerable<R> seconds, Func<T, S> equalityOne, Func<R, S> equalityTwo)
            => from second in seconds
               join first in firsts on equalityTwo(second) equals equalityOne(first) into temp
               from a in temp.DefaultIfEmpty()
               select (a, second);

        public static IEnumerable<(T, T)> FullOuterJoin<T, R>(this IEnumerable<T> firsts, IEnumerable<T> seconds, Func<T, R> equality)
            => LeftOuterJoin(firsts, seconds, equality).Concat(RightOuterJoin(firsts, seconds, equality));

        public static IEnumerable<(T, R)> FullOuterJoin<T, R, S>(this IEnumerable<T> firsts, IEnumerable<R> lasts, Func<T, S> keySelector1, Func<R, S> keySelector2)
            => LeftOuterJoin(firsts, lasts, keySelector1, keySelector2).Concat(RightOuterJoin(firsts, lasts, keySelector1, keySelector2));

        /// <summary>
        /// Selects the differences between values in <see cref="sequence"/>
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static IEnumerable<double> SelectDifferences(this IEnumerable<double> sequence)
        {
            using (var e = sequence.GetEnumerator())
            {
                e.MoveNext();
                double last = e.Current;
                while (e.MoveNext())
                    yield return e.Current - last;
            }
        }

        /// <summary>
        /// Selects all items in <see cref="first"/> that are not in <see cref="second"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> SelectFromFirstNotInSecond<T>(IEnumerable<T> first, IEnumerable<T> second)
            => from n in first
               join n2 in second
               on n equals n2 into temp
               where temp.Count() == 0
               select n;

        /// <summary>
        /// Selects all items in <see cref="first"/> that are not in <see cref="second"/> using keySelectors
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> SelectFromFirstNotInSecond<T, R, S>(IEnumerable<T> first, IEnumerable<S> second, Func<T, R> keySelectorFirst, Func<S, R> keySelectorSecond)
            => from n in first
               join n2 in second
               on keySelectorFirst(n) equals keySelectorSecond(n2) into temp
               where temp.Count() == 0
               select n;

        /// <summary>
        /// Selects all items in <see cref="first"/> that are not in <see cref="second"/> using keySelectors
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> FilterFirstNotInSecond<T, R>(IEnumerable<T> first, IEnumerable<R> second, Func<T, R> keySelectorFirst)

                => from n in first
                   join n2 in second
                   on keySelectorFirst(n) equals n2 into temp
                   where temp.Count() == 0
                   select n;

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => enumerable.Any() == false;

        /// <summary>
        /// Aggregate <see cref="enumerable"/> based on <see cref="func"/> until a condition,<see cref="predicate"/>, is met.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="seed"></param>
        /// <param name="func"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T1 AggregateUntil<T1, T2>(this IEnumerable<T2> enumerable,
            T1 seed,
            Func<T1, T2, T1> func,
            Func<T1, bool> predicate)
        {
            return enumerable
                .Scan(seed, func)
                .TakeWhile(a => predicate(a) != true)
                .Last();
        }
    }
}