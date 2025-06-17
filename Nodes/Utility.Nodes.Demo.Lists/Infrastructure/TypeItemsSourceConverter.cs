using CsvHelper;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.WPF;
using Utility.WPF.Controls.Lists;

namespace Utility.Nodes.Demo.Lists
{
    internal class TypeItemsSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Utility.Models.Trees.ModelType { Type: string stype })
            {
                object? instance = createCollectionInstance(stype);
                subscribe(instance);
                return instance ?? throw new NullReferenceException("vsd ee3434");
            }
            return DependencyProperty.UnsetValue;

            static object? createCollectionInstance(string stype)
            {
                var type = Utility.Helpers.Reflection.TypeHelper.FromString(stype);
                var constructedListType = typeof(ObservableCollection<>).MakeGenericType(type);
                var instance = Activator.CreateInstance(constructedListType);
                return instance;
            }
            static void subscribe(object? instance)
            {
                typeof(Utility.Persists.DatabaseHelper)
                    .GetMethod(nameof(Utility.Persists.DatabaseHelper.ToManager))
                    .MakeGenericMethod(instance.GetType())
                    .Invoke(null, parameters: [instance, new Func<object, Guid>(a => (a as IId<Guid>).Id), null]);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    internal class TypeObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Utility.Models.Trees.ModelType { Type: string stype })
            {
                return createTask(stype);
            }
            return DependencyProperty.UnsetValue;

            static async Task<object> createTask(string stype)
            {
                var type = Utility.Helpers.Reflection.TypeHelper.FromString(stype);
                var instance = Locator.Current.GetService<IFactory<IId<Guid>>>();
                var c = await instance.Create(type);
                return new ObjectWrapper() { Object = c };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //internal class TypeObjectUndefinedConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is Utility.Models.Trees.ModelType { Type: string stype })
    //        {
    //            Task task = createTask(stype);
    //            var vm = new ValueModel<object>() { Name = "sdfsdf" };
    //            task.ContinueWith(a =>
    //            {
    //                var result = task.GetType().GetProperty("Result").GetValue(task);
    //                vm.Set(result);
    //            });
    //            task.Start();
    //            return vm;
    //        }
    //        return DependencyProperty.UnsetValue;

    //        static Task createTask(string stype)
    //        {
    //            var type = Utility.Helpers.Reflection.TypeHelper.FromString(stype);

    //            var constructedFactoryType = typeof(IFactory<>).MakeGenericType(type);
    //            var instance = Locator.Current.GetService(constructedFactoryType);
    //            return subscribe(instance, constructedFactoryType);
    //        }
    //        static Task subscribe(object? instance, Type type)
    //        {
    //            var x = (Task)type
    //                .GetMethod(nameof(IFactory<>.Create))
    //                .Invoke(instance, [null]);
    //            return x;
    //        }
    //    }




    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
