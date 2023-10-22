using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
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
            var contains = names.Contains(propertyInfo.PropertyInfo.Name);
            return contains;
        }
        return false;
    }
}

public class MyObject
{
    public string String => nameof(MyObject);
    public bool Bool { get; set; }
    public Guid Guid => Guid.NewGuid();
    public double Double => 0.01;
    public int Integer { get; set; } = -101;
    public System.Drawing.Color DrawingColor => System.Drawing.Color.FromName("IndianRed");
    public Color Color => Colors.AliceBlue;
    public Orientation Orientation => Orientation.Horizontal;

    public MyObject Child { get; }
}

public class MyObject2 : MyObject
{
    public long Long { get; }

    public double Double { get; set; }
}