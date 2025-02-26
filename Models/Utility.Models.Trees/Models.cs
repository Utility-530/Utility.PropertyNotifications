using Newtonsoft.Json;
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
using Utility.Changes;
using Utility.Enums;
using Utility.Trees.Extensions;
using Utility.Helpers;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Observables;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Models;
using Utility.Trees.Abstractions;
using Type = System.Type;
using Utility.Collections;
using Utility.Helpers.NonGeneric;

namespace Utility.Models.Trees
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

    public interface IValue
    {
        object Value { get; set; }
    }

    public class NodePropertyRootModel : ResolvableModel
    {
        private Type type;

        public NodePropertyRootModel()
        {
            Assemblies.InsertSpecial(0, typeof(INode).Assembly);
            Types.InsertSpecial(0, typeof(INode));
            Properties.Changes().Subscribe(a => PropertyType = Properties.Last().PropertyType);
        }

        public Type PropertyType
        {
            get => type;
            set => this.RaisePropertyChanged(ref type, value);
        }

        public override IEnumerable<Model> CreateChildren()
        {
            yield return new NodeModel { Name = "_node_" };
        }
    }



    public class NodeModel : Model, IBreadCrumb
    {

        public NodeModel()
        {

        }

        public override IEnumerable<Model> CreateChildren()
        {
            yield return new NodePropertiesModel() { Name = "npm" };
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

        public override IEnumerable<Model> CreateChildren()
        {
            foreach (var prop in typeof(INode).PublicInstanceProperties().Where(a => types.Any(x => a.PropertyType.Equals(x))))
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
        //public AssemblyModel(Assembly assembly):this(assembly.GetName().Name)
        //{

        //    Assembly = assembly;
        //}

        //private AssemblyModel(string name)
        //{
        //    Name = name;
        //}
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
        public override IEnumerable<Model> CreateChildren()
        {
            foreach (Assembly ass in AssemblyHelper.GetNonSystemAssembliesInCurrentDomain().Where(GlobalModelFilter.Instance.AssemblyPredicate.Invoke))
            {
                var _node = AssemblyModel.Create(ass);
                yield return _node;
            }
        }
        public override void SubtractDescendant(IReadOnlyTree node, int level)
        {
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
            if (Value.PropertyType == typeof(object))
            {
                yield return (new GlobalAssembliesModel { Name = "ass_root" });
            }
        }

        public override void SubtractDescendant(IReadOnlyTree node, int level)
        {
        }
    }

    public class ValueModel : ValueModel<object>
    {
    }



    public class MethodModel : Model, IBreadCrumb
    {
        public MethodInfo MethodInfo { get; set; }


        public override IEnumerable<Model> CreateChildren()
        {
            foreach (var p in MethodInfo.GetParameters())
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
            foreach (var m in typeof(Methods).StaticMethods())
                yield return
                    new MethodModel { Name = m.Item1, MethodInfo = m.Item2 }
                    ;
        }
    }

    public class ExceptionModel(Exception Exception) : Model
    {

        //public ExceptionModel(string name) : this((Exception)null)
        //{
        //    Name = name;
        //}

        public Exception Exception { get; } = Exception;

        public static ExceptionModel Create(Exception ex) => new ExceptionModel(ex) { Name = ex.Message };
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
                RaisePropertyChanged(ref method, value);

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
                            Method = _m.MethodInfo;
                        }
                    });
        }

        public override void SetNode(INode node)
        {
            node.IsPersistable = true;
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
            set => this.RaisePropertyChanged(ref searchText, value);
        }
    }

    public class IndexModel : Model
    {
        protected int value = 0;

        public int Value
        {
            get => value;
            set => base.RaisePropertyChanged(ref this.value, value);
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
            set => base.RaisePropertyChanged(ref this.value, value);
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
            set => base.RaisePropertyChanged(ref this.value, value);
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
            set => base.RaisePropertyChanged(ref this.value, value);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class AndOrModel : ValueModel<AndOr>, IAndOr, IObservable<Unit>
    {
        protected AndOr value = 0;
        List<IObserver<Unit>> observers = new List<IObserver<Unit>>();
        List<AndOrModel> list = new List<AndOrModel>();
        List<FilterModel> filters = new List<FilterModel>();
        Dictionary<Model, IDisposable> dictionary = new();

        public AndOrModel()
        {
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



        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
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

        public override void SetNode(INode node)
        {
            node.IsPersistable = true;
            node.IsExpanded = true;
            node.IsEditable = true;
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

    public enum CustomStringComparison
    {
        EqualTo, NotEqualTo, Contains, DoesNotContain, StartsWith, EndsWith, IsNull, IsNotNull, IsNotNullOrWhiteSpace
    }

    public enum NumberComparison
    {
        EqualTo, NotEqualTo, GreaterThan, GreaterThanOrEqualTo, LessThan, LessThanOrEqualTo,
    }

    public enum BooleanComparison
    {
        EqualTo, NotEqualTo
    }

    public enum ComparisonType
    {
        String, Number, Boolean
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
                    case ComparisonType.String:
                        Value = CustomStringComparison.EqualTo; break;
                    case ComparisonType.Number:
                        Value = NumberComparison.EqualTo; break;
                    case ComparisonType.Boolean:
                        Value = BooleanComparison.EqualTo; break;
                }
                RaisePropertyChanged();
            }
        }

        public Enum Value
        {
            get => _value; set
            {
                if (value == null)
                    return;
                RaisePropertyChanged(ref _value, value);
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
                    Value = NumberComparison.EqualTo; break;
                case ComparisonType.Boolean:
                    Value = BooleanComparison.EqualTo; break;
            }
            throw new NotImplementedException("f 33 dfd33");
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


    public class TransformersModel : Model
    {
        [JsonIgnore]
        public ObservableCollection<TransformerModel> Transformers { get; } = new();

        public override void SetNode(INode node)
        {
            node.Orientation = Orientation.Vertical;
            node.IsEditable = true;
            node.IsExpanded = true;
            node.IsPersistable = true;
            node.Items.AndAdditions<INode>().Subscribe(a => Transformers.Add(a.Data as TransformerModel));
            base.SetNode(node);
        }

        public override IEnumerable Proliferation()
        {
            yield return new TransformerModel { Name = "transformer" };
        }
    }

    public class FiltersModel : Model
    {
        [JsonIgnore]
        public ObservableCollection<FilterModel> Filters { get; } = new();

        public override void SetNode(INode node)
        {
            node.Orientation = Orientation.Vertical;
            node.IsEditable = true;
            node.IsExpanded = true;
            node.IsPersistable = true;
            node.Items.AndAdditions<INode>().Subscribe(a => Filters.Add(a.Data as FilterModel));
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
            set => base.RaisePropertyChanged(ref this.value, value);
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
        public StringModel Alias { get => name; set => name = value; }

        [JsonIgnore]
        public FileModel FilePath { get => filePath; set => filePath = value; }


        public override IEnumerable<Model> CreateChildren()
        {
            yield return new StringModel { Name = _string };
            //yield return new Node(_guid, new GuidModel { Name = _guid }) { IsExpanded = true };
            yield return new FileModel { Name = _filePath };
        }

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            switch (a.Data)
            {
                case StringModel stringModel: Alias = stringModel; break;
                //case _guid: Guid = a.Data as GuidModel; break;
                case FileModel fileModel: FilePath = fileModel; break;
                //case converter: Converter = a.Data as ConverterModel; break;
                default: throw new ArgumentOutOfRangeException("ds 33` 33kfl.. ");
            }
            base.Addition(value, a);
        }

        public override void SetNode(INode node)
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

        public override void SetNode(INode node)
        {
            node.Orientation = Orientation.Vertical;
            node.IsExpanded = true;
            node.IsEditable = true;
            node.IsPersistable = true;
            node.Items.AndAdditions<INode>().Subscribe(a => Collection.Add(a.Data as DatabaseModel));
            base.SetNode(node);
        }

        public override IEnumerable Proliferation()
        {
            yield return new DatabaseModel { Name = "db", Alias = new StringModel { Name = "new", Value = "New" } };
        }
    }

    public class SelectionsModel : Model
    {
        [JsonIgnore]
        public ObservableCollection<SelectionModel> Collection { get; } = new();

        public override void SetNode(INode node)
        {
            node.Orientation = Orientation.Vertical;
            node.IsExpanded = true;
            node.IsEditable = true;
            node.IsPersistable = true;
            node.Items.AndAdditions<INode>().Subscribe(a => Collection.Add(a.Data as SelectionModel));
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

        public override void SetNode(INode node)
        {
            node.IsEditable = true;
            node.Orientation = Orientation.Vertical;
            node.Items.AndAdditions<INode>().Subscribe(a => Models.Add(a.Data as NodePropertyRootModel));
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
            get => _filter;
            set => base.RaisePropertyChanged(ref this._filter, value);
        }

        [JsonIgnore]
        public NodePropertyRootsModel Element
        {
            get => _element;
            set => base.RaisePropertyChanged(ref this._element, value);
        }

        public ParameterInfo Parameter { get; set; }

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

    }

    public class InputsModel : Model
    {

        [JsonIgnore]
        public ObservableCollection<ThroughPutModel> Models { get; } = new();


        public override void SetNode(INode node)
        {
            //node.IsEditable = true;
            node.Orientation = Orientation.Vertical;
            node.Items.AndAdditions<INode>().Subscribe(a => Models.Add(a.Data as ThroughPutModel));
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
        IDisposable disposable;

        [JsonIgnore]
        public InputsModel Inputs
        {
            get => _inputs;
            set => base.RaisePropertyChanged(ref this._inputs, value);
        }

        [JsonIgnore]
        public ThroughPutModel Output
        {
            get => _output;
            set => base.RaisePropertyChanged(ref this._output, value);
        }

        [JsonIgnore]
        public ConverterModel Converter
        {
            get => converterModel;
            set
            {
                var previous = converterModel;
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
                                a.Node.Add(new ThroughPutModel() { Name = param.Name, Parameter = param });
                            }
                        });

                    });

                RaisePropertyChanged(ref previous, value);
            }
        }


        [JsonIgnore]
        public ExceptionsModel Exceptions
        {
            get => _exceptions;
            set => base.RaisePropertyChanged(ref this._exceptions, value);
        }

        public override IEnumerable<Model> CreateChildren()
        {
            yield return new ConverterModel { Name = converter };
            yield return new InputsModel { Name = inputs };
            yield return new ThroughPutModel { Name = output };
            yield return new ExceptionsModel { Name = exceptions };
        }

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            switch (a.Data.ToString())
            {
                case inputs: Inputs = a.Data as InputsModel; break;
                case output: Output = a.Data as ThroughPutModel; break;
                case converter: Converter = a.Data as ConverterModel; break;
                case exceptions: Exceptions = a.Data as ExceptionsModel; break;
                default: throw new ArgumentOutOfRangeException("ds 33` 33kfl.. ");
            }
            base.Addition(value, a);
        }

        public override void SetNode(INode node)
        {
            node.WithChangesTo(a => a.Parent).Subscribe(a =>
            {
                node.LocalIndex = a.Items.Count();
                node.IsEditable = false;
                node.IsExpanded = true;
                node.IsPersistable = true;
                base.SetNode(node);
            });


        }
    }

    public class FilterModel : Model
    {
        const string res = nameof(res);
        const string b_ool = nameof(b_ool);
        //const string type = nameof(type);
        const string _value = nameof(_value);

        //private NodePropertyRootModel resolvableModel;
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
            set => base.RaisePropertyChanged(ref this.resolvableModel, value);
        }

        [JsonIgnore]
        [Child(b_ool)]

        public ComparisonModel ComparisonModel
        {
            get => comparisonModel;
            set => base.RaisePropertyChanged(ref this.comparisonModel, value);
        }

        [JsonIgnore]
        [Child(_value)]
        public ValueModel ValueModel
        {
            get => valueModel;
            set => base.RaisePropertyChanged(ref this.valueModel, value);
        }

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            switch (a.Data.ToString())
            {
                case res:
                    ResolvableModel = a.Data as ResolvableModel;
                    ResolvableModel.Properties
                        .Changes().Select(a => ResolvableModel.Properties.LastOrDefault()?.PropertyType).WhereIsNotNull()
                        .CombineLatest(this.WithChangesTo(a => a.ValueModel))
                        .Subscribe(a =>
                        {
                            if (ValueModel.Value == null)
                            {
                                ValueModel.Value = ActivateAnything.Activate.New(a.First);
                                ValueModel.RaisePropertyChanged(nameof(ValueModel.Value));
                            }
                        });
                    ResolvableModel.Properties
                        .Changes().Select(a => ResolvableModel.Properties.LastOrDefault()?.PropertyType).WhereIsNotNull()
                        .CombineLatest(this.WithChangesTo(a => a.ComparisonModel))
                        .Subscribe(a =>
                        {
                            if (a.First == typeof(string))
                            {
                                ComparisonModel.Type = ComparisonType.String;
                            }
                            else if (a.First == typeof(long) || a.First == typeof(int) || a.First == typeof(short) || a.First == typeof(byte)
                            || (a.First == typeof(ulong) || a.First == typeof(uint) || a.First == typeof(ushort) || a.First == typeof(double) || a.First == typeof(float)))
                            {
                                ComparisonModel.Type = ComparisonType.Number;
                            }
                            else
                            {
                                ComparisonModel.Type = ComparisonType.Boolean;
                            }
                            ComparisonModel.RaisePropertyChanged(nameof(ComparisonModel.Value));
                        });
                    break;
                //ResolvableModel. break;
                case b_ool:
                    ComparisonModel = a.Data as ComparisonModel; break;
                //case type:
                //    TypeModel = a.Data as TypeModel; break;
                case _value:
                    ValueModel = a.Data as ValueModel; break;
            }
            base.Addition(value, a);
        }

        public bool Get(object instance)
        {
            if (ResolvableModel.TryGetValue(instance, out var value))
            {
                return comparisonModel.Compare(value, ValueModel.Value);
            }
            return false;

        }

        public override void SubtractDescendant(IReadOnlyTree node, int level)
        {
            //base.SubtractDescendant(node, level);
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
            //child.IsExpanded = false;
            while (child.Current != null)
            {
                items.Add(child = child.Current);
                //Extract(child);
                child.IsExpanded = false;
            }
            //while (currentNode!= top)
            //{
            //    items.Add(currentNode as INode);
            //    currentNode = currentNode.Parent;
            //    (currentNode as INode).IsExpanded = false;
            //}
            //items.Reverse();
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

    public class ContentRootModel : Model
    {

        [JsonIgnore]
        [Child("selections")]
        public SelectionsModel SelectionsModel { get; set; }




    }

    public class ResolvableModel : BreadCrumbModel, IRoot
    {
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

        public override IEnumerable<Model> CreateChildren()
        {
            yield return new GlobalAssembliesModel { Name = "ass_root" };
        }

        public override void AddDescendant(IReadOnlyTree change, int level)
        {
            if (change is INode node)
            {
                var _level = node.Level(Node);

                switch (node?.Data)
                {
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
                            propertyInfos.InsertSpecial(_level, pInfo);
                        }
                        break;
                }
            }
        }

        public override void Subtraction(IReadOnlyTree node, IReadOnlyTree a)
        {

            switch (a?.Data)
            {
                case AssemblyModel { Assembly: { } assembly }:
                    {
                        var level = a.Level(node);
                        assemblies.RemoveAtSpecial(level);
                        break;
                    }
                case TypeModel { Type: { } type }:
                    {
                        var level = a.Level(node);
                        types.RemoveAtSpecial(level);
                        break;
                    }
                case PropertyModel { Value: { } pInfo }:
                    {
                        var level = a.Level(node);
                        propertyInfos.RemoveAtSpecial(level);
                    }

                    break;

            }
        }

        public override void SubtractDescendant(IReadOnlyTree node, int level)
        {
            //var date = source.Remove(Guid.Parse(node.Key));
            //if (node is IRemoved removed)
            //{
            //    removed.Removed = date;
            //}
        }

        public override void ReplaceDescendant(IReadOnlyTree node, IReadOnlyTree old, int level)
        {

            switch (node?.Data)
            {
                case AssemblyModel { Assembly: { } assembly }:
                    {
                        assemblies.ReplaceSpecial(level, assembly);
                        break;
                    }
                case TypeModel { Type: { } type }:
                    {
                        types.ReplaceSpecial(level, type);
                        break;
                    }
                case PropertyModel { Value: { } pInfo }:
                    {
                        propertyInfos.ReplaceSpecial(level, pInfo);
                    }

                    break;
            }
        }

        public virtual bool TryGetValue(object instance, out object value)
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
        public bool TrySetValue(object instance, object value)
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

        public override void SetNode(INode node)
        {
            node.IsExpanded = true;
            base.SetNode(node);
        }
    }
}
