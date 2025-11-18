using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Xml.Linq;
using Newtonsoft.Json;
using Splat;
using Utility.Enums;
using Utility.Helpers;
using Utility.Helpers.NonGeneric;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Observables;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions;
using Type = System.Type;

namespace Utility.Models.Trees
{
    public readonly record struct ValueChanged(PropertyInfo PropertyInfo, object Value);

    public class TypeModel : Model<Type>, IBreadCrumb, IType
    {
        public TypeModel(string name) : this(Globals.Types.FirstOrDefault(t => t.Name == name) ?? throw new Exception("DSC££!!!C c"))
        {
        }

        public TypeModel(Type type)
        {
            this.Value = type;
        }

        public Type Type { get => Value as Type; set => Value = value; }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            if (Type != null)
                foreach (var prop in Type.GetProperties())
                {
                    var pnode = new PropertyModel { Name = prop.Name, Value = prop };
                    yield return pnode;
                }
        }

        public override void SubtractDescendant(IReadOnlyTree node, int level)
        {
        }
    }

    public class NodePropertyRootModel : ResolvableModel
    {
        public NodePropertyRootModel()
        {
            var type = Locator.Current.GetService<Type>();
            Assemblies.InsertSpecial(0, type.Assembly);
            Types.InsertSpecial(0, type);
        }

        public NodeModel NodeModel { get; set; }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            yield return NodeModel ??= new NodeModel { Name = "_node_" };
        }
    }

    public class NodeModel : Model, IBreadCrumb
    {
        public NodeModel()
        {
        }

        public NodePropertiesModel Model { get; set; }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            yield return Model ??= new NodePropertiesModel() { Name = "npm" };
        }

        public override void SubtractDescendant(IReadOnlyTree node, int level)
        {
            //base.SubtractDescendant(node, level);
        }
    }

    public class TypeConstants
    {
        public static Type[] ValueTypes = new[]{
             typeof(decimal),
        typeof(double),
        typeof(float),
        typeof(ulong),
        typeof(long),
        typeof(uint),
        typeof(int),
        typeof(ushort),
                typeof(short), typeof(byte), typeof(char), typeof(bool)};
    }

    public class NodePropertiesModel : Model, IBreadCrumb
    {
        private static Type[] types = new[]{
             typeof(string),
             typeof(object),
             typeof(decimal),
        typeof(double),
        typeof(float),
        typeof(ulong),
        typeof(long),
        typeof(uint),
        typeof(int),
        typeof(ushort),
                typeof(short), typeof(byte), typeof(char), typeof(bool)};

        public NodePropertiesModel()
        {
        }

        public NodePropertiesModel(Func<IEnumerable<IReadOnlyTree>> func) : base(func)
        {
        }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            if (childrenLambda != null)
            {
                foreach (var item in childrenLambda.Invoke())
                {
                    yield return item;
                }
                yield break;
            }
            Type[] _types = types;

            if (this.Ancestor(a => a.tree is ThroughPutModel { Parameter: { } }) is { } ancestor)
            {
                _types = [(ancestor as ThroughPutModel).Parameter.ParameterType, typeof(object)];
            }

            foreach (var prop in Locator.Current.GetService<IEnumerableFactory<PropertyInfo>>().Create(_types))
            {
                var pnode = new PropertyModel { Name = prop.Name, Value = prop };
                yield return pnode;
            }
        }

        public override void SubtractDescendant(IReadOnlyTree node, int level)
        {
        }
    }

    public class AssemblyModel : Model<Assembly>, IBreadCrumb, IGetAssembly
    {
        public AssemblyModel()
        {
        }

        public Assembly Assembly => Value as Assembly;

        public override string Name
        {
            get => Assembly?.FullName ?? name;
            set
            {
                name = value;
                if (Assembly == null)
                {
                    Value = Assembly.Load(value);
                }
                //base.RaisePropertyChanged();
            }
        }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            if (Assembly == null)
                throw new Exception("d90222fs sd");
            //var types = Assembly.ExportedTypes.Where(t => t.IsAssignableTo(typeof(Model)));

            foreach (Type type in Assembly.ExportedTypes.Where(GlobalModelFilter.Instance.TypePredicate.Invoke))
            {
                var _node = new TypeModel(type) { Name = type.Name };
                yield return _node;
            }
        }

        public static AssemblyModel Create(Assembly assembly) => new() { Value = assembly, Name = assembly.FullName };

        public override void SubtractDescendant(IReadOnlyTree node, int level)
        {
        }
    }

    public class GlobalModelFilter
    {
        public Predicate<Assembly> AssemblyPredicate { get; set; } = new Predicate<Assembly>(a => true);
        public Predicate<Type> TypePredicate { get; set; } = new Predicate<Type>(a => true);

        public static GlobalModelFilter Instance { get; } = new GlobalModelFilter();
    }

    /// <summary>
    ///
    /// </summary>
    public class GlobalAssembliesModel : Model, IBreadCrumb
    {
        public GlobalAssembliesModel(Func<IEnumerable<IReadOnlyTree>> predicate) : base(predicate)
        {
        }

        public GlobalAssembliesModel() : base()
        {
        }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            if (childrenLambda != null)
            {
                foreach (var x in childrenLambda.Invoke())
                    yield return x;
                yield break;
            }

            foreach (var prop in Locator.Current.GetService<IEnumerableFactory<PropertyInfo>>().Create(null))
            {
                yield return new AssemblyTypePropertyModel { Name = prop.PropertyType.Name + "." + prop.Name, Value = prop };
            }
            //yield return new PropertyModel { Name = prop.PropertyType.Name + "." + prop.Name, Value = prop };

            foreach (Assembly ass in AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().Where(GlobalModelFilter.Instance.AssemblyPredicate.Invoke))
            {
                var _node = AssemblyModel.Create(ass);
                yield return _node;
            }
        }
    }

    public class AssemblyTypePropertyModel : Model<PropertyInfo>, IBreadCrumb
    {
        public AssemblyTypePropertyModel()
        {
        }

        public Assembly Assembly => Type?.Assembly;

        public Type Type => Get()?.DeclaringType;
    }

    public class RelationModel : Model
    {
        private Relation relation;

        public Relation Relation
        {
            get => relation; set
            {
                relation = value;
                RaisePropertyChanged(ref relation, value);
            }
        }
    }

    public class PropertyModel : Model<PropertyInfo>, IBreadCrumb
    {
        public PropertyModel()
        {
        }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            if (Get()?.PropertyType == typeof(object))
            {
                yield return (new GlobalAssembliesModel { Name = "ass_root" });
            }
        }

        public override void SubtractDescendant(IReadOnlyTree node, int level)
        {
        }
    }

    public class ValueModel : Model<object>, IAutoList
    {
        private bool isListening;

        public ValueModel(object value) : base()
        {
            Value = value;
        }

        public ValueModel()
        { }

        public IEnumerable AutoList { get; set; }

        public bool IsListening { get => isListening; set => RaisePropertyChanged(ref isListening, value); }
    }

    public class MethodModel : Model<MethodInfo>, IBreadCrumb
    {
        public MethodModel()
        {
        }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            foreach (var p in Get()?.GetParameters() ?? Array.Empty<ParameterInfo>())
                yield return
                    new ParameterModel { Name = p.Name, Value = p };
        }
    }

    public class ParameterModel : Model<ParameterInfo>, IBreadCrumb
    {
        public ParameterInfo Parameter => Value as ParameterInfo;
    }

    public class MethodsModel : Model, IBreadCrumb
    {
        public override IEnumerable<IReadOnlyTree> Items()
        {
            foreach (var m in Locator.Current.GetService<IEnumerableFactory<MethodInfo>>().Create(nameof(MethodsModel)))
                yield return new MethodModel { Name = m.GetDescription(), Value = m };
        }
    }

    public class ExceptionModel(Exception Exception) : Model
    {
        public Exception Exception { get; } = Exception;

        public static ExceptionModel Create(Exception ex) => new ExceptionModel(ex) { Name = ex.Message };
    }

    public class ConverterModel : BreadCrumbModel, IRoot
    {
        private MethodInfo method;

        public ConverterModel()
        {
        }

        public MethodInfo Method
        {
            get => method; set
            {
                if (method != value)
                {
                    var _previous = method;
                    method = value;
                    RaisePropertyChanged(_previous, value);
                }
            }
        }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            yield return new MethodsModel { Name = "methods" };
        }

        public override void AddDescendant(IReadOnlyTree node, int level)
        {
            if (node is INodeViewModel _node)
                _node.WithChangesTo(a => a.Current)
                    .Where(a => a != null)
                    .Subscribe(a =>
                    {
                        if (a is MethodModel _m)
                        {
                            _m.WithChangesTo(a => a.Value).Update(this, a => a.Method);
                            //.Subscribe(a =>
                            //{
                            //    Method = _m.Value;
                            //});
                        }
                    });
        }
    }

    public class CommandModel<T> : CommandModel where T : Entities.Comms.Event
    {
        public CommandModel() : base(typeof(T))
        {
        }
    }

    public class CommandModel : Model, IGetType
    {
        private Type type;

        public event Action? Executed;

        public CommandModel(Type type)
        {
            Command = new Commands.Command(() => Utility.Globals.Events.OnNext((Entities.Comms.Event)Activator.CreateInstance(type, [this])));
            this.type = type;
        }

        public CommandModel()
        {
            Command = new Commands.Command(() => Executed?.Invoke());
            this.type = typeof(void);
        }

        //public override object Data { get => type; set => type = (Type)value; }

        public Type Type => type;

        public ICommand Command { get; }

        public new Type GetType()
        {
            if (Type == typeof(void))
            {
                return typeof(CommandModel);
            }
            Type[] typeArguments = { Type };
            Type genericType = typeof(CommandModel<>).MakeGenericType(typeArguments);
            return genericType;
        }
    }

    public class EditModel(Func<IEnumerable<IReadOnlyTree>>? func = null, Action<IReadOnlyTree>? addition = null, Action<EditModel>? attach = null) : Model(func, addition, a => attach?.Invoke((EditModel)a))
    {
        public EditModel() : this(null, null, null)
        {
        }

        public override object Value { get; set; }
    }

    public class RelationshipModel : Model
    {
        public RelationshipModel()
        {
        }

        public Relation? Relation { get; set; }

        public int? Level { get; set; }

        public IEnumerable<IReadOnlyTree> Filter(object instance)
        {
            if (instance is not INodeViewModel { } node)
            {
                throw new Exception("dsds4 34");
            }

            if (Relation == null)
                yield break;

            if (Relation.Value == Enums.Relation.Parent)
            {
                foreach (var parent in node.Ancestors(new Predicate<(IReadOnlyTree node, int index)>(a => Level.HasValue ? a.index == Level.Value : true)))
                {
                    yield return parent;
                }
            }
            else if (Relation.Value == Enums.Relation.Child)
            {
                bool b = false;
                foreach (var x in node.Descendants(new Predicate<(IReadOnlyTree node, int index)>(a => Level.HasValue ? a.index == Level.Value : true)))
                    yield return x;
            }
            else
            {
                yield return node;
            }
        }

        public override void AddDescendant(IReadOnlyTree node, int level)
        {
            if (node == null)
                throw new NullReferenceException();
            switch (node)
            {
                case Model<int> { Value: int value } indexModel:
                    {
                        indexModel.PropertyChanged += Level_PropertyChanged;
                        Level = value;
                        break;
                    }
                case RelationModel { Relation: { } value } Model:
                    {
                        Model.PropertyChanged += Model_PropertyChanged;
                        Relation = value;
                        break;
                    }
            }
        }

        private void Level_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Level = (int)sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Relation = (Relation)sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
            //Value = sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
            //foreach (var s in list)
            //{
            //    s.OnNext(new ValueChanged(Guid, Value));
            //}
            //subject.OnNext(resolve(instance));
        }

        private List<System.IObserver<ValueChanged>> list = new();

        public IDisposable Subscribe(System.IObserver<ValueChanged> observer)
        {
            list.Add(observer);
            return new ActionDisposable(() => list.Remove(observer));
        }
    }

    public class SelectionModel : Model
    {
        public const string _string = nameof(_string);
        public const string _type = nameof(_type);

        private Model<string> @string;
        private TypeModel type;

        public Model<string> String { get => @string; set => @string = value; }
        public TypeModel Type { get => type; set => type = value; }

        public SelectionModel()
        {
        }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            yield return new Model<string> { Name = _string };
            yield return new TypeModel((Type)null) { Name = _type };
        }

        public override void Addition(IReadOnlyTree a)
        {
            switch (a.ToString())
            {
                case _string: String = a as Model<string>; break;
                case _type: Type = a as TypeModel; break;
                default: throw new ArgumentOutOfRangeException("ds 33` 33kfl.. ");
            }
        }
    }

    public class FileModel : Model<string>, IFileSystemInfo
    {
        public FileModel()
        {
            Value = "C:\\";
            DataTemplate = "ReadOnlyStringTemplate";
        }

        public FileSystemInfo FileSystemInfo => new FileInfo(Get());
    }

    public class AliasFileModel : Model
    {
        public AliasFileModel()
        {
        }

        public Model<string> Alias { get; set; }

        public FileModel File { get; set; }
    }

    public readonly record struct DataFile(string TableName, string FileName, string FilePath);

    public class DataFileModel : Model<DataFile>
    {
        public DataFileModel()
        {
            this.IsRemovable = true;
            this.IsReplicable = true;
            this.IsValueSaved = true;
            this.DoesValueRequireLoading = true;
        }

        public virtual string TableName
        {
            get => ((DataFile)this.Value).TableName;
            set { this.Value = (Get() with { TableName = value }); }
        }

        public virtual string FileName
        {
            get => ((DataFile)this.Value).FileName;
            set { this.Value = (Get() with { FileName = value }); }
        }

        public virtual string FilePath
        {
            get => ((DataFile)this.Value).FilePath;
            set { this.Value = (Get() with { FilePath = value }); }
        }

        public override object Clone()
        {
            return new DataFileModel { Name = Name, Value = Value };
        }
    }

    public class ThroughPutModel : Model<string>
    {
        private const string element = nameof(element);
        private const string filter = nameof(filter);

        private AndOrModel _filter;
        private NodePropertyRootsModel _element;

        public ThroughPutModel()
        {
            IsExpanded = true;
            IsRemovable = true;
        }

        [JsonIgnore]
        public AndOrModel Filter
        {
            get => _filter;
            set
            {
                if (_filter != value)
                {
                    var _previous = _filter;
                    _filter = value;
                    this.RaisePropertyChanged(_previous, value);
                }
            }
        }

        [JsonIgnore]
        public NodePropertyRootsModel Element
        {
            get => _element;
            set
            {
                if (_element != value)
                {
                    var _previous = _element;
                    _element = value;
                    this.RaisePropertyChanged(_previous, value);
                }
            }
        }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            yield return new NodePropertyRootsModel { Name = element };
            yield return new AndOrModel { Name = filter };
        }

        public override void Addition(IReadOnlyTree addition)
        {
            switch (addition)
            {
                case NodePropertyRootsModel: Element = addition as NodePropertyRootsModel; break;
                case AndOrModel: Filter = addition as AndOrModel; break;
                //case converter: Converter = a.Data as ConverterModel; break;
                default: throw new ArgumentOutOfRangeException("ds 33` 33kfl.. ");
            }
        }

        public ParameterInfo Parameter
        {
            get { return this.Value is string s ? JsonConvert.DeserializeObject<ParameterInfo>(s) : null; }
            set
            {
                if (value != null)
                    this.Value = JsonConvert.SerializeObject(value);
            }
        }
    }

    public class ExpandedModel : Model
    {
        private readonly Action<INodeViewModel> action;

        public ExpandedModel(Func<IEnumerable<IReadOnlyTree>> func, Action<INodeViewModel> action = null) : base(func)
        {
            this.action = action;
        }

        public ExpandedModel() : base()
        {
            action?.Invoke(this);
            this.ConnectorPosition = Position2D.Right;
            //node.IsEditable = true;
            this.IsExpanded = true;
            this.Arrangement = Arrangement.Stack;
        }
    }

    public class FileNameModel : Model<string>
    {
    }

    public class DirectoryModel : Model<string>, IBreadCrumb, IFileSystemInfo
    {
        public DirectoryModel(bool includeFiles = true)
        {
            IncludeFiles = includeFiles;
            DataTemplate = "ReadOnlyStringTemplate";
            IsExpanded = false;
        }

        public FileSystemInfo FileSystemInfo { get => new DirectoryInfo((string)Get(nameof(Value))); set => Value = value.FullName; }

        public override string Name
        {
            get => FileSystemInfo?.FullName ?? this.name;
            set
            {
                this.name = value;
                if (FileSystemInfo == null)
                {
                    FileSystemInfo = new DirectoryInfo(value);
                }
            }
        }

        public bool IncludeFiles { get; }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            if (FileSystemInfo == null)
                throw new Exception("d90222fs sd");

            foreach (var d in Directory.EnumerateDirectories(FileSystemInfo.FullName).Select(item => new DirectoryModel(IncludeFiles) { Value = item, Name = item }))
            {
                yield return d;
            }
            if (IncludeFiles)
                foreach (var d in Directory.EnumerateFiles(FileSystemInfo.FullName).Select(item => new FileModel() { Value = item, Name = item }))
                {
                    yield return d;
                }
        }

        public static DirectoryModel Create(DirectoryInfo assembly, bool includeFiles = true) => new(includeFiles) { FileSystemInfo = assembly, Name = assembly.FullName };
    }

    //public class FilePathModel : CollectionModel<Model>
    //{
    //    const string fileName = nameof(fileName);
    //    const string directoryName = nameof(directoryName);
    //    private FilePath filePath;

    //    public FilePathModel()
    //    {
    //        this.IsExpanded = true;
    //        this.IsAugmentable = false;
    //    }

    //    public FilePath FilePath
    //    {
    //        get => filePath;
    //        set
    //        {
    //            filePath = value;
    //            RaisePropertyChanged(nameof(FilePath));
    //        }
    //    }

    //    public override IEnumerable<IReadOnlyTree> Items()
    //    {
    //        yield return new FileNameModel() { Name = fileName };
    //        yield return new DirectoryModel() { Name = directoryName };
    //    }

    //    public override void Addition(IReadOnlyTree add)
    //    {
    //        var _level = add.Level(this);

    //        switch (add)
    //        {
    //            case FileModel { Name: { } name } fileModel:
    //                fileModel
    //                    .WhenReceivedFrom(a => a.Value)
    //                    .Cast<string>()
    //                    .Subscribe(a =>
    //                    {
    //                        FilePath = FilePath with { FileName = a };

    //                    });
    //                break;
    //            case DirectoryModel { Name: { } name } dirModel:
    //                dirModel
    //                    .WhenReceivedFrom(a => a.Value)
    //                    .Cast<string>()
    //                    .Subscribe(a =>
    //                    {
    //                        FilePath = FilePath with { Directory = a };

    //                    });
    //                break;

    //        }
    //    }
    //}

    public class StringModel(Func<IEnumerable<IReadOnlyTree>>? func = null, Action<IReadOnlyTree>? addition = null, Action<StringModel>? attach = null) : Model<string>(func, addition, a => attach?.Invoke((StringModel)a))
    {
        public StringModel() : this(null, null, null)
        {
        }
    }

    public class ContentRootModel : Model
    {
        [JsonIgnore]
        [Child("selections")]
        public SelectionsModel SelectionsModel { get; set; }
    }

    //public class DirtyModel : Model, ICollectionItem, IIgnore
    //{
    //    public string SourceKey { get; set; }
    //    public string PropertyName { get; set; }
    //    public object OldValue { get; set; }
    //    public object NewValue { get; set; }
    //}

    //public class DirtyModels : CollectionModel<DirtyModel>, IIgnore
    //{
    //}

    public interface IIgnore
    {
    }
}