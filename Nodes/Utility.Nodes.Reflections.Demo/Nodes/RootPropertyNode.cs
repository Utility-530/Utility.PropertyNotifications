using Splat;
using System.Reactive.Disposables;
using System.Reactive.Threading.Tasks;
using Utility.Changes;
using Utility.Interfaces;
using Utility.Meta;
using Utility.Nodes.Reflections;
using Utility.Repos;

namespace Utility.Nodes.Demo
{
    public class MasterSlaveNode : Node<int>
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

        public override Task<ITree> ToTree(object value)
        {
            if (value is 0)
                return Task.FromResult<ITree>(new MasterNode() { Parent = this });
            else if (value is 1)
                return Task.FromResult<ITree>(new SlaveNode() { Parent = this });

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

        public MasterNode() : base()
        {
        }

        public override IObservable<object?> GetChildren()
        {
            hasMoreChildren = false;
            return Observable.Create<Change<IDescriptor>>(observable =>
            {
                return Locator.Current.GetService<ITreeRepository>()
                    .SelectKeys()
                    .Subscribe(async keys =>
                    {
                        int i = 0;
                        foreach (var key in keys)
                        {
                            if (key.Instance != null)
                            {
                                var collectionItemDescriptor = await DescriptorFactory.CreateItem(key, ++i, key.Instance.GetType()e, null, key.Guid);
                                collectionItemDescriptor.Initialise();
                                observable.OnNext(new(collectionItemDescriptor, Changes.Type.Add));
                            }
                        }
                    });
            });
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(hasMoreChildren);
        }
    }

    public class SlaveNode : ReflectionNode
    {
        bool flag;
        public SlaveNode() : base()
        {
            EventListener.Instance.Subscribe(async data =>
            {
                if (data is IInstance { Instance: Descriptors.Repositorys.Key { Type: { } type, Name: { } name, Guid: { } guid } })
                {
                    var instance = Activator.CreateInstance(type);
                    var rootDescriptor = new RootDescriptor(type);
                    this.Data = await DescriptorFactory.ToValue(instance, rootDescriptor, guid);
                    this.OnPropertyChanged(nameof(Data));
                    flag = false;
                    base.RefreshChildrenAsync();
                    flag = true;
                }
            });
        }

        public override async Task<bool> HasMoreChildren()
        {
            return (Data as IDescriptor)?.Type.IsValueOrString() == false && flag == false;
        }

        public override string ToString()
        {
            return nameof(SlaveNode);
        }
    }

    public class SelectionNode : ReflectionNode
    {
        bool flag;
        public SelectionNode() : base()
        {
            CustomDataTemplateSelector.Instance.Subscribe(async data =>
            {
                if (data is IDescriptor { Guid: { } guid })
                {
                    var viewModel = ViewModelStore.Instance.Get(guid);
                    ViewModelStore.Instance.Save(viewModel);
                    if (string.IsNullOrEmpty(viewModel.Name))
                        viewModel.Name = (data as IDescriptor)?.Name;
                    var rootDescriptor = new RootDescriptor(typeof(ViewModel)) {  };
                    var propertyData = await DescriptorFactory.ToValue(viewModel, rootDescriptor, guid);
                    propertyData.Set(viewModel);
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

    public class RootNode : Node<T>
    {
        bool flag;

        public RootNode() : base()
        {
        }

        public override object Data { get => "root"; set { } }

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

