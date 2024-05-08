using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Utility.Descriptors;
using Utility.Descriptors.Repositorys;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Nodes.Reflections;
using Utility.PropertyNotifications;

namespace Utility.Trees.Demo.MVVM.Infrastructure
{
    //Type t when t == typeof(string) => new StringValue(descriptor, value),
    //            Type t when t.BaseType == typeof(Enum) => new EnumValue(descriptor, value),
    //            Type t when t == typeof(bool) => new BooleanValue(descriptor, value),
    //            Type t when t == typeof(int) => new IntegerValue(descriptor, value),
    //            Type t when t == typeof(short) => new IntegerValue(descriptor, value),
    //            Type t when t == typeof(long) => new LongValue(descriptor, value),
    //            Type t when t == typeof(double) => new DoubleValue(descriptor, value),
    //            Type t when t == typeof(byte) => new ByteValue(descriptor, value),
    //            Type t when t == typeof(Guid) => new GuidValue(descriptor, value),
    //            Type t when t == typeof(DateTime) => new DateTimeValue(descriptor, value),
    //            Type t when t == typeof(Type) => new TypeValue(descriptor, value),

    //            Type t when t.IsNullableEnum() => new NullableEnumValue(descriptor, value),
    //            Type t when t == typeof(bool?) => new NullableBooleanValue(descriptor, value),
    //            Type t when t == typeof(int?) => new NullableIntegerValue(descriptor, value),
    //            Type t when t == typeof(short?) => new NullableIntegerValue(descriptor, value),
    //            Type t when t == typeof(long?) => new NullableLongValue(descriptor, value),
    //            Type t when t == typeof(double?) => new NullableDoubleValue(descriptor, value),
    //            Type t when t == typeof(byte?) => new NullableByteValue(descriptor, value),
    //            Type t when t == typeof(Guid?) => new NullableGuidValue(descriptor, value),
    //            Type t when t == typeof(DateTime?) => new NullableDateTimeValue(descriptor, value),




    public static class Keys
    {
        public static (Guid, string) Zero = new(Guid.Parse("8c58f0e5-a2de-46e2-873e-8075f44aa2c9"), "Zero");
        //public static (Guid, string) One = new(Guid.Parse("7f6eea88-6537-4b7a-8bed-8c310d3ae268"), "One");
        //public static (Guid, string) Two = new(Guid.Parse("e3728beb-ba2d-4958-81ae-0ab880365015"), "Two");
        //public static (Guid, string) Three = new(Guid.Parse("ad7ec1f9-baad-4eaa-9f84-95c283023b9e"), "Three");
        //public static (Guid, string) Four = new(Guid.Parse("9f4a7928-5ce8-4edb-b797-d03da125cb57"), "Four");
        //public static (Guid, string) Five = new(Guid.Parse("de715ef4-e8a3-4ddf-ab2e-b0d8521645df"), "Five");
        //public static (Guid, string) Six = new(Guid.Parse("fa3ef30a-4bb9-4e02-bd78-bcd757d48815"), "Six");
    }


    public class _RootNode : ModelTree
    {
        public static readonly System.Type[] Types = new[]{
                //typeof(void),
                typeof(string),
                typeof(Enum),
                typeof(bool),
                typeof(int),
                typeof(short),
                typeof(long),
                typeof(double),
                typeof(byte),
                typeof(Guid),
                typeof(DateTime),
                typeof(object),
        };

        static Guid guid = Guid.Parse("1e6eea88-6537-4b7a-8bed-8c310d3ae268");

        public _RootNode() : base(typeof(void))
        {
            Key = new GuidKey(guid);

            foreach (var type in Types)
            {
                this.Add(new ModelTree(type));
            }

            //this.Add(new ModelTree(typeof(object)));


            TreeRepository.Instance.InsertRoot(guid, "root", typeof(void));
        }
    }

    public class RootNode : ReflectionNode
    {
        private bool flag;
        private readonly Guid guid;

        //static Guid guid = Guid.Parse("ea3ef30a-4bb9-4e02-bd78-bcd757d48815");
        //Guid guid = Guid.Parse("25ee5731-11cf-4fc1-a925-50272fb99bba");
        public RootNode(Guid guid) : base()
        {
            Key = new GuidKey(guid);
            this.guid = guid;
        }

        public async Task Initialise(Type type, string name)
        {
            //var type = typeof(Utility.Trees.Demo.Models.Model);
            var instance = Activator.CreateInstance(type);
            var rootDescriptor = new RootDescriptor(type, name: name);
            rootDescriptor.SetValue(null, instance);
            var root= await DescriptorFactory.CreateRoot(rootDescriptor, guid);
            root.Initialise();
            this.Data = root;
            this.OnPropertyChanged(nameof(Data));
            //flag = false;
            //await base.RefreshChildrenAsync();
            //flag = true;
        }
        public override async Task<bool> HasMoreChildren()
        {
            return (Data as IDescriptor)?.Type.IsValueOrString() == false && flag == false;
        }

        public override string ToString()
        {
            return nameof(Utility.Trees.Demo.Models.Model);
        }
    }



    public class ViewModelTree : Tree
    {
        public ViewModelTree()
        {
            var viewModel = new ViewModel();
            Data = viewModel;


            viewModel.WhenReceived()
                      .Subscribe(a =>
                      {
                          var model = a.Value;
                          var guid = ((GuidKey)this.Key).Value;

                          TreeRepository.Instance.Find(guid, a.Name)
                          .ToObservable()
                          .Subscribe(guid =>
                          {
                              TreeRepository.Instance.Set(guid, model, DateTime.Now);
                          });
                      });

            viewModel.WhenCalled()
                .Subscribe(a =>
                {
                    var guid = ((GuidKey)this.Key).Value;
                    if (guid == default)
                        return;
                    TreeRepository.Instance.Find(guid, a.Name)
                    .ToObservable()
                    .Subscribe(guid =>
                    {
                        var key = TreeRepository.Instance.Get(guid);
                        if (key != null && viewModel.SetPrivateFieldValue(a.Name, key.Value.Value))
                            viewModel.RaisePropertyChanged(a.Name);
                    });
                });

        }
    }

    public class ModelTree : ObservableTree
    {
        private GuidKey key;
        public ModelTree(System.Type key)
        {
            this.key = new GuidKey(key.GUID);
            TreeRepository.Instance.InsertRoot(key.GUID, key.Name, key);
            var model = new Model() { Type = key, IsReadOnly = true };
            Data = model;
        }
        public override string Key { get => key; set => key = (GuidKey)value; }

        public ModelTree(string name, Guid guid, Guid parentGuid)
        {
            var type = TreeRepository.Instance.TypeId(typeof(void));
            TreeRepository.Instance.InsertByParent(parentGuid, name, typeId: type);

            var model = new Model();
            Data = model;
            model.WhenReceived()
                .Subscribe(a =>
                {
                    var guid = (this.key).Value;
                    if (guid == default)
                        return;
                    TreeRepository.Instance.Set(guid, a, DateTime.Now);
                });


            model.WhenCalled()
                .Subscribe(a =>
                {
                    var guid = (this.key).Value;
                    if (guid == default)
                        return;
                    TreeRepository.Instance.Find(guid, a.Name)
                    .ToObservable()
                    .Subscribe(guid =>
                    {
                        var key = TreeRepository.Instance.Get(guid);
                        if (key != null && model.SetPrivateFieldValue(a.Name, key))
                            model.RaisePropertyChanged(a.Name);
                    });
                });
        }

    }
}
