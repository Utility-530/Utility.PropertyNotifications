using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Demo.Infrastructure;
using Utility.Nodes.Reflections;
using Utility.Nodes.Reflections.Demo.Infrastructure;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using Utility.ViewModels;

namespace Utility.Nodes.Demo
{
    public class LedModelRootPropertyNode : ReflectionNode
    {
        static readonly Guid guid = Guid.Parse("2b581d2f-506d-439a-9822-229d831f73b0");
        public LedModelRootPropertyNode() : base(new RootDescriptor(Model) { Guid = guid})
        {
        }
        static LedModel Model { get; } = new();
    }


    public class ModelRootPropertyNode : ReflectionNode
    {
        static readonly Guid guid = Guid.Parse("c25b9ff5-54d2-4a73-9509-471d7c307fb0");

        public ModelRootPropertyNode() : base(new RootDescriptor(Model) { Guid = guid })
        {
        }

        static Model Model { get; } = new();
    }

    public class SelectionNode : ReflectionNode
    {
        bool flag;
        public SelectionNode() : base()
        {
            CustomDataTemplateSelector.Instance.Subscribe(async data =>
            {
                if (data is IMemberDescriptor {  Guid: { } guid } )
                {
                    var viewModel = ViewModelStore.Instance.Get(guid);
                    ViewModelStore.Instance.Save(viewModel);
                    if (string.IsNullOrEmpty(viewModel.Name))
                        viewModel.Name = (data as IMemberDescriptor)?.Name;
                    var propertyData = new RootDescriptor(viewModel) { Guid = guid };
                    this.Data = propertyData;
                    flag = false;
                    RefreshChildrenAsync();
                    flag = true;
                }
            });
        }

        public override async Task<bool> HasMoreChildren()
        {
            return Data != null && flag==false;
        }

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

        public override IObservable<object?> GetChildren()
        {
            flag = true;
            return Observable.Create<object>(observer=>
            {
                foreach (var x in new int[] { 0, 1, 2, 3 })
                    observer.OnNext(x);
                return Disposable.Empty;
            });
        }

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is 0 or 1)
                return Task.FromResult<IReadOnlyTree>(new ModelRootPropertyNode() { Parent = this });
            else if (value is 2)
                return Task.FromResult<IReadOnlyTree>(new SelectionNode() { Parent = this });
            else if (value is 3)
                return Task.FromResult<IReadOnlyTree>(new CustomMethodsNode() { Parent = this });

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
        public override IObservable<object?> GetChildren()
        {
            flag = true;
            return Observable.Create<object>(observer =>
            {
                foreach (var x in new int[] { 0, 1 })
                    observer.OnNext(x);
                return Disposable.Empty;
            });
        }

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is 0)
                return Task.FromResult<IReadOnlyTree>(new RefreshNode() { Parent = this });
            else if (value is 1)
                return Task.FromResult<IReadOnlyTree>(new SaveNode() { Parent = this });


            throw new Exception("2r 11 4333");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }
}

