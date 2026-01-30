using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Attributes
{
    public class GuidAttribute: Attribute
    {
        public GuidAttribute(Guid guid)
        {
            Guid = guid;
        }
        public GuidAttribute(string guid)
        {
            Guid = Guid.Parse(guid);
        }

        public Guid Guid { get; }
    }
}
