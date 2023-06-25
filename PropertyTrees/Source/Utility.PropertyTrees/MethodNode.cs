using Fasterflect;
using System;
using System.Reflection;
using System.Windows.Input;
using Utility.Commands;
using Utility.Infrastructure;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Abstractions;
using static Utility.Observables.NonGeneric.ObservableExtensions;
using static Utility.Observables.Generic.ObservableExtensions;

namespace Utility.PropertyTrees
{
    public class MethodNode : BaseObject, INode
    {
        private readonly ICommand command;
        private readonly Guid guid;
        private object data;

        public MethodNode(Guid guid)
        {
            this.guid = guid;

            command = new ObservableCommand(a =>
            {
                Observe<MethodParametersResponse, MethodParametersRequest>(new(MethodInfo, Data))
                .Subscribe(a =>
                {
                    Output = MethodInfo.Invoke(data, a.Parameters);
                });     
            }); 
        }

        public override Key Key => new(guid, MethodInfo.Name, MethodInfo.ReturnType);

        IEquatable INode.Key => this.Key;

        public Guid Guid => guid;

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

        public IObservable Children
        {
            get
            {
                return Empty();
            }
        }

        public IEnumerable Ancestors => throw new NotImplementedException();

    }
}