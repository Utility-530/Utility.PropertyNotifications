using DynamicData;
using Evan.Wpf;
using Microsoft.Xaml.Behaviors;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.Commands;
using Utility.Helpers;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;

namespace Utility.WPF.Behaviors
{
    public class EnumComboBehavior : Behavior<ComboBox>
    {
        record EnumType(Type Type, Enum? Value);

        public static readonly DependencyProperty ValueProperty = DependencyHelper.Register<Enum>(new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty EnumProperty = DependencyHelper.Register<Type>(new FrameworkPropertyMetadata());



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
                Value = (Enum)System.Enum.ToObject(Value?.GetType() ?? Enum as Type, 0);
            });


            this.WhenAnyValue(a => a.Enum)
                .WhereNotNull()
                .Select(a => Nullable.GetUnderlyingType(a) is Type type ? type : a)
                .Select(a => new EnumType(a, null))
                .Merge(this.WhenAnyValue(a => a.Value).WhereNotNull().Where(a => a != internalValue).Select(a => new EnumType(a.GetType(), a)))
                .CombineLatest(this.WhenAnyValue(a => a.IsReadOnly))
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

                        item.Command.Subscribe(e =>
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
                                Utility.Helpers.EnumHelper.CombineFlags(enums.Where(a => a.IsChecked).Select(e => e.Value), Enum) :
                                e;

                            }
                            Value = internalValue;
                            AssociatedObject.SelectedIndex = enums.Select(a => a.Value).IndexOf(internalValue);

                        }).DisposeWith(disposable);
                    }
                });

            this.WhenAnyValue(a => a.Enum)
                .WhereNotNull()
                .Subscribe(e =>
                {
                    Value = (Enum)System.Enum.ToObject(e, 0);
                });


            static EnumItem[] BuildFromEnum(Type t, Enum? Value, bool isReadOnly)
            {
                return System.Enum.GetValues(t)
                    .Cast<Enum>()
                    .Select(e => new EnumItem(e, ReactiveCommand.Create(() => e), isReadOnly)
                    {
                        IsChecked = Value?.Equals(e) == true
                    })
                    .ToArray();
            }

            AssociatedObject.WhenAnyValue(a => a.SelectedItem)
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

        public Type Enum
        {
            get => (Type)GetValue(EnumProperty);
            set => SetValue(EnumProperty, value);
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

        public class EnumItem : NotifyPropertyClass, IValue<Enum>, IIsReadOnly
        {
            private bool isChecked;

            public EnumItem(Enum @enum, ReactiveCommand<Unit, Enum> command, bool isReadOnly)
            {
                Value = @enum;
                Command = command;
                //IsReadOnly = isReadOnly;
            }

            public Enum Value { get; }

            public ReactiveCommand<Unit, Enum> Command { get; }

            public bool IsChecked
            {
                get => isChecked; set
                {
                    isChecked = value;
                    this.RaisePropertyChanged();
                }
            }

            public bool IsReadOnly { get; set; }

            object IValue.Value => Value;

            public override string ToString()
            {
                return Value.ToString();
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