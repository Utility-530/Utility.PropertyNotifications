using System;
using Utility.Models;
//using Autofac;

namespace Utility.Nodify.Demo
{
    public class ViewModel : BaseViewModel
    {
        public Guid Guid => Guids.Default;

        private object _value;

        public object Value
        {
            get => _value;
            set => this.Set(ref _value, value);
        }
    }
}
