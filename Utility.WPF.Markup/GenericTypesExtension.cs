using System;
using System.Windows.Markup;

namespace Utility.WPF.Markup
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/54092789/datatemplates-and-generics/54124755#54124755"></a>
    /// </summary>
    public class GenericTypesExtension : MarkupExtension
    {
        //    public GenericTypesExtension()
        //    {
        //    }

        public GenericTypesExtension(Type baseType, params Type[] innerTypes)
        {
            BaseType = baseType;
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
        //public GenericTypeExtension()
        //{
        //}

        public GenericTypeExtension(Type baseType, Type innerType)
        {
            BaseType = baseType;
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