using Fasterflect;
using System;
using System.Reflection;
using System.Windows.Input;
using Utility.Commands;
using Utility.Infrastructure;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Trees.Abstractions;
using static Utility.Observables.NonGeneric.ObservableExtensions;
using static Utility.Observables.Generic.ObservableExtensions;

namespace Utility.PropertyTrees
{
    public class MethodNode : BaseObject, IReadOnlyTree
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

        IEquatable IReadOnlyTree.Key => this.Key;

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

        public IReadOnlyTree Parent { get; set; }

        public IEnumerable Items
        {
            get
            {
                return Empty();
            }
        }

        public IEnumerable Ancestors => throw new NotImplementedException();


        public bool Equals(IReadOnlyTree? other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IReadOnlyTree> GetEnumerator()
        {
            throw new NotImplementedException();
        }

    }
}