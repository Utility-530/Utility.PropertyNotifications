using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Attached;
using Utility.Enums;
using Utility.WPF.Controls.Base;
using Evan.Wpf;
using Jellyfish;
using Utility.Helpers;
using System.Windows.Input;
using System.Reactive.Subjects;
using LanguageExt.TypeClasses;
using MintPlayer.ObservableCollection;

namespace UtilityWpf.Controls
{
    public class EnumItemsControl : LayOutItemsControl
    {
        private static readonly DependencyPropertyKey OutputPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Output), typeof(Enum), typeof(EnumItemsControl), new FrameworkPropertyMetadata(default(Enum)));
        public static readonly DependencyProperty EnumProperty = DependencyHelper.Register<Type>(new FrameworkPropertyMetadata(typeof(Switch)));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyHelper.Register<bool>();
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsMultiSelectProperty = DependencyHelper.Register<bool>();
        public static readonly DependencyProperty ClearCommandProperty = DependencyHelper.Register<ICommand>();
        private Subject<Type> subject = new();
        private ObservableCollection<EnumItem> items = new();
        static EnumItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumItemsControl), new FrameworkPropertyMetadata(typeof(EnumItemsControl)));
        }

        public EnumItemsControl()
        {
            this.SetValue(ItemsControlEx.ArrangementProperty, Arrangement.Wrapped);
            CompositeDisposable? disposable = null;
            ClearCommand = new RelayCommand(a => subject.OnNext(Enum));
            ItemsSource = items;
            this.WhenAnyValue(a => a.Enum)
                .Merge(subject)
                .CombineLatest(this.WhenAnyValue(a => a.IsReadOnly))
                .Select(a => BuildFromEnum(a.First, a.Second).ToArray())
                .Subscribe(enums =>
                {
                    items.Clear();
                    items.AddRange(enums);
                    items.AddRange(enums);
                    disposable?.Dispose();
                    disposable = new();
           
                    foreach (var item in enums)
                    {
                        item.Command.Subscribe(a =>
                        {

                            if (IsMultiSelect == false)
                            {
                                Output = a;
                                foreach (var x in enums)
                                {
                                    if (x.Enum != a)
                                        x.IsChecked = false;
                                }
                            }
                            else
                            {
                                Output = a.HasFlag(a) ?
                                EnumHelper.CombineFlags(enums.Where(a => a.IsChecked).Select(e => e.Enum), Enum) :
                                a;
                            }

                        }).DisposeWith(disposable);
                    }
                });

            static IEnumerable<EnumItem> BuildFromEnum(Type t, bool isReadOnly)
            {
                return System.Enum.GetValues(t).Cast<Enum>().Select(a => new EnumItem(a, ReactiveCommand.Create(() => a), isReadOnly));
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

        public Enum Output
        {
            get => (Enum)GetValue(OutputProperty);
            protected set => SetValue(OutputPropertyKey, value);
        }
            
        public ICommand ClearCommand
        {
            get => (ICommand)GetValue(ClearCommandProperty);
            set => SetValue(ClearCommandProperty, value);
        }

        #endregion properties

        public class EnumItem : ViewModel
        {
            private bool isChecked;

            public EnumItem(Enum @enum, ReactiveCommand<Unit, Enum> command, bool isReadOnly)
            {
                Enum = @enum;
                Command = command;
                IsReadOnly = isReadOnly;
            }

            public Enum Enum { get; }

            public ReactiveCommand<Unit, Enum> Command { get; }

            public bool IsReadOnly { get; }
            public bool IsChecked
            {
                get => isChecked; set
                {
                    isChecked = value;
                    this.OnPropertyChanged();
                }
            }
        }
    }

    public class EnumItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ReadOnlyTemplate { get; set; }

        public DataTemplate? InteractiveTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not EnumItemsControl.EnumItem { IsReadOnly: bool isReadonly })
            {
                throw new ArgumentException($"Unexpected type. Expected {nameof(EnumItemsControl.EnumItem)}");
            }
            return (isReadonly ? ReadOnlyTemplate : InteractiveTemplate) ?? throw new NullReferenceException("fsdeeeee");
        }
    }
}