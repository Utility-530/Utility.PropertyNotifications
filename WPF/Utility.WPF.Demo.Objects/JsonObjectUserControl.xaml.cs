using Newtonsoft.Json;
using Nito.Disposables.Internals;
using SQLite;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utility.Attributes;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Helpers;
using Utility.Interfaces.Generic.Data;
using Utility.Models;
using Utility.PropertyNotifications;
using Utility.WPF.Controls.Objects;
using Ignore = Utility.Attributes.IgnoreAttribute;


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

    [Model]
    public class AuctionItem : NotifyPropertyClass, IId<Guid>
    {
        private string? indexPath;
        private string? title;
        private string? subTitle;
        private string? descriptionOne;
        private string? descriptionTwo;
        private string? imagePathOne;
        private string? imagePathTwo;
        private string? disclaimerOne;
        private double? sleeveLengthInCentimetres;
        private double? lengthInCentimetres;
        private double? pitToPitWidthInCentimetres;
        private double? shouldertWidthInCentimetres;
        private bool hasShipping;

        public AuctionItem()
        {

        }

        public Guid Id { get; set; }
        //public string? IndexPath { get => indexPath; set => RaisePropertyChanged(ref indexPath, value); }

        [JsonIgnore]
        public string? Title { get => title; set => RaisePropertyChanged(ref title, value); }
        //public string? SubTitle { get => subTitle; set => RaisePropertyChanged(ref subTitle, value); }
        //public string? DescriptionOne { get => descriptionOne; set => RaisePropertyChanged(ref descriptionOne, value); }
        //public string? DescriptionTwo { get => descriptionTwo; set => RaisePropertyChanged(ref descriptionTwo, value); }
        //public string? ImagePathOne { get => imagePathOne; set => RaisePropertyChanged(ref imagePathOne, value); }
        //public string? ImagePathTwo { get => imagePathTwo; set => RaisePropertyChanged(ref imagePathTwo, value); }
        //public string? DisclaimerOne { get => disclaimerOne; set => RaisePropertyChanged(ref disclaimerOne, value); }
        //public double? SleeveLengthInCentimetres { get => sleeveLengthInCentimetres; set => RaisePropertyChanged(ref lengthInCentimetres, value); }
        public double? LengthInCentimetres { get => lengthInCentimetres; set => RaisePropertyChanged(ref lengthInCentimetres, value); }
        //public double? PitToPitWidthInCentimetres { get => pitToPitWidthInCentimetres; set => RaisePropertyChanged(ref pitToPitWidthInCentimetres, value); }
        //public double? ShouldertWidthInCentimetres { get => shouldertWidthInCentimetres; set => RaisePropertyChanged(ref shouldertWidthInCentimetres, value); }
        public bool HasShipping
        {
            get => hasShipping; set => RaisePropertyChanged(ref hasShipping, value);
        }

        public bool HasTitle => Title != null;
        //[Ignore]
        //public bool HasDescriptions => Descriptions.Length != 0;
        //[Ignore]
        //public string[] Descriptions => [.. new string?[] { DescriptionOne, DescriptionTwo }.WhereNotNull()];
        //[Ignore]
        //public string[] ImagePaths => [.. new string?[] { ImagePathOne, ImagePathTwo }.WhereNotNull()];
        //[Ignore]
        //public bool HasImagePaths => ImagePaths.Length != 0;
        //[Ignore]
        //public bool HasMeasurements => SleeveLengthInCentimetres.HasValue || LengthInCentimetres.HasValue || PitToPitWidthInCentimetres.HasValue || ShouldertWidthInCentimetres.HasValue;
        //[Ignore]
        //public string[] Disclaimers => [.. new string?[] { DisclaimerOne }.WhereNotNull()];
        //[Ignore]
        //public bool HasDisclaimers => Disclaimers.Length != 0;
    }

    public class JsonSerialiser : JsonSerializer
    {
        public JsonSerialiser() : base()
        {

            Converters.Add(new MetadataConverter());
        }
    }
}