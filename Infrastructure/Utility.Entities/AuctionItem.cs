using Newtonsoft.Json;
using SQLite;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;

namespace Utility.Entities
{
    public class PathAttribute : Attribute
    {
        public PathAttribute(string[] path)
        {
        }
    }   

    public class AuctionItem : Entity, IClone
    {

        public AuctionItem()
        {
        }

        public string? RelativePath { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Description1 { get; set; }
        public string? Description2 { get; set; }
        public string? Description3 { get; set; }
        public string? Description4 { get; set; }
        public string? Description5 { get; set; }
        public string? ImagePath1 { get; set; }
        public string? ImagePath2 { get; set; } // Changed from ImagePathTwo to ImagePath2
        public string? ImagePath3 { get; set; } // Changed from ImagePathThree to ImagePath3
        public string? ImagePath4 { get; set; } // Changed from ImagePathFour to ImagePath4
        public string? ImagePath5 { get; set; } // Changed from ImagePathFive to ImagePath5
        public string? ImagePath6 { get; set; } // No change needed
        public string? ImagePath7 { get; set; } // Changed from ImagePathSeven to ImagePath7
        public string? ImagePath8 { get; set; } // Changed from ImagePathEight to ImagePath8
        public string? ImagePath9 { get; set; } // Changed from ImagePathNine to ImagePath9
        public string? ImagePath10 { get; set; } // Changed from ImagePathTen to ImagePath10
        public string? ImagePath11 { get; set; } // Changed from ImagePathEleven to ImagePath11
        public string? ImagePath12 { get; set; } // Changed from ImagePathTwelve to ImagePath12
        public string? Disclaimer1 { get; set; } // Changed from DisclaimerOne to Disclaimer1
        public string? Disclaimer2 { get; set; } // Changed from DisclaimerTwo to Disclaimer2
        public int? SleeveLengthInMillimetres { get; set; }
        public int? LengthInMillimetres { get; set; }
        public int? PitToPitWidthInMillimetres { get; set; }
        public int? ShoulderWidthInMillimetres { get; set; }
        public bool HasShipping { get; set; }

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
            var clone = new AuctionItem();
            clone.Id = Guid.NewGuid();
            clone.RelativePath = RelativePath;
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
        //public string Copy() => File.ReadAllText(RelativePath);
    }
}