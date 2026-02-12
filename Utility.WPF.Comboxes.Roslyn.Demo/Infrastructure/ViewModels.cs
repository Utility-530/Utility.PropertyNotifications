using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utility.PropertyNotifications;

namespace Utility.WPF.ComboBoxes.Roslyn.Demo
{
    public class ViewModel : NotifyPropertyClass
    {
        public string Name { get; set; }

    }

    public class BooleanViewModel : ViewModel
    {
        private bool value = false;

        public bool Value { get => value; set => this.RaisePropertyChanged(ref this.value, value); }
    }

    internal class EnumViewModel : ViewModel
    {
        private CommonTypes? value;

        public CommonTypes? Value { get => value; set => this.RaisePropertyChanged(ref this.value, value); }
    }

}
