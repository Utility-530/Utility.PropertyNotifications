using Microsoft.Xaml.Behaviors;
using NetPrints.Compilation;
using NetPrints.Core;
using NetPrints.Interfaces;
using NetPrints.Reflection;
using NetPrints.ViewModels;
using NetPrints.WPF.Demo;
using NetPrints.WPF.Views;
using NetPrintsEditor.Converters;
using NetPrintsEditor.Reflection;
using Splat;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Utility.Models.Trees;
using Utility.WPF.Trees.Connectors;
using System.Reactive.Linq;
using Utility.Nodes.Demo.Filters.Services;

namespace Utility.Nodes.Demo.Transformers
{
    public class Initialiser
    {
        public static void InitialiseNetPrints()
        {
            Locator.CurrentMutable.RegisterLazySingleton<IReflectionProvider>(() => ReflectionProvider.From(Project.CreateNew("MyProject", "MyNamespace")));
            Locator.CurrentMutable.RegisterLazySingleton(() => new CodeCompiler());
            Locator.CurrentMutable.RegisterLazySingleton<IValueConverter>(() => new SuggestionListConverter());
            Locator.CurrentMutable.RegisterLazySingleton<ISpecifierConverter>(() => new SpecifierConverter());
            Locator.CurrentMutable.RegisterLazySingleton<IAssemblyReferences>(() => new DefaultReferences());
        }
    }

    public class MethodTypesEnumerable : ITypesEnumerable
    {
        Lazy<IEnumerable<ITypesProvider>> types = new(() =>
        {
            var methodQuery = Helper.CreateMethodQuery()
                        .WithArgumentType(TypeSpecifier.FromType<int>())
                        //.WithVisibleFrom(TypeSpecifier.FromType<string>())
                        .WithReturnType(TypeSpecifier.FromType<string>())
                        .WithStatic(true)
                        .AndName("Static Methods");

            return [methodQuery];
        });

        public IEnumerable<ITypesProvider> Types => types.Value;
    }

    public class VariableTypesEnumerable : ITypesEnumerable
    {
        Lazy<IEnumerable<ITypesProvider>> types = new(() =>
        {
            var methodQuery = Helper.CreateVariableQuery()
                        //.WithArgumentType(TypeSpecifier.FromType<int>())
                        //.WithVisibleFrom(TypeSpecifier.FromType<string>())
                        .WithType(TypeSpecifier.FromType<Node>())
                        .WithStatic(true)
                        .AndName("Static Methods");

            return [methodQuery];
        });

        public IEnumerable<ITypesProvider> Types => types.Value;
    }

    public class DefaultReferences : IAssemblyReferences
    {
        private static readonly IEnumerable<IAssemblyReference> references = new IAssemblyReference[]
        {
            new FrameworkAssemblyReference(".NETFramework/v4.5/System.dll"),
            new FrameworkAssemblyReference(".NETFramework/v4.5/System.Core.dll"),
            new FrameworkAssemblyReference(".NETFramework/v4.5/mscorlib.dll"),
            new AssemblyReference(@"..\..\..\Utility.Nodes\Utility.Nodes.dll")
            //new AssemblyReference(@"O:\Users\rytal\source\repos\Utility\External\NetPrints.Reflection\NetPrints.WPF.Demo\bin\Debug\net8.0-windows\\NetPrints.Core.dll")
        };

        public IEnumerable<IAssemblyReference> References => references;

    }

    public class MyAction : TargetedTriggerAction<FrameworkElement>
    {
        bool flag = false;
        ConnectionEventArgs _args;
        private AdornerLayer adornerLayer;
        ComboAdorner ca;
        Action removeAdorner;

        public MyAction()
        {
            Locator.Current.GetService<IObservable<ControlEvent>>().Subscribe(x =>
            {
                if(x.ControlEventType== ControlEventType.Select)
                {
                    removeAdorner.Invoke();
                    removeAdorner = null;

                }
                else if(x.ControlEventType== ControlEventType.Cancel)
                {
                    removeAdorner.Invoke();
                    removeAdorner = null;

                }
            });
        }

        protected override void Invoke(object parameter)
        {
            if (flag == false)
            {
                Initialiser.InitialiseNetPrints();
                flag = true;
            }
            if (parameter is ConnectionEventArgs { } args)
            {
                if (args.IsComplete == false)
                {
                    if (removeAdorner == null)
                    {                      
                        Locator.CurrentMutable.Register<ITypesEnumerable>(() => new VariableTypesEnumerable());
                        _args = args;
                        var _element = AssociatedObject as FrameworkElement;
                        adornerLayer = AdornerLayer.GetAdornerLayer(_element);
                        var inputSelection = Locator.Current.GetService<InputSelectionViewModel>();
                        ContentAdorner contentAdorner = new(_element, inputSelection, args.Location);
                        adornerLayer.Add(contentAdorner);
                        removeAdorner = new(() => adornerLayer.Remove(contentAdorner));
                    }
                    else
                    {
                        //args.Handle(true);
                    }
                }
                else
                {
                    Locator.CurrentMutable.Register<ITypesEnumerable>(() => new MethodTypesEnumerable());

                    _args = args;
                    var _element = AssociatedObject as FrameworkElement;
                    adornerLayer = AdornerLayer.GetAdornerLayer(_element);
                    ca = new ComboAdorner(_element, args.Location);
                    ca.SelectionChanged += Ca2_selectionChanged;
                    adornerLayer.Add(ca);
                }



            }
        }


        TransformerModel transformerModel = null;

        //private IObservable<TransformersModel> _transformersModel()
        //{
        //    return Observable.Create<TransformersModel>(observer =>
        //    {
        //        return Locator.Current.GetService<INodeSource>()
        //           .Single(nameof(Nodes.Filters.Factory.BuildTransformersRoot))
        //           .Subscribe(a =>
        //           {
        //               if (a.Data is TransformersModel transformersModel)
        //               {
        //                   observer.OnNext(transformersModel);
        //               }
        //           });
        //    });
        //}



        private void Ca2_selectionChanged(object obj)
        {
            if (obj is RoutedEventArgs { OriginalSource: SelectionChangedEventArgs selectionChangedEventArgs } args)
            {
                if (selectionChangedEventArgs.AddedItems.Cast<object>().Single() is SearchableItemViewModel { Value: MethodSpecifier specifier } searchableItemViewModel)
                {
                    adornerLayer.Remove(ca);

                    var rootsModel = new NodePropertyRootsModel { Name = "a" };
                    rootsModel.Collection.Add(new NodePropertyRootModel { Name = "csd ", NodeModel = new NodeModel { Name = "s", Model = new NodePropertiesModel(() => [new PropertyModel { Name = "", Value = typeof(Node).GetProperty(nameof(Node.Key)) }]) { Name = "", } } });

                    var methodInfo = MethodSpecifier.ToMethod(specifier);


                    var andorModel = new AndOrModel { Name = "", };
                    andorModel.Collection.Add(
                        new FilterModel
                        {
                            Name = "",
                            ResolvableModel = new ResolvableModel { Name = "", GlobalAssembliesModel = new GlobalAssembliesModel(() => [new AssemblyTypePropertyModel { Name = "", Value = typeof(Node).GetProperty(nameof(Node.Key)) }]) { Name = "" } },
                            ComparisonModel = new ComparisonModel { Name = "", Type = Models.Trees.ComparisonType.String },
                            Model = new ValueModel { Name = "", Value = (_args.SourceConnector.element.DataContext as Node).Key }
                        });


                    transformerModel.
                        Output = new ThroughPutModel
                        {
                            Name = TransformerModel.output,
                            Parameter = methodInfo.ReturnParameter,
                            Element = rootsModel,
                            Filter = andorModel
                        };

                    transformerModel.
                        Converter = new ConverterModel { Name = TransformerModel.converter, Method = methodInfo };

                };

            }
        }
        

        private void Ca_selectionChanged(object obj)
        {
            if (obj is RoutedEventArgs { OriginalSource: SelectionChangedEventArgs selectionChangedEventArgs } args)
            {
                if (selectionChangedEventArgs.AddedItems.Cast<object>().Single() is SearchableItemViewModel { Value: VariableSpecifier specifier } searchableItemViewModel)
                {
                    adornerLayer.Remove(ca);

                    var x = TypeSpecifier.ToType(specifier.Type);

                    //foreach (var param in )
                    //{
                    //    var roots2Model = new NodePropertyRootsModel { Name = "a" };
                    //    roots2Model.Collection.Add(new NodePropertyRootModel { Name = "csd ", NodeModel = new NodeModel { Name = "s", Model = new NodePropertiesModel(() => [new PropertyModel { Name = "", Value = typeof(Node).GetProperty(nameof(Node.Key)) }]) { Name = "", } } });
                    //    var andor2Model = new AndOrModel { Name = "", };
                    //    andor2Model.Collection.Add(
                    //        new FilterModel
                    //        {
                    //            Name = "",
                    //            ResolvableModel = new ResolvableModel { Name = "", GlobalAssembliesModel = new GlobalAssembliesModel(() => [new AssemblyTypePropertyModel { Name = "", Value = typeof(Node).GetProperty(nameof(Node.Key)) }]) { Name = "" } },
                    //            ComparisonModel = new ComparisonModel { Name = "", Type = Models.Trees.ComparisonType.String },
                    //            ValueModel = new ValueModel { Name = "", Value = (_args._Source.DataContext as Node).Key }
                    //        });

                    //    inputs.Collection.Add(new ThroughPutModel
                    //    {
                    //        Name = "",
                    //        Parameter = methodInfo.ReturnParameter,
                    //        Element = roots2Model,
                    //        Filter = andor2Model

                    //    });
                    //}
                }
            }
        }
    }

    public class ContentAdorner : Adorner
    {
        private readonly ContentControl _comboBox;
        private readonly Point position;

        public ContentAdorner(UIElement adornedElement, object model, Point position) : base(adornedElement)
        {
            _comboBox = new ContentControl
            {
                Width = 300,
                Content = model,
            };
            AddVisualChild(_comboBox);
            this.position = position;
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => _comboBox;

        protected override Size MeasureOverride(Size constraint)
        {
            _comboBox.Measure(constraint);
            return _comboBox.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _comboBox.Arrange(new Rect(new Point(position.X, position.Y /*+_comboBox.DesiredSize.Height*/), _comboBox.DesiredSize));
            return finalSize;
        }
    }


    public class ComboAdorner : Adorner
    {
        private readonly SearchableView _comboBox;
        private readonly Point position;
        public event Action<object> SelectionChanged;

        public ComboAdorner(UIElement adornedElement, Point position) : base(adornedElement)
        {
            _comboBox = new SearchableView()
            {
                Width = 300
            };
            _comboBox.SelectionChanged += _comboBox_SelectionChanged;
            AddVisualChild(_comboBox);
            this.position = position;
        }

        private void _comboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectionChanged.Invoke(e);
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => _comboBox;

        protected override Size MeasureOverride(Size constraint)
        {
            _comboBox.Measure(constraint);
            return _comboBox.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _comboBox.Arrange(new Rect(new Point(position.X, position.Y /*+_comboBox.DesiredSize.Height*/), _comboBox.DesiredSize));
            return finalSize;
        }

    }
}
