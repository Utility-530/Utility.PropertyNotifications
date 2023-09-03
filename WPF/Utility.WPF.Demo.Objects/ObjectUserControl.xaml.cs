using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Filters;

namespace Utility.WPF.Demo.Objects;

/// <summary>
/// Interaction logic for EnumUserControl.xaml
/// </summary>
public partial class ObjectUserControl : UserControl
{
    public ObjectUserControl()
    {
        InitializeComponent();
    }


}
class TypeFilter : IPredicate
{
    string[] names = new[] { "Name", "FullName" };
    public bool Invoke(object value)
    {
        if (value is IPropertyInfo propertyInfo)
        {
            var contains = names.Contains(propertyInfo.Property.Name);
            return contains;
        }
        return false;
    }
}

public class MyObject
{
    public string Name => nameof(MyObject);

    public MyObject Child { get; }
}