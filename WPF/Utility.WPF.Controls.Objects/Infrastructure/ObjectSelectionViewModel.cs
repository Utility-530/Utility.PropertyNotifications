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
using Utility.Commands;
using static Utility.WPF.Controls.Objects.Infrastructure.ObjectItemsControlViewModel;
using UnitsNet;
using System.ComponentModel;
using Utility.Helpers.Types;
using System.Collections.Generic;

namespace Utility.WPF.Controls.Objects.Infrastructure
{
    public class ObjectSelectionViewModel : BaseViewModel
    {
        private ObservableCollection<Item<ObservableCommand<PropertyInfo>>> items = new();
        private bool isReadOnly;
        private object @object;
        private object value;
        private int level;

        public ObjectSelectionViewModel()
        {

            CompositeDisposable? disposable = null;

            this
                .WhenAnyValue(a => a.Object)
                .WhereNotNull()
                .CombineLatest(this.WhenAnyValue(a => a.IsReadOnly, a => a.Level))
                .Select(a => Build(a.First, a.Second.Item1, a.Second.Item2))
                .Subscribe(items =>
                {
                    this.items.Clear();
                    this.items.AddRange(items);
                    disposable?.Dispose();
                    disposable = new();

                    foreach (var item in items)
                    {
                        item.Subscribe(e =>
                        {
                            //var value = e.GetValue(Object);
                            //Value = value;
                            //foreach (var x in enums)
                            //{
                            //    x.IsChecked = x.Property == e;
                            //}

                        }).DisposeWith(disposable);
                    }
                });



            static Item<Utility.Commands.ObservableCommand<PropertyInfo>>[] Build(object? value, bool isReadOnly, int level)
            {
                return Filter()
                    .Select(e => new Item<Utility.Commands.ObservableCommand<PropertyInfo>>(e, value, new Utility.Commands.ObservableCommand<PropertyInfo>(e), isReadOnly)
                    {
                        //IsChecked = Value?.Equals(e) == true
                    })
                    .ToArray();

                IEnumerable<PropertyInfo> Filter()
                {
                    var properties = value.GetType()
                        //.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(a => a.IsSpecialName == false)
                        .ToDictionary(a => a.Name, a => a);
                    
                    var highestType = value.GetType();

                    foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(value).Cast<PropertyDescriptor>().OrderBy(d => d.Name))
                    {
                        int _level = descriptor.ComponentType.CompareLevel(highestType);

                        if (_level <= level && descriptor.IsBrowsable)
                        {
                            yield return properties[descriptor.Name];
                        }
                    }

                }
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

        public int Level
        {
            get => level;
            set => Set(ref level, value);
        }

        public object Value
        {
            get => value;
            private set => Set(ref this.value, value);
        }



        #endregion properties
    }
}