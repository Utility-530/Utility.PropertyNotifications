using System;
using System.Windows.Markup;

namespace Utility.WPF.Markup
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/54092789/datatemplates-and-generics/54124755#54124755"></a>
    /// </summary>
    public class GenericTypesExtension : MarkupExtension
    {
        public GenericTypesExtension(string baseType, params Type[] innerTypes)
        {
            //That’s because XAML’s type parser doesn’t recognize the CLR backtick notation (like List\1, Dictionary`2, etc.).
            //It only supports **non-generic type names** — or **generic classes defined in XAML using x:TypeArguments`**.
            BaseType = Type.GetType(baseType);
            InnerTypes = innerTypes;
        }

        public Type BaseType { get; set; }

        public Type[] InnerTypes { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Type result = BaseType.MakeGenericType(InnerTypes);
            return result;
        }
    }

    public class GenericTypeExtension : MarkupExtension
    {
        public GenericTypeExtension(string baseType, Type innerType)
        {
            BaseType = Type.GetType(baseType);
            InnerType = innerType;
        }

        public Type BaseType { get; }

        public Type InnerType { get; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Type result = BaseType.MakeGenericType(InnerType);
            return result;
        }
    }

}