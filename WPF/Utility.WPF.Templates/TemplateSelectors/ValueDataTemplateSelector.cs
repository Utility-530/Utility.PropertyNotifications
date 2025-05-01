using System;
using System.Linq;
using System.Windows;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;

namespace Utility.WPF.Templates
{
    public interface ITemplates
    {
        ResourceDictionary Templates { get; }
    }

    public class ValueDataTemplateSelector : GenericDataTemplateSelector, ITemplates
    {
        private ValueTemplates valueTemplates;
        private Type[] types;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not IValue { Value: var value })
            {
                throw new Exception($"Unexpected type for item {item.GetType().Name}");
            }
            if(item is IAutoList { AutoList:{ } list } autoList)
            {
                return this.Templates["Combo"] as DataTemplate;
            }
            if (get() is Type type)
            {
                return this.FromType(type) ?? Templates["Missing"] as DataTemplate ?? throw new Exception("d3091111111");
            }

            if (item is DataTemplate dataTemplate)
                return dataTemplate;


            if (value == null)
                return NullTemplate ??= TemplateFactory.CreateNullTemplate();


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
                types ??= newMethod();
                return types;
            }
        }

        private Type[] newMethod()
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

    public static class TemplateTypeHelper
    {
        public static DataTemplate FromType(this ITemplates template, Type type)
        {
            if (Nullable.GetUnderlyingType(type) != null)
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                var underlyingTypeName = underlyingType.BaseType == typeof(Enum) ? underlyingType.BaseType.Name : underlyingType.Name;
                if (template.Templates[$"{type.Name}[{underlyingTypeName}]"] is DataTemplate dt)
                    return dt;
            }
            if (type.BaseType == typeof(Enum))
            {
                if (template.Templates[new DataTemplateKey(type.BaseType)] is DataTemplate __dt)
                    return __dt;
            }

            if (type == typeof(object))
            {
                return template.Templates["Object"] as DataTemplate;
            }
            if (type.ToString() is "System.RuntimeType")
            {
                return template.Templates[new DataTemplateKey(typeof(Type))] as DataTemplate;
            }


            if (template.Templates.Contains(new DataTemplateKey(type)))
                return template.Templates[new DataTemplateKey(type)] as DataTemplate;

            return null;
        }
    }
}