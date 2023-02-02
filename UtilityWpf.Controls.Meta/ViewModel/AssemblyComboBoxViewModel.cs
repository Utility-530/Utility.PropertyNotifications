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
using System.Windows.Controls;
using Utility.Common;
using Utility.Common.Model;
using Utility.Enums;
using Utility.Persist;
using Utility.ViewModels.Filters;
using Utility.Interfaces.NonGeneric;
using UtilityWpf.Model;

namespace UtilityWpf.Controls.Meta.ViewModels
{
    public class StringMatchFilter : StringMatchFilter<AssemblyKeyValue>
    {
    }

    public class AssemblyTypeFilter : PropertyFilter<AssemblyKeyValue>
    {
        public AssemblyTypeFilter() : base(nameof(AssemblyKeyValue.CategoryKey))
        {
        }

        protected override object Set(string value)
        {
            return Enum.Parse(typeof(AssemblyType), value);
        }
    }

    internal class AssemblyComboBoxViewModel
    {
        public static readonly DependencyProperty DemoTypeProperty = DependencyHelper.Register();

        public OutputNode<FilteredCustomCheckBoxesViewModel> demoTypeViewModel;
        public FunctionNode<object, object> selectedItemViewModel;
        //public FunctionNode<(CheckedRoutedEventArgs, AssemblyType), Unit> checkedViewModel;

        public AssemblyComboBoxViewModel()
        {
            FreeSqlFactory.InitialiseSQLite();

            demoTypeViewModel = OutputNode<FilteredCustomCheckBoxesViewModel>.Create(() =>
            {
                var assemblies = FindAssemblies().Select(a => new AssemblyKeyValue(a.Item1, a.Item2));
                var filters = new Filter[] { new StringMatchFilter(), new AssemblyTypeFilter() };

                return UpdateAndCreate(assemblies, filters);

                static IObservable<FilteredCustomCheckBoxesViewModel> UpdateAndCreate(IEnumerable<AssemblyKeyValue> assemblies, Filter[] filters)
                {
                    return Update(assemblies.ToArray())
                    .ToObservable()
                    .Select(a =>
                    {
                        var viewModel = new FilteredCustomCheckBoxesViewModel(
                            a.ToObservable().ToObservableChangeSet(),
                            filters.ToObservable().ToObservableChangeSet());
                        return viewModel;
                        //var view = CollectionViewSource.GetDefaultView(array);
                        //view.GroupDescriptions.Clear();
                        //view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Model.KeyValue.GroupKey)));
                    });
                }
            });
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
                    .OfType<AssemblyKeyValue>()
                    .Select(a => UpdateSelected(a))
                    .SelectMany(a => a.ToObservable());

                static async System.Threading.Tasks.Task<AssemblyKeyValue> UpdateSelected(AssemblyKeyValue item)
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

        private static async System.Threading.Tasks.Task<ViewModelEntity[]> Update(AssemblyKeyValue[] collection)
        {
            ViewModelEntity[] array = collection.Select(a => new ViewModelEntity { Key = a.Key }).OrderByDescending(a => BaseEntityOrderer<ViewModelEntity>.Order(a.Key)).ToArray();
            var items = (await ViewModelEntity.Select.ToListAsync()).OrderBy(a => a.UpdateTime == default ? a.CreateTime : a.UpdateTime);

            List<AssemblyKeyValue> list = new();
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

        private class BaseEntityOrderer<T> where T : BaseEntity, IKey
        {
            public static DateTime Order(string key)
            {
                var where = BaseEntity.Orm.Select<T>().Where(a => a.Key == key);
                var match = where.MaxAsync(a => a.UpdateTime);
                if (match.Result == default)
                {
                    return where.MaxAsync(a => a.CreateTime).Result;
                }
                return match.Result;
            }
        }

        public record AssemblyRecord(string Key, DateTime Inserted);

        private const string DemoAppNameAppendage = "Demo";

        //private static IEnumerable<Assembly> FindDemoAppAssemblies()
        //{
        //    return from a in AssemblySingleton.Instance.Assemblies
        //           where a.GetName().Name.Contains(DemoAppNameAppendage)
        //           where a.DefinedTypes.Any(a => a.IsAssignableTo(typeof(UserControl)))
        //           select a;
        //}

        //private static IEnumerable<Assembly> FindResourceDictionaryAssemblies(Predicate<string>? predicate = null)
        //{
        //    return from a in AssemblySingleton.Instance.Assemblies
        //           let resNames = a.GetManifestResourceNames()
        //           where resNames.Length > 0
        //           select a;
        //}

        private static IEnumerable<(Assembly, AssemblyType)> FindAssemblies()
        {
            return from a in AssemblySingleton.Instance.Assemblies
                   let contains = a.GetName().Name?.Contains(DemoAppNameAppendage) ?? false ? AssemblyType.Application : default
                   let userControls = a.DefinedTypes.Any(a => a.IsAssignableTo(typeof(UserControl))) ? AssemblyType.UserControl : default
                   let controls = a.DefinedTypes.Any(a => a.IsAssignableTo(typeof(Control))) ? AssemblyType.Control : default
                   let viewModels = a.DefinedTypes.Any(a => a.IsAssignableTo(typeof(ReactiveObject))) ? AssemblyType.ViewModel : default
                   let resNames = a.GetManifestResourceNames().Length > 0 ? AssemblyType.ResourceDictionary : default
                   select (a, Utility.Helpers.EnumHelper.CombineFlags(new[] { contains, userControls, controls, viewModels, resNames }));
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

        public class ViewModelEntity : BaseEntity<ViewModelEntity, Guid>, IKey
        {
            public string Key { get; init; }

            //public string Group { get; init; }
            //public AssemblyType Category { get; init; }
            public string Value { get; set; }

            public bool IsSelected { get; set; }
            public bool IsChecked { get; set; }
        }
    }
}