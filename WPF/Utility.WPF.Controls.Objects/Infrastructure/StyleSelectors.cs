using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using Utility.Helpers.Reflection;
using Utility.WPF.ResourceDictionarys;

namespace Utility.WPF.Controls.Objects
{
    public class JsonContainerStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            //if (item is JObject jObject && jObject["$values"] != null)
            //    return Application.Current.Resources["RootStyle"] as Style;
            if (item is JArray { })
            {
                var token = item as JToken;
                while (token.Type != JTokenType.Object)
                {
                    token = token.Parent;
                }

                if (token["$type"] is { } type)
                {
                    var _type = Type.GetType(type.ToString());
                    var elementType = TypeHelper.GetElementType(_type);

                    if (elementType.IsValueType || elementType == typeof(string))
                        return find("RootValueStyle");
                }

                return find("RootStyle");
            }
            else if (item is JProperty property)
                return find("PropertyStyle");
            else if (item is JObject _object)
                return find("ObjectStyle");

            return find("JsonTreeViewItem");

            Style find(string key) => Resources.FindResource<Style>(key);
        }

        public Collection<ResourceDictionary> Resources { get; } = new();
    }

    public class JsonContainerStyle2Selector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            return find("TreeListViewItemStyle");

            Style find(string key) => Resources.FindResource<Style>(key);
        }

        public Collection<ResourceDictionary> Resources { get; } = new();
    }
}