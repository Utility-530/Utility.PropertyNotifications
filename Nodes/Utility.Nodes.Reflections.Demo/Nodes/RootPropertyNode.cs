using Fasterflect;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Demo.Infrastructure;
using Utility.Objects;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo
{
    public class RootPropertyNode : PropertyNode
    {
        private readonly int _index;

        public RootPropertyNode(int index) : base(new PropertyData(new RootDescriptor(Model), Model))
        {
            _index = index;
        }

        static LedModel Model { get; } = new LedModel();
    }


    public class SelectionNode : EmptyNode
    {
        object data;

        public SelectionNode()
        {
            CustomDataTemplateSelector.Instance.Subscribe(a =>
            {
                this.Data = a;                
            });
        }

        public override object Data { get => data; set { data = value; this.OnPropertyChanged(); } }
    }


    public class RootNode : Node
    {

        bool flag;

        public RootNode() : base()
        {
        }

        public override async Task<object?> GetChildren()
        {
            flag = true;
            return await Task.Run<object?>(() =>
            {
                return new int[] { 0, 1, 2 };
            });
        }

        public override ITree ToNode(object value)
        {
            if (value is 0 or 1)
                return new RootPropertyNode((int)value);
            else if (value is 2)
                return new SelectionNode();

            throw new Exception("2r 11 4333");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }


        //public override IEquatable Key => new Key(guid, "Root1", typeof(Model));
    }
}

