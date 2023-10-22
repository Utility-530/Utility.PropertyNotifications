using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Helpers;
using System.Windows.Input;
using MintPlayer.ObservableCollection;
using NetFabric.Hyperlinq;
using System.Reflection;
using Utility.Models;
using System.Collections;
using Utility.Interfaces.NonGeneric;

namespace Utility.WPF.Controls.Objects.Infrastructure
{
    public class ObjectItemsControlViewModel : BaseViewModel
    {
        private ObservableCollection<Item<Utility.Commands.ObservableCommand<PropertyInfo>>> items = new();
        private bool isReadOnly;
        private object @object;
        private object value;

        public ObjectItemsControlViewModel()
        {

            CompositeDisposable? disposable = null;


            this
                .WhenAnyValue(a => a.Object)
                .WhereNotNull()
                .CombineLatest(this.WhenAnyValue(a => a.IsReadOnly))
                .Select(a => Build(a.First, a.Second))
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
                            var value = e.GetValue(Object);
                            Value = value;
                            foreach (var x in enums)
                            {
                                x.IsChecked = x.PropertyInfo == e;
                            }

                        }).DisposeWith(disposable);
                    }
                });

            static Item<Utility.Commands.ObservableCommand<PropertyInfo>>[] Build(object? value, bool isReadOnly)
            {
                var properties = value.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(a => a.IsSpecialName == false)
                    .ToArray();

                return properties
                    .Select(e => new Item<Utility.Commands.ObservableCommand<PropertyInfo>>(e, value, new Utility.Commands.ObservableCommand<PropertyInfo>(e), isReadOnly)
                    {
                        //IsChecked = Value?.Equals(e) == true
                    })
                    .ToArray();
            }
        }

        #region properties

        public IEnumerable Items => items;

        public bool IsReadOnly
        {
            get => isReadOnly;
            set => Set(ref isReadOnly, value);
        }

        public object Object
        {
            get => @object;
            set => Set(ref @object, value);
        }

        public object Value
        {
            get => value;
            private set => Set(ref this.value, value);
        }

        #endregion properties

        public class Item<T> : ValueViewModel, IObservable<PropertyInfo>, IPropertyInfo where T : IObservable<PropertyInfo>, ICommand
        {
            private bool isChecked;
            private readonly PropertyInfo info;
            private readonly object @object;
            private T command;
            private object? value;

            public Item(PropertyInfo info, object @object, T command, bool isReadOnly)
            {
                this.info = info;
                this.@object = @object;
                this.command = command;
                IsReadOnly = isReadOnly;

                try
                {
                    value = info.GetValue(@object) ?? (info.PropertyType.GetConstructor(Type.EmptyTypes) != null ? Activator.CreateInstance(info.PropertyType) : null);

                }
                catch (Exception ex)
                {
                    value = ex.Message;
                }
            }

            public PropertyInfo PropertyInfo => info;

            //public string Name => info.Name;
            public override object? Value => value;

            public ICommand Command => command;

            public bool IsChecked
            {
                get => isChecked; set
                {
                    isChecked = value;
                    OnPropertyChanged();
                }
            }

            public IDisposable Subscribe(IObserver<PropertyInfo> observer)
            {
                return command.Subscribe(observer);
            }
        }
    }
}