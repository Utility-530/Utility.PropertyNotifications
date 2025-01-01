using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tonic.UI
{
    public class EnumValue
    {
        private static Dictionary<object, EnumValue> cache = new Dictionary<object, EnumValue>();
        private static EnumValue nullValue = new EnumValue(null);
        public static EnumValue Create(object value)
        {
            if (value == null)
                return nullValue;

            lock (cache)
            {
                if (!cache.TryGetValue(value, out EnumValue ret))
                {
                    ret = new EnumValue(value);
                    cache.Add(value, ret);
                }
                return ret;
            }
        }
        private EnumValue(object Value)
        {
            this.Value = Value;
            this.Description = EnumConverter.EnumToString(Value);
        }

        public object Value { get; private set; }

        public string Description { get; private set; }

        public override string ToString()
        {
            return Description;
        }
    }

}