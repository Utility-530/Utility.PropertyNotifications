namespace Utility.PropertyTrees.Infrastructure
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class PropertyGridAttribute : Attribute
    {
        public PropertyGridAttribute()
        {
            Type = typeof(object);
        }

        public object Value { get; set; }
        public string Name { get; set; }
        public System.Type Type { get; set; }

        public override object TypeId
        {
            get
            {
                return Name;
            }
        }
    }
}