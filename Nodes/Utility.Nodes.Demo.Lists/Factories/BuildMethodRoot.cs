using System;
using System.Collections.Generic;
using System.Text;
using NetPrints.Core;
using NetPrints.Interfaces;
using NetPrintsEditor.Converters;
using NetPrintsEditor.Reflection;
using Splat;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Models;
using Utility.Models.Templates;
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Meta;
using Utility.Services;
using Utility.Services.Meta;
using Utility.ServiceLocation;
using Utility.Collections;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        public INodeViewModel BuildMethodRoot()
        {
            Locator.CurrentMutable.RegisterLazySingleton<IReflectionProvider>(() => ReflectionProvider.Empty());
            Locator.CurrentMutable.RegisterLazySingleton<ISpecifierConverter>(() => new SpecifierConverter());

            var guid = Guid.Parse(MetaDataFactory.methodInfoGuid);
            buildNetwork(guid);

            return new Model(() => [
                new Model(()=>[
                     new Model(attach: model => model.Observe<nameInputParam>(guid:guid)){
                          Name = "Search",
                          DataTemplate = Templates.SearchEditor,
                      },

                      new Model(attach: model=>{
                            model.Observe<isStaticInputParam>(guid: guid);
                      }){
                          Name = "IsStatic",
                          DataTemplate = "StaticInstanceTemplate",
                          Title = "Is Static",
                          Row =1,
                      },

                      new Model(attach: model => model.Observe<argumentTypeInputParam>(guid:guid)){
                          Name = "ArgumentType",
                          Type = typeof(Type),
                          Row =1,
                      },
                      new Model(attach: model => model.Observe<returnTypeInputParam>(guid:guid)){
                          Name = "ReturnType",
                          Type = typeof(Type),
                          Row =1,
                      }
                    ])
                {
                    Name="Variables",
                    Arrangement = Enums.Arrangement.MasterSlave,
                    Orientation = Enums.Orientation.Vertical,
                },
                new Model(attach:
                model =>
                {
                    model.ReactTo<collectionReturnParam, ExtendableWindowedCollection<ISpecifier>>(guid: guid);
                })
                {
                    Name = "Items",
                    DataTemplate = "CustomDataGridTemplate",
                    ShouldValueBeTracked = false
                }])
            {
                Guid = guid
            };
        }

        static void buildNetwork(Guid guid)
        {
            var serviceResolver = Globals.Resolver.Resolve<IServiceResolver>(guid.ToString());
            serviceResolver.Connect<specifiersReturnParam, specifiersInputParam>();
        }
    }

    public record isStaticInputParam() : Param<Service>(nameof(Service.Specifiers), "isStatic");
    public record nameInputParam() : Param<Service>(nameof(Service.Specifiers), "name");
    public record argumentTypeInputParam() : Param<Service>(nameof(Service.Specifiers), "argumentType");
    public record returnTypeInputParam() : Param<Service>(nameof(Service.Specifiers), "returnType");
    public record specifiersReturnParam() : Param<Service>(nameof(Service.Specifiers));
    public record specifiersInputParam() : Param<Service>(nameof(Service.ToCollection), "specifiers");
    public record collectionReturnParam() : Param<Service>(nameof(Service.ToCollection));
  
    public class Service
    {
        public static IEnumerable<ISpecifier> Specifiers(bool? isStatic = default, string? name = null, Type argumentType = null, Type returnType = null)
        {
            return Locator.Current.GetService<IReflectionProvider>()
            .GetMethods(
                new ReflectionProviderMethodQuery()
                      .WithArgumentType(argumentType == null ? null : TypeSpecifier.FromType(argumentType))
                      //.WithVisibleFrom(TypeSpecifier.FromType<string>())
                      .WithReturnType(returnType == null ? null : TypeSpecifier.FromType(returnType))
                      .WithNameLike(name)
                      .WithStatic(isStatic));
        }

        public static ExtendableWindowedCollection<ISpecifier> ToCollection(IEnumerable<ISpecifier> specifiers)
        {
            return new ExtendableWindowedCollection<ISpecifier>(specifiers, new object(), 10);
        }
    }
}
