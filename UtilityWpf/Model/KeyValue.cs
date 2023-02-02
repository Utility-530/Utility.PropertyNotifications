using MoreLinq;
using System.Linq;

namespace UtilityWpf.Model
{
    public abstract class KeyValue
    {
        public abstract string Key { get; }
        public abstract object Value { get; }

        public virtual string GroupKey { get; }

        //public bool IsSelected { get; set; }
        //public bool IsChecked { get; set; }



    }
}