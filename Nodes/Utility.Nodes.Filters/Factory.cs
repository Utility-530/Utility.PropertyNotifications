using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Utility.Interfaces.Exs;
using Utility.Keys;
using Utility.Models;
using Utility.Models.Trees;

namespace Utility.Nodes.Filters
{
    public class Factory
    {

        INodeSource nodeSource = Locator.Current.GetService<INodeSource>();

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

        // readonly Guid assGuid = Guid.Parse("10126895-6855-45ab-97af-21ed90c02fe8");
        // readonly Guid relGuid = Guid.Parse("5c90fcd6-2324-4f88-bffb-73b8f9fbcf6b");
        // readonly Guid rootModelGuid = Guid.Parse("76ae564a-fda0-419c-9b88-dee3ac7430c1");
        // readonly Guid assemblyGuid = Guid.Parse("ae00fed1-c58d-4e09-ac24-99cad4fbbc56");

        readonly Guid and_orGuid = Guid.Parse("2afe3e32-eb9e-4d2c-b862-5fe8ac95559f");
        readonly Guid breadcrumbGuid = Guid.Parse("ae00fed1-c58d-4e09-ac24-99cad4fbbc56");
        readonly Guid subGuid = Guid.Parse("cd79fd35-46c4-4da6-a53f-2a0fb307781a");
        readonly Guid transformerGuid = Guid.Parse("76ae564a-fda0-419c-9b88-dee3ac7430c1");
        readonly Guid filterGuid = Guid.Parse("2db6ac81-590e-4f94-98b9-215a1cd880bb");
        readonly Guid contentGuid = Guid.Parse("004cf888-a762-4149-a3b9-7a0911cdf1a9");
        readonly Guid demoContentGuid = Guid.Parse("8beee93e-fede-4c73-912f-1f3fea885c8e");
        readonly Guid htmlGuid = Guid.Parse("d44f1125-66ac-4b2e-8330-b2bdfb4797cb");
        readonly Guid htmlRenderGuid = Guid.Parse("48e723e9-5e67-4381-940d-e1f240d31ea6");
        readonly Guid controlsGuid = Guid.Parse("f4565c31-c35b-4cbf-911a-26a841d3bc04");
        private readonly Guid guid = Guid.Parse("d7bccfe8-2818-4e64-b399-f6230b087a86");
        private readonly Guid dirtyGuid = Guid.Parse("6c9ee869-9d3b-4745-b6de-6de4a8f011f1");
        private readonly Guid collectionGuid = Guid.Parse("d6fa45af-e543-4ac9-bec4-fcbd3a800097");

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


        public IObservable<INode> BuildRoot()
        {
            return nodeSource.Create(root, guid, s => new Node(s), s => new Model() { Name = s });
        }

        public IObservable<INode> BreadcrumbRoot()
        {
            return nodeSource.Create(breadcrumb, breadcrumbGuid, s => new Node(s) { }, s => new NodePropertyRootModel { Name = s });
        }

        public IObservable<INode> BuildControlRoot()
        {
            return nodeSource.Create(controls,
                controlsGuid,
                s => new Node(s) { IsExpanded = true, Orientation = Enums.Orientation.Horizontal },
                s => new Model(() => [new CommandModel { Name = Save }, new CommandModel { Name = Refresh }, new CommandModel { Name = Run }]) { Name = s });
        }

        public IObservable<INode> BuildComboRoot()
        {
            return nodeSource.Create(combo,
                subGuid,
                s => new Node(s) { },
                s => new DataFilesModel { Name = s });
        }

        public IObservable<INode> BuildTransformersRoot()
        {
            return nodeSource.Create(transformers,
                transformerGuid,
                s => new Node(s) { IsExpanded = true, Orientation = Enums.Orientation.Vertical },
                s => new TransformersModel { Name = s });
        }

        public IObservable<INode> BuildFiltersRoot()
        {
            return nodeSource.Create(filters,
                filterGuid,
                s => new Node(s) { IsExpanded = true, Orientation = Enums.Orientation.Vertical },
                s => new FiltersModel { Name = s });
        }


        public IObservable<INode> BuildAndOrRoot()
        {
            return nodeSource.Create(and_or,
                and_orGuid,
                s => new Node(s) { IsExpanded = true, Orientation = Enums.Orientation.Vertical },
                s => new AndOrModel { Name = s });
        }


        public IObservable<INode> BuildCollectionRoot()
        {
            return nodeSource.Create(collection,
                collectionGuid,
                s => new Node(s) { IsExpanded = true, Orientation = Enums.Orientation.Vertical },
                s => new StringRootModel { Name = s });
        }


        public IObservable<INode> BuildHtmlRoot()
        {
            return nodeSource.Create(html,
                htmlGuid,
                s => new Node(s),
                s => new HtmlModel { Name = s });
        }

        public IObservable<INode> BuildHtmlRenderRoot()
        {
            return nodeSource.Create(_html,
                htmlRenderGuid,
                s => new Node(s),
                s => new HtmlModel { Name = s });
        }

        public INode BuildDirty()
        {
            var data = new DirtyModels { Name = dirty };
            var node = new Node(data)
            {
                Key = new GuidKey(guid)
            };
            return node;
        }

        public IObservable<INode> BuildContentRoot()
        {
            return nodeSource.Create("Root", contentGuid, (s) => new Node(s), s => build(s));

            Model build(string s)
            {
                return new Model()
                { Name = s };
            };
        }

        public IObservable<INode> BuildDemoContentRoot()
        {
            return nodeSource.Create(demo_content, demoContentGuid, (s) => new Node(s), s => build(s));

            Model build(string s)
            {
                return new ExpandedModel(() =>
                {
                    return [
                        new ExpandedModel(() =>
                        {
                            return new List<string> { "test 1", "test 2", "test 3" }.Select(a => new Model { Name = a });
                        })
                        { Name = "Group 1" },
                        new ExpandedModel(() =>
                        {
                            return [
                             new IndexModel { Name="index 1", Value = 5 },
                             new ExpandedModel(()=> [new ExpandedModel { Name = "lower" }]) { Name = "test 5" }
                             ];
                    }){ Name = "Group 2"},
                    new ExpandedModel(() =>
                    {
                        return [
                            new ExpandedModel { Name = "test 6" } ,
                            new IndexModel { Name="index 2", Value = 8 },
                       ];
                    }){ Name = "Group 3"}
                    ];
                })
                { Name = s }; 
      
          
            };
        }
    }
}