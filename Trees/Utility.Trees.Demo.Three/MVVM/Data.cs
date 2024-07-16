using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;

namespace Utility.Trees.Demo.MVVM.MVVM
{
    public partial class Data
    {


    }

    public class Value : IValue, IType
    {
        public Type Type => typeof(object);

        object IValue.Value => null;
    }
}
