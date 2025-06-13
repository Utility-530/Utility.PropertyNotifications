using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Utility.Interfaces.Generic.Data;
using Utility.Models.Trees;

namespace Utility.Nodes.Demo.Lists
{
    internal class TypeItemsSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ModelType { Type: string stype })
            {
                object? instance = CreateCollectionInstance(stype);
                subscribe(instance);
                return instance ?? throw new NullReferenceException("vsd ee3434");
            }
            return DependencyProperty.UnsetValue;

            static object? CreateCollectionInstance(string stype)
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
}
