using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Utility.Interfaces.Exs;
using Utility.Keys;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Repos;
using Observable = System.Reactive.Linq.Observable;

namespace Utility.Nodes.Filters
{
    public class Factory
    {
        //public const string tableName = "TableName";
        public const string content_root = "content_root";
        public const string filter_root = "filter_root";
        public const string transformer_root = "transformer_root";
        public const string filters = nameof(filters);
        public const string root = nameof(root);

        //static readonly Guid assGuid = Guid.Parse("10126895-6855-45ab-97af-21ed90c02fe8");
        //static readonly Guid relGuid = Guid.Parse("5c90fcd6-2324-4f88-bffb-73b8f9fbcf6b");
        //static readonly Guid rootModelGuid = Guid.Parse("76ae564a-fda0-419c-9b88-dee3ac7430c1");
        //static readonly Guid assemblyGuid = Guid.Parse("ae00fed1-c58d-4e09-ac24-99cad4fbbc56");
        //static readonly Guid transformerGuid = Guid.Parse("2afe3e32-eb9e-4d2c-b862-5fe8ac95559f");

        static readonly Guid assemblyGuid = Guid.Parse("ae00fed1-c58d-4e09-ac24-99cad4fbbc56");
        static readonly Guid subGuid = Guid.Parse("cd79fd35-46c4-4da6-a53f-2a0fb307781a");
        static readonly Guid transformerGuid = Guid.Parse("76ae564a-fda0-419c-9b88-dee3ac7430c1");
        static readonly Guid filterGuid = Guid.Parse("2db6ac81-590e-4f94-98b9-215a1cd880bb");
        static readonly Guid contentGuid = Guid.Parse("004cf888-a762-4149-a3b9-7a0911cdf1a9");
        static readonly Guid htmlGuid = Guid.Parse("d44f1125-66ac-4b2e-8330-b2bdfb4797cb");
        static readonly Guid htmlRenderGuid = Guid.Parse("48e723e9-5e67-4381-940d-e1f240d31ea6");
        static readonly Guid controlsGuid = Guid.Parse("f4565c31-c35b-4cbf-911a-26a841d3bc04");
        private static readonly Guid guid = Guid.Parse("d7bccfe8-2818-4e64-b399-f6230b087a86");

        public const string Refresh = nameof(Refresh);
        public const string Save = nameof(Save);
        public const string Save_Filters = nameof(Save_Filters);
        public const string Clear = nameof(Clear);
        public const string New = nameof(New);
        public const string Expand = nameof(Expand);
        public const string Collapse = nameof(Collapse);
        public const string Search = nameof(Search);
        public const string Next = nameof(Next);
        public const string Load = nameof(Load);


        public static IObservable<INode> BuildRoot()
        {
            return create(root, guid, s => new Node(s), s => new Model() { Name = s });
        }

        public static IObservable<INode> BreadcrumbRoot()
        {
            return create("too", assemblyGuid, s => new Node(s) {  }, s => new NodePropertyRootModel { Name = s });
        }

        public static IObservable<INode> BuildControlRoot()
        {
            return create("controls",
                controlsGuid,
                s => new Node(s) { IsExpanded = true, Orientation = Enums.Orientation.Horizontal },
                s => new Model(() => [new CommandModel { Name = Save }]) { Name = s });
        }

        public static IObservable<INode> BuildComboRoot()
        {
            return create("combo",
                subGuid,
                s => new Node(s) { },
                s => new DatabasesModel { Name = s });
        }

        public static IObservable<INode> BuildTransformersRoot()
        {
            return create("transformers",
                transformerGuid,
                s => new Node(s) { IsExpanded = true, Orientation = Enums.Orientation.Vertical },
                s => new TransformersModel { Name = s });
        }

        public static IObservable<INode> BuildFiltersRoot()
        {
            return create(filters,
                filterGuid,
                s => new Node(s) { IsExpanded = true, Orientation = Enums.Orientation.Vertical },
                s => new FiltersModel { Name = s });
        }


        public static IObservable<INode> BuildHtmlRoot()
        {
            return create("html",
                htmlGuid,
                s => new Node(s),
                s => new StringModel { Name = s });
        }

        public static IObservable<INode> BuildHtmlRenderRoot()
        {
            return create("_html",
                htmlRenderGuid,
                s => new Node(s),
                s => new HtmlModel { Name = s });
        }

        public static IObservable<INode> BuildContentRoot()
        {
            return create("Groups", contentGuid, (s) => new Node(s), s => build(s));

            static Model build(string s)
            {
                return new Model(() =>
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
                             new IndexModel { Name="index 1", Value = 5 },
                             new Model(()=> [new Model { Name = "lower" }]) { Name = "test 5" }
                             ];
                    }){ Name = "Group 2"},
                    new Model(() =>
                    {
                        return [
                            new Model { Name = "test 6" } ,
                            new IndexModel { Name="index 2", Value = 8 },
                       ];
                    }){ Name = "Group 3"}
                    ];
                })
                { Name = s };
            };
        }

        private static IObservable<INode> create(string name, Guid guid, Func<string, Node> nodeFactory, Func<string, Model> modelFactory)
        {
            return Observable.Create<INode>(observer =>
            {
                var data = modelFactory(name);
                return Locator.Current.GetService<ITreeRepository>()
                .InsertRoot(guid, name, data.GetType())
                .Subscribe(a =>
                {
                    var node = nodeFactory(name);
                    if (a.HasValue)
                    {
                        node.Data = data;
                    }
                    node.Key = new GuidKey(guid);
                    observer.OnNext(node);
                });
            });
        }

    }
}