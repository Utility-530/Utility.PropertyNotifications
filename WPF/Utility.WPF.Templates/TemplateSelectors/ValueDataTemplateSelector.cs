using System;
using System.Linq;
using System.Windows;
using Utility.Interfaces.NonGeneric;

namespace Utility.WPF.Templates
{


    public class ValueDataTemplateSelector : GenericDataTemplateSelector
    {
        private ValueTemplates valueTemplates;
        private Type[] types;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not IValue { Value: var value })
            {
                throw new Exception($"Unexpected type for item {item.GetType().Name}");
            }
            if (get() is Type type)
            {   
                if (Nullable.GetUnderlyingType(type) != null)
                {
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    var underlyingTypeName = underlyingType.BaseType == typeof(Enum) ? underlyingType.BaseType.Name : underlyingType.Name;
                    if (Templates[$"{type.Name}[{underlyingTypeName}]"] is DataTemplate dt)
                        return dt;
                }
                if (type.BaseType == typeof(Enum))
                {
                    if (Templates[new DataTemplateKey(type.BaseType)] is DataTemplate __dt)
                        return __dt;
                }
                // DataTemplate.DataType cannot be type Object.
                if (type == typeof(object))
                {
                    return Templates["Object"] as DataTemplate;
                }
                if (Templates[new DataTemplateKey(type)] is DataTemplate _dt)
                    return _dt;
            }  



            if (value == null)
                return NullTemplate ??= TemplateFactory.CreateNullTemplate();

            //var type = value.GetType();
            //var _descriptor = item is IPropertyDescriptor descriptor ? descriptor : null;
            //var _info = item is IPropertyInfo propertyInfo ? propertyInfo : null;

           

            return Templates["Missing"] as DataTemplate ?? throw new Exception("dfs 33091111111");
        
            Type? get()
            {
                if (item is IType { Type: { } type })
                    return type;
                if (value?.GetType() is { } _type)
                    return _type;
                return null;
            }
        }


        public Type[] Types
        {
            get
            {
                types ??= NewMethod();
                return types;
            }
        }

        private Type[] NewMethod()
        {
            var keys = this.Templates.Keys.OfType<DataTemplateKey>().ToArray();
            return keys.Select(a => a.DataType).OfType<Type>().ToArray(); 
        }

        public override ResourceDictionary Templates
        {
            get
            {
                if (valueTemplates == null)
                {
                    valueTemplates = new ValueTemplates();
                    valueTemplates.InitializeComponent();
                }
                return valueTemplates;
            }
        }


        public static ValueDataTemplateSelector Instance => new();
    }
}