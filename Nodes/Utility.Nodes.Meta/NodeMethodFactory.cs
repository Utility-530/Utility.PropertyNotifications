using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Utility.Entities.Comms;
using Utility.Enums;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Models;
using Utility.Models.Trees;
using Utility.ServiceLocation;
using Utility.Services.Meta;

namespace Utility.Nodes.Meta
{
    public class EnumerableMethodFactory : IEnumerableFactory<Method>
    {
        // needs to be a property in case INodeSource changes
        protected INodeSource nodeSource => Globals.Resolver.Resolve<INodeSource>();
        protected IServiceResolver serviceResolver => Globals.Resolver.Resolve<IServiceResolver>();
        public IEnumerable<Method> Create(object config) => this.GetType().InstantMethods().Where(a => a.Name != nameof(Create)).Select(a => new Method(a, this));
    }

    public class NodeMethodFactory : EnumerableMethodFactory
    {
        //public const string tableName = "TableName";
        public const string content_root = nameof(content_root);

        public const string filter_root = nameof(filter_root);
        public const string transformer_root = nameof(transformer_root);
        public const string and_or = nameof(and_or);
        public const string filters = nameof(filters);
        public const string root = nameof(root);
        public const string transformers = nameof(transformers);
        public const string combo = nameof(combo);
        public const string controls = nameof(controls);
        public const string breadcrumb = nameof(breadcrumb);
        public const string html = nameof(html);
        public const string _html = nameof(_html);
        public const string dirty = nameof(dirty);
        public const string collection = nameof(collection);
        public const string demo_content = nameof(demo_content);
        public const string input_control = nameof(input_control);
        public const string input_node = nameof(input_node);

        // readonly Guid assGuid = Guid.Parse("10126895-6855-45ab-97af-21ed90c02fe8");
        // readonly Guid relGuid = Guid.Parse("5c90fcd6-2324-4f88-bffb-73b8f9fbcf6b");
        // readonly Guid rootModelGuid = Guid.Parse("76ae564a-fda0-419c-9b88-dee3ac7430c1");
        // readonly Guid assemblyGuid = Guid.Parse("ae00fed1-c58d-4e09-ac24-99cad4fbbc56");

        private readonly Guid and_orGuid = Guid.Parse("2afe3e32-eb9e-4d2c-b862-5fe8ac95559f");
        private readonly Guid breadcrumbGuid = Guid.Parse("ae00fed1-c58d-4e09-ac24-99cad4fbbc56");
        private readonly Guid subGuid = Guid.Parse("cd79fd35-46c4-4da6-a53f-2a0fb307781a");
        private readonly Guid transformerGuid = Guid.Parse("76ae564a-fda0-419c-9b88-dee3ac7430c1");
        private readonly Guid filterGuid = Guid.Parse("2db6ac81-590e-4f94-98b9-215a1cd880bb");
        private readonly Guid contentGuid = Guid.Parse("004cf888-a762-4149-a3b9-7a0911cdf1a9");
        private readonly Guid demoContentGuid = Guid.Parse("8beee93e-fede-4c73-912f-1f3fea885c8e");
        private readonly Guid htmlGuid = Guid.Parse("d44f1125-66ac-4b2e-8330-b2bdfb4797cb");
        private readonly Guid htmlRenderGuid = Guid.Parse("48e723e9-5e67-4381-940d-e1f240d31ea6");
        private readonly Guid controlsGuid = Guid.Parse("f4565c31-c35b-4cbf-911a-26a841d3bc04");
        private readonly Guid guid = Guid.Parse("d7bccfe8-2818-4e64-b399-f6230b087a86");
        private readonly Guid dirtyGuid = Guid.Parse("6c9ee869-9d3b-4745-b6de-6de4a8f011f1");
        private readonly Guid collectionGuid = Guid.Parse("d6fa45af-e543-4ac9-bec4-fcbd3a800097");
        private readonly Guid input_controlGuid = Guid.Parse("d31ca5b1-0de0-41cb-8c7d-e83a4f0c8237");
        private readonly Guid input_nodeGuid = Guid.Parse("3dded14d-0f46-4ab8-bce3-50ac339e6d97");

        public const string Refresh = nameof(Refresh);
        public const string Run = nameof(Run);
        public const string Save = nameof(Save);
        public const string Save_Filters = nameof(Save_Filters);
        public const string Clear = nameof(Clear);
        public const string New = nameof(New);
        public const string Expand = nameof(Expand);
        public const string Collapse = nameof(Collapse);
        public const string Search = nameof(Search);
        public const string Next = nameof(Next);
        public const string Load = nameof(Load);
        public const string Select = nameof(Select);
        public const string Cancel = nameof(Cancel);

        private NodeMethodFactory()
        {
        }

        public IObservable<INodeViewModel> BuildRoot()
        {
            return nodeSource.Create(root, guid, s => new Model() { Name = s });
        }

        public IObservable<INodeViewModel> BreadcrumbRoot()
        {
            return nodeSource.Create(breadcrumb, breadcrumbGuid, s => new NodePropertyRootModel { Name = s });
        }

        public IObservable<INodeViewModel> BuildControlRoot()
        {
            return nodeSource.Create(controls,
                controlsGuid,
                s => new Model(() => [new CommandModel<SaveEvent> { Name = Save }, new CommandModel<RefreshEvent> { Name = Refresh }, new CommandModel<RunEvent> { Name = Run }],
                attach: n => { n.IsExpanded = true; n.Orientation = Enums.Orientation.Horizontal; })
                { Name = s });
        }

        public IObservable<INodeViewModel> BuildInputControlRoot()
        {
            return nodeSource.Create(input_control,
                input_controlGuid,
                s => new Model(() => [new CommandModel<SelectEvent> { Name = Select }, new CommandModel<CancelEvent> { Name = Cancel }], attach: n =>
                {
                    n.IsExpanded = true; n.Orientation = Enums.Orientation.Horizontal; n.IsContentVisible = false;
                })
                { Name = s });
        }

        public IObservable<INodeViewModel> BuildInputNodeRoot()
        {
            return nodeSource.Create(input_node,
                input_nodeGuid,
                s => new Model(() => [new NodePropertyRootModel { Name = "npm" }], attach: n => n.IsExpanded = true) { Name = s });
        }

        public IObservable<INodeViewModel> BuildComboRoot()
        {
            return nodeSource.Create(combo,
                subGuid,
                s => new DataFilesModel { Name = s });
        }

        public IObservable<INodeViewModel> BuildTransformersRoot()
        {
            return nodeSource.Create(transformers,
                transformerGuid,
                s => new TransformersModel() { Name = s, IsExpanded = true });
        }

        public IObservable<INodeViewModel> BuildFiltersRoot()
        {
            return nodeSource.Create(filters,
                filterGuid,
                s => new FiltersModel() { Name = s, IsExpanded = true, Orientation = Orientation.Vertical });
        }

        public IObservable<INodeViewModel> BuildAndOrRoot()
        {
            return nodeSource.Create(and_or,
                and_orGuid,
                s => new AndOrModel() { IsExpanded = true, Orientation = Enums.Orientation.Vertical });
        }

        public IObservable<INodeViewModel> BuildCollectionRoot()
        {
            return nodeSource.Create(collection,
                collectionGuid,
                s => new Model<string>(attach: n => { n.IsExpanded = true; n.Orientation = Enums.Orientation.Vertical; }) { Name = s });
        }

        public IObservable<INodeViewModel> BuildHtmlRoot()
        {
            return nodeSource.Create(html,
                htmlGuid,
                s => new Model<string>(attach: n => n.DataTemplate = "Html") { Name = s });
        }

        public IObservable<INodeViewModel> BuildHtmlRenderRoot()
        {
            return nodeSource.Create(_html,
                htmlRenderGuid,
                s => new Model<string>(attach: n => n.DataTemplate = "Html") { Name = s });
        }

        //public INodeViewModel BuildDirty()
        //{
        //    var data = new DirtyModels { Name = dirty };
        //    var node = new NodeViewModel(data)
        //    {
        //        Key = new GuidKey(guid)
        //    };
        //    return node;
        //}

        //public IObservable<INodeViewModel> BuildContentRoot()
        //{
        //    return nodeSource.Create("Root", contentGuid, s => new CollectionModel() { Name = s });
        //}

        public IObservable<INodeViewModel> BuildDemoContentRoot()
        {
            return nodeSource.Create(demo_content, demoContentGuid, s =>
                new ExpandedModel(() =>
                {
                    return [
                        new Model(() =>
                        {
                            return new List<string> { "test 1", "test 2", "test 3" }.Select(a => new Model { Name = a });
                        })
                        { Name = "Group 1" },
                        new Model(() =>
                        {
                            return [
                            new Model<int> { Name="index 1", Value = 5, DataTemplate = "IndexTemplate" },
                            new Model(()=> [new Model { Name = "lower" }]) { Name = "test 5" }
                            ];
                        }){ Name = "Group 2"},
                    new ExpandedModel(() =>
                    {
                        return [
                            new Model { Name = "test 6" } ,
                            new Model<int> { Name="index 2", Value = 8, DataTemplate = "IndexTemplate" },
                        ];
                    }){ Name = "Group 3"}
                    ];
                })
                { Name = s });
        }

        public static NodeMethodFactory Instance { get; } = new();
    }
}