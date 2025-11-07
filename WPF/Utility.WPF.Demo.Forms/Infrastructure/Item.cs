using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace Utility.WPF.Demo.Forms.Infrastructure
{
    public class Item : IEquatable<Item>
    {
        public Item(Guid id, string title, string subTitle, IReadOnlyCollection<string> descriptions,
            IReadOnlyCollection<string> images, IReadOnlyCollection<string> shipping,
            IReadOnlyCollection<string> disclaimers,
            double[] measurements)
        {
            Id = id;
            Title = title;
            SubTitle = subTitle;
            Descriptions = descriptions;
            DescriptionHeader = "Descriptions";
            Images = images;
            ImagesHeader = "Images";
            Shipping = shipping;
            ShippingHeader = "Shipping";
            Disclaimers = disclaimers;
            DisclaimersHeader = "Disclaimers";
            Measurements = measurements;
            MeasurementsHeader = "Measurements";
        }

        public Item()
        {
            Id = Guid.NewGuid();
            Title = string.Empty;
            SubTitle = string.Empty;
            Descriptions = new string[0];
            DescriptionHeader = "Descriptions";
            Images = new string[0];
            ImagesHeader = "Images";
            Shipping = new string[0];
            ShippingHeader = "Shipping";
            Disclaimers = new string[0];
            DisclaimersHeader = "Disclaimers";
            MeasurementsHeader = "Measurements";
        }

        public Guid Id { get; init; }
        public string Title { get; init; }
        public string SubTitle { get; init; }
        public bool IsTitleDisabled { get; init; }

        public IReadOnlyCollection<string> Descriptions { get; init; }
        public string DescriptionHeader { get; init; }

        public bool IsDescriptionDisabled { get; init; }

        public IReadOnlyCollection<string> Images { get; init; }
        public string ImagesHeader { get; init; }

        public bool IsImagesDisabled { get; init; }

        public IReadOnlyCollection<string> Shipping { get; init; }
        public string ShippingHeader { get; init; }

        public bool IsShippingDisabled { get; init; }

        public IReadOnlyCollection<string> Disclaimers { get; init; }
        public string DisclaimersHeader { get; init; }
        public bool IsDisclaimersDisabled { get; init; }

        public double[] Measurements { get; init; } = new double[] { 0, 0, 0, 0 };

        public string MeasurementsHeader { get; init; }
        public bool IsMeasurementsDisabled { get; init; }

        public Measurements MeasurementsInches => new Measurements { Shoulder = GetIn(Measurements[MapperFactory.ShoulderIndex]), Length = GetIn(Measurements[MapperFactory.LengthIndex]), PitToPit = GetIn(Measurements[MapperFactory.PitToPitIndex]), SleeveLength = GetIn(Measurements[MapperFactory.SleeveLengthIndex]) };

        public Measurements MeasurementsCentimetres => new Measurements { Shoulder = GetCm(Measurements[MapperFactory.ShoulderIndex]), Length = GetCm(Measurements[MapperFactory.LengthIndex]), PitToPit = GetCm(Measurements[MapperFactory.PitToPitIndex]), SleeveLength = GetCm(Measurements[MapperFactory.SleeveLengthIndex]) };

        private static double GetIn(double length)
        {
            return new Length(length, UnitsNet.Units.LengthUnit.Centimeter).Inches;
        }

        private static double GetCm(double length)
        {
            return new Length(length, UnitsNet.Units.LengthUnit.Centimeter).Centimeters;
        }

        public override bool Equals(object? obj)
        {
            return this.Equals(obj as Item);
        }

        public bool Equals(Item? other)
        {
            return Id == other?.Id &&
                Title == other.Title &&
                SubTitle == other.SubTitle &&
                Descriptions.SequenceEqual(other.Descriptions) &&
                Images.SequenceEqual(other.Images) &&
                Shipping.SequenceEqual(other.Shipping) &&
                Disclaimers.SequenceEqual(other.Disclaimers);
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(Id.ToByteArray(), 0);
        }

        public override string ToString()
        {
            return $"{Id} {Title} {SubTitle}";
        }
    }

    public class Measurements
    {
        public double PitToPit { get; init; }
        public double Shoulder { get; init; }
        public double Length { get; init; }
        public double SleeveLength { get; init; }
    }
}