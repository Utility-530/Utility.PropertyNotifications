using DryIoc.ImTools;
using System;


namespace Utility.WPF.Meta
{


    public record TypeKeyValue(string Key, Type Type) : KeyValue(Key)
    {
        public override Type Value => Type;
    }
}