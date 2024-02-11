using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Utility.Models;

namespace Utility.Nodes.Demo.Infrastructure
{
    public class Model
    {
        //public Property[] Models { get; } = new[] { new Property() {/* Orientation = Orientation.Vertical*/ }, new Property() {  BooleanValue=true } };

        public ObservableCollection<Property> Models2 { get; } = new ObservableCollection<Property> { new Property() {/* Orientation = Orientation.Vertical*/ }, new Property() { BooleanValue = true } };

        //public void Run(int inter)
        //{
        //}

        //public void End(bool bArg)
        //{
        //}

        //public void Test(Property inter)
        //{
        //}
    }                                    

    public class Property
    {
        public int Value { get; } = 1;
        public bool BooleanValue { get; set; } = false;
    }
}

