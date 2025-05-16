using NetPrints.Enums;
using NetPrints.Graph;
using NetPrints.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace NetPrints.Core
{
    /// <summary>
    /// Class graph type. Contains methods, attributes and other common things usually associated
    /// with classes.
    /// </summary>
    [DataContract]
    public partial class ClassGraph : NodeGraph, IClassGraph
    {
        /// <summary>
        /// Return node of this class that receives the metadata for it.
        /// </summary>
        public ClassReturnNode ReturnNode
        {
            get => Nodes.OfType<ClassReturnNode>().Single();
        }

        /// <summary>
        /// Properties of this class.
        /// </summary>
        [DataMember]
        public IObservableCollection<IVariable> Variables { get; set; } = new ObservableRangeCollection<IVariable>();

        /// <summary>
        /// Methods of this class.
        /// </summary>
        [DataMember]
        public IObservableCollection<IMethodGraph> Methods { get; set; } = new ObservableRangeCollection<IMethodGraph>();

        /// <summary>
        /// Constructors of this class.
        /// </summary>
        [DataMember]
        public IObservableCollection<IConstructorGraph> Constructors { get; set; } = new ObservableRangeCollection<IConstructorGraph>();

        /// <summary>
        /// Base / super type of this class. The ultimate base type of all classes is System.Object.
        /// </summary>
        public TypeSpecifier SuperType
        {
            get => (TypeSpecifier)ReturnNode.SuperTypePin.InferredType ?? TypeSpecifier.FromType<object>();
        }

        /// <summary>
        /// Type this class inherits from and interfaces this class implements.
        /// </summary>
        public IEnumerable<ITypeSpecifier> AllBaseTypes
        {
            get => new[] { SuperType }.Concat(ReturnNode.InterfacePins.Select(pin => pin.InferredType ?? TypeSpecifier.FromType<object>())).Cast< ITypeSpecifier>();
        }

        /// <summary>
        /// Namespace this class is in.
        /// </summary>
        [DataMember]
        public string Namespace { get; set; }

        /// <summary>
        /// Name of the class without namespace.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Name of the class with namespace if any.
        /// </summary>
        public string FullName
        {
            get => string.IsNullOrWhiteSpace(Namespace) ? Name : $"{Namespace}.{Name}";
        }

        /// <summary>
        /// Modifiers this class has.
        /// </summary>
        [DataMember]
        public ClassModifiers Modifiers { get; set; }

        /// <summary>
        /// Visibility of this class.
        /// </summary>
        [DataMember]
        public MemberVisibility Visibility { get; set; } = MemberVisibility.Internal;

        /// <summary>
        /// Generic arguments this class takes.
        /// </summary>
        [DataMember]
        public IObservableCollection<IGenericType> DeclaredGenericArguments { get; set; } = new ObservableRangeCollection<IGenericType>();

        /// <summary>
        /// TypeSpecifier describing this class.
        /// </summary>
        public ITypeSpecifier Type
        {
            get => new TypeSpecifier(FullName, SuperType.IsEnum, SuperType.IsInterface,
                DeclaredGenericArguments.Cast<BaseType>().ToList());
        }

        public ClassGraph()
        {
            _ = new ClassReturnNode(this);
        }
    }
}
