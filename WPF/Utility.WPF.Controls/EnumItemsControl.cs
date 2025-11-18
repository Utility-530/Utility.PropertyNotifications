using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Evan.Wpf;
using MintPlayer.ObservableCollection;
using Utility.WPF.Reactives;
using Utility.Commands;
using Utility.Enums;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.WPF.Attached;
using Utility.WPF.Controls.Base;
using Utility.WPF.Helpers;
using Utility.Reactives;

namespace Utility.WPF.Controls
{
    public class EnumItemsControl : LayOutItemsControl
    {
        record EnumType(Type Type, Enum? Value);

        public static readonly DependencyProperty ValueProperty = DependencyHelper.Register<Enum>(new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty EnumProperty = DependencyHelper.Register<Type>(new FrameworkPropertyMetadata());
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyHelper.Register<bool>();
        public static readonly DependencyProperty IsMultiSelectProperty = DependencyHelper.Register<bool>();
        public static readonly DependencyProperty ClearCommandProperty = DependencyHelper.Register<ICommand>();
        //private Subject<Type> subject = new();

        private ObservableCollection<EnumItem> items = new();
        private Enum internalValue;

        static EnumItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumItemsControl), new FrameworkPropertyMetadata(typeof(EnumItemsControl)));

            if (DesignModeHelper.IsInDesignMode)
            {
                EnumProperty.OverrideMetadata(typeof(EnumItemsControl), new FrameworkPropertyMetadata(typeof(Switch)));
            }
        }

        public EnumItemsControl()
        {
            this.SetValue(ItemsControlEx.ArrangementProperty, Utility.Enums.Arrangement.Wrap);
            CompositeDisposable? disposable = null;

            ClearCommand = new Command<object>(a =>
            {
                Value = (Enum)System.Enum.ToObject(Value?.GetType() ?? Enum, 0);
            });

            ItemsSource = items;

            this.Observe(a => a.Enum)
                .WhereIsNotNull()
                .Subscribe(e =>
                {
                    Value = (Enum)System.Enum.ToObject(e, 0);
                });

            this.Observe(a => a.Enum)
                .WhereIsNotNull()
                .Select(a => new EnumType(a, null))
                .Merge(this.Observe(a => a.Value).WhereIsNotNull().Where(a => a != internalValue).Select(a => new EnumType(a.GetType(), a)))
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
                        item.Subscribe(e =>
                        {
                            if (IsMultiSelect == false)
                            {
                                internalValue = e;
                                foreach (var x in enums)
                                {
                                    if (x.Enum != e)
                                        x.IsChecked = false;
                                }
                            }
                            else
                            {
                                internalValue = e.HasFlag(e) ?
                                EnumHelper.CombineFlags(enums.Where(a => a.IsChecked).Select(e => e.Enum), Enum) :
                                e;
                            }
                            Value = internalValue;
                        }).DisposeWith(disposable);
                    }
                });

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
            set => SetValue(EnumProperty, Enum);
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

        public class EnumItem : NotifyPropertyClass, IIsReadOnly, IObservable<Enum>
        {
            private bool isChecked;
            SingleReplaySubject<Enum> replaySubject = new();
            public EnumItem(Enum @enum, bool isReadOnly)
            {
                Enum = @enum;
                Command = new Command(()=> replaySubject.OnNext(@enum));
                IsReadOnly = isReadOnly;
            }

            public Enum Enum { get; }

            public ICommand Command { get; }
            public bool IsReadOnly { get; set; }

            public bool IsChecked
            {
                get => isChecked; set
                {
                    isChecked = value;
                    this.RaisePropertyChanged();
                }
            }

            public IDisposable Subscribe(IObserver<Enum> observer)
            {
                return replaySubject.Subscribe(observer);
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
                throw new ArgumentException($"Unexpected type. Expected {nameof(EnumItemsControl.EnumItem)}");
            }
            return (isReadonly ? ReadOnlyTemplate : InteractiveTemplate) ?? throw new NullReferenceException("fsdeeeee");
        }
    }
}