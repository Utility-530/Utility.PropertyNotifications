using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;

namespace Utility.WPF.Helpers
{
    public static class DependencyPropertyHelper
    {
        public static DependencyProperty? FindDependencyProperty(this DependencyObject target, string propName)
        {
            FieldInfo? fInfo = target.GetType().GetField(propName, BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);

            return fInfo == null ? null : (DependencyProperty?)fInfo.GetValue(null);
        }

        public static bool HasDependencyProperty(this DependencyObject target, string propName)
        {
            return target.FindDependencyProperty(propName) != null;
        }

        /// <summary>
        /// <example>
        /// <code>
        /// var dp = GetDependencyProperty(typeof(TextBox), "TextProperty");
        /// </code>
        /// </example>
        /// </summary>
        public static DependencyProperty? FindDependencyPropertyByName(this DependencyObject target, string name)
        {
            FieldInfo? fieldInfo = target.GetType().GetField(name, BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Static);
            return fieldInfo != null ? (DependencyProperty?)fieldInfo.GetValue(null) : null;
        }

        public static DependencyProperty? FindDependencyPropertyByPropertyType<T>(this DependencyObject target)
        {
            return target.FindDependencyPropertyByPropertyType(typeof(T));
        }

        public static DependencyProperty? FindDependencyPropertyByPropertyType(this DependencyObject target, Type type)
        {
            var arr = target.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Static)
                .Select(fieldInfo => fieldInfo?.GetValue(null))
                .ToArray();
            return arr
                .OfType<DependencyProperty>()
                .Where(dp => dp?.PropertyType == type)
                .SingleOrDefault();
        }

        public static string GetStaticMemberName<TMember>(Expression<Func<TMember>> expression)
        {
            if (expression.Body is not MemberExpression body)
                throw new ArgumentException($"{nameof(GetStaticMemberName)}'s argument, {nameof(expression)}, should be a member expression");

            return body.Member.Name;
        }
    }
}