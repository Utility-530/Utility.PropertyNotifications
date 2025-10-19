using System;
using System.Collections.Generic;
using System.Text;
using Utility.Enums;

namespace Utility.Meta
{
    public class EntityMetaData
    {
        public Type Type { get; set; }

        public string TransformationMethod { get; set; }

        public Guid Guid { get; set; }

        public PropertyMetaData[] Properties { get; set; } 
        public int Index { get; set; }
    }

    public class PropertyMetaData()
    {

        public string Name { get; set; }

        public DataType DataType { get; set; }
        public int ColumnWidth { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
