
namespace ProductModels
{
    internal class Product
    {
        public string Title { get; set; }
        public List<Description> Descriptions { get; set; }
        public List<Image> Images { get; set; }
        public List<Measurement> Measurements { get; set; } 
    }

    public class Description
    {
        public string Value { get; set; }
    }


    public class Image
    {
        public string Name { get; set; }
    }

    public class Measurement
    {
        public double PitToPit { get; set; }
        public double SleeveLength { get; set; }
        public double Length { get; set; }
        public LengthUnits Units { get; set; }
    }

    public enum LengthUnits
    {
        Centimetre, Inches
    }
}
