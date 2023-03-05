using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnitsNet.Units;
using Utility.Models;
using _ViewModel = Utility.ViewModels.ViewModel;

namespace UtilityWpf.Demo.Forms.ViewModels
{
    public class MeasurementsViewModel : _ViewModel
    {
        private LengthUnit? unit;

        public MeasurementsViewModel(string header, IReadOnlyCollection<MeasurementViewModel> collection) : base(header)
        {
            Children = new(collection);
            //  Intialise();
        }

        public override ObservableCollection<MeasurementViewModel> Children { get; } /*= new MeasurementViewModel[] { new MeasurementViewModel { Header = "asd", Value = 0 } };*/

        public LengthUnit? Unit { get => unit; set => unit = value; }
        public override Property Model { get; }
        //public override StringProperty Model { get; }
    }

    public class MeasurementViewModel
    {
        public string? Header { get; init; }
        public double Value { get; set; }
    }
}