using NetPrints.Enums;
using NetPrints.Graph;
using NetPrints.Interfaces;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace NetPrints.Core
{
    /// <summary>
    /// Specifier describing a property of a class.
    /// </summary>
    [Serializable]
    [DataContract(Name = "PropertySpecifier")]
    //[AddINotifyPropertyChangedInterface]
    public class Variable
    {
        /// <summary>
        /// Name of the variable without any prefixes.
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Class this variable is contained in.
        /// </summary>
        [DataMember]
        public IClassGraph Class
        {
            get;
            private set;
        }

        /// <summary>
        /// Specifier for the type of the variable.
        /// </summary>
        public ITypeSpecifier Type => TypeGraph.ReturnType;

        [DataMember(Name="Type", EmitDefaultValue = false, IsRequired = false)]
        private TypeSpecifier OldType
        {
            get => null;
            set
            {
                TypeGraph = new TypeGraph();
                GraphUtil.CreateNestedTypeNode(TypeGraph, value, 500, 500);
            }
        }

        /// <summary>
        /// Get method for this variable. Can be null.
        /// </summary>
        [DataMember]
        public IMethodGraph GetterMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Set method for this variable. Can be null.
        /// </summary>
        [DataMember]
        public IMethodGraph SetterMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Graph specifying the type of this variable.
        /// </summary>
        [DataMember]
        public ITypeGraph TypeGraph
        {
            get;
            set;
        }

        /// <summary>
        /// Whether this variable has a public getter.
        /// </summary>
        public bool HasPublicGetter
        {
            get => HasAccessors ?
                (GetterMethod?.Visibility == MemberVisibility.Public) :
                Visibility == MemberVisibility.Public;
        }

        /// <summary>
        /// Whether this variable has a public setter.
        /// </summary>
        public bool HasPublicSetter
        {
            get => HasAccessors ?
                (SetterMethod?.Visibility == MemberVisibility.Public) :
                Visibility == MemberVisibility.Public;
        }

        /// <summary>
        /// Whether this variable declares a get or set method.
        /// </summary>
        public bool HasAccessors
        {
            get => GetterMethod != null || SetterMethod != null;
        }

        /// <summary>
        /// Whether this property is static.
        /// </summary>
        [DataMember]
        [Obsolete]
        public bool IsStatic
        {
            get;
            private set;
        }

        /// <summary>
        /// Visibility of this property.
        /// </summary>
        [DataMember]
        public MemberVisibility Visibility
        {
            get;
            set;
        } = MemberVisibility.Private;

        /// <summary>
        /// Modifiers of this variable.
        /// </summary>
        [DataMember]
        public VariableModifiers Modifiers
        {
            get;
            set;
        }

        public VariableSpecifier Specifier
        {
            get => new VariableSpecifier(Name, Type, GetterMethod?.Visibility ?? Visibility, SetterMethod?.Visibility ?? Visibility, Class.Type, Modifiers);
        }

        /// <summary>
        /// Creates a PropertySpecifier.
        /// </summary>
        /// <param name="cls">Graph the variable is a part of.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Specifier for the type of this property.</param>
        /// <param name="getter">Get method for the property. Can be null if there is none.</param>
        /// <param name="setter">Set method for the property. Can be null if there is none.</param>
        /// <param name="modifiers">Modifiers of the variable.</param>
        public Variable(ClassGraph cls, string name, TypeSpecifier type, MethodGraph getter,
            MethodGraph setter, VariableModifiers modifiers)
        {
            Class = cls;
            Name = name;
            GetterMethod = getter;
            SetterMethod = setter;
            Modifiers = modifiers;

            // Create a type graph with the type as its return type.
            TypeGraph = new TypeGraph();
            INodeOutputTypePin typePin = GraphUtil.CreateNestedTypeNode(TypeGraph, type, 500, 300).OutputTypePins[0];
            TypeGraph.ReturnNode.PositionX = 800;
            TypeGraph.ReturnNode.PositionY = 300;
            GraphUtil.ConnectTypePins(typePin, TypeGraph.ReturnNode.TypePin);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (TypeGraph is null)
            {
                TypeGraph = new TypeGraph();
            }
        }
    }
}
