using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using Utility.Interfaces.Methods;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using N = Nodify;
namespace Utility.Nodify.Views.Infrastructure
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IGetData { Data: MethodInfo methodInfo})
            {
                return null;
            }
            if (value is IGetData { Data: IMethod  })
            {
                return null;
            }
            if(value is ITreeIndex { Index:{ } index } )
            {
                return new N.Panels.Index([.. index]);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
