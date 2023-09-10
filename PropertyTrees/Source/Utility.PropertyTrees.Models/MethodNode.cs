using System.Reflection;
using System.Windows.Input;
using Utility.Commands;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Abstractions;
using static Utility.Observables.NonGeneric.ObservableExtensions;
using static Utility.Observables.Generic.ObservableExtensions;
using System.Collections;

namespace Utility.PropertyTrees
{
    public class MethodNode 
    {
        private readonly ICommand command;
        private object data;

        public MethodNode()
        {
            command = new ObservableCommand(a =>
            {
                //Observe<MethodParametersResponse, MethodParametersRequest>(new(MethodInfo, Data))
                //.Subscribe(a =>
                //{
                //    Output = MethodInfo.Invoke(data, a.Parameters);
                //});     
            }); 
        }
            
        public ICommand Command => command;

        public object[]? Parameters { get; set; } = new object[0];

        public object? Output { get; set; }

        public object Content => MethodInfo.Name;

        public object Data
        {
            get => data; set
            {
                data = value;              
            }
        }

        public virtual MethodInfo MethodInfo { get; set; }
        public INode Parent { get; set; }

        public IObservable Children => Empty();

        public IEnumerable Ancestors => throw new NotImplementedException();
    }
}