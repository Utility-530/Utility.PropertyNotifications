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

        Guid MyGuid = Guid.Parse("111b7df3-9ae7-49d6-aabc-a492c6254718");

        class Guids
        {
            public static readonly Guid Model = Guid.Parse("01b52a3b-6e73-4204-a4e7-ca9ed3cf3c24");
            public static readonly Guid Server = Guid.Parse("7e0c787a-30d0-4038-9376-2808cc66a389");
            //public static readonly Guid Guid3 = Guid.Parse("af561734-bd38-472a-85e8-fdb17193bc12");
        }
  
        public override Key Key => new(MyGuid, nameof(LightBootStrapper), typeof(LightBootStrapper));

        DryIoc.Container container = new DryIoc.Container();

        public Container Build()
        {
            //using var browserScope = container.OpenScope();
            // Register individual components
            container.Register<History>(Reuse.Singleton);
            container.Register<Playback>(Reuse.Singleton);
            //container.Register<IRepository, HttpRepository>(Reuse.Singleton);
            container.Register<IRepository, SqliteRepository>(Reuse.Singleton);
            container.Register<HistoryViewModel>(Reuse.Singleton);
            container.Register<PropertyStore>(Reuse.Singleton);
            container.Register<PropertyActivator>(Reuse.Singleton);
            container.Register<PropertyFilter>(Reuse.Singleton);
            container.Register<InterfaceStore>(Reuse.Singleton);
            container.Register<Utility.GraphShapes.GraphController>(Reuse.Singleton);
            container.Register<ViewModelEngine>(Reuse.Singleton);
            container.RegisterInstance(this);
            foreach (var connection in Outputs)
                container.RegisterInstance(connection);
            container.RegisterInstance(new PropertyNode(Guids.Model), serviceKey: PropertyView.Keys.Model);
            container.RegisterInstance(new PropertyNode(Guids.Server), serviceKey: PropertyView.Keys.Server);
            //container.RegisterInstance(new PropertyNode(Guids.Guid1), serviceKey: PropertyView.Keys.Screensaver);
            //container.RegisterInstance(new PropertyNode(Guids.Guid2), serviceKey: PropertyView.Keys.PrizeWheel);
            //container.RegisterInstance(new PropertyNode(Guids.Guid3), serviceKey: PropertyView.Keys.Leaderboard);
            container.Register<PropertyViewModel>(Reuse.Singleton);
            container.Register<ViewBuilder>(Reuse.Singleton);
            container.Register<PropertyController>(Reuse.Singleton);
            container.Register<UdpServerController>(Reuse.Singleton);
         
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
            new Outputs(new Key(Guid.Empty, nameof(AutoObject), typeof(AutoObject)), new IConnection[] { new Connection<PropertyStore>(container) {}, new Connection<PropertyController>(container) { } }),
            new Outputs(new Key(Guid.Empty, nameof(LightBootStrapper), typeof(LightBootStrapper)), new IConnection[] { new Connection<PropertyActivator>(container) }),
            new Outputs(new Key(Guid.Empty, nameof(History), typeof(History)), new IConnection[] { new Connection<HistoryViewModel>(container) { IsPriority = true, SkipContext =false },  new Connection<Playback>(container) { IsPriority = true, SkipContext =false } }),
            new Outputs(new Key(Guid.Empty, nameof(HistoryViewModel), typeof(HistoryViewModel)), new IConnection[] { new Connection<Playback>(container) { IsPriority = true,  SkipContext =false } }),
            new Outputs(new Key(Guid.Empty, nameof(Playback), typeof(Playback)), new IConnection[] { new Connection<History>(container) { IsPriority = true } }),
            new Outputs(new Key(Guid.Empty, nameof(PropertyActivator), typeof(PropertyActivator)), new IConnection[] { new Connection<InterfaceStore>(container) { SkipContext = false }, new Connection<LightBootStrapper>(container) { }, 
                new Connection<PropertyFilter>(container){ }, 
                new Connection<ViewModelEngine>(container),new Connection<PropertyStore>(container),  
                new Connection<ViewBuilder>(container),
            }),
            new Outputs(new Key(Guid.Empty, nameof(InterfaceStore), typeof(InterfaceStore)), new IConnection[] {  new Connection<PropertyActivator>(container){  } }),
            new Outputs(new Key(Guid.Empty, nameof(PropertyFilter), typeof(PropertyFilter)), new IConnection[] { new Connection<PropertyNode>(container) { SkipContext = false }, new Connection<PropertyActivator>(container) { } }),
            new Outputs(new Key(Guid.Empty, nameof(PropertyNode), typeof(PropertyNode)), new IConnection[] { new Connection<PropertyFilter>(container) { IsPriority= true /*false*/ } }),
            new Outputs(new Key(Guid.Empty, nameof(Utility.Infrastructure.Resolver), typeof(Utility.Infrastructure.Resolver)), new IConnection[]{ new Connection<Utility.GraphShapes.GraphController>(container){ IsPriority = true, SkipContext = false } }),
            new Outputs(new Key(Guid.Empty, nameof(ViewModelEngine), typeof(ViewModelEngine)), new IConnection[]{ new Connection<PropertyActivator>(container){ IsPriority = true, SkipContext = false } }),
            new Outputs(new Key(Guid.Empty, nameof(PropertyStore), typeof(PropertyStore)), new IConnection[]{ new Connection<PropertyActivator>(container){ IsPriority = true, SkipContext = false }, new Connection<PropertyNode>(container){ IsPriority = true, SkipContext = false } }),
            new Outputs(new Key(Guid.Empty, nameof(PropertyViewModel),typeof(PropertyViewModel)), new IConnection[]{ new Connection<ViewBuilder>(container){ IsPriority = true, SkipContext = false }, new Connection<UdpServerController>(container){ IsPriority = true, SkipContext = false } }),
            new Outputs(new Key(Guid.Empty, nameof(ViewBuilder), typeof(ViewBuilder)), new IConnection[]{
                new Connection<PropertyViewModel>(container){ IsPriority = true, SkipContext = false }, 
                new Connection<PropertyActivator>(container){ IsPriority = true, SkipContext = false } }),
            new Outputs(new Key(Guid.Empty, nameof(UdpServerController), typeof(UdpServerController)), new IConnection[] { new Connection<PropertyViewModel>(container) { IsPriority= true /*false*/ } }),

        };

        public override bool OnNext(object value)
        {
            if (value is GuidValue { Value: ObjectCreationRequest { Args: var args, Type: var type, RegistrationType: var regType } } keyType)
            {
                var instance = Activator.CreateInstance(type, args);
                if (instance == null)
                    throw new Exception("hyh dfgdfs3");
                container.RegisterInstance(regType, instance);
                Broadcast(new GuidValue(keyType.Guid, new ObjectCreationResponse(instance), 0));
                return true;
            }
            return base.OnNext(value);
        }
    }
}