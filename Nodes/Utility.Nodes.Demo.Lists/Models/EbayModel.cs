using Nito.Disposables.Internals;
using SQLite;
using System.IO;
using System.Text.Json.Serialization;
using Utility.Attributes;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Demo.Lists.Factories;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Lists.Infrastructure
{
    [Model("a223440a-a171-4989-91f3-a11fbea82546", nameof(NodeMethodFactory.BuildEbayRoot), 3)]
    public class EbayModel : NotifyPropertyClass, IId<Guid>, IIdSet<Guid>, IClone
    {
        //private string? indexPath;
        //private string? title;
        //private string? subTitle;
        //private string? descriptionOne;
        //private string? descriptionTwo;
        //private string? imagePathOne;
        //private string? imagePathTwo;
        //private string? disclaimerOne;
        //private double? sleeveLengthInCentimetres;
        //private double? lengthInCentimetres;
        //private double? pitToPitWidthInCentimetres;
        //private double? shoulderWidthInCentimetres;
        //private bool hasShipping;

        public EbayModel()
        {

        }

        [PrimaryKey]
        [Attributes.Column(ignore: true)]
        [JsonIgnore]
        public Guid Id { get; set; }
        public string? RelativePath { get; set; }
        //public string? Title { get; set; }
        //public string? SubTitle { get; set; }
        //public string? DescriptionOne { get; set; }
        //public string? DescriptionTwo { get; set; }
        //public string? ImagePathOne { get; set; }
        //public string? ImagePathTwo { get; set; }
        //public string? DisclaimerOne { get; set; }
        //public double? SleeveLengthInCentimetres { get; set; }
        //public double? LengthInCentimetres { get; set; }
        //public double? PitToPitWidthInCentimetres { get; set; }
        //public double? ShoulderWidthInCentimetres { get; set; }
        //public bool HasShipping { get; set; }

        //[Attributes.Column(ignore: true)]
        //[JsonIgnore]
        //[SQLite.Ignore]
        //public bool HasTitle => Title != null;
        //[Attributes.Column(ignore: true)]
        //[JsonIgnore]
        //[SQLite.Ignore]
        //public bool HasDescriptions => Descriptions.Length != 0;
        //[Attributes.Column(ignore: true)]
        //[JsonIgnore]
        //[SQLite.Ignore]
        //public string[] Descriptions => [.. new string?[] { DescriptionOne, DescriptionTwo }.WhereNotNull()];
        //[Attributes.Column(ignore: true)]
        //[JsonIgnore]
        //[SQLite.Ignore]
        //public string[] ImagePaths => [.. new string?[] { ImagePathOne, ImagePathTwo }.WhereNotNull()];
        //[Attributes.Column(ignore: true)]
        //[JsonIgnore]
        //[SQLite.Ignore]
        //public bool HasImagePaths => ImagePaths.Length != 0;
        //[Attributes.Column(ignore: true)]
        //[JsonIgnore]
        //[SQLite.Ignore]
        //public bool HasMeasurements => SleeveLengthInCentimetres.HasValue || LengthInCentimetres.HasValue || PitToPitWidthInCentimetres.HasValue || ShoulderWidthInCentimetres.HasValue;
        //[Attributes.Column(ignore: true)]
        //[JsonIgnore]
        //[SQLite.Ignore]
        //public string[] Disclaimers => [.. new string?[] { DisclaimerOne }.WhereNotNull()];
        //[Attributes.Column(ignore: true)]
        //[JsonIgnore]
        //[SQLite.Ignore]
        //public bool HasDisclaimers => Disclaimers.Length != 0;

        public object Clone()
        {
            var clone = AnyClone.CloneExtensions.Clone(this);
            clone.Id = Guid.NewGuid();
            return clone;
        }

        //public int CompareTo(object? obj)
        //{
        //    if (obj is EbayModel m)
        //    {
        //        return m.Title?.CompareTo(m.Title) ?? 0;
        //    }
        //    return 0;
        //}
        public string Copy() => File.ReadAllText(RelativePath);
    }
}
