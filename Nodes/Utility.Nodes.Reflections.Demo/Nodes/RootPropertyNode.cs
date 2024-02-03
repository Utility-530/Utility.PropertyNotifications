using Fasterflect;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using TreeCollections;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Demo.Infrastructure;
using Utility.Nodes.Reflections.Demo.Infrastructure;
using Utility.Objects;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using Utility.ViewModels;

namespace Utility.Nodes.Demo
{
    public class LedModelRootPropertyNode : RootPropertyNode
    {
        Guid guid = Guid.Parse("2b581d2f-506d-439a-9822-229d831f73b0");
        public LedModelRootPropertyNode() : base(Model)
        {
        }

        public override IEquatable Key => new Key(guid, "root", typeof(LedModel));

        static LedModel Model { get; } = new();
    }


    public class ModelRootPropertyNode : RootPropertyNode
    {
        Guid guid = Guid.Parse("c25b9ff5-54d2-4a73-9509-471d7c307fb0");
        public ModelRootPropertyNode() : base(Model)
        {
        }

        public override IEquatable Key => new Key(guid, "root", typeof(Model));

        static Model Model { get; } = new();
    }

    public class SelectionNode : PropertyNode
    {
        Guid guid = Guid.Parse("46f59dbd-680e-4c7d-ab35-28c1ba8cdcd3");

        public SelectionNode() : base()
        {
            CustomDataTemplateSelector.Instance.Subscribe(async data =>
            {
                if (data is IReadOnlyTree { Key: Key { Guid: { } guid } })
                {
                    var viewModel = ViewModelStore.Instance.Get(guid);
                    ViewModelStore.Instance.Save(viewModel);
                    if (string.IsNullOrEmpty(viewModel.Name))
                        viewModel.Name = ((data as IReadOnlyTree)?.Data as PropertyData)?.Name;
                    var propertyData = new PropertyData(new RootDescriptor(viewModel), viewModel) { };
                    this.Data = propertyData;
                    flag = false;
                    await RefreshChildrenAsync();
                }
            });
        }

        public override IEquatable Key => new Key(guid, "root", typeof(ViewModel));
    }


    public class RefreshNode : EmptyNode
    {

        public RefreshNode()
        {

        }
    }

    public class SaveNode : EmptyNode
    {

        public SaveNode()
        {

        }
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
                return new int[] { 0, 1, 2, 3 };
            });
        }

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is 0 or 1)
                return Task.FromResult<IReadOnlyTree>(new ModelRootPropertyNode());
            else if (value is 2)
                return Task.FromResult<IReadOnlyTree>(new SelectionNode());
            else if (value is 3)
                return Task.FromResult<IReadOnlyTree>(new CustomMethodsNode());

            throw new Exception("2r 11 4333");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }

    public class CustomMethodsNode : Node
    {
        bool flag;
        public override async Task<object?> GetChildren()
        {
            flag = true;
            return await Task.Run<object?>(() =>
            {
                return new int[] { 0, 1 };
            });
        }

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is 0)
                return Task.FromResult<IReadOnlyTree>(new RefreshNode());
            else if (value is 1)
                return Task.FromResult<IReadOnlyTree>(new SaveNode());


            throw new Exception("2r 11 4333");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }
}

