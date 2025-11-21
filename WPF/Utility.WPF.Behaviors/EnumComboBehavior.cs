using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Evan.Wpf;
using Microsoft.Xaml.Behaviors;
using Utility.Commands;
using Utility.Helpers.Generic;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.Generic;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.WPF.Reactives;

namespace Utility.WPF.Behaviors
{
    public class EnumComboBehavior : Behavior<ComboBox>
    {
        record Enum_Type(Type Type, Enum? Value);

        public static readonly DependencyProperty ValueProperty = DependencyHelper.Register<Enum>(new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty EnumTypeProperty = DependencyHelper.Register<Type>(new FrameworkPropertyMetadata());

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyHelper.Register<bool>();
        public static readonly DependencyProperty IsMultiSelectProperty = DependencyHelper.Register<bool>();
        public static readonly DependencyProperty ClearCommandProperty = DependencyHelper.Register<ICommand>();
        private ObservableCollection<EnumItem> items = new();
        private Enum internalValue;

        static EnumComboBehavior()
        {
        }

        protected override void OnAttached()
        {
            AssociatedObject.ItemsSource = items;
            //AssociatedObject.DisplayMemberPath = nameof(EnumItem.Value);

            CompositeDisposable? disposable = null;

            ClearCommand = new Command<object>(a =>
            {
                Value = (Enum)System.Enum.ToObject(Value?.GetType() ?? EnumType as Type, 0);
            });


            this.Observe(a => a.EnumType)
                .WhereIsNotNull()
                .Select(a => Nullable.GetUnderlyingType(a) is Type type ? type : a)
                .Select(a => new Enum_Type(a, null))
                .Merge(this.Observe(a => a.Value).Where(a => a != internalValue).WhereIsNotNull().Select(a => new Enum_Type(a.GetType(), a)))
                .DistinctUntilChanged(a => a.Type.ToString())
                .CombineLatest(this.Observe(a => a.IsReadOnly))
                .Select(a => BuildFromEnum(a.First.Type, a.First.Value, a.Second))
                .Subscribe(enums =>
                {
                    items.Clear();
                    items.AddRange(enums);

                    disposable?.Dispose();
                    disposable = new();

                    foreach (var item in enums)
                    {
                        if (item.IsChecked)
                            AssociatedObject.SelectedIndex = enums.IndexOf(item);

                        item.Subscribe(e =>
                        {
                            if (IsMultiSelect == false)
                            {
                                internalValue = e;
                                foreach (var x in enums)
                                {
                                    if (x.Value != e)
                                        x.IsChecked = false;
                                }
                            }
                            else
                            {
                                internalValue = e.HasFlag(e) ?
                                Utility.Helpers.EnumHelper.CombineFlags(enums.Where(a => a.IsChecked).Select(e => e.Value), EnumType) :
                                e;
                            }
                            Value = internalValue;
                            AssociatedObject.SelectedIndex = enums.Select(a => a.Value).IndexOf(internalValue);
                        }).DisposeWith(disposable);
                    }
                });

            if (Value == default)
                if (AssociatedObject.IsLoaded)
                {
                    Value = (Enum)System.Enum.ToObject(EnumType, 0);
                }
                else
                {
                    this.Observe(a => a.EnumType)
                        .WhereIsNotNull()
                        .Subscribe(e =>
                        {
                            Value = (Enum)System.Enum.ToObject(e, 0);
                        });
                }

            static EnumItem[] BuildFromEnum(Type t, Enum? Value, bool isReadOnly)
            {
                return System.Enum.GetValues(t)
                    .Cast<Enum>()
                    .Select(e => new EnumItem(e, isReadOnly)
                    {
                        IsChecked = Value?.Equals(e) == true
                    })
                    .ToArray();
            }

            AssociatedObject.Observe(a => a.SelectedItem)
                .Subscribe(a =>
                {
                    if (a is EnumItem enumItem)
                        this.Value = enumItem.Value;
                });

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        public EnumComboBehavior()
        {
        }

        #region properties

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public bool IsMultiSelect
        {
            get => (bool)GetValue(IsMultiSelectProperty);
            set => SetValue(IsMultiSelectProperty, value);
        }

        public Type EnumType
        {
            get => (Type)GetValue(EnumTypeProperty);
            set => SetValue(EnumTypeProperty, value);
        }

        public Enum Value
        {
            get => (Enum)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public ICommand ClearCommand
        {
            get => (ICommand)GetValue(ClearCommandProperty);
            set => SetValue(ClearCommandProperty, value);
        }

        #endregion properties

        public class EnumItem : NotifyPropertyClass, IGetValue<Enum>, IIsReadOnly, IObservable<Enum>
        {
            private bool isChecked;
            ReplaySubject<Enum> replay = new(1);
            public EnumItem(Enum @enum, bool isReadOnly)
            {
                Value = @enum;
                Command = new Command(() => replay.OnNext(@enum));
                //IsReadOnly = isReadOnly;
            }

            public Enum Value { get; }

            object IGetValue.Value => Value;

            public ICommand Command { get; }

            public bool IsChecked
            {
                get => isChecked; set
                {
                    isChecked = value;
                    this.RaisePropertyChanged();
                }
            }

            public bool IsReadOnly { get; set; }

            public override string ToString()
            {
                return Value.ToString();
            }

            public IDisposable Subscribe(IObserver<Enum> observer)
            {
                return replay.Subscribe(observer);
            }
        }
    }

    public class EnumItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ReadOnlyTemplate { get; set; }

        public DataTemplate? InteractiveTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not IIsReadOnly { IsReadOnly: bool isReadonly })
            {
                //throw new ArgumentException($"Unexpected type. Expected {nameof(EnumItemsControl.EnumItem)}");
                throw new ArgumentException($"Unexpected type");
            }
            return (isReadonly ? ReadOnlyTemplate : InteractiveTemplate) ?? throw new NullReferenceException("fsdeeeee");
        }
    }
}