using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Nodes.Filters
{
    internal class Services
    {

        public bool Resolve(object Data, Assembly Assembly, Type Type, PropertyInfo PropertyInfo, object Value)
        {
            //if (instance is not Node { Data: { } data })
            //{
            //    throw new Exception("dsds4 34");
            //}

            var type = Data.GetType();

            //if (Assembly == null && CurrentNode == null)
            //    throw new Exception("dsds2222222222");

            if (Assembly == type.Assembly)
            {
                if (Type == null)
                    return true;
                if (type.Equals(Type))
                {
                    if (PropertyInfo == null)
                        return true;
                    if (PropertyInfo.GetValue(Data) is { } _value)
                    {
                        if (Value == null)
                        {
                            return true;
                        }
                        return _value.Equals(Value);
                    }
                }
            }
            return false;
        }
    }
}
