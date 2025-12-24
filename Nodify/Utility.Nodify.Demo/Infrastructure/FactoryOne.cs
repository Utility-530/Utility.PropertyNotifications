using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Diagrams;
using Utility.Nodes;
using Utility.Nodes.Meta;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Demo.Services;
using Utility.ServiceLocation;

namespace Utility.Nodify.Demo
{
    internal class FactoryOne : EnumerableMethodFactory
    {
        const string react_to_3 = nameof(react_to_3);
        const string react_to_4 = nameof(react_to_4);
        const string action = nameof(action);
        const string root = nameof(root);
        const string Guid = "abf119dc-4dc2-4172-975f-3bcb5980bf3a";

        public static void connect(IServiceResolver serviceResolver)
        {
            serviceResolver.Connect<PredicateReturnParam, PredicateParam>();
            serviceResolver.Connect<ListInstanceReturnParam, ListInParam>();
            serviceResolver.Connect<ListInstanceReturnParam, ListParam>();
        }

        public Model BuildContainer()
        {
            return
                new Model(() =>
                [
                    new Model<Type>(attach: a=>{
                                attach(a);
                        a.Observe<InstanceTypeParam>();
                

                    }) { Name = react_to_3 },
                    new Model<string>(attach:a=>{
                            attach(a);
                        a.Observe<FilterParam>();
                    
                    }) { Name = react_to_4 },
                    new Model<bool>(attach:attach)
                    {
                        DataTemplate = nameof(Utility.Models.Templates.Templates.ActionTemplate),
                        Name = action
                    }
                ])
                { Key = Guid, IsExpanded = true };

            void attach(Model a)
            {
                var x = Utility.Globals.Resolver.Resolve<IViewModelFactory>().CreatePendingConnector(true);
                var y = Utility.Globals.Resolver.Resolve<IViewModelFactory>().CreatePendingConnector(null);
                x.Node = a;
                y.Node = a;
                a.Inputs = new CollectionWithFixedLast<IConnectorViewModel>(x);
                a.Outputs = new CollectionWithFixedLast<IConnectorViewModel>(y);
            }
        }

    }
}
