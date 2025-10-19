using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Windows.Data;
using Syncfusion.UI.Xaml.Grid;
using Humanizer;
using System.Windows;

namespace Utility.Nodes.WPF.Templates.SyncFusion
{
    public class ColumnsConverter : IValueConverter
    {
        private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Type type)
                return DependencyProperty.UnsetValue;

            // Determine item type (e.g., type of first element in collection)


            // Build the columns
            var columns = CreateGridColumns(type).ToArray();
            return columns;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;

        // ────────────────────────────────
        // 🧱 Column Construction
        // ────────────────────────────────
        private static IEnumerable<GridColumn> CreateGridColumns(Type type)
        {
            if (!_propertyCache.TryGetValue(type, out var properties))
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                _propertyCache[type] = properties;
            }

            foreach (var prop in properties)
            {
                if (HasIgnoreAttribute(prop))
                    continue;

                var (hasColAttr, colAttr) = prop.GetAttributeSafe<ColumnAttribute>();
                if (hasColAttr && colAttr.Ignore)
                    continue;

                string displayName = colAttr?.DisplayName ?? prop.Name.Humanize(LetterCasing.Title);
                double width = colAttr?.Width > 0 ? colAttr.Width : 120;

                // Use special column types if applicable
                GridColumn column = CreateAppropriateColumn(prop, displayName, width, colAttr);
                yield return column;
            }
        }

        // ────────────────────────────────
        // ⚙️ Helper Methods
        // ────────────────────────────────
        private static bool HasIgnoreAttribute(PropertyInfo prop)
        {
            var attrs = prop.GetCustomAttributes(true);
            return attrs.Any(a => a is JsonIgnoreAttribute || a is JsonIgnoreAttribute || a is Attributes.IgnoreAttribute);
        }

        private static GridColumn CreateAppropriateColumn(PropertyInfo prop, string header, double width, ColumnAttribute? colAttr)
        {
            // Example: create checkbox column for bools
            if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
            {
                return new GridCheckBoxColumn
                {
                    MappingName = prop.Name,
                    HeaderText = header,
                    Width = width
                };
            }

            // Example: numeric column for numbers
            if (prop.PropertyType == typeof(int) ||
                prop.PropertyType == typeof(double) ||
                prop.PropertyType == typeof(decimal) ||
                prop.PropertyType == typeof(float))
            {
                return new GridNumericColumn
                {
                    MappingName = prop.Name,
                    HeaderText = header,
                    Width = width,
                };
            }

            // Example: date column
            if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
            {
                return new GridDateTimeColumn
                {
                    MappingName = prop.Name,
                    HeaderText = header,
                    Width = width,
                };
            }

            // Default text column
            return new GridTextColumn
            {
                MappingName = prop.Name,
                HeaderText = header,
                Width = width
            };
        }

        private static Type? GetItemType(object collection)
        {
            // Try to infer the element type from IEnumerable<T>
            Type type = collection.GetType();

            // For List<T> or ObservableCollection<T>
            var enumerableInterface = type.GetInterfaces()
                .Concat(new[] { type })
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return enumerableInterface?.GetGenericArguments().FirstOrDefault();
        }
    }

    // ────────────────────────────────
    // 🧩 Optional Attribute Support
    // ────────────────────────────────
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string? DisplayName { get; set; }
        public bool Ignore { get; set; }
        public double Width { get; set; } = 120;
    }

    // Safe helper for attribute access
    public static class ReflectionExtensions
    {
        public static (bool, T?) GetAttributeSafe<T>(this MemberInfo info) where T : Attribute
        {
            var attr = info.GetCustomAttribute<T>();
            return (attr != null, attr);
        }
    }
}
