using Hardcodet.Wpf.DataBinding;
using System;
using System.Linq;
using System.Windows.Markup;
using System.Reflection;

namespace Utility.WPF.Markup
{
    [ContentProperty(nameof(Path))]
    public class TypeBinding : BindingDecoratorBase
    {
        public TypeBinding()
        {
        }
                
        public Type? Type { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Type ??= GetType(serviceProvider);

            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (Type == null)
                throw new ArgumentNullException(nameof(Type));
            if (Path == null)
                throw new ArgumentNullException(nameof(Path));

            if (Type == null || string.IsNullOrEmpty(Path.Path) || Path.Path.Contains('.'))
                throw new ArgumentException("Syntax for x:NameOf is Type={x:Type [className]} Member=[propertyName]");

            var propertyInfo = Type.GetRuntimeProperties().FirstOrDefault(pi => pi.Name == Path.Path);
            if (propertyInfo != null)
                return propertyInfo.PropertyType;
            var fieldInfo = Type.GetRuntimeFields().FirstOrDefault(fi => fi.Name == Path.Path);
            if (fieldInfo != null)
                return fieldInfo.FieldType;
            var eventInfo = Type.GetRuntimeEvents().FirstOrDefault(ei => ei.Name == Path.Path);
            if (eventInfo != null)
                return eventInfo.EventHandlerType;

            throw new ArgumentException($"No property or field found for {Path.Path} in {Type}");
        }

        private Type? GetType(IServiceProvider serviceProvider)
        {
            if (TryGetTargetItems(serviceProvider, out var targetObject, out var _))
            {
                return NameOfExtension.GetType(targetObject);
            }
            throw new Exception("gre");
        }
    }
}
