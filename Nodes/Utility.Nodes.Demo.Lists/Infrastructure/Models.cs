using CustomModels;
using Nito.Disposables.Internals;
using SQLite;
using System.ComponentModel;
using System.Linq;
using Utility.Interfaces.Generic.Data;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Lists.Infrastructure
{

    [Model]
    public class Model
    {
        public int Id { get; set; }
    }


    [Model]
    public class EbayModel : NotifyPropertyClass, IId<Guid>
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

        public EbayModel()
        {

        }

        [PrimaryKey]
        [Attributes.Ignore]
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
        public double? ShouldertWidthInCentimetres { get => shouldertWidthInCentimetres; set => RaisePropertyChanged(ref shouldertWidthInCentimetres, value); }
        public bool HasShipping
        {
            get => hasShipping; set => RaisePropertyChanged(ref hasShipping, value);
        }

        [Attributes.Ignore]
        [SQLite.Ignore]
        public bool HasTitle => Title != null;
        [Attributes.Ignore]
        [SQLite.Ignore]
        public bool HasDescriptions => Descriptions.Any();
        [Attributes.Ignore]
        [SQLite.Ignore]
        public string[] Descriptions => [.. new string?[] { DescriptionOne, DescriptionTwo }.WhereNotNull()];
        [Attributes.Ignore]
        [SQLite.Ignore]
        public string[] ImagePaths => [.. new string?[] { ImagePathOne, ImagePathTwo }.WhereNotNull()];
        [Attributes.Ignore]
        [SQLite.Ignore]
        public bool HasImagePaths => ImagePaths.Any();
        [Attributes.Ignore]
        [SQLite.Ignore]
        public bool HasMeasurements => SleeveLengthInCentimetres.HasValue || LengthInCentimetres.HasValue || PitToPitWidthInCentimetres.HasValue || ShouldertWidthInCentimetres.HasValue;
        [Attributes.Ignore]
        [SQLite.Ignore]
        public string[] Disclaimers => [.. new string?[] { DisclaimerOne }.WhereNotNull()];
        [Attributes.Ignore]
        [SQLite.Ignore]
        public bool HasDisclaimers => Disclaimers.Any();
    }
}
