#nullable enable

using Evan.Wpf;
using Microsoft.Xaml.Behaviors;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Endless;
using Utility.WPF.Behavior;

namespace Utility.WPF.Behavior
{
    public class EnumItemsControlBehavior : Behavior<ItemsControl>
    {
        private CompositeDisposable? disposable = null;
        private ReplaySubject<IEnumerable> itemsSourceSubject = new(1);

        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register("EnumType", typeof(Type), typeof(EnumItemsControlBehavior), new PropertyMetadata(EnumTypeChanged));
        public static readonly DependencyProperty EnumFilterCollectionProperty = DependencyHelper.Register<IEnumerable>();
        public static readonly DependencyProperty IsNullOptionIncludedProperty =
            DependencyProperty.Register("IsNullOptionIncluded", typeof(bool), typeof(EnumItemsControlBehavior), new PropertyMetadata());
        //public static readonly DependencyProperty SelectedEnumProperty =
        //    DependencyProperty.Register("SelectedEnum", typeof(Enum), typeof(EnumSelectorBehavior), new FrameworkPropertyMetadata
        //    {
        //        BindsTwoWayByDefault = true,
        //        DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        //    });

        protected override void OnAttached()
        {
            disposable = new CompositeDisposable();

            itemsSourceSubject
                .Subscribe(a =>
                {
                    AssociatedObject.ItemsSource = a;
                }).DisposeWith(disposable);

            FormatAssociatedObject(AssociatedObject);

            //this
            //    .WhenAnyValue(a => a.SelectedEnum)
            //    .DistinctUntilChanged()
            //    .WhereNotNull()
            //    .CombineLatest(AssociatedObject.WhenAnyValue(a => a.ItemsSource).WhereNotNull())
            //    .Subscribe(c => AssociatedObject.SetValue(Selector.SelectedIndexProperty, Index(c.First, c.Second)))
            //    .DisposeWith(disposable);

            this
                .WhenAnyValue(a => a.EnumFilterCollection)
                .WhereNotNull()
                .Select(ToEnumMembers)
                .Select(AppendNullOptionAndToArray)
                .Subscribe(itemsSourceSubject.OnNext)
                .DisposeWith(disposable);

            //AssociatedObject
            //    .WhenAnyValue(a => a.SelectedValue)
            //    .WhereNotNull()
            //    .DistinctUntilChanged()
            //    .Subscribe(c =>
            //    {
            //        SetValue(SelectedEnumProperty, (Enum?)c);
            //    }).DisposeWith(disposable);

            base.OnAttached();

            //static int Index(Enum? @enum, IEnumerable? enumerable)
            //{
            //    var arr = enumerable?.Cast<EnumMember>().ToArray() ?? throw new Exception("SDDf44 44 h");
            //    var def = arr.SingleOrDefault(a => a.StringValue == @enum?.ToString());
            //    return Array.IndexOf(arr, def);
            //}

            static IEnumerable<EnumMember> ToEnumMembers(IEnumerable enumerable)
            {
                var flags = EnumHelper.Filter(enumerable).ToArray();
                var members = EnumMember.EnumerateEnumMembers(enumerable.GetType().GetElementType() ?? throw new Exception("dsg33  55"));
                var joins = from flag in flags
                            join mem in members on flag equals mem.Value
                            select mem;
                return joins;
            }

            EnumMember[] AppendNullOptionAndToArray(IEnumerable<EnumMember> members)
            {
                if (IsNullOptionIncluded)
                    return members.Concat(new[] { new EnumMember() }).ToArray();
                return members.ToArray();
            }
        }

        protected override void OnDetaching()
        {
            disposable?.Dispose();
            base.OnDetaching();
        }

        #region properties

        public Type EnumType
        {
            get => (Type)GetValue(EnumTypeProperty);
            set => SetValue(EnumTypeProperty, value);
        }

        public IEnumerable EnumFilterCollection
        {
            get => (IEnumerable)GetValue(EnumFilterCollectionProperty);
            set => SetValue(EnumFilterCollectionProperty, value);
        }

        //public Enum SelectedEnum
        //{
        //    get => (Enum)GetValue(SelectedEnumProperty);
        //    set => SetValue(SelectedEnumProperty, value);
        //}

        public bool IsNullOptionIncluded
        {
            get { return (bool)GetValue(IsNullOptionIncludedProperty); }
            set { SetValue(IsNullOptionIncludedProperty, value); }
        }


        #endregion properties

        private static void EnumTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not EnumItemsControlBehavior { EnumType: { } type } behavior) return;

            if (EnumEnumerable(type) is { } enm)
                behavior.itemsSourceSubject.OnNext(enm.Cast<EnumMember>().Concat(new[] { new EnumMember() }));
            else
                throw new ApplicationException("F c44 SDfd");
        }

        private static IEnumerable? EnumEnumerable(Type type)
        {
            MethodInfo? method = typeof(EnumMember).GetMethod(nameof(EnumMember.EnumerateEnumMembers));
            IEnumerable? itemsSource = (IEnumerable?)method?.Invoke(null, new[] { type });
            return itemsSource;
        }

        /// <summary>
        /// Simple attribute class for storing String Values
        /// </summary>
        [AttributeUsage(AttributeTargets.All)]
        public sealed class StringValueAttribute : System.Attribute
        {
            public StringValueAttribute(string value)
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
                Value = value;
            }

            public string Value { get; }
        }

        private static void FormatAssociatedObject(ItemsControl associatedObject)
        {
            //if (associatedObject is ComboBox box)
            //{
            //    box.IsEditable = false;
            //    if (double.IsNaN(associatedObject.Width))
            //        associatedObject.Width = 200;
            //    if (double.IsNaN(associatedObject.Height))
            //        associatedObject.Height = 40;
            //}

            associatedObject.DisplayMemberPath = string.IsNullOrEmpty(associatedObject.DisplayMemberPath) ?
                nameof(EnumMember.Description) :
                associatedObject.DisplayMemberPath;

            //associatedObject.SelectedValuePath = string.IsNullOrEmpty(associatedObject.SelectedValuePath) ?
            //    nameof(EnumMember.Value) :
            //    associatedObject.SelectedValuePath;
            //associatedObject.SelectedValuePath = "Value";
            //associatedObject.SelectedIndex = 0;
            //associatedObject.HorizontalAlignment = HorizontalAlignment.Center;
            //associatedObject.VerticalAlignment = VerticalAlignment.Center;
            //associatedObject.HorizontalContentAlignment = HorizontalAlignment.Center;
            //associatedObject.VerticalContentAlignment = VerticalAlignment.Center;
        }
    }

    //public class EnumMember
    //{
    //    public object Value { get; init; } = default!;

    //    public string Description { get; init; } = null!;

    //    public string StringValue { get; init; } = null!;

    //    public bool IsChecked { get; set; }

    //    /// <exception cref="ArgumentException">T must be of type enumeration.</exception>
    //    public static IEnumerable<EnumMember> EnumerateEnumMembers(Type type)
    //    {
    //        if (!type.IsEnum)
    //        {
    //            throw new ArgumentException("T must be of type enumeration.");
    //        }

    //        foreach (var memberInfo in type.GetMembers(BindingFlags.Public | BindingFlags.Static))
    //        {
    //            var descriptionAttribute = memberInfo.GetCustomAttributes(true).OfType<DescriptionAttribute>().SingleOrDefault();
    //            var attr = memberInfo.GetCustomAttributes(true).OfType<StringValueAttribute>().SingleOrDefault();

    //            var enumMember = new EnumMember
    //            {
    //                Value = Enum.Parse(type, memberInfo.Name),
    //                Description = attr?.Value ?? descriptionAttribute?.Description ?? memberInfo.Name,
    //                StringValue = memberInfo.Name
    //            };

    //            yield return enumMember;
    //        }
    //    }
    //}

    //internal static class EnumHelper
    //{
    //    public static IEnumerable<Enum> Filter(IEnumerable input)
    //    {
    //        var elementType = input.GetType().GetElementType() ?? throw new Exception("SDg4 gfd");
    //        var cached = input.OfType<Enum>().Cached();
    //        foreach (Enum value in Enum.GetValues(elementType))
    //            if (cached.Contains(value))
    //                yield return value;
    //    }

    //    public static IEnumerable<Enum> GetFlags(object input)
    //    {
    //        foreach (Enum value in Enum.GetValues(input.GetType()))
    //            if (((Enum)input).HasFlag(value))
    //                yield return value;
    //    }
    //}
}