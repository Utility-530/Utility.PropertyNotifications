using System;
using Utility.Models;
using Utility.ViewModels.Base;
//using Autofac;

namespace Utility.Nodify.Demo
{
    public class BooleanViewModel : BaseViewModel
    {
        public Guid Guid => Guids.Boolean;

        private bool _value;

        public bool Value
        {
            get => _value;
            set => this.Set(ref _value, value);
        }
    }
}
