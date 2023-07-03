using Utility.Infrastructure.Abstractions;
using DryIoc;
using Utility.Infrastructure;
using Utility.Repos;
using Utility.PropertyTrees.WPF.Demo;
using Utility.PropertyTrees;
using Utility.PropertyTrees.WPF;
using Utility.PropertyTrees.Services;

internal class BootStrapper : BaseObject
{
    public override Key Key => new(Guids.Bootstrapper, nameof(BootStrapper), typeof(BootStrapper));

    public static Container container { get; } = new Container(DiConfiguration.SetRules);
    public static class DiConfiguration
    {
        public static Rules SetRules(Rules rules)
        {
            rules = rules
                .WithDefaultReuse(Reuse.Singleton)
                .WithAutoConcreteTypeResolution(a => true)
                .With(FactoryMethod.ConstructorWithResolvableArguments);

            return rules;
        }

    }
    public static IContainer Build()
    {
        container.RegisterMany<History>();
        container.RegisterMany<Playback>();
        //container.Register<IRepository, HttpRepository>(Reuse.Singleton);
        container.Register<IRepository, SqliteRepository>(serviceKey: nameof(SqliteRepository));
        container.Register<IRepository, LiteDBRepository>(serviceKey: nameof(LiteDBRepository));
        container.Register<IRepository, InMemoryRepository>(serviceKey: nameof(InMemoryRepository));
        container.RegisterMany<HistoryViewModel>();
        container.RegisterMany<PropertyStore>(made: Parameters.Of.Type<IRepository>(serviceKey: nameof(SqliteRepository)));
        container.RegisterMany<ViewModelStore>(made: Parameters.Of.Type<IRepository>(serviceKey: nameof(LiteDBRepository)));
        container.RegisterMany<PropertyActivator>();
        container.RegisterMany<MethodParameterActivator>();
        container.RegisterMany<MethodsBuilder>();
        container.RegisterMany<MethodActivator>();
        container.RegisterMany<ChildPropertyExplorer>();
        container.RegisterMany<MethodsExplorer>();
        container.RegisterMany<InterfaceController>();
        container.RegisterMany<VisibilityController>();

        container.RegisterMany<DescriptorFilterController>();
        container.RegisterMany<RepositorySwitchController>();
        container.RegisterMany<BootStrapper>();
        //container.RegisterInstance<BaseObject>(this);
        //foreach (var connection in Outputs)
        //    container.RegisterInstance(connection);
        container.RegisterMany<RootModelProperty>();
        container.RegisterMany<ViewBuilder>();
        container.RegisterMany<UdpServerController>();
        container.RegisterInstance(App.Current.Resources["ContentTemplateSelector"] as DataTemplateSelector);
        container.Register<StyleSelector, TreeStyleSelector>();

        container.RegisterInstance(SynchronizationContext.Current);

        //container.RegisterMany<ModelController>(nonPublicServiceTypes: true);
        container.RegisterMany<ModelController>();
        //container.RegisterMany<ViewModelController>(Reuse.Singleton);
        container.RegisterMany<ViewController>();

        //container.RegisterMany<Logger>();
        container.RegisterMany<DummyLogger>();

#if DEBUG
        container.RegisterInstance<SqliteRepository.DatabaseDirectory>(new("../../../Data"));
        container.RegisterInstance<LiteDBRepository.DatabaseSettings>(new("../../../Data", typeof(ViewModel)));
#else
        container.RegisterInstance<SqliteRepository.DatabaseDirectory>(new("Data"));
        container.RegisterInstance<LiteDBRepository.DatabaseSettings>(new("Data", typeof(ViewModel)));
#endif

        return container;
    }


    public IObservable<ObjectCreationResponse> OnNext(ObjectCreationRequest value)
    {
        var instance = Activator.CreateInstance(value.Type, value.Args);
        if (instance == null)
            throw new Exception("hyh dfgdfs3");
        foreach (var type in value.RegistrationTypes)
            container.RegisterInstance(type, instance);

        return Return(new ObjectCreationResponse(instance));
    }
}

public class RootModelProperty : RootProperty
{
    static readonly Guid guid = Guid.Parse("febe5f0b-6024-4913-8017-74475096fc52");

    public RootModelProperty() : base(guid)
    {
        Data = new RootModel();
    }
}


