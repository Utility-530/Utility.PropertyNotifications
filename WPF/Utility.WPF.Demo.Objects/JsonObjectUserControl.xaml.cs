using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Utility.Attributes;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Helpers;
using Utility.Models;
using Utility.PropertyNotifications;
using Utility.WPF.Controls.Objects;

namespace Utility.WPF.Demo.Objects
{
    /// <summary>
    /// Interaction logic for JsonViewUserControl.xaml
    /// </summary>
    public partial class JsonObjectUserControl : UserControl
    {
        private static JsonSerializerSettings combined;

        public JsonObjectUserControl()
        {
            JsonConvert.DefaultSettings = () => Combined;
            InitializeComponent();
            this.Resources.Add("exceptions", new List<Exception> { new ExceptionOne(), new ExceptionTwo(), new Exception() });
            (this.Resources["AuctionItem"] as DemoItem).WithChangesTo(a => a.NullString).Subscribe(c =>
            {
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                string clipboardText = Clipboard.GetText(TextDataFormat.Text);
                JsonControl.Json = clipboardText;
            }
        }

        public static JsonSerializerSettings Combined
        {
            get
            {
                if (combined != null)
                    return combined;

                var _settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    CheckAdditionalContent = false
                };

                foreach (var setting in converters())
                {
                    _settings.Converters.Add(setting);
                }
                return combined = _settings;

                IEnumerable<JsonConverter> converters()
                {
                    yield return new DimensionConverter();
                }
            }
        }
    }

    public static class SchemaLoader
    {
        public static Schema LoadSchema()
        {
            Schema schema = JsonConvert.DeserializeObject<Schema>(Utility.Helpers.ResourceHelper.GetEmbeddedResource("exception.schema.json").AsString());
            return schema;
        }

        public static Schema LoadExceptionSchema()
        {
            Schema schema = JsonConvert.DeserializeObject<Schema>(Utility.Helpers.ResourceHelper.GetEmbeddedResource("exception.schema.json").AsString());
            return schema;
        }
    }

    [Model("f65a5bcd-f725-4890-860d-ecd13ca6babb")]
    public class DemoItem : NotifyPropertyClass
    {
        private string? title;
        private double? lengthInCentimetres;

        public DemoItem()
        {
        }

        [JsonIgnore]
        public int Ignored { get; set; }

        public Guid Guid { get; set; }

        public string? NullString { get => title; set => RaisePropertyChanged(ref title, value); }
        public double? NullDouble { get => lengthInCentimetres; set => RaisePropertyChanged(ref lengthInCentimetres, value); }
        public bool ReadOnlyBoolean => NullString != null;
        public bool Boolean { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? NullDateTime { get; set; }

        [DataType(Enums.DataType.Money)]
        public decimal Money { get; set; }

        [DataType(Enums.DataType.PIN)]
        public string Pin { get; set; }

        [DataType(Enums.DataType.Percentage)]
        public decimal Percentage { get; set; }
    }

    public class JsonSerialiser : JsonSerializer
    {
        public JsonSerialiser() : base()
        {
            Converters.Add(new MetadataConverter());
        }
    }
}