using Nito.Disposables.Internals;
using SQLite;
using System.Text.Json.Serialization;
using Utility.Attributes;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Lists.Infrastructure
{
    [Model("a223440a-a171-4989-91f3-a11fbea82546", nameof(Services.NodeMethodFactory.BuildEbayRoot))]
    public class EbayModel : NotifyPropertyClass, IId<Guid>, IClone
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
        private double? shoulderWidthInCentimetres;
        private bool hasShipping;

        public EbayModel()
        {

        }

        [PrimaryKey]
        [Attributes.Column(ignore: true)]
        [JsonIgnore]
        public Guid Id { get; set; }
        public string? IndexPath { get => indexPath; set => RaisePropertyChanged(ref indexPath, value); }
        public string? Title { get => title; set => RaisePropertyChanged(ref title, value); }
        public string? SubTitle { get => subTitle; set => RaisePropertyChanged(ref subTitle, value); }
        public string? DescriptionOne { get => descriptionOne; set => RaisePropertyChanged(ref descriptionOne, value); }
        public string? DescriptionTwo { get => descriptionTwo; set => RaisePropertyChanged(ref descriptionTwo, value); }
        public string? ImagePathOne { get => imagePathOne; set => RaisePropertyChanged(ref imagePathOne, value); }
        public string? ImagePathTwo { get => imagePathTwo; set => RaisePropertyChanged(ref imagePathTwo, value); }
        public string? DisclaimerOne { get => disclaimerOne; set => RaisePropertyChanged(ref disclaimerOne, value); }
        public double? SleeveLengthInCentimetres { get => sleeveLengthInCentimetres; set => RaisePropertyChanged(ref lengthInCentimetres, value); }
        public double? LengthInCentimetres { get => lengthInCentimetres; set => RaisePropertyChanged(ref lengthInCentimetres, value); }
        public double? PitToPitWidthInCentimetres { get => pitToPitWidthInCentimetres; set => RaisePropertyChanged(ref pitToPitWidthInCentimetres, value); }
        public double? ShoulderWidthInCentimetres { get => shoulderWidthInCentimetres; set => RaisePropertyChanged(ref shoulderWidthInCentimetres, value); }
        public bool HasShipping { get => hasShipping; set => RaisePropertyChanged(ref hasShipping, value); }

        [Attributes.Column(ignore: true)]
        [JsonIgnore]
        [SQLite.Ignore]
        public bool HasTitle => Title != null;
        [Attributes.Column(ignore: true)]
        [JsonIgnore]
        [SQLite.Ignore]
        public bool HasDescriptions => Descriptions.Length != 0;
        [Attributes.Column(ignore: true)]
        [JsonIgnore]
        [SQLite.Ignore]
        public string[] Descriptions => [.. new string?[] { DescriptionOne, DescriptionTwo }.WhereNotNull()];
        [Attributes.Column(ignore: true)]
        [JsonIgnore]
        [SQLite.Ignore]
        public string[] ImagePaths => [.. new string?[] { ImagePathOne, ImagePathTwo }.WhereNotNull()];
        [Attributes.Column(ignore: true)]
        [JsonIgnore]
        [SQLite.Ignore]
        public bool HasImagePaths => ImagePaths.Length != 0;
        [Attributes.Column(ignore: true)]
        [JsonIgnore]
        [SQLite.Ignore]
        public bool HasMeasurements => SleeveLengthInCentimetres.HasValue || LengthInCentimetres.HasValue || PitToPitWidthInCentimetres.HasValue || ShoulderWidthInCentimetres.HasValue;
        [Attributes.Column(ignore: true)]
        [JsonIgnore]
        [SQLite.Ignore]
        public string[] Disclaimers => [.. new string?[] { DisclaimerOne }.WhereNotNull()];
        [Attributes.Column(ignore: true)]
        [JsonIgnore]
        [SQLite.Ignore]
        public bool HasDisclaimers => Disclaimers.Length != 0;

        public object Clone()
        {
            var clone = AnyClone.CloneExtensions.Clone(this);
            clone.Id = Guid.NewGuid();
            return clone;
        }
    }
}
