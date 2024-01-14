using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Utility.Models;

namespace Utility.Nodes.Demo.Infrastructure
{
    public class Model
    {
        //public int Value { get; } = 1;

        //public List<string> List { get; } = new List<string> { "a", "b" };

        //public bool IsTrue { get; set; } = true;
        //public double Number { get; set; } = 0.094;

        //public Guid Guid { get; set; }


        public Property[] Models { get; } = new[] { new Property() { Orientation = Orientation.Vertical }, new Property() };
    }

    public class Property
    {
        public Orientation Orientation { get; set; }
    }
}

