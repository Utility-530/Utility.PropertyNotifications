using Splat;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using Utility.Descriptors;

using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Nodes;
using Utility.PropertyNotifications;
using Utility.Repos;
using Utility.Trees.Abstractions;
using Utility.Trees.Demo.FilterBuilder.Infrastructure;
using Utility.Trees.WPF;
using Views.Trees;

namespace Utility.Trees.Demo.FilterBuilder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Assembly[] Assemblies => new Assembly[] { };

        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();
            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(TreeRepository.Instance);
            var rootNode = new RootNode() { Data = new Root() };
            var window = new Window() { Content = ModelTreeViewer(rootNode) };
            window.Show();
            base.OnStartup(e);
        }

        public static TreeViewer ModelTreeViewer(object model)
        {
            return new TreeViewer
            {
                ViewModel = model,
                TreeViewItemFactory = Default.TreeViewItemFactory.Instance,
                TreeViewBuilder = TreeViewBuilder.Instance,
                PanelsConverter = Default.ItemsPanelConverter.Instance,
                DataTemplateSelector = Data.DataTemplateSelector.Instance,
                TreeViewFilter = Model.Filter.Instance,
                StyleSelector = Model.StyleSelector.Instance,
                EventListener = Default.EventListener.Instance
            };
        }
    }

    //public class ViewModelTree : Node
    //{
    //    public override IObservable<object?> GetChildren()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override Task<IReadOnlyTree> ToNode(object value)
    //    {
    //        return Task.FromResult((IReadOnlyTree)new Tree() { Data =value});
    //    }
    //}

    public class Root : IChildren
    {
        public IObservable<object> Children => Observable.Return(new Changes.Change<ViewModel>(new ViewModel(TypeNode.guid), Changes.Type.Add));
    }


    public class RootNode : Node<ViewModel>
    {
        static Guid _guid = Guid.Parse("25ee5731-11cf-4fc1-a925-50272fb99bba");

        public RootNode():base(false)
        {

        }

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            return Task.FromResult((IReadOnlyTree)new TypeNode() { Data = value, Key = new GuidKey(_guid) });
        }
    }


    public class TypeNode : Node<IDescriptor>, IExpand
    {

        public static Guid guid = Guid.Parse("ea3ef30a-4bb9-4e02-bd78-bcd757d48815");

        public TypeNode() : base()
        {
            Key = new GuidKey(guid);
        }

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            return Task.FromResult((IReadOnlyTree)new PropertyNode() { Data = value, Key = new GuidKey(guid) });
        }

        public override string ToString()
        {
            return nameof(TypeNode);
        }
    }


    public class PropertyNode : Node<IDescriptor>, IExpand
    {

        static Guid _guid = Guid.Parse("45ee5731-11cf-4fc1-a925-50272fb99bba");

        public PropertyNode() : base()
        {
            Key = new GuidKey(_guid);
        }

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            return Task.FromResult((IReadOnlyTree)new TypeNode() { Data = value, Key = new GuidKey(_guid) });
        }

        public override string ToString()
        {
            return nameof(TypeNode);
        }
    }


    public record ViewModel : NotifyProperty, IChildren
    {
        ReplaySubject<object> changes = new();

        private Type? type;
        private readonly Guid guid;

        public ViewModel(Guid guid)
        {
            this.guid = guid;
        }

        public Type Type
        {
            get
            {

                this.RaisePropertyCalled(type);
                return type;

            }
            set
            {
                if (value.Equals(type))
                    return;
                type = value;
                Initialise(value, default);
                this.RaisePropertyReceived(value);
            }
        }


        public void Initialise(System.Type type, string name)
        {
            //var type = typeof(Utility.Trees.Demo.Models.Model);
            var instance = Activator.CreateInstance(type);
            var rootDescriptor = new RootDescriptor(type, name: name) { };
            rootDescriptor.SetValue(null, instance);
            //root = await DescriptorFactory.CreateRoot(rootDescriptor, guid);

            var root = new PropertiesDescriptor(rootDescriptor, instance) { Guid = guid };
            root.Children.Subscribe(changes);
            //RefreshChildrenAsync();
            //root.Initialise();
            //this.Data = root;
            //this.OnPropertyChanged(nameof(Data));
        }

        public IObservable<object> Children => changes;
    }


    public record PropertyModel : NotifyProperty, IChildren
    {
        ReplaySubject<object> changes = new();

        private Type? type;

        public Type Type
        {
            get
            {

                this.RaisePropertyCalled(type);
                return type;

            }
            set
            {
                if (value.Equals(type))
                    return;
                type = value;
                Initialise(value, default);
                this.RaisePropertyReceived(value);
            }
        }


        public void Initialise(System.Type type, string name)
        {
            //var type = typeof(Utility.Trees.Demo.Models.Model);
            var instance = Activator.CreateInstance(type);
            var rootDescriptor = new RootDescriptor(type, name: name);
            rootDescriptor.SetValue(null, instance);
            //root = await DescriptorFactory.CreateRoot(rootDescriptor, guid);

            var root = new PropertiesDescriptor(rootDescriptor, instance);
            root.Children.Subscribe(changes);
            //RefreshChildrenAsync();
            //root.Initialise();
            //this.Data = root;
            //this.OnPropertyChanged(nameof(Data));
        }

        public IObservable<object> Children => changes;
    }
}