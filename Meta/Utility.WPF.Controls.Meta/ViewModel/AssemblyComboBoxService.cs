using DynamicData;
using Evan.Wpf;
using FreeSql;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Windows;
using Utility.Common.Model;
using Utility.Persists;
using Utility.Interfaces.NonGeneric;
using Utility.WPF.Meta;
using Utility.Models.Filters;
using Utility.Helpers;
using System.Reactive.Subjects;
using System.Reactive;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TypeSerialization;

namespace Utility.WPF.Controls.Meta.ViewModels
{
    //public class StringMatchFilter : StringMatchFilter<AssemblyKeyValue>
    //{
    //}

    //public class AssemblyTypeFilter : PropertyFilter<AssemblyKeyValue>
    //{
    //    public AssemblyTypeFilter() : base(nameof(AssemblyKeyValue.CategoryKey))
    //    {
    //    }

    //    protected override object Set(string value)
    //    {
    //        return Enum.Parse(typeof(AssemblyType), value);
    //    }
    //}


    public class AssemblyComboBoxService : ComboBoxServcice
    {
        //public FunctionNode<(CheckedRoutedEventArgs, AssemblyType), Unit> checkedViewModel;

        public AssemblyComboBoxService()
        {
            FreeSqlFactory.InitialiseSQLite();

            //demoTypeViewModel = OutputNode<FilteredCustomCheckBoxesViewModel>.Create(() =>
            //{
            //    //var assemblies = Helper.FindAssemblies().Select(a => new AssemblyKeyValue(a.Item1));
            //    //var filters = new Filter[] { new StringMatchFilter(), /*new AssemblyTypeFilter()*/ };

            //    return UpdateAndCreate(assemblies, filters, deselectSubject);


            //});
        }
    }

    public class TypeComboBoxService : ComboBoxServcice
    {
        public TypeComboBoxService()
        {
            FreeSqlFactory.InitialiseSQLite();

            demoTypeViewModel = OutputNode<FilteredCustomCheckBoxesViewModel>.Create(() =>
            {
                var assemblies = Helper.Types(Assembly.GetEntryAssembly());
                var filters = new Filter[] { };
                return UpdateAndCreate(assemblies, filters, deselectSubject);
            });


        }
    }



    public class ComboBoxServcice
    {
        protected Subject<Unit> deselectSubject = new();
        public static readonly DependencyProperty DemoTypeProperty = DependencyHelper.Register();

        public FunctionNode<object, object> selectedItemViewModel;

        public OutputNode<FilteredCustomCheckBoxesViewModel> demoTypeViewModel;

        public ComboBoxServcice()
        {
            FreeSqlFactory.InitialiseSQLite();


            //.Select(demoType =>
            //{
            //    return demoType switch
            //    {
            //        AssemblyType.UserControl => (demoType, collection: FindDemoAppAssemblies()),
            //        AssemblyType.ResourceDictionary => (demoType, collection: FindResourceDictionaryAssemblies()),
            //        _ => throw new Exception("££!!!!$$4"),
            //    };
            //})

            selectedItemViewModel = FunctionNode<object, object>.Create(@in =>
            {
                return @in
                    .OfType<KeyValue>()
                    .Select(a => UpdateSelected(a))
                    .SelectMany(a => a.ToObservable());

                static async System.Threading.Tasks.Task<KeyValue> UpdateSelected(KeyValue item)
                {
                    if (item.Key != null)
                    {
                        var match = await ViewModelEntity.Where(a => a.Key == item.Key).FirstAsync();
                        if (match == null)
                        {
                            //repo.Add(new AssemblyRecord(item.Key, DateTime.Now));
                            var viewModelEntity = new ViewModelEntity { Key = item.Key, IsChecked = true };
                            await viewModelEntity.InsertAsync();
                            SelectAndUpdateOtherSelections(viewModelEntity);
                        }
                        else if (match.IsSelected != true)
                        {
                            SelectAndUpdateOtherSelections(match);
                        }
                    }
                    return item;
                }
            });
        }

        public void Deselect()
        {
            deselectSubject.OnNext(new Unit());
        }


        protected static IObservable<FilteredCustomCheckBoxesViewModel> UpdateAndCreate(IEnumerable<KeyValue> assemblies, Filter[] filters, IObservable<Unit> observable)
        {
            return Update(assemblies.ToArray())
            .ToObservable()
            .Select(entities =>
            {
                var viewModel = new FilteredCustomCheckBoxesViewModel(
                    entities.ToObservable().ToObservableChangeSet(),
                    filters.ToObservable().ToObservableChangeSet(),
                    observable);
                return viewModel;
                //var view = CollectionViewSource.GetDefaultView(array);
                //view.GroupDescriptions.Clear();
                //view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Model.KeyValue.GroupKey)));
            });
        }

        private static async System.Threading.Tasks.Task<ViewModelEntity[]> Update(KeyValue[] collection)
        {
            ViewModelEntity[] array = collection
                .Select(a => new ViewModelEntity { Key = a.Key, Value = (a.Value as Type)?.AsString() ?? "No Value" , IsChecked = true})
                .OrderByDescending(a => BaseEntityOrderer<ViewModelEntity>.Order(a.Key))
                .ToArray();
            var items = (await ViewModelEntity.Select.ToListAsync()).OrderBy(a => a.UpdateTime == default ? a.CreateTime : a.UpdateTime);

            List<KeyValue> list = new();
            foreach (var item in items)
                for (int i = 0; i < collection.Length; i++)
                {
                    if (collection[i].Key == item.Key)
                    {
                        //collection[i].IsSelected = item.IsSelected;
                        //collection[i].IsChecked = item.IsChecked;
                        //collection[i].Value = item.Value;
                        list.Add(collection[i]);
                    }
                }

            return array;
        }

        //private static AssemblyKeyValue[] ToKeyValues(IEnumerable<Assembly> a)
        //{
        //    return a
        //    .Select(a => new AssemblyKeyValue(a))
        //    .Where(a => a.Key != null)
        //    .OrderByDescending(a => BaseEntityOrderer<AssemblyEntity>.Order(a.Key))
        //    .ToArray();
        //}

        private class BaseEntityOrderer<T> where T : BaseEntity, IEquatable
        {
            public static DateTime Order(string key)
            {
                var where = BaseEntity.Orm.Select<T>().Where(a => a.Equals(key));
                var match = where.MaxAsync(a => a.UpdateTime);
                if (match.Result == default)
                {
                    return where.MaxAsync(a => a.CreateTime).Result;
                }
                return match.Result;
            }
        }

        private static async void SelectAndUpdateOtherSelections(ViewModelEntity match)
        {
            var matches = await ViewModelEntity.Where(a => a.IsSelected).ToListAsync();
            foreach (var match2 in matches)
            {
                if (match2 != match)
                {
                    match2.IsSelected = false;
                    await match2.UpdateAsync();
                }
            }
            match.IsSelected = true;
            await match.UpdateAsync();

            var count = await ViewModelEntity.WhereIf(true, a => a.IsSelected).CountAsync();
            if (count != 1)
            {
                throw new Exception("Expected count to be 1 since only item can be selected in any given moment");
            }
        }
    }

    public class ViewModelEntity : BaseEntity<ViewModelEntity, Guid>, IEquatable, IGetKey, INotifyPropertyChanged
    {
        private bool isSelected;
        private bool isChecked;

        public string Key { get; init; }

        public string Value { get; set; }

        public bool IsSelected { get => isSelected; set => Set(ref isSelected, value); }
        public bool IsChecked { get => isChecked; set => Set(ref isChecked, value); }

        public bool Equals(IEquatable? other)
        {
            return (other as IKey).Equals(this);
        }

        /// <inheritdoc />
        /// <summary>
        ///     The event on property changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///     Raise the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The caller member name of the property (auto-set)</param>
        //[NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Set a property and raise the <see cref="PropertyChanged" /> event
        /// </summary>
        /// <typeparam name="T">The type of the Property</typeparam>
        /// <param name="field">A reference to the backing field from the property</param>
        /// <param name="value">The new value being set</param>
        /// <param name="callerName">The caller member name of the property (auto-set)</param>
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string? callerName = default)
        {
            if (field?.Equals(value) != true)
            {
                field = value;
                OnPropertyChanged(callerName);
                return true;
            }
            return false;
        }
    }
}