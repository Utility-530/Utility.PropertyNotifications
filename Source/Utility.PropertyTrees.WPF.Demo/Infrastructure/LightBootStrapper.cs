using Utility.PropertyTrees.Infrastructure;
using Utility.Infrastructure.Abstractions;
using DryIoc;
using Utility.Infrastructure;
using Utility.PropertyTrees.WPF.Demo.Infrastructure;
using System;
using Utility.Models;
using Outputs = Utility.Infrastructure.Outputs;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class LightBootStrapper : BaseObject
    {

        Guid Guid = Guid.Parse("111b7df3-9ae7-49d6-aabc-a492c6254718");
        public override Key Key => new(Guid, nameof(LightBootStrapper), typeof(LightBootStrapper));

        DryIoc.Container container = new DryIoc.Container();

        public Container Build()
        {
            //using var browserScope = container.OpenScope();
            // Register individual components
            container.Register<History>(Reuse.Singleton);
            container.Register<Playback>(Reuse.Singleton);
            container.Register<IRepository, HttpRepository>(Reuse.Singleton);
            container.Register<HistoryViewModel>(Reuse.Singleton);
            container.Register<PropertyStore>(Reuse.Singleton);
            container.Register<PropertyActivator>(Reuse.Singleton);
            container.Register<PropertyFilter>(Reuse.Singleton);
            container.Register<InterfaceStore>(Reuse.Singleton);
            container.Register<Utility.GraphShapes.Interface>(Reuse.Singleton);
            container.Register<ViewModelEngine>(Reuse.Singleton);
            container.RegisterInstance(this);
            foreach (var connection in Outputs)
                container.RegisterInstance(connection);
            var propertyNode = new PropertyNode(Guid.Parse("7e0c787a-30d0-4038-9376-2808cc66a389"));
            container.RegisterInstance(propertyNode, serviceKey: PropertyView.Key1);

            //container.RegisterSelf();
            // Scan an assembly for components
            //builder.RegisterAssemblyTypes(myAssembly)
            //       .Where(t => t.Name.EndsWith("Repository"))
            //       .AsImplementedInterfaces();

            //var container = builder.Compile();
            return container;
        }

        private Outputs[] Outputs => new[]
        {
            new Outputs(key => key.Type.IsAssignableTo(typeof(AutoObject)), new IConnection[] { new Connection<PropertyStore>(container) {}}),
            new Outputs(key => key.Name == nameof(LightBootStrapper), new IConnection[] { new Connection<PropertyActivator>(container) }),
            new Outputs(key => key.Name == nameof(History), new IConnection[] { new Connection<HistoryViewModel>(container) { IsPriority = true, SkipContext =false } }),
            new Outputs(key => key.Name == nameof(HistoryViewModel), new IConnection[] { new Connection<Playback>(container) { IsPriority = true,  SkipContext =false } }),
            new Outputs(key => key.Name == nameof(Playback), new IConnection[] { new Connection<History>(container) { IsPriority = true } }),
            new Outputs(key => key.Name == nameof(PropertyActivator), new IConnection[] { new Connection<InterfaceStore>(container) { SkipContext = false }, new Connection<LightBootStrapper>(container) { }, 
                new Connection<PropertyFilter>(container){ }, new Connection<ViewModelEngine>(container),new Connection<PropertyStore>(container)  }),
            new Outputs(key => key.Name == nameof(InterfaceStore), new IConnection[] {  new Connection<PropertyActivator>(container){  } }),
            new Outputs(key => key.Name == nameof(PropertyFilter), new IConnection[] { new Connection<PropertyNode>(container) { SkipContext = false }, new Connection<PropertyActivator>(container) { } }),
            new Outputs(key => key.Name == nameof(PropertyNode), new IConnection[] { new Connection<PropertyFilter>(container) { IsPriority= false } }),
            new Outputs(key => key.Name == nameof(Utility.Infrastructure.Resolver), new IConnection[]{ new Connection<Utility.GraphShapes.Interface>(container){ IsPriority = true, SkipContext = false } }),
            new Outputs(key => key.Name == nameof(ViewModelEngine), new IConnection[]{ new Connection<PropertyActivator>(container){ IsPriority = true, SkipContext = false } }),
            new Outputs(key => key.Name == nameof(PropertyStore), new IConnection[]{ new Connection<PropertyActivator>(container){ IsPriority = true, SkipContext = false } })
        };



        public override void OnNext(object value)
        {
            if (value is GuidValue { Value: ObjectCreationRequest { Args: var args, Type: var type, RegistrationType: var regType } } keyType)
            {
                var instance = Activator.CreateInstance(type, args);
                if (instance == null)
                    throw new Exception("hyh dfgdfs3");
                container.RegisterInstance(regType, instance);
                Broadcast(new GuidValue(keyType.Guid, new ObjectCreationResponse(instance), 0));
            }
        }
    }
}