using Utility.Infrastructure.Abstractions;
using DryIoc;
using Utility.Infrastructure;
using Utility.Models;
using Outputs = Utility.Infrastructure.Outputs;
using Connection = Utility.Infrastructure.Connection;
using Utility.Repos;
using Utility.PropertyTrees.WPF.Demo;
using Utility.PropertyTrees;
using Utility.PropertyTrees.WPF;
using Utility.PropertyTrees.Demo.Model;
using Utility.Nodes;
using Utility.Observables.NonGeneric;
using Utility.PropertyTrees.Services;

internal class BootStrapper : BaseObject
{
    public override Key Key => new(Guids.LightBootstrapper, nameof(BootStrapper), typeof(BootStrapper));

    public static Container container { get; } = new Container(DiConfiguration.SetRules);
    public static class DiConfiguration
    {
        public static Rules SetRules(Rules rules)
        {
            rules = rules
                .WithDefaultReuse(Reuse.Singleton)
                .With(FactoryMethod.ConstructorWithResolvableArguments);
            return rules;
        }

    }
    public IContainer Build()
    {
        //using var browserScope = container.OpenScope();
        // Register individual components
        container.RegisterMany<History>();
        container.RegisterMany<Playback>();
        //container.Register<IRepository, HttpRepository>(Reuse.Singleton);
        container.Register<IRepository, SqliteRepository>();
        container.RegisterMany<HistoryViewModel>();
        container.RegisterMany<PropertyStore>();
        container.RegisterMany<PropertyActivator>();
        container.RegisterMany<ChildPropertyExplorer>();
        container.RegisterMany<InterfaceController>();
       
        container.RegisterMany<ViewModelEngine>();
        container.RegisterInstanceMany(this);
        //container.RegisterInstance<BaseObject>(this);
        //foreach (var connection in Outputs)
        //    container.RegisterInstance(connection);
        container.RegisterMany<ModelProperty>();
        container.RegisterMany<ServerProperty>();
        container.RegisterMany<ModelViewModel>();
        container.RegisterMany<ViewBuilder>();
        //container.RegisterMany<PropertyController>(Reuse.Singleton);
        container.RegisterMany<UdpServerController>();
        container.RegisterInstance(App.Current.Resources["ContentTemplateSelector"] as ContentTemplateSelector);
        container.RegisterInstance(SynchronizationContext.Current);
        //container.RegisterMany<ModelController>(Reuse.Singleton);
        container.RegisterMany<ModelController>();

        //container.RegisterMany<Logger>();
        container.RegisterMany<DummyLogger>();

        container.RegisterInstance<SqliteRepository.DatabaseDirectory>(new("../../../Data"));
        //container.RegisterSelf();
        // Scan an assembly for components
        //builder.RegisterAssemblyTypes(myAssembly)
        //       .Where(t => t.Name.EndsWith("Repository"))
        //       .AsImplementedInterfaces();

        //var container = builder.Compile();
        return container;
    }

    //private Outputs[] Outputs => new[]
    //{
    //        new Outputs(new DynamicConnection(), new Connection[] { new Connection<PropertyStore>() {}, new Connection<ChildPropertyExplorer>() /*new Connection<PropertyController>() { }*/ }),
    //        new Outputs(new Connection<LightBootStrapper>(), new Connection[] { new Connection<PropertyActivator>() }),
    //        //new Outputs(new Connection<HistoryController>(), new IConnection[] { new Connection<HistoryViewModel>(container) { IsPriority = true, SkipContext =false },  new Connection<Playback>(container) { IsPriority = true, SkipContext =false } }),
    //        //new Outputs(new Connection<HistoryViewModel>(), new IConnection[] { new Connection<Playback>(container) { IsPriority = true,  SkipContext =false } }),
    //        //new Outputs(new Connection<Playback>(), new Connection[] { new Connection<History>() {  } }),
    //        new Outputs(new Connection<PropertyActivator>(), new Connection [] {
    //            new Connection<InterfaceController>() {  },
    //            new Connection<LightBootStrapper>() { },
    //            new Connection<ChildPropertyExplorer>(){ },
    //            new Connection<ViewModelEngine>(),
    //            new Connection<PropertyStore>(),
    //            new Connection<ViewBuilder>(),
    //            new Connection<ModelViewModel>(),
    //        }),
    //        new Outputs(new Connection<InterfaceController>(), new Connection[] {  new Connection<PropertyActivator>(){  } }),
    //        new Outputs(new Connection<ChildPropertyExplorer>(), new Connection[] { new DynamicConnection() { }, new Connection<PropertyActivator>() { } }),
         
    //        //new Outputs(new Connection<Utility.Infrastructure.Resolver>(), new IConnection[]{ new Connection<Utility.Graph.Shapes.GraphController>(container){ IsPriority = true, SkipContext = false } }),
    //        new Outputs(new Connection<ViewModelEngine>(), new Connection[]{ new Connection<PropertyActivator>(){ } }),
    //        new Outputs(new Connection<PropertyStore>(), new Connection[]{ new Connection<PropertyActivator>(){  }, new DynamicConnection(){  } }),
    //        new Outputs(new Connection<ModelController>(), new Connection[]{
    //            new Connection<ViewBuilder>(){  },
    //            new Connection<UdpServerController>(){  } ,
    //            new Connection<PropertyActivator>(){  }
    //        }),
    //        new Outputs(new Connection < ViewBuilder > (),
    //            new Connection[]{
    //            new Connection<ModelViewModel>( ){  },
    //            new Connection<PropertyActivator>( ){},
    //        }),
    //        new Outputs(new Connection<UdpServerController>(), new Connection[] { new Connection<ModelViewModel>() {  } }),

    //    };

    public IObservable<ObjectCreationResponse> OnNext(ObjectCreationRequest value)
    {
        var instance = Activator.CreateInstance(value.Type, value.Args);
        if (instance == null)
            throw new Exception("hyh dfgdfs3");
        foreach (var type in value.RegistrationTypes)
            container.RegisterInstance(type, instance);

        return Create<ObjectCreationResponse>(observer => 
        { 
            observer.OnNext(new ObjectCreationResponse(instance));
            observer.OnCompleted(); 
            return Disposer.Empty; 
        });
    }
}

public class ModelProperty : RootProperty
{
    public ModelProperty() : base(Guids.Model)
    {
        Data = new Model();
    }
}

public class ServerProperty : RootProperty
{
    public ServerProperty() : base(Guids.Server)
    {
        Data = new Server();
    }
}

