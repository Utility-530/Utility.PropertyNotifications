using Newtonsoft.Json;
using System.ComponentModel;
using System.Text.Json.Nodes;
using Utility.Conversions.Json.Newtonsoft;

namespace Utility.Conversion.Tests
{
    public class Tests
    {
        private JsonSerializerSettings settings;

        [SetUp]
        public void Setup()
        {
            settings = new JsonSerializerSettings { Converters = [new AssemblyJsonConverter(), new PropertyInfoJsonConverter(), new AttributeCollectionConverter()] };
        }

        [Test]
        public void Test1()
        {
            var property = typeof(string).GetProperties().First();

            var x = JsonConvert.SerializeObject(property, settings);

            Assert.Pass();
        }


        [Test]
        public void Test2()
        {
            var x = TypeDescriptor.GetProperties(typeof(string));
            foreach (var xx in x)
            {
                string ser = JsonConvert.SerializeObject(xx, settings);
            }
            Assert.Pass();
        }

        [Test]
        public void Test3()
        {
            string ser = JsonConvert.SerializeObject(new DDx(), settings);
            var d_ser = JsonConvert.DeserializeObject<DDx>(ser, settings);
            Assert.Pass();
        }
    }

    class DDx : PropertyDescriptor
    {
        public DDx() : base("stingg", [new System.ComponentModel.DescriptionAttribute()])
        {
        }

        [JsonIgnore]
        public override TypeConverter Converter => base.Converter;

        public override Type ComponentType { get; }
        public override bool IsReadOnly { get; }
        public override Type PropertyType { get; }

        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override object? GetValue(object? component)
        {
            throw new NotImplementedException();
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object? component, object? value)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            throw new NotImplementedException();
        }
    }
}