using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Diagrams;
using Utility.Nodes.Meta;
using Utility.Nodify.Generator.Services;
using Utility.ServiceLocation;

namespace Utility.Nodify.Demo
{
    internal class FactoryOne : EnumerableMethodFactory
    {
        const string react_to_3 = nameof(react_to_3);
        const string react_to_4 = nameof(react_to_4);
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
                        a.Observe<InstanceTypeParam>();
                    }) { Name = react_to_3 },
                    new Model<string>(attach:a=>{
                        a.Observe<FilterParam>();
                    }) { Name = react_to_4 }
                ])
                { Key = Guid, IsExpanded = true };
        }

    }
}
