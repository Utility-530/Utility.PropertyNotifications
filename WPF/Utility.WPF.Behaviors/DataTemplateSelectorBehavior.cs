#nullable enable

using Evan.Wpf;
using Microsoft.Xaml.Behaviors;
using ReactiveUI;
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Utility.WPF.Helpers;

namespace Utility.WPF.Behaviors
{
    /// <summary>
    /// Assigns available <see cref="DataTemplate"/>s as the <see cref="ItemsControl.ItemsSource"/>
    /// of the <see cref="Microsoft.Xaml.Behaviors.Behavior.AssociatedObject"/>
    /// </summary>
    public class DataTemplateSelectorBehavior : Behavior<Selector>
    {
        private CompositeDisposable? disposable = null;
        private readonly ReplaySubject<IEnumerable> itemsSourceSubject = new(1);

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(DataTemplateSelectorBehavior));
        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register("Object", typeof(object), typeof(DataTemplateSelectorBehavior));

        //public static readonly DependencyProperty ResourceDictionaryProperty = DependencyProperty.Register("ResourceDictionary", typeof(ResourceDictionary), typeof(DataTemplateSelectorBehavior), new PropertyMetadata(ResourceDictionaryChanged));
        public static readonly DependencyProperty DataTemplateFilterCollectionProperty = DependencyHelper.Register<IEnumerable>();

        public static readonly DependencyProperty SelectedDataTemplateProperty =
            DependencyProperty.Register("SelectedDataTemplate", typeof(DataTemplate), typeof(DataTemplateSelectorBehavior), new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        protected override void OnAttached()
        {
            disposable = new CompositeDisposable();

            FormatAssociatedObject(AssociatedObject);

            _ = AssociatedObject
                .WhenAnyValue(a => a.SelectedItem)
                .DistinctUntilChanged()
                .WhereNotNull()
                .CombineLatest(AssociatedObject.WhenAnyValue(a => a.ItemsSource).WhereNotNull())
                .Subscribe(c =>
                {
                    var (dicEntry, enumerable) = c;
                    var arr = enumerable.Cast<DictionaryEntry>().ToArray();
                    var def = arr.SingleOrDefault(dt => dt.Key == ((DictionaryEntry)dicEntry).Key);
                    var index = Array.IndexOf(arr, def);
                    AssociatedObject.SetValue(Selector.SelectedIndexProperty, index);
                })
                .DisposeWith(disposable);

            _ = AssociatedObject
                .WhenAnyValue(a => a.SelectedValue)
                .WhereNotNull()
                .DistinctUntilChanged()
                .Subscribe(c =>
                {
                    SetValue(SelectedDataTemplateProperty, (DataTemplate?)c);
                })
                .DisposeWith(disposable);

            _ = itemsSourceSubject
                .Subscribe(a =>
                {
                    AssociatedObject.ItemsSource = a;
                })
                .DisposeWith(disposable);

            _ = this.WhenAnyValue(a => a.Type)
                .Merge(this.WhenAnyValue(a => a.Object).Select(a => a?.GetType()))
                .WhereNotNull()
                .Select(type =>
                {
                    return type.DefaultDataTemplates().Concat(type.CustomDataTemplates(this.AssociatedObject.Resources)).ToArray();
                })
                .Subscribe(dts => this.itemsSourceSubject.OnNext(dts))
                .DisposeWith(disposable);

            base.OnAttached();

            static void FormatAssociatedObject(Selector associatedObject)
            {
                if (associatedObject is ComboBox box)
                {
                    box.IsEditable = false;
                    if (double.IsNaN(associatedObject.Width))
                        associatedObject.Width = 200;
                    if (double.IsNaN(associatedObject.Height))
                        associatedObject.Height = 40;
                }

                associatedObject.DisplayMemberPath = string.IsNullOrEmpty(associatedObject.DisplayMemberPath) ?
                    nameof(DictionaryEntry.Key) :
                    associatedObject.DisplayMemberPath;

                associatedObject.SelectedValuePath = string.IsNullOrEmpty(associatedObject.SelectedValuePath) ?
                    nameof(DictionaryEntry.Value) :
                    associatedObject.SelectedValuePath;

                associatedObject.SelectedValuePath = "Value";
                associatedObject.SelectedIndex = 0;
                associatedObject.HorizontalAlignment = HorizontalAlignment.Center;
                associatedObject.VerticalAlignment = VerticalAlignment.Center;
                associatedObject.HorizontalContentAlignment = HorizontalAlignment.Center;
                associatedObject.VerticalContentAlignment = VerticalAlignment.Center;
            }
        }

        protected override void OnDetaching()
        {
            disposable?.Dispose();
            base.OnDetaching();
        }

        #region properties

        public Type Type
        {
            get => (Type)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public object Object
        {
            get => GetValue(ObjectProperty);
            set => SetValue(ObjectProperty, value);
        }

        //public ResourceDictionary ResourceDictionary
        //{
        //    get => (ResourceDictionary)GetValue(ResourceDictionaryProperty);
        //    set => SetValue(ResourceDictionaryProperty, value);
        //}

        public DataTemplate SelectedDataTemplate
        {
            get => (DataTemplate)GetValue(SelectedDataTemplateProperty);
            set => SetValue(SelectedDataTemplateProperty, value);
        }

        #endregion properties

    }
}