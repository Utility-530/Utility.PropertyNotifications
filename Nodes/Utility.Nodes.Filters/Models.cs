using Bogus.DataSets;
using Jellyfish;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Xml.Linq;
using Utility.Changes;
using Utility.Enums;
using Utility.Extensions;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Observables;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees;
using Utility.Trees.Abstractions;
using Type = System.Type;

namespace Utility.Nodes.Filters
{
    public interface IAndOr
    {
        AndOr Value { get; }
    }

    public interface IResolvable
    {
        bool IsEqual(object _value);
    }


    public readonly record struct ValueChanged(PropertyInfo PropertyInfo, object Value);

    public class TypeModel : Model, IBreadCrumb
    {
        public Type Type { get; set; }


        public override IEnumerable<Node> CreateChildren()
        {
            if (Type != null)
                foreach (var prop in Type.GetProperties())
                {
                    var pnode = new Node(prop.Name, new PropertyModel { Name = prop.Name, PropertyInfo = prop }) { };
                    yield return pnode;

                }
        }
    }


    public interface IValue
    {
        object Value { get; set; }
    }

    public class NodeRootModel : NodePropertyRootModel, IValue, IObservable<ValueChanged>, IRoot
    {
        List<IObserver<ValueChanged>> list = new();
        private object? value;

        public NodeRootModel()
        {
        }


        public object? Value
        {
            get => value; set
            {

                this.value = value;
            }
        }

        public override void AddDescendant(IReadOnlyTree node, int level)
        {
            if (node == null)
                throw new NullReferenceException();

            switch (node.Data)
            {
                case ValueModel { Value: var value } valueModel:
                    {
                        valueModel.PropertyChanged -= ValueModel_PropertyChanged;
                        valueModel.PropertyChanged += ValueModel_PropertyChanged;
                        this.Value = value;
                        break;
                    }
            }

            base.AddDescendant(node, level);
        }

        List<ValueChanged> values = new();

        private void ValueModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var prop = sender.GetType().GetProperty(e.PropertyName);
            Value = prop.GetValue(sender);
            values.Add(new ValueChanged(prop, Value));

            foreach (var s in list)
            {
                s.OnNext(new ValueChanged(prop, Value));
            }
        }

        public override bool TryGetValue(object instance, out object? value)
        {
            Update(Node, (Node)Node.Current);

            //var type = instance.GetType();
            if (base.TryGetValue(instance, out value))
            {
                if (Value?.Equals(value) ?? false)
                    return true;
            }
            return false;
        }

        public IDisposable Subscribe(IObserver<ValueChanged> observer)
        {
            list.Add(observer);
            return new ActionDisposable(() => list.Remove(observer));
        }
    }

    public class NodePropertyRootModel : ResolvableModel
    {
        public NodePropertyRootModel()
        {
            Assemblies.InsertSpecial(0, typeof(Node).Assembly);
            Types.InsertSpecial(0, typeof(Node));
        }

        public ParameterInfo Parameter { get; set; }

        public override IEnumerable<Node> CreateChildren()
        {
            yield return new Node("_node_", new NodeModel { Name = "_node_" });
        }
    }



    public class NodeModel : Model, IBreadCrumb
    {

        public NodeModel()
        {

        }

        public override IEnumerable<Node> CreateChildren()
        {
            yield return (new Node("npm", new NodePropertiesModel() { Name = "npm" }) { });
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

        public override IEnumerable<Node> CreateChildren()
        {
            foreach (var prop in typeof(Node).GetProperties().Where(a => types.Any(x => a.PropertyType.Equals(x))))
            {
                var pnode = new Node(prop.Name, new PropertyModel { Name = prop.Name, PropertyInfo = prop }) { };
                yield return (pnode);
            }
        }
    }

    public class AssemblyModel : Model, IBreadCrumb
    {
        public Assembly Assembly { get; set; }

        public override IEnumerable<Node> CreateChildren()
        {
            if (Assembly == null)
                throw new Exception("d90222fs sd");
            var types = Assembly.ExportedTypes.Where(t => t.IsAssignableTo(typeof(Model)));

            foreach (Type type in types)
            {
                var _node = new Node(type.Name, new TypeModel { Name = type.Name, Type = type }) { };
                yield return (_node);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GlobalAssembliesModel : Model, IBreadCrumb
    {
        public override IEnumerable<Node> CreateChildren()
        {
            foreach (Assembly ass in new[] { typeof(IndexModel).Assembly })
            {
                var _node = new Node(ass.GetName().Name, new AssemblyModel { Name = ass.GetName().Name, Assembly = ass }) { };
                yield return (_node);
            }
        }
    }

    public class RelationModel : Model
    {
        private Relation relation;

        public Relation Relation
        {
            get => relation; set
            {
                relation = value;
                this.RaisePropertyChanged();
            }
        }
    }

    public class PropertyModel : Model, IBreadCrumb
    {
        public PropertyInfo PropertyInfo { get; set; }

        public override IObservable<Change<Node>> ChildrenAsync()
        {
            return Observable.Create<Change<Node>>(observer =>
            {
                return Node.SelfAndAncestorsAsync(a => a.Item1?.Data is BreadCrumbModel)
                            .Subscribe(ancestor =>
                            {
                                if (ancestor.Data is IValue)
                                {
                                    if (PropertyInfo.PropertyType.Equals(typeof(string)))
                                    {
                                        observer.OnNext(Change<Node>.Add(
                                            new Node("value", new ValueModel { Name = PropertyInfo.Name, Value = string.Empty })
                                            { }));
                                    }
                                    else if (TypeConstants.ValueTypes.Contains(PropertyInfo.PropertyType))
                                    {
                                        observer.OnNext(Change<Node>.Add(
                                            new Node("value", new ValueModel { Name = PropertyInfo.Name, Value = Activator.CreateInstance(PropertyInfo.PropertyType) })
                                            { }));
                                    }
                                }

                                if (PropertyInfo.PropertyType == typeof(object))
                                {
                                    //observer.OnNext(
                                    //    new Node("ref_value", new ResolvableModel { Name = PropertyInfo.Name })
                                    //    { IsExpanded = true });

                                    observer.OnNext(
                                        Change<Node>.Add(new Node("ass_root", new GlobalAssembliesModel { Name = "ass_root" }) { }));
                                }
                            });
            });
        }
    }

    public class ValueModel : Model, IBreadCrumb
    {
        private object _value;

        public object Value
        {
            get => _value;
            set
            {
                this._value = value;
                this.RaisePropertyChanged();
            }
        }
    }



    public class MethodModel : Model, IBreadCrumb
    {
        public MethodInfo MethodInfo { get; set; }


        public override IEnumerable<Node> CreateChildren()
        {
            foreach (var p in MethodInfo.GetParameters())
                yield return
                    new Node("method", new ParameterModel { Name = p.Name, Parameter = p })
                    { };
        }
    }

    public class ParameterModel : Model, IBreadCrumb
    {
        public ParameterInfo Parameter { get; set; }

    }

    public class MethodsModel : Model, IBreadCrumb
    {
        public override IEnumerable<Node> CreateChildren()
        {
            foreach (var m in typeof(Methods).StaticMethods())
                yield return
                    new Node("method", new MethodModel { Name = m.Item1, MethodInfo = m.Item2 })
                    { };
        }
    }

    public class ExceptionModel(Exception Exception) : Model
    {

        public ExceptionModel() : this(null)
        {
        }

        public Exception Exception { get; } = Exception;
    }
    
    public class ExceptionsModel() : Model
    {
        public ObservableCollection<Exception> Exceptions { get; } = [];
    }


    public class ConverterModel : BreadCrumbModel, IRoot
    {
        private object output;
        private MethodInfo method;

        public MethodInfo Method
        {
            get => method; set
            {
                method = value; RaisePropertyChanged();
            }
        }

        public override IEnumerable<Node> CreateChildren()
        {
            yield return new Node("methods", new MethodsModel { Name = "methods" }) { };

        }

        public override void AddDescendant(IReadOnlyTree node, int level)
        {
            if (node is Node _node)
                _node.WithChangesTo(a => a.Current)
                    .Where(a => a != null)
                    .Subscribe(a =>
                    {
                        if (a.Data is MethodModel _m)
                        {
                            Method = _m.MethodInfo;
                        }
                    });
        }

        public override void SetNode(Node node)
        {
            node.IsPersistable = true;
            //node.Items.CollectionChanged += Items_CollectionChanged;
            base.SetNode(node);


            //void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            //{
            //    if (node.Current == null && e.NewItems is IEnumerable newItems)
            //    {
            //        foreach (Node item in newItems)
            //        //node.Current = item;
            //        {
            //            if (item.Name == "methods")
            //            {

            //            }
            //        }
            //    }
            //}
        }
    }


    public class CommandModel : Model
    {
        protected Unit m_name;

        public CommandModel()
        {
            Command = new RelayCommand((a) => base.RaisePropertyChanged(null));
        }


        public ICommand Command { get; }
    }

    public class SearchModel : Model
    {
        private string searchText;

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                RaisePropertyChanged();
            }
        }
    }

    public class IndexModel : Model
    {
        protected int value = 0;

        public int Value
        {
            get => value;
            set
            {
                this.value = value;
                base.RaisePropertyChanged();
            }
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class StringModel : Model
    {
        protected string value;

        public string Value
        {
            get => value;
            set
            {
                this.value = value;
                base.RaisePropertyChanged();
            }
        }

        public override string ToString()
        {
            return value?.ToString() ?? "_unknown_";
        }
    }
    
    public class HtmlModel : Model
    {
        protected string value;

        public string Value
        {
            get => value;
            set
            {
                this.value = value;
                base.RaisePropertyChanged();
            }
        }

        public override string ToString()
        {
            return value?.ToString() ?? "_unknown_";
        }
    }

    public class GuidModel : Model
    {
        protected Guid value;

        public Guid Value
        {
            get => value;
            set
            {
                this.value = value;
                base.RaisePropertyChanged();
            }
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class AndOrModel : Model, IAndOr, IObservable<Unit>
    {
        protected AndOr value = 0;
        List<IObserver<Unit>> observers = new List<IObserver<Unit>>();
        List<AndOrModel> list = new List<AndOrModel>();
        List<FilterModel> filters = new List<FilterModel>();

        public AndOrModel()
        {
        }

        public AndOr Value
        {
            get => value;
            set
            {
                this.value = value;
                base.RaisePropertyChanged();
            }
        }

        public bool Get(object data)
        {
            if (Value == AndOr.And)
            {
                return list.All(x => x.Get(data)) && filters.All(a => a.Get(data));
            }
            else if (Value == AndOr.Or)
            {
                return list.Any(x => x.Get(data)) || filters.Any(a => a.Get(data));
            }
            else
                throw new ArgumentOutOfRangeException("sd 3433 33 x");
        }

        Dictionary<Model, IDisposable> dictionary = new();


        public override void Addition(Node value, Node a)
        {
            if (a.Data is AndOrModel aoModel)
            {
                list.Add(aoModel);
                {
                    var dis = aoModel.Subscribe(_ => onNext());
                    dictionary[aoModel] = dis;
                }
            }
            else if (a.Data is FilterModel filterModel)
            {
                filters.Add(filterModel);
                {
                    System.Reactive.Disposables.CompositeDisposable composite = new();
                    filterModel
                        .WithChangesTo(a => a.ResolvableModel)
                        .Subscribe(a =>
                        {
                            a.Types.Changes().Subscribe(a =>
                            {
                                onNext();
                            }).DisposeWith(composite);
                            a.Properties.Changes().Subscribe(a =>
                            {
                                onNext();
                            }).DisposeWith(composite);
                            a.Subscribe(_a =>
                            {
                                onNext();
                            }).DisposeWith(composite);
                        }).DisposeWith(composite);
                    dictionary[filterModel] = composite;

                }
            }
            else
                throw new Exception("7 hhjkhj9099   ");

            void onNext()
            {
                foreach (var obs in observers)
                    obs.OnNext(Unit.Default);
            }
        }


        public override void Subtraction(Node value, Node a)
        {
            if (a.Data is AndOrModel aoModel)
            {
                dictionary[aoModel].Dispose();
                list.Remove(aoModel);
            }
            else if (a.Data is FilterModel filterModel)
            {
                dictionary[filterModel]?.Dispose();
                filters.Remove(filterModel);
            }
            else
                throw new Exception("877 hhj9099   ");
        }

        public override void SetNode(Node node)
        {
            node.IsPersistable = true;
            node.IsEditable = true;
            base.SetNode(node);
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            observers.Add(observer);
            return new ActionDisposable(() => observers.Remove(observer));
        }

        public override IEnumerable Proliferation()
        {
            yield return new AndOrModel() { Name = "andor" };
            yield return new FilterModel() { Name = "filter" };
        }
    }


    public class RelationshipModel : Model
    {
        public Relation? Relation { get; set; }

        public int? Level { get; set; }

        public IEnumerable<IReadOnlyTree> Filter(object instance)
        {
            if (instance is not Node { Data: { } data } node)
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
            switch (node.Data)
            {
                case IndexModel { Value: { } value } indexModel:
                    {
                        indexModel.PropertyChanged += Level_PropertyChanged;
                        Level = value;
                        break;
                    }
                case RelationModel { Relation: { } value } valueModel:
                    {
                        valueModel.PropertyChanged += ValueModel_PropertyChanged;
                        Relation = value;
                        break;
                    }
            }
        }

        private void Level_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Level = (int)sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
        }

        private void ValueModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Relation = (Relation)sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
            //Value = sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
            //foreach (var s in list)
            //{
            //    s.OnNext(new ValueChanged(Guid, Value));
            //}
            //subject.OnNext(resolve(instance));
        }

        List<IObserver<ValueChanged>> list = new();

        public IDisposable Subscribe(IObserver<ValueChanged> observer)
        {
            list.Add(observer);
            return new ActionDisposable(() => list.Remove(observer));
        }
    }
    public class BooleanModel : Model
    {
        private bool _value;


        public BooleanModel()
        {

        }

        public bool Value
        {
            get => _value; set
            {
                _value = value;
                this.RaisePropertyChanged();
            }
        }

        public override void SetNode(Node node)
        {
            node.IsPersistable = true;
            base.SetNode(node);
        }
    }

    public interface ISet
    {
        void Set(object instance, object v);
    }

    public class SetModel : ResolvableModel, /*ISet,*/ IValue, IObservable<ValueChanged>
    {
        private object value;
        List<IObserver<ValueChanged>> list = new();
        private List<ValueChanged> values = [];



        public object Value
        {
            get => value; set
            {

                this.value = value;
            }
        }
        public override void AddDescendant(IReadOnlyTree node, int level)
        {
            if (node == null)
                throw new NullReferenceException();
            switch (node.Data)
            {
                case ValueModel { Value: var value } valueModel:
                    {
                        valueModel.PropertyChanged -= ValueModel_PropertyChanged;
                        valueModel.PropertyChanged += ValueModel_PropertyChanged;
                        this.Value = value;
                        break;
                    }
            }
        }

        public override void ReplaceDescendant(IReadOnlyTree @new, IReadOnlyTree old, int level)
        {
            switch (@new.Data)
            {
                case ValueModel { Value: var value } valueModel:
                    {
                        valueModel.PropertyChanged += ValueModel_PropertyChanged;
                        this.Value = value;
                        break;
                    }
            }

            switch (old.Data)
            {
                case ValueModel { Value: var value } valueModel:
                    {
                        valueModel.PropertyChanged -= ValueModel_PropertyChanged;
                        this.Value = value;
                        break;
                    }
            }


        }
        public override void SubtractDescendant(IReadOnlyTree @new, int level)
        {
            switch (@new.Data)
            {
                case ValueModel { Value: var value } valueModel:
                    {
                        valueModel.PropertyChanged -= ValueModel_PropertyChanged;
                        this.Value = null;
                        break;
                    }
            }
        }

        private void ValueModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var prop = sender.GetType().GetProperty(e.PropertyName);
            Value = prop.GetValue(sender);
            values.Add(new ValueChanged(prop, Value));

            foreach (var s in list)
            {
                s.OnNext(new ValueChanged(prop, Value));
            }
        }



        public IDisposable Subscribe(IObserver<ValueChanged> observer)
        {
            foreach (var value in values)
            {
                observer.OnNext(value);
            }
            list.Add(observer);
            return new ActionDisposable(() => list.Remove(observer));
        }
    }


    public class TransformersModel : Model
    {
        [JsonIgnore]
        public ObservableCollection<TransformerModel> Transformers { get; } = new();

        public override void SetNode(Node node)
        {
            node.Orientation = Orientation.Vertical;
            node.IsEditable = true;
            node.IsExpanded = true;
            node.IsPersistable = true;
            node.Items.AndAdditions<Node>().Subscribe(a => Transformers.Add(a.Data as TransformerModel));
            base.SetNode(node);
        }

        public override IEnumerable Proliferation()
        {
            yield return new TransformerModel { Name = "transformer" };
        }
    }

    public class SelectionModel : Model
    {
        public const string _string = nameof(_string);
        //public const string _guid = nameof(_guid);
        public const string _type = nameof(_type);

        private StringModel @string;
        //private GuidModel guid;
        private TypeModel type;

        public StringModel String { get => @string; set => @string = value; }
        //public GuidModel Guid { get => guid; set => guid = value; }
        public TypeModel Type { get => type; set => type = value; }

        public override IEnumerable<Node> CreateChildren()
        {
            yield return new Node(_string, new StringModel { Name = _string }) { IsPersistable = true };
            //yield return new Node(_guid, new GuidModel { Name = _guid }) { IsExpanded = true };
            yield return new Node(_type, new TypeModel { Name = _type }) {  };
        }

        public override void Addition(Node value, Node a)
        {
            switch (a.Name)
            {
                case _string: String = a.Data as StringModel; break;
                //case _guid: Guid = a.Data as GuidModel; break;
                case _type: Type = a.Data as TypeModel; break;
                //case converter: Converter = a.Data as ConverterModel; break;
                default: throw new ArgumentOutOfRangeException("ds 33` 33kfl.. ");
            }
        }

        public override void SetNode(Node node)
        {
            node.IsPersistable = true;
            node.IsExpanded = true;
            node.IsEditable = false;
            base.SetNode(node);
        }
    }

    public class FileModel : Model
    {
        public FileModel()
        {
            Value = "C:\\";
        }

        protected string value;

        public string Value
        {
            get => value;
            set
            {
                this.value = value;
                base.RaisePropertyChanged();
            }
        }
    }

    public class DatabaseModel : Model
    {
        public const string _string = nameof(_string);
        //public const string _guid = nameof(_guid);
        public const string _filePath = nameof(_filePath);

        private StringModel name;
        //private GuidModel guid;
        private FileModel filePath;

        [JsonIgnore]
        public StringModel Name { get => name; set => this.name = value; }

        [JsonIgnore]
        public FileModel FilePath { get => filePath; set => filePath = value; }


        public override IEnumerable<Node> CreateChildren()
        {
            yield return new Node(_string, new StringModel { Name = _string }) { IsPersistable = true };
            //yield return new Node(_guid, new GuidModel { Name = _guid }) { IsExpanded = true };
            yield return new Node(_filePath, new FileModel { Name = _filePath }) { };
        }

        public override void Addition(Node value, Node a)
        {
            switch (a.Name)
            {
                case _string: Name = a.Data as StringModel; break;
                //case _guid: Guid = a.Data as GuidModel; break;
                case _filePath: FilePath = a.Data as FileModel; break;
                //case converter: Converter = a.Data as ConverterModel; break;
                default: throw new ArgumentOutOfRangeException("ds 33` 33kfl.. ");
            }
        }

        public override void SetNode(Node node)
        {
            node.IsPersistable = true;
            node.IsExpanded = true;
            node.IsEditable = false;
            base.SetNode(node);
        }
    }


    public interface ISelection
    {

    }

    public class DatabasesModel : Model, ISelection
    {
        [JsonIgnore]
        public ObservableCollection<DatabaseModel> Collection { get; } = new();

        public override void SetNode(Node node)
        {
            node.Orientation = Orientation.Vertical;
            node.IsExpanded = true;
            node.IsEditable = true;
            node.IsPersistable = true;
            node.Items.AndAdditions<Node>().Subscribe(a => Collection.Add(a.Data as DatabaseModel));
            base.SetNode(node);
        }

        public override IEnumerable Proliferation()
        {
            yield return new DatabaseModel { Name = new StringModel { Value = "New" } };
        }
    }

    public class SelectionsModel : Model
    {
        [JsonIgnore]
        public ObservableCollection<SelectionModel> Collection { get; } = new();

        public override void SetNode(Node node)
        {
            node.Orientation = Orientation.Vertical;
            node.IsExpanded = true;
            node.IsEditable = true;
            node.IsPersistable = true;
            node.Items.AndAdditions<Node>().Subscribe(a => Collection.Add(a.Data as SelectionModel));
            base.SetNode(node);
        }

        public override IEnumerable Proliferation()
        {
            yield return new SelectionModel { Name = "selection" };
        }
    }

    public class NodePropertyRootsModel : Model
    {
        [JsonIgnore]
        public ObservableCollection<NodePropertyRootModel> Models { get; } = new();

        public override void SetNode(Node node)
        {
            node.IsEditable = true;
            node.Orientation = Orientation.Vertical;
            node.Items.AndAdditions<Node>().Subscribe(a => Models.Add(a.Data as NodePropertyRootModel));
            base.SetNode(node);
        }

        public override IEnumerable Proliferation()
        {
            yield return new NodePropertyRootModel { Name = "npr" };
        }
    }

    public class ThroughPutModel : Model
    {
        const string element = nameof(element);
        const string filter = nameof(filter);

        private AndOrModel _filter;
        private NodePropertyRootsModel _element;


        [JsonIgnore]
        public AndOrModel Filter
        {
            get => _filter; set
            {
                _filter = value;
                RaisePropertyChanged();
            }
        }

        [JsonIgnore]
        public NodePropertyRootsModel Element
        {
            get => _element; set
            {
                _element = value;
                RaisePropertyChanged();
            }
        }

        public ParameterInfo Parameter { get; set; }

        public override IEnumerable<Node> CreateChildren()
        {

            yield return new Node(element, new NodePropertyRootsModel { Name = element }) { IsExpanded = true };
            yield return new Node(filter, new AndOrModel { Name = filter }) { IsExpanded = true };
        }

        public override void Addition(Node value, Node a)
        {
            switch (a.Name)
            {
                case element: Element = a.Data as NodePropertyRootsModel; break;
                case filter: Filter = a.Data as AndOrModel; break;
                //case converter: Converter = a.Data as ConverterModel; break;
                default: throw new ArgumentOutOfRangeException("ds 33` 33kfl.. ");
            }
        }

    }

    public class InputsModel : Model
    {

        [JsonIgnore]
        public ObservableCollection<ThroughPutModel> Models { get; } = new();


        public override void SetNode(Node node)
        {
            //node.IsEditable = true;
            node.Orientation = Orientation.Vertical;
            node.Items.AndAdditions<Node>().Subscribe(a => Models.Add(a.Data as ThroughPutModel));
            base.SetNode(node);
        }
    }


    public class TransformerModel : Model
    {
        const string inputs = nameof(inputs);
        const string output = nameof(output);
        const string exceptions = nameof(exceptions);
        const string converter = nameof(converter);

        private InputsModel _inputs;
        private ExceptionsModel _exceptions;
        private ThroughPutModel _output;
        private ConverterModel converterModel;
        IDisposable? disposable;

        [JsonIgnore]
        public InputsModel Inputs
        {
            get => _inputs; set
            {
                _inputs = value;
                RaisePropertyChanged();
            }
        }

        [JsonIgnore]
        public ThroughPutModel Output
        {
            get => _output; set
            {
                _output = value;
                RaisePropertyChanged();
            }
        }

        [JsonIgnore]
        public ConverterModel Converter
        {
            get => converterModel;
            set
            {
                converterModel = value;
                disposable?.Dispose();
                disposable = value.WithChangesTo(a => a.Method)
                    .Where(a => a != null)
                    .Subscribe(a =>
                    {
                        this.WithChangesTo(a => a.Output)
                        .Where(a => a != null)
                        .Subscribe(a =>
                        {
                            a.Parameter = value.Method.ReturnParameter;
                        });

                        this.WithChangesTo(a => a.Inputs)
                        .Where(a => a != null)
                        .Subscribe(a =>
                        {
                            a.Node.Clear();
                            foreach (var param in value.Method.GetParameters())
                            {
                                a.Node.Add(new Node(param.Name, new ThroughPutModel() { Parameter = param }) { IsExpanded = true, Parent = Inputs.Node });
                            }
                        });

                    });

                RaisePropertyChanged();
            }
        }


        [JsonIgnore]
        public ExceptionsModel Exceptions
        {
            get => _exceptions; set
            {
                _exceptions = value;
                RaisePropertyChanged();
            }
        }

        public override IEnumerable<Node> CreateChildren()
        {
            yield return new Node(converter, new ConverterModel { Name = converter }) { IsExpanded = true };
            yield return new Node(inputs, new InputsModel { Name = inputs }) { IsExpanded = true };
            yield return new Node(output, new ThroughPutModel { Name = output }) { IsExpanded = true };
            yield return new Node(exceptions, new ExceptionsModel { Name = exceptions }) { IsExpanded = true };
        }

        public override void Addition(Node value, Node a)
        {
            switch (a.Name)
            {
                case inputs: Inputs = a.Data as InputsModel; break;
                case output: Output = a.Data as ThroughPutModel; break;
                case converter: Converter = a.Data as ConverterModel; break;
                case exceptions: Exceptions = a.Data as ExceptionsModel; break;
                default: throw new ArgumentOutOfRangeException("ds 33` 33kfl.. ");
            }
        }

        public override void SetNode(Node node)
        {
            node.IsEditable = false;
            node.IsExpanded = true;
            node.IsPersistable = true;
            base.SetNode(node);
        }
    }

    public class FilterModel : Model
    {
        const string res = nameof(res);
        const string b_ool = nameof(b_ool);
        private NodeRootModel resolvableModel;

        public FilterModel()
        {

        }

        public override void SetNode(Node node)
        {
            node.IsPersistable = true;
            node.IsExpanded = true;
            base.SetNode(node);
        }

        [JsonIgnore]
        public BooleanModel BooleanModel { get; set; }

        [JsonIgnore]
        public NodeRootModel ResolvableModel
        {
            get => resolvableModel; set
            {
                resolvableModel = value;
                this.RaisePropertyChanged();
            }
        }

        public override IEnumerable<Node> CreateChildren()
        {
            yield return (new Node("res", new NodeRootModel { Name = "res" }) { IsExpanded = true });
            yield return (new Node("b_ool", new BooleanModel { Name = "b_ool" }) { IsExpanded = true });
        }

        public override void Addition(Node value, Node a)
        {
            switch (a.Name)
            {
                case res:
                    ResolvableModel = a.Data as NodeRootModel; break;
                case b_ool:
                    BooleanModel = a.Data as BooleanModel; break;
            }
        }

        public bool Get(object instance)
        {
            if (ResolvableModel.TryGetValue(instance, out var value))
                if (value?.ToString().Equals(ResolvableModel.Value) == true)
                {
                    return true;
                }
            return false;

        }
    }

    public abstract class BreadCrumbModel : Model, ISetNode
    {
        public BreadCrumbModel()
        {

        }
        public override void Update(Node top, Node current)
        {
            if (top == current)
                return;

            List<Node> items = new();
            var child = top;
            child.IsExpanded = false;
            while (child.Current != null)
            {
                items.Add(child = (Node)child.Current);
                //Extract(child);
                child.IsExpanded = false;
            }

            int i = 0;

            lock (top.Items)
                while (true)
                {
                    if (items.Count > i)
                    {

                        if (top.Count > i)
                        {
                            if (top[i] != items[i])
                            {
                                top[i] = items[i];
                            }
                        }
                        else
                        {
                            try
                            {
                                top.Add(items[i]);
                            }
                            catch (InvalidOperationException ex) when (ex.Message == "Cannot change ObservableCollection during a CollectionChanged event.")
                            {
                                this.current.Post(a => top.Add(a as Node), items[i]);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    i++;
                }

            for (int j = top.Count - 1; j > items.Count - 1; j--)
            {
                top.RemoveAt(j);
            }
        }

        public override void SetNode(Node node)
        {
            node.IsPersistable = true;
            node.CollectionChanged += Items_CollectionChanged;
            base.SetNode(node);


            void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if (node.Current == null && e.NewItems is IEnumerable newItems)
                {
                    foreach (Node item in newItems)
                        node.Current = item;
                }
            }
        }
    }

    public class ContentRootModel : Model
    {

        [JsonIgnore]
        [ChildAttribute("selections")]
        public SelectionsModel SelectionsModel { get; set; }




    }

    public class ResolvableModel : BreadCrumbModel, IRoot
    {
        private Assembly assembly;
        private CustomCollection<Type> types = new();
        private CustomCollection<PropertyInfo> propertyInfos = new();
        private CustomCollection<Assembly> assemblies = [];

        public ResolvableModel()
        {
        }


        [JsonIgnore]
        public CustomCollection<Assembly> Assemblies
        {
            get => assemblies; set
            {
                assemblies = value;
            }
        }

        [JsonIgnore]
        public CustomCollection<Type> Types
        {
            get => types; set
            {
                types = value;
            }
        }

        [JsonIgnore]
        public CustomCollection<PropertyInfo> Properties
        {
            get => propertyInfos; set
            {
                propertyInfos = value;
            }
        }

        public override IEnumerable<Node> CreateChildren()
        {
            yield return (new Node("ass_root", new GlobalAssembliesModel { Name = "ass_root" }) { });
        }

        public override void AddDescendant(IReadOnlyTree change, int level)
        {
            if (change is Node node)
                node.WithChangesTo(a => a.Current)
                    .Subscribe(node =>
                    {
                        switch (node?.Data)
                        {
                            case AssemblyModel { Assembly: { } assembly }:
                                {
                                    var level = node.Level(this.Node);
                                    this.assemblies.InsertSpecial(level, assembly);
                                    break;
                                }
                            case TypeModel { Type: { } type }:
                                {
                                    var level = node.Level(this.Node);
                                    this.types.InsertSpecial(level, type);
                                    break;
                                }
                            case PropertyModel { PropertyInfo: { } pInfo }:
                                {
                                    var level = node.Level(this.Node);
                                    this.propertyInfos.InsertSpecial(level, pInfo);
                                }

                                break;
                        }

                    });
        }


        public override void Subtraction(Node node, Node a)
        {

            switch (a?.Data)
            {
                case AssemblyModel { Assembly: { } assembly }:
                    {
                        var level = a.Level(node);
                        this.assemblies.RemoveAtSpecial(level);
                        break;
                    }
                case TypeModel { Type: { } type }:
                    {
                        var level = a.Level(node);
                        this.types.RemoveAtSpecial(level);
                        break;
                    }
                case PropertyModel { PropertyInfo: { } pInfo }:
                    {
                        var level = a.Level(node);
                        this.propertyInfos.RemoveAtSpecial(level);
                    }

                    break;

            }
        }

        public override void ReplaceDescendant(IReadOnlyTree node, IReadOnlyTree old, int level)
        {

            switch (node?.Data)
            {
                case AssemblyModel { Assembly: { } assembly }:
                    {
                        this.assemblies.ReplaceSpecial(level, assembly);
                        break;
                    }
                case TypeModel { Type: { } type }:
                    {
                        this.types.ReplaceSpecial(level, type);
                        break;
                    }
                case PropertyModel { PropertyInfo: { } pInfo }:
                    {
                        this.propertyInfos.ReplaceSpecial(level, pInfo);
                    }

                    break;
            }
        }

        public virtual bool TryGetValue(object instance, out object? value)
        {
            value = instance;

            int i = 0;
            while (Assemblies.Count > i)
            {
                if (Assemblies[i] != value.GetType().Assembly)
                    return false;
                if (Types.Count > i == false)
                    return false;
                if (Properties.Count > i == false)
                    return false;
                if (value.GetType().Equals(Types[i]) == false)
                    return false;

                if (Properties[i].GetValue(value) is { } _value)
                {
                    i++;
                    value = _value;
                }
                else
                    return false;
            }
            return true;
        }
        public bool TrySetValue(object instance, object? value)
        {
            if (Types.Any() == false)
                return true;
            int i = 0;
            while (Assemblies.Count > i + 1)
            {
                if (Assemblies[i] != instance.GetType().Assembly)
                    return false;
                if (Types.Count > i == false)
                    return false;
                if (Properties.Count > i == false)
                    return false;
                if (instance.GetType().Equals(Types[i]) == false)
                    return false;

                if (Properties[i].GetValue(instance) is { } _instance)
                {
                    i++;
                    instance = _instance;
                }
                else
                    return false;
            }
            Properties[i].SetValue(instance, value);
            return true;

        }

        public override void SetNode(Node node)
        {
            node.IsExpanded = true;
            base.SetNode(node);
        }
    }

    public class ResolvableNode : Node
    {
        public ResolvableNode(System.Guid guid, Assembly assembly) : base("main", new ResolvableModel { Name = "main" })
        {

            Key = new GuidKey(guid);
            IsExpanded = true;

            //this.Items.Add(new Node("root2", new AssemblyModel { Name = "root2", Assembly = assembly }) { Parent = this });


            //var types = assembly.ExportedTypes;

            //foreach (Type type in types)
            //{
            //    var node = new Node(type.Name, new TypeModel { Name = type.Name, Type = type }) { Parent = Root };
            //    foreach (var prop in type.GetProperties())
            //    {
            //        var pnode = new Node(prop.Name, new PropertyModel { Name = prop.Name, PropertyInfo = prop }) { Parent = node };
            //        node.Items.Add(pnode);
            //        if (type.ContainsGenericParameters == false)
            //            pnode.Items.Add(new Node(prop.Name + "value", new ValueModel { Value = type.IsValueType ? Activator.CreateInstance(type) : null }) { Parent = pnode });
            //    }
            //    Root.Items.Add(node);

            //}
        }
    }
}
