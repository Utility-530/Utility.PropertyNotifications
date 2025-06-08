using DynamicData;
using Newtonsoft.Json;
using Splat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Xml.Linq;
using Utility.Collections;
using Utility.Enums;
using Utility.Helpers;
using Utility.Helpers.NonGeneric;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
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

    public class TypeModel : Model, IBreadCrumb, IType
    {
        public Type Type { get; set; }

        public override IEnumerable<Model> CreateChildren()
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

        public override IEnumerable<Model> CreateChildren()
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

        public override IEnumerable<Model> CreateChildren()
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

        public NodePropertiesModel(Func<IEnumerable<Model>> func) : base(func)
        {
        }

        public override IEnumerable<Model> CreateChildren()
        {
            if (func != null)
            {
                foreach (var item in func.Invoke())
                {
                    yield return item;
                }
                yield break;
            }
            Type[] _types = types;

            if (this.Node.Ancestor(a => a.tree.Data is ThroughPutModel { Parameter: { } }) is { } ancestor)
            {
                _types = [(ancestor.Data as ThroughPutModel).Parameter.ParameterType, typeof(object)];
            }

            foreach (var prop in Locator.Current.GetService<INodePropertyFactory>().Properties(_types))
            {
                var pnode = new PropertyModel { Name = prop.Name, Value = prop };
                yield return pnode;
            }

        }

        public override void SubtractDescendant(IReadOnlyTree node, int level)
        {
        }
    }

    public class AssemblyModel : Model, IBreadCrumb, IGetAssembly
    {
        public AssemblyModel()
        {
        }

        public Assembly Assembly { get; set; }

        public override required string Name
        {
            get => Assembly?.FullName ?? m_name;
            set
            {
                m_name = value;
                if (Assembly == null)
                {
                    Assembly = Assembly.Load(value);
                }
                //base.RaisePropertyChanged();
            }
        }


        public override IEnumerable<Model> CreateChildren()
        {
            if (Assembly == null)
                throw new Exception("d90222fs sd");
            //var types = Assembly.ExportedTypes.Where(t => t.IsAssignableTo(typeof(Model)));

            foreach (Type type in Assembly.ExportedTypes.Where(GlobalModelFilter.Instance.TypePredicate.Invoke))
            {
                var _node = new TypeModel { Name = type.Name, Type = type };
                yield return _node;
            }
        }

        public static AssemblyModel Create(Assembly assembly) => new() { Assembly = assembly, Name = assembly.FullName };

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

        public GlobalAssembliesModel(Func<IEnumerable<Model>> predicate) : base(predicate)
        {

        }

        public GlobalAssembliesModel() : base()
        {

        }

        public override IEnumerable<Model> CreateChildren()
        {
            if (func != null)
            {
                foreach (var x in func.Invoke())
                    yield return x;
                yield break;
            }

            foreach (var prop in Locator.Current.GetService<IPropertyFactory>().Properties)
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

    public class AssemblyTypePropertyModel : ValueModel<PropertyInfo>, IBreadCrumb
    {
        public AssemblyTypePropertyModel()
        {
        }

        public Assembly Assembly => Type?.Assembly;

        public Type Type => Value?.DeclaringType;
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

    public class PropertyModel : ValueModel<PropertyInfo>, IBreadCrumb
    {
        public PropertyModel()
        {

        }

        public override IEnumerable<Model> CreateChildren()
        {
            if (Value?.PropertyType == typeof(object))
            {
                yield return (new GlobalAssembliesModel { Name = "ass_root" });
            }
        }

        public override void SubtractDescendant(IReadOnlyTree node, int level)
        {
        }
    }

    public class ValueModel : ValueModel<object>, IAutoList
    {
        private bool isListening;

        public ValueModel(object value) : base(value) { }
        public ValueModel() { }

        public IEnumerable AutoList { get; set; }

        public bool IsListening { get => isListening; set => RaisePropertyChanged(ref isListening, value); }
    }


    public class MethodModel : ValueModel<MethodInfo>, IBreadCrumb
    {
        public MethodModel()
        {

        }

        public override IEnumerable<Model> CreateChildren()
        {
            foreach (var p in Value?.GetParameters() ?? Array.Empty<ParameterInfo>())
                yield return
                    new ParameterModel { Name = p.Name, Parameter = p };
        }
    }

    public class ParameterModel : Model, IBreadCrumb
    {
        public ParameterInfo Parameter { get; set; }

    }

    public class MethodsModel : Model, IBreadCrumb
    {
        public override IEnumerable<Model> CreateChildren()
        {
            foreach (var m in Locator.Current.GetService<IMethodFactory>().Methods)
                yield return new MethodModel { Name = m.GetDescription(), Value = m };
        }
    }

    public class ExceptionModel(Exception Exception) : Model
    {
        public Exception Exception { get; } = Exception;

        public static ExceptionModel Create(Exception ex) => new ExceptionModel(ex) { Name = ex.Message };
    }

    public class ExceptionsModel() : CollectionModel<ExceptionModel>
    {
    }


    public class ConverterModel : BreadCrumbModel, IRoot
    {
        private object output;
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

        public override IEnumerable<Model> CreateChildren()
        {
            yield return new MethodsModel { Name = "methods" };

        }

        public override void AddDescendant(IReadOnlyTree node, int level)
        {
            if (node is INode _node)
                _node.WithChangesTo(a => a.Current)
                    .Where(a => a != null)
                    .Subscribe(a =>
                    {
                        if (a.Data is MethodModel _m)
                        {
                            _m.WithChangesTo(a => a.Value).Update(this, a => a.Method);
                            //.Subscribe(a =>
                            //{
                            //    Method = _m.Value;
                            //});
                        }
                    });
        }

        public override void SetNode(INode node)
        {
            node.IsPersistable = true;
            node.IsExpanded = true;
            base.SetNode(node);
        }
    }


    public class CommandModel : Model
    {
        protected Unit m_name;

        public CommandModel()
        {
            Command = new Commands.Command(() => base.RaisePropertyChanged());
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
                if (searchText != value)
                {
                    var _previous = searchText;
                    searchText = value;
                    this.RaisePropertyChanged(_previous, value);
                }
            }
        }
    }

    public class IndexModel : ValueModel<int>
    {
    }

    public class StringModel : ValueModel<string>
    {
    }

    public class StringRootModel : StringModel, IRoot
    {
    }

    public class HtmlModel : StringModel
    {
    }

    public class GuidModel : ValueModel<Guid>
    {
    }





    public class AndOrModel : ValueModel<AndOr, IPredicate>, IAndOr, IObservable<Unit>, IPredicate
    {
        protected AndOr value = 0;
        List<IObserver<Unit>> observers = [];
        Dictionary<Model, IDisposable> dictionary = [];

        public AndOrModel()
        {
        }

        public bool Evaluate(object data)
        {
            if (Value == AndOr.And)
            {
                return Collection.All(x => x.Evaluate(data));
            }
            else if (Value == AndOr.Or)
            {
                return Collection.Any(x => x.Evaluate(data));
            }
            else
                throw new ArgumentOutOfRangeException("sd 3433 33 x");
        }



        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            if (a.Data is AndOrModel aoModel)
            {
                Collection.Add(aoModel);
                {
                    var dis = aoModel.Subscribe(_ => onNext());
                    dictionary[aoModel] = dis;
                }
            }
            else if (a.Data is FilterModel filterModel)
            {
                Collection.Add(filterModel);
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
                            //a.Subscribe(_a =>
                            //{
                            //    onNext();
                            //}).DisposeWith(composite);
                        }).DisposeWith(composite);
                    dictionary[filterModel] = composite;

                }
            }
            else
                throw new Exception("7 hhjkhj9099   ");

            base.Addition(value, a);

            void onNext()
            {
                foreach (var obs in observers)
                    obs.OnNext(Unit.Default);
            }
        }


        public override void Subtraction(IReadOnlyTree value, IReadOnlyTree a)
        {
            if (a.Data is AndOrModel aoModel)
            {
                dictionary[aoModel].Dispose();
                Collection.Remove(aoModel);
            }
            else if (a.Data is FilterModel filterModel)
            {
                dictionary[filterModel]?.Dispose();
                Collection.Remove(filterModel);
            }
            else
                throw new Exception("877 hhj9099   ");
        }

        public override void SetNode(INode node)
        {
            node.IsPersistable = true;
            node.IsExpanded = true;
            node.IsAugmentable = true;
            base.SetNode(node);
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
            if (instance is not INode { Data: { } data } node)
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

        private void Level_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Level = (int)sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
        }

        private void ValueModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
    public class BooleanModel : ValueModel<bool>
    {
    }

    public class ComparisonModel : Model
    {
        private Enum _value;
        private ComparisonType type;

        public ComparisonModel()
        {

        }

        public ComparisonType Type
        {
            get => type; set
            {
                type = value;
                switch (value)
                {
                    case ComparisonType.Default:
                        _value = null; break;
                    case ComparisonType.String:
                        _value = CustomStringComparison.EqualTo; break;
                    case ComparisonType.Number:
                        _value = NumberComparison.EqualTo; break;
                    case ComparisonType.Boolean:
                        _value = BooleanComparison.EqualTo; break;
                    case ComparisonType.Type:
                        _value = TypeComparison.EqualTo; break;
                }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Value));
            }
        }

        public Enum? Value
        {
            get => _value; set
            {
                if (_value != value)
                {
                    var _previous = _value;
                    _value = value;
                    RaisePropertyChanged(_previous, value);
                }
            }
        }

        public override void SetNode(INode node)
        {
            node.IsPersistable = true;
            base.SetNode(node);
        }

        internal bool Compare(object value1, object value2)
        {
            switch (Type)
            {
                case ComparisonType.String:
                    switch ((CustomStringComparison)Value)
                    {
                        case CustomStringComparison.Contains:
                            return value1.ToString().Contains(value2.ToString());
                        case CustomStringComparison.EqualTo:
                            return value1.ToString().Equals(value2.ToString());
                        case CustomStringComparison.NotEqualTo:
                            return value1.ToString().Equals(value2.ToString()) == false;
                        case CustomStringComparison.DoesNotContain:
                            return value1.ToString().Contains(value2.ToString()) == false;
                    }
                    break;
                case ComparisonType.Number:
                    bool success1 = int.TryParse(value1.ToString(), out int int1);
                    bool success2 = int.TryParse(value1.ToString(), out int int2);
                    if (success1 && success2)
                        switch ((NumberComparison)Value)
                        {
                            case NumberComparison.GreaterThanOrEqualTo:
                                return int1 >= int2;
                            case NumberComparison.GreaterThan:
                                return int1 > int2;
                            case NumberComparison.LessThan:
                                return int1 < int2;
                            case NumberComparison.EqualTo:
                                return int1 == int2;
                            case NumberComparison.NotEqualTo:
                                return int1 != int2;
                            case NumberComparison.LessThanOrEqualTo:
                                return int1 <= int2;
                        }
                    return false;
                    break;
                case ComparisonType.Boolean:
                    switch ((BooleanComparison)Value)
                    {
                        case BooleanComparison.EqualTo:
                            return value1.ToString().Equals(value2.ToString());
                        case BooleanComparison.NotEqualTo:
                            return value1.ToString().Equals(value2.ToString()) == false;

                    }
                    break;
            }
            throw new NotImplementedException("f 33 dfd33");
        }
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
                        Value = value;
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
                        Value = value;
                        break;
                    }
            }

            switch (old.Data)
            {
                case ValueModel { Value: var value } valueModel:
                    {
                        valueModel.PropertyChanged -= ValueModel_PropertyChanged;
                        Value = value;
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
                        Value = null;
                        break;
                    }
            }
        }

        private void ValueModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
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


    public class TransformersModel : CollectionModel<TransformerModel>
    {
        public const string transformer = nameof(transformer);
        public override void SetNode(INode node)
        {
            node.Orientation = Orientation.Vertical;
            node.IsEditable = false;
            node.IsRemovable = false;
            node.IsExpanded = true;
            node.IsPersistable = true;
            base.SetNode(node);
        }

        public override IEnumerable Proliferation()
        {
            yield return new TransformerModel { Name = transformer };
        }
    }

    public class FiltersModel : CollectionModel<FilterModel>
    {

        public override void SetNode(INode node)
        {
            node.Orientation = Orientation.Vertical;
            node.IsExpanded = true;
            node.IsPersistable = true;
            base.SetNode(node);
        }
        public override IEnumerable Proliferation()
        {
            yield return new FilterModel { Name = "filter" };
        }

        public override void SubtractDescendant(IReadOnlyTree node, int level)
        {
            //base.SubtractDescendant(node, level);
        }
    }

    public class SelectionModel : Model
    {
        public const string _string = nameof(_string);
        public const string _type = nameof(_type);

        private StringModel @string;
        private TypeModel type;

        public StringModel String { get => @string; set => @string = value; }
        public TypeModel Type { get => type; set => type = value; }

        public override IEnumerable<Model> CreateChildren()
        {
            yield return new StringModel { Name = _string };
            yield return new TypeModel { Name = _type };
        }

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            switch (a.Data.ToString())
            {
                case _string: String = a.Data as StringModel; break;
                case _type: Type = a.Data as TypeModel; break;
                default: throw new ArgumentOutOfRangeException("ds 33` 33kfl.. ");
            }
            base.Addition(value, a);
        }

        public override void SetNode(INode node)
        {
            node.IsPersistable = true;
            node.IsExpanded = true;
            base.SetNode(node);
        }
    }

    public class FileModel : StringModel
    {
        public FileModel()
        {
            Value = "C:\\";
        }


    }

    public class AliasFileModel : Model
    {
        public AliasFileModel()
        {
        }

        public StringModel Alias { get; set; }

        public FileModel File { get; set; }

    }

    public readonly record struct DataFile(string Alias, string FilePath);


    public class DataFileModel : ValueModel<DataFile>
    {

        public DataFileModel()
        {
        }

        public virtual string Alias
        {
            get => this.Value.Alias;
            set { this.Value = Value with { Alias = value }; }
        }

        public virtual string FilePath
        {
            get => this.Value.FilePath;
            set { this.Value = Value with { FilePath = value }; }
        }

        public override void SetNode(INode node)
        {
            node.IsPersistable = true;
            node.IsExpanded = true;
            node.IsRemovable = true;
            node.IsReplicable = true;
            base.SetNode(node);
        }

        public override object Clone()
        {
            return new DataFileModel { Name = Name, Value = Value };
        }
    }

    public class DataFilesModel : Model, ISelectable
    {
        [JsonIgnore]
        public ObservableCollection<DataFileModel> Collection { get; } = new();

        public override void SetNode(INode node)
        {
            node.Orientation = Orientation.Vertical;
            node.IsExpanded = true;
            node.IsAugmentable = true;
            node.IsPersistable = true;
            node.Items.AndAdditions<INode>().Subscribe(a => Collection.Add(a.Data as DataFileModel));
            base.SetNode(node);
        }

        public override IEnumerable Proliferation()
        {
            yield return new DataFileModel { Name = "db", FilePath = "c:\\", Alias = "New" };
        }

        public override void Update(IReadOnlyTree node, IReadOnlyTree current)
        {

        }
    }

    public class SelectionsModel : CollectionModel<SelectionModel>
    {
        public override void SetNode(INode node)
        {
            node.Orientation = Orientation.Vertical;
            node.IsExpanded = true;
            node.IsPersistable = true;
            base.SetNode(node);
        }

        public override IEnumerable Proliferation()
        {
            yield return new SelectionModel { Name = "selection" };
        }
    }

    public class NodePropertyRootsModel : CollectionModel<NodePropertyRootModel>
    {
        public NodePropertyRootsModel()
        {
            Limit = 1;
        }

        public override void SetNode(INode node)
        {
            node.Orientation = Orientation.Vertical;
            base.SetNode(node);
        }
        public override IEnumerable Proliferation()
        {
            yield return new NodePropertyRootModel { Name = "npr" };
        }
    }

    public class ThroughPutModel : ValueModel<string>
    {
        const string element = nameof(element);
        const string filter = nameof(filter);

        private AndOrModel _filter;
        private NodePropertyRootsModel _element;


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

        public override IEnumerable<Model> CreateChildren()
        {
            yield return new NodePropertyRootsModel { Name = element };
            yield return new AndOrModel { Name = filter };
        }

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            switch (a.Data)
            {
                case NodePropertyRootsModel: Element = a.Data as NodePropertyRootsModel; break;
                case AndOrModel: Filter = a.Data as AndOrModel; break;
                //case converter: Converter = a.Data as ConverterModel; break;
                default: throw new ArgumentOutOfRangeException("ds 33` 33kfl.. ");
            }

            base.Addition(value, a);
        }

        public override void SetNode(INode node)
        {
            node.IsExpanded = true;
            node.IsRemovable = true;
            base.SetNode(node);
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

    public class InputsModel : CollectionModel<ThroughPutModel>
    {
        private ParameterInfo[] parameters;

        public ParameterInfo[] Parameters
        {
            get => parameters; set
            {
                parameters = value;
                Limit = value.Count();
                RaisePropertyChanged();
            }
        }

        public override void SetNode(INode node)
        {
            node.IsExpanded = true;
            node.Orientation = Orientation.Vertical;
            base.SetNode(node);
        }

        public override IEnumerable Proliferation()
        {
            var oc = new ObservableCollection<ThroughPutModel>();
            this.WithChangesTo(a => a.Parameters)
                .Subscribe(x =>
                {
                    oc.Clear();
                    foreach (var param in Parameters.Where(a => this.Collection.All(ac => ac.Parameter != a)))
                    {
                        oc.Add(new ThroughPutModel() { Name = param.Name, Parameter = param });
                    }
                });
            return oc;
        }
    }

    public class ExpandedModel : Model
    {
        private readonly Action<INode> action;

        public ExpandedModel(Func<IEnumerable<Model>> func, Action<INode> action = null) : base(func)
        {
            this.action = action;
        }

        public ExpandedModel() : base()
        {
        }

        public override void SetNode(INode node)
        {
            action?.Invoke(node);
            node.ConnectorPosition = Position2D.Right;
            //node.IsEditable = true;
            node.IsExpanded = true;
            node.Arrangement = Arrangement.Stack;
            base.SetNode(node);
        }
    }


    public class TransformerModel : Model
    {
        public const string inputs = nameof(inputs);
        public const string output = nameof(output);
        public const string converter = nameof(converter);

        private InputsModel _inputs;
        private ThroughPutModel _output;
        private ConverterModel _converter;
        IDisposable disposable;

        [JsonIgnore]
        public InputsModel Inputs
        {
            get => _inputs;
            set
            {
                if (_inputs != value)
                {
                    var _previous = _inputs;
                    _inputs = value;
                    this.RaisePropertyChanged(_previous, value);
                }
            }
        }

        [JsonIgnore]
        public ThroughPutModel Output
        {
            get => _output;
            set
            {
                if (_output != value)
                {
                    var _previous = _output;
                    _output = value;
                    this.RaisePropertyChanged(_previous, value);
                }
            }
        }

        [JsonIgnore]
        public ConverterModel Converter
        {
            get => _converter;
            set
            {
                var previous = _converter;
                _converter = value;
                disposable?.Dispose();
                disposable = value.WithChangesTo(a => a.Method)
                    .Where(a => a != null)
                    .Subscribe(method =>
                    {
                        this.WithChangesTo(a => a.Output)
                        .Where(a => a != null)
                        .Subscribe(a =>
                        {
                            a.Parameter = value.Method.ReturnParameter;
                        });

                        this.WithChangesTo(a => a.Inputs)
                        .Where(i => i != null)
                        .Subscribe(async i =>
                        {
                            i.Parameters = value.Method.GetParameters();

                        });

                    });

                RaisePropertyChanged(previous, value);
            }
        }


        public override IEnumerable<Model> CreateChildren()
        {
            yield return _converter ??= new ConverterModel { Name = converter };
            yield return _inputs ??= new InputsModel { Name = inputs };
            yield return _output ??= new ThroughPutModel { Name = output };
        }

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            switch (a.Data.ToString())
            {
                case inputs: Inputs = a.Data as InputsModel; break;
                case output: Output = a.Data as ThroughPutModel; break;
                case converter: Converter = a.Data as ConverterModel; break;
                default: throw new ArgumentOutOfRangeException("ds 33` 33kfl.. ");
            }
            base.Addition(value, a);
        }

        public override void SetNode(INode node)
        {
            node.WithChangesTo(a => a.Parent).Subscribe(a =>
            {
                node.LocalIndex = a.Items.Count();
                node.Arrangement = Arrangement.Uniform;
                node.Columns.Add(new Structs.Dimension());
                node.Columns.Add(new Structs.Dimension());
                node.Columns.Add(new Structs.Dimension());
                node.Rows.Add(new Structs.Dimension());
                node.IsExpanded = true;
                node.IsPersistable = true;
                base.SetNode(node);
            });


        }
    }

    public class FilterModel : Model, IPredicate
    {
        const string res = nameof(res);
        const string b_ool = nameof(b_ool);
        const string _value = nameof(_value);
        private ResolvableModel resolvableModel;
        private ComparisonModel comparisonModel;
        private ValueModel valueModel;

        public FilterModel()
        {
        }

        public override void SetNode(INode node)
        {
            node.WithChangesTo(a => a.Parent).Subscribe(a =>
            {
                node.LocalIndex = a.Items.Count();
                node.IsPersistable = true;
                node.IsExpanded = true;
                base.SetNode(node);
            });
        }

        [JsonIgnore]
        [Child(res)]
        public ResolvableModel ResolvableModel
        {
            get => resolvableModel;
            set
            {
                if (resolvableModel != value)
                {
                    var previous = resolvableModel;
                    resolvableModel = value;
                    this.RaisePropertyChanged(previous, value);
                }
            }
        }

        [JsonIgnore]
        [Child(b_ool)]

        public ComparisonModel ComparisonModel
        {
            get => comparisonModel;
            set
            {
                if (comparisonModel != value)
                {
                    var previous = comparisonModel;
                    comparisonModel = value;
                    this.RaisePropertyChanged(previous, value);
                }

            }
        }

        [JsonIgnore]
        [Child(_value)]
        public ValueModel ValueModel
        {
            get => valueModel;
            set
            {
                if (valueModel != value)
                {
                    var previous = valueModel;
                    valueModel = value;
                    this.RaisePropertyChanged(previous, value);
                }
            }
        }

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            switch (a.Data.ToString())
            {
                case res:
                    ResolvableModel = a.Data as ResolvableModel;
                    ResolvableModel.Types.Changes()
                        .CombineLatest(ResolvableModel.Properties.Changes(), this.WithChangesTo(a => a.ValueModel))
                        .Subscribe(a =>
                        {
                            var typesCount = ResolvableModel.Types.Count;
                            var propertiesCount = ResolvableModel.Properties.Count;
                            ObservableCollection<object> list = new();
                            source.Nodes.AndChanges<INode>().Subscribe(a =>
                            {
                                foreach (var item in a)
                                    if (item.Type == Changes.Type.Add)
                                        if (ResolvableModel.TryGetValue(item.Value, out var x))
                                        {
                                            list.Add(x);
                                        }
                            });

                            if (typesCount > propertiesCount)
                            {
                                ValueModel.Set(ResolvableModel.Types.Last());
                                ComparisonModel.Type = a.First == null ? ComparisonType.Default : ComparisonType.Type;
                            }
                            else if (typesCount == 0)
                            {
                                ValueModel.Set(null);
                                ComparisonModel.Type = ComparisonType.Default;
                            }
                            else if (typesCount == propertiesCount)
                            {
                                var propertyType = ResolvableModel.Properties.Last().PropertyType;
                                ValueModel.AutoList = list;
                                if (ValueModel.Value?.GetType() != propertyType)
                                {
                                    ValueModel.Set(_value ?? ActivateAnything.Activate.New(propertyType));
                                    ComparisonModel.Type = toComparisonType(propertyType);
                                }
                            }
                            else
                                throw new Exception("ee33 ffp[oe");
                        });
                    break;
                case b_ool:
                    ComparisonModel = a.Data as ComparisonModel; break;
                case _value:
                    ValueModel = a.Data as ValueModel;
                    ValueModel.WithChangesTo(a => a.IsListening)
                        .CombineLatest(source.Selections)
                        .Where(a => a.First)
                        .Select(a => a.Second)
                        .Subscribe(selected =>
                        {
                            if (ResolvableModel.TryGetValue(selected, out var _value))
                            {
                                //ValueModel.Value = _value;
                                ValueModel.Set(_value);
                            }
                        });
                    break;
            }
            base.Addition(value, a);

            static ComparisonType toComparisonType(Type type)
            {
                if (type == null)
                    return ComparisonType.Default;
                else if (type == typeof(string))
                    return ComparisonType.String;
                else if (TypeHelper.IsNumericType(type))
                    return ComparisonType.Number;
                else
                    return ComparisonType.Boolean;
            }
        }

        public bool Evaluate(object instance)
        {
            if (ResolvableModel.TryGetValue(instance, out var value))
            {
                return comparisonModel.Compare(value, ValueModel.Value);
            }
            return false;

        }
    }

    public abstract class BreadCrumbModel : Model, ISetNode
    {
        public BreadCrumbModel()
        {

        }
        public override void Update(IReadOnlyTree top, IReadOnlyTree current)
        {
            if (top == current)
                return;

            var topNode = top as INode;
            var currentNode = current as IReadOnlyTree;

            List<INode> items = new();
            var child = topNode;
            while (child.Current != null)
            {
                items.Add(child = child.Current);
                child.IsExpanded = false;
            }

            int i = 0;

            lock (top.Items)
                while (true)
                {
                    if (items.Count > i)
                    {

                        if (topNode.Count > i)
                        {
                            if (topNode[i] != items[i] && topNode[i].Equals(items[i]) == false)
                            {
                                topNode[i] = items[i];
                            }
                        }
                        else
                        {
                            try
                            {
                                topNode.Add(items[i]);
                            }
                            catch (InvalidOperationException ex) when (ex.Message == "Cannot change ObservableCollection during a CollectionChanged event.")
                            {
                                this.context.Value.UI.Post(a => topNode.Add(a as INode), items[i]);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    i++;
                }

            for (int j = topNode.Count - 1; j > items.Count - 1; j--)
            {
                topNode.RemoveAt(j);
            }
        }

        public override void SetNode(INode node)
        {
            node.IsPersistable = true;
            base.SetNode(node);
        }

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            if (value.Items.Count() == 1)
            {
                (value as INode).Current = a as INode;
            }
            base.Addition(value, a);
        }
    }

    public readonly record struct ModelType(string Alias, string Type);

    public class ModelTypeModel : ValueModel<ModelType>
    {
        public override void SetNode(INode node)
        {
            node.IsPersistable = true;
            node.IsExpanded = true;
            base.SetNode(node);
        }
    }

    public class ModelTypesModel : CollectionModel<ModelTypeModel>
    {
        public override void SetNode(INode node)
        {
            node.IsExpanded = true;
            Node = node;
        }

        public override void Initialise()
        {
        }

        public override IEnumerable<Model> CreateChildren()
        {
            foreach (var type in Locator.Current.GetService<IModelTypesFactory>().Types())
            {
                var pnode = new ModelTypeModel { Name = type.Name, Value = new(type.Name, type.AsString()) };
                yield return pnode;
            }
        }
    }

    public class ContentRootModel : Model
    {

        [JsonIgnore]
        [Child("selections")]
        public SelectionsModel SelectionsModel { get; set; }
    }

    public class ResolvableModel : BreadCrumbModel, IRoot
    {
        private CustomCollection<Type> types = new();
        private CustomCollection<PropertyInfo> properties = new();
        private CustomCollection<Assembly> assemblies = [];
        private GlobalAssembliesModel globalAssembliesModel;

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
            get => properties; set
            {
                properties = value;
            }
        }


        [JsonIgnore]
        public GlobalAssembliesModel GlobalAssembliesModel
        {
            get => globalAssembliesModel; set
            {
                globalAssembliesModel = value;
            }
        }



        public override IEnumerable<Model> CreateChildren()
        {
            yield return globalAssembliesModel ??= new GlobalAssembliesModel { Name = "ass_root" };
        }

        public override void Addition(IReadOnlyTree value, IReadOnlyTree add)
        {
            var _level = add.Level(Node);

            switch (add?.Data)
            {
                case AssemblyTypePropertyModel { Assembly: { } _assembly, Type: { } _type, Value: { } _property }:
                    {
                        assemblies.InsertSpecial(_level, _assembly);
                        types.InsertSpecial(_level, _type);
                        properties.InsertSpecial(_level, _property);
                        break;
                    }
                case AssemblyTypePropertyModel am:
                    {
                        am.WithChangesTo(a => a.Value).Subscribe(a =>
                        {
                            assemblies.InsertSpecial(_level, am.Assembly);
                            types.InsertSpecial(_level, am.Type);
                            properties.InsertSpecial(_level, am.Value);
                        });
                        break;
                    }
                case AssemblyModel { Assembly: { } assembly }:
                    {
                        assemblies.InsertSpecial(_level, assembly);
                        break;
                    }
                case TypeModel { Type: { } type }:
                    {
                        types.InsertSpecial(_level, type);
                        break;
                    }
                case PropertyModel { Value: { } pInfo }:
                    {
                        properties.InsertSpecial(_level, pInfo);
                    }
                    break;
                case PropertyModel { } pm:
                    {
                        pm.WithChangesTo(a => a.Value).Subscribe(x =>
                        {
                            properties.InsertSpecial(_level, x);
                        });
                    }
                    break;
            }
            base.Addition(value, add);
        }

        public override void Subtraction(IReadOnlyTree node, IReadOnlyTree subtract)
        {

            switch (subtract?.Data)
            {
                case AssemblyTypePropertyModel { Assembly: { } _assembly, Type: { } _type, Value: { } _property }:
                    {
                        var level = subtract.Level(node);
                        assemblies.RemoveAtSpecial(level);
                        types.RemoveAtSpecial(level);
                        properties.RemoveAtSpecial(level);
                        break;
                    }
                case AssemblyModel { Assembly: { } assembly }:
                    {
                        var level = subtract.Level(node);
                        assemblies.RemoveAtSpecial(level);
                        break;
                    }
                case TypeModel { Type: { } type }:
                    {
                        var level = subtract.Level(node);
                        types.RemoveAtSpecial(level);
                        break;
                    }
                case PropertyModel { Value: { } pInfo }:
                    {
                        var level = subtract.Level(node);
                        properties.RemoveAtSpecial(level);
                    }
                    break;

            }
            base.Subtraction(node, subtract);

        }

        public override void Replacement(IReadOnlyTree @new, IReadOnlyTree old)
        {

            switch (@new?.Data)
            {
                case AssemblyModel { Assembly: { } assembly }:
                    {
                        var level = @new.Level(Node);
                        assemblies.ReplaceSpecial(level, assembly);
                        break;
                    }
                case TypeModel { Type: { } type }:
                    {
                        var level = @new.Level(Node);
                        types.ReplaceSpecial(level, type);
                        break;
                    }
                case PropertyModel { Value: { } pInfo }:
                    {
                        var level = @new.Level(Node);
                        properties.ReplaceSpecial(level, pInfo);
                        break;

                    }

                default:
                    throw new Exception("uiyi 333");
            }
            base.Replacement(@new, old);
        }

        public virtual bool TryGetValue(object instance, out object value)
        {
            value = instance;

            int i = 0;
            while (Properties.Count > i)
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
        public bool TrySetValue(object instance, object value)
        {
            if (Types.Any() == false)
                return true;
            int i = 0;
            while (Properties.Count > i + 1)
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

        public override void SetNode(INode node)
        {
            node.IsExpanded = true;
            base.SetNode(node);
        }
    }

    public class DirtyModel : Model, ICollectionItem, IIgnore
    {
        public string SourceKey { get; set; }
        public string PropertyName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }

    public class DirtyModels : CollectionModel<DirtyModel>, IIgnore
    {
    }

    public interface IIgnore
    {
    }
}
