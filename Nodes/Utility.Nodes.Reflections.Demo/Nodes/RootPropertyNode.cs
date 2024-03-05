using Splat;
using System.Reactive.Disposables;
using System.Reactive.Threading.Tasks;
using Utility.Changes;
using Utility.Nodes.Reflections;

namespace Utility.Nodes.Demo
{
    public class MasterSlaveNode : Node
    {
        bool flag;

        public int Value { get; set; }

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
                return Task.FromResult<IReadOnlyTree>(new MasterNode() { Parent = this });
            else if (value is 1)
                return Task.FromResult<IReadOnlyTree>(new SlaveNode() { Parent = this });

            throw new Exception("2r 11 4333");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }

    public class MasterNode : ReflectionNode
    {
        private bool hasMoreChildren = true;
        //static readonly Guid guid = Guid.Parse("c25b9ff5-54d2-4a73-9509-471d7c307fb0");

        public MasterNode() : base()
        {
            //GuidRepository.Instance.Insert(guid, "master", typeof(MasterModel));
        }

        public override IObservable<object?> GetChildren()
        {
            hasMoreChildren = false;
            return Observable.Create<Change<IMemberDescriptor>>(observable =>
            {
                return Locator.Current.GetService<ITreeRepository>()
                    .Protos()
                    .ToObservable()
                    .Subscribe(protos =>
                    {
                        int i = 0;
                        foreach (var proto in protos)
                        {
                            observable.OnNext(new(new CollectionItemDescriptor(proto, ++i, null) { Guid = proto.Guid }, Changes.Type.Add));
                        }
                    });
            });
        }

        //public override Task<IReadOnlyTree> ToNode(object value)
        //{
        //    if (value is IMemberDescriptor refl)
        //    {
        //        ReflectionNode node = new(refl)
        //        {
        //            Parent = this
        //        };
        //        return Task.FromResult((IReadOnlyTree)node);
        //    }
        //    throw new Exception("j00 888 ggjh7669");
        //}

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(hasMoreChildren);
        }
    }

    //public class ProtoNode : Node
    //{
    //    public ProtoNode(Proto data)
    //    {
    //        Data = data;
    //    }

    //    public override IObservable<object?> GetChildren()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override Task<bool> HasMoreChildren()
    //    {
    //        return Task.FromResult(false);
    //    }

    //    public override Task<IReadOnlyTree> ToNode(object value)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    public class SlaveNode : ReflectionNode
    {
        bool flag;
        public SlaveNode() : base()
        {
            EventListener.Instance.Subscribe(async data =>
            {
                if (data is PropertyDescriptor { Instance: Proto { Type: { } type, Name:{ } name,  Guid: { } guid } })
                {  
                    this.Data = new RootDescriptor(type, name) { Guid = guid }; ;
                    flag = false;
                    RefreshChildrenAsync();
                    flag = true;
                }
            });
        }

        public override async Task<bool> HasMoreChildren()
        {
            return Data != null && flag == false;
        }

        public override string ToString()
        {
            return nameof(SlaveNode);
        }
    }

    //public class ModelRootPropertyNode : ReflectionNode
    //{
    //    static readonly Guid guid = Guid.Parse("c25b9ff5-54d2-4a73-9509-471d7c307fb0");

    //    public ModelRootPropertyNode() : base(new RootDescriptor(Model) { Guid = guid })
    //    {
    //    }

    //    static Model Model { get; } = new();
    //}

    public class SelectionNode : ReflectionNode
    {
        bool flag;
        public SelectionNode() : base()
        {
            CustomDataTemplateSelector.Instance.Subscribe(async data =>
            {
                if (data is IMemberDescriptor { Guid: { } guid })
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
            return Data != null && flag == false;
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
            return Observable.Create<object>(observer =>
            {
                foreach (var x in new int[] { 0, 1, 2, 3 })
                    observer.OnNext(x);
                return Disposable.Empty;
            });
        }

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is 0 or 1 and { })
                return Task.FromResult<IReadOnlyTree>(new MasterSlaveNode() { Parent = this, Value = (int)value });
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

