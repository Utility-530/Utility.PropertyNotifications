using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup.Primitives;

namespace UtilityWpf.Properties
{
    public static class ReflectionHelper
    {
        public static IEnumerable<DependencyProperty> SelectDependencyProperties(this Type type) =>
             type
            .GetFields(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public)
            .Where(f => f.FieldType == typeof(DependencyProperty))
            .Select(f => f.GetValue(null))
            .Cast<DependencyProperty>();

        //Filters only highest level DependencyProperties. (i.e not those associated with inherited classes)
        public static IEnumerable<DependencyProperty> SelectDependencyPropertiesDeclaredOnly(this Type type) =>
            type
            .GetFields(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(f => f.FieldType == typeof(DependencyProperty))
            .Select(f => f.GetValue(null))
            .Cast<DependencyProperty>();

        //https://stackoverflow.com/questions/4794071/how-to-enumerate-all-dependency-properties-of-control
        public static IEnumerable<DependencyProperty> EnumerateDependencyProperties(this DependencyObject obj)
        {
            if (obj != null)
            {
                LocalValueEnumerator lve = obj.GetLocalValueEnumerator();
                while (lve.MoveNext())
                {
                    yield return lve.Current.Property;
                }
            }
        }

        //https://stackoverflow.com/questions/4794071/how-to-enumerate-all-dependency-properties-of-control
        public static IEnumerable<DependencyProperty> EnumerateDependencyProperties(object element)
        {
            if (element != null)
            {
                MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
                if (markupObject != null)
                {
                    foreach (MarkupProperty mp in markupObject.Properties)
                    {
                        if (mp.DependencyProperty != null)
                            yield return mp.DependencyProperty;
                    }
                }
            }
        }

        //https://stackoverflow.com/questions/4794071/how-to-enumerate-all-dependency-properties-of-control
        public static IEnumerable<DependencyProperty> EnumerateAttachedProperties(object element)
        {
            if (element != null)
            {
                MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
                if (markupObject != null)
                {
                    foreach (MarkupProperty mp in markupObject.Properties)
                    {
                        if (mp.IsAttached)
                            yield return mp.DependencyProperty;
                    }
                }
            }
        }
    }
}