using System;
using System.Windows.Controls;
using System.Windows.Markup;

/// <summary>
/// xaml in comments visual studio
/// 
/// Examples
/// 
/// <KeyBinding Key="Enter"
///             Command="{Binding ReturnResultCommand}"
///             CommandParameter="{mx:True}" />
///<Button Visibility = "{Binding SomeProperty, Converter={StaticResource SomeBoolConverter}, ConverterParameter ={ mx:True}}">
///    
///< !--This guarantees the value passed is a double equal to 42.5 -->
///<Button Visibility="{Binding SomeProperty,
///    Converter={SomeDoubleConverter},
///    ConverterParameter ={ mx: Double 42.5}}">
/// </summary>
namespace Utility.WPF.Markup
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/4997446/boolean-commandparameter-in-xaml"></a>
    ///</summary>
    public class TrueExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => true;
    }

    public class FalseExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => false;
    }



    public class DoubleExtension : ValueExtension<double>
    {
    }

    public class IntegerExtension : ValueExtension<int>
    {
    }   
    
    public class ValueExtension<T> : MarkupExtension
    {
        public ValueExtension() { }
        public ValueExtension(T value) => Value = value;
        public T Value { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider) => Value;
    }
}
