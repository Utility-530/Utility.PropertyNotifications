using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Models
{
    public interface IChange
    {

        //public DateTime TimeStamp { get; }
    }

    public interface IDictionaryAddChange : IChange
    {
        public string Key { get; }
        public object Value { get; }
    }
    public interface IDictionaryRemoveChange : IChange
    {
        public string Key { get; }
    }

    public interface IStringChange : IChange
    {
        public string Value { get; }
    }
    public interface IObjectChange : IChange
    {
        public object Value { get; }
    }
    public interface IIntChange : IChange
    {
        public int Value { get; }
    }
    public interface IBooleanChange : IChange
    {
        public bool Value { get; }
    }


}
