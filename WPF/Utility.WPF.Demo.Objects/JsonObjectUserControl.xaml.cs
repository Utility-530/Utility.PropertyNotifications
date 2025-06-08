using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Helpers;
using Utility.Models;
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
}