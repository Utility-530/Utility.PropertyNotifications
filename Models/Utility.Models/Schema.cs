using Newtonsoft.Json;
using Utility.Structs;

namespace Utility.Models
{
    public class SchemaStore
    {
        Dictionary<Type, Schema> Schemas { get; } = [];

        public Schema this[Type type] => Schemas[type];
        public void Add(Type type, Schema schema) => Schemas.Add(type, schema);
        public bool TryGetValue(Type type, out Schema? schema)
        {
            bool v = Schemas.TryGetValue(type, out var _schema);
            schema = _schema;
            return v;
        }
        public static SchemaStore Instance { get; } = new SchemaStore();
    }

    public class SchemaProperty
    {
        public string Name { get; set; }

        //
        // Summary:
        //     Gets or sets the default value.
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public object Default { get; set; }

        //
        // Summary:
        //     Gets or sets the required multiple of for the number value.
        [JsonProperty("multipleOf", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public decimal? MultipleOf { get; set; }

        //
        // Summary:
        //     Gets or sets the maximum allowed value.
        [JsonProperty("maximum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public decimal? Maximum { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether the maximum value is excluded.
        [JsonProperty("exclusiveMaximum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsExclusiveMaximum { get; set; }

        //
        // Summary:
        //     Gets or sets the minimum allowed value.
        [JsonProperty("minimum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public decimal? Minimum { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether the minimum value is excluded.
        [JsonProperty("exclusiveMinimum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsExclusiveMinimum { get; set; }

        //
        // Summary:
        //     Gets or sets the maximum length of the value string.
        [JsonProperty("maxLength", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int? MaxLength { get; set; }

        //
        // Summary:
        //     Gets or sets the minimum length of the value string.
        [JsonProperty("minLength", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int? MinLength { get; set; }

        //
        // Summary:
        //     Gets or sets the validation pattern as regular expression.
        [JsonProperty("pattern", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Pattern { get; set; }

        //
        // Summary:
        //     Gets or sets the maximum length of the array.
        [JsonProperty("maxItems", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int MaxItems { get; set; }

        //
        // Summary:
        //     Gets or sets the minimum length of the array.
        [JsonProperty("minItems", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int MinItems { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether the items in the array must be unique.
        [JsonProperty("uniqueItems", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool UniqueItems { get; set; }

        //
        // Summary:
        //     Gets or sets the maximal number of allowed properties in an object.
        [JsonProperty("maxProperties", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int MaxProperties { get; set; }

        //
        // Summary:
        //     Gets or sets the minimal number of allowed properties in an object.
        [JsonProperty("minProperties", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int MinProperties { get; set; }

        //
        // Summary:
        //     Gets the collection of required properties.
        [JsonIgnore]
        public ICollection<object> Enumeration { get; set; }

        //
        // Summary:
        //     Gets a value indicating whether this is enumeration.
        [JsonIgnore]
        public bool IsEnumeration => Enumeration.Count > 0;

        public string Template { get; set; }
        public string EnumType { get; set; }
        public string Type { get; set; }
        public bool IsVisible { get; set; } = true;
        public Dimension ColumnWidth { get; set; }
    }

    public class Schema
    {
        public SchemaProperty[] Properties { get; set; }
    }
}
