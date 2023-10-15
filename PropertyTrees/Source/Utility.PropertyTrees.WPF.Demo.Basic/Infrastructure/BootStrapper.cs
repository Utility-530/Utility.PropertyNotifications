using DryIoc;
using Utility.Infrastructure;
using Utility.Repos;
using Utility.PropertyTrees.WPF.Demo;
using Utility.PropertyTrees;
using Utility.PropertyTrees.WPF;
using Utility.PropertyTrees.Services;
using Utility.Interfaces.NonGeneric;
using System.Threading;
using System;
using System.Reactive.Linq;
using Utility.PropertyTrees.WPF.Demo.Basic;
using Utility.Models;

internal class BootStrapper : BaseObject
{
    public override Key Key => new(Utility.Guids.Bootstrapper, nameof(BootStrapper), typeof(BootStrapper));

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
        container.Register<IRepository, InMemoryRepository>();
        container.RegisterMany<RepositorySwitchController>();
        container.RegisterMany<BootStrapper>();
        container.RegisterMany<RootModelProperty>();
        container.RegisterMany<DummyLogger>();
        RegisterServices();

        return container;

        void RegisterServices()
        {
            container.RegisterMany<PropertyStore>();
            container.RegisterMany<ViewModelStore>();
            container.RegisterMany<PropertyActivator>();
            container.RegisterMany<MethodParameterActivator>();
            container.RegisterMany<MethodActivator>();
            container.RegisterMany<ChildPropertyExplorer>();
            container.RegisterMany<MethodsExplorer>();
            container.RegisterMany<DescriptorFilterController>();

            //container.RegisterMany<VisibilityController>();
            //container.RegisterMany<RepositorySwitchController>();
        }
    }


    public IObservable<ObjectCreationResponse> OnNext(ObjectCreationRequest value)
    {
        var instance = Activator.CreateInstance(value.Type, value.Args);
        if (instance == null)
            throw new Exception("hyh dfgdfs3");
        foreach (var type in value.RegistrationTypes)
            container.RegisterInstance(type, instance);

        return Observable.Return(new ObjectCreationResponse(instance));
    }
}


