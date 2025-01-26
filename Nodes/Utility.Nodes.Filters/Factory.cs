using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Utility.Keys;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Repos;
using Observable = System.Reactive.Linq.Observable;

namespace Utility.Nodes.Filters
{
    public class Factory
    {
        public const string tableName = "TableName";
        public const string content_root = "content_root";
        public const string filter_root = "filter_root";
        public const string transformer_root = "transformer_root";

        //static readonly Guid assGuid = Guid.Parse("10126895-6855-45ab-97af-21ed90c02fe8");
        //static readonly Guid relGuid = Guid.Parse("5c90fcd6-2324-4f88-bffb-73b8f9fbcf6b");
        //static readonly Guid rootModelGuid = Guid.Parse("76ae564a-fda0-419c-9b88-dee3ac7430c1");
        static readonly Guid assemblyGuid = Guid.Parse("ae00fed1-c58d-4e09-ac24-99cad4fbbc56");



        static readonly Guid subGuid = Guid.Parse("cd79fd35-46c4-4da6-a53f-2a0fb307781a");
        static readonly Guid filterGuid = Guid.Parse("76ae564a-fda0-419c-9b88-dee3ac7430c1");
        static readonly Guid contentGuid = Guid.Parse("004cf888-a762-4149-a3b9-7a0911cdf1a9");
        static readonly Guid htmlGuid = Guid.Parse("d44f1125-66ac-4b2e-8330-b2bdfb4797cb");
        static readonly Guid htmlRenderGuid = Guid.Parse("48e723e9-5e67-4381-940d-e1f240d31ea6");
        static readonly Guid controlsGuid = Guid.Parse("f4565c31-c35b-4cbf-911a-26a841d3bc04");

        //static readonly Guid assemblyGuid = Guid.Parse("ae00fed1-c58d-4e09-ac24-99cad4fbbc56");

        //static readonly Guid transformerGuid = Guid.Parse("2afe3e32-eb9e-4d2c-b862-5fe8ac95559f");

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


        public static IObservable<Node> BuildRoot()
        {
            return
                Observable.Create<Node>(observer =>
                {
                    var node = new Node("root");

                    return TreeRepository.Instance.InsertRoot(guid, tableName, typeof(object)).Subscribe(a =>
                    {
                        if (a.HasValue)
                        {
                            node.Data = new Model { Name = "root" };
                        }
                        node.Key = new GuidKey(guid);
                        observer.OnNext(node);
                    });
                });
        }
        public static IObservable<Node> BuildControlRoot()
        {
            Guid guid = Guid.NewGuid();

            return Observable.Create<Node>(observer =>
            {
                Node node = new("controls") { IsExpanded = true, Orientation = Enums.Orientation.Horizontal };

                return TreeRepository.Instance
                .InsertRoot(controlsGuid, "controls", typeof(object))
                .Subscribe(a =>
                {
                    if (a.HasValue)
                    {
                        node.Data = new Model(() => [new CommandModel { Name = Save }]) { Name = "controls" };
                    }
                    node.Key = new GuidKey(controlsGuid);
                    observer.OnNext(node);
                });
            });
        }

        public static IObservable<Node> BuildComboRoot()
        {
            Guid guid = Guid.NewGuid();

            return Observable.Create<Node>(observer =>
            {
                var node = new Node("combo");
                return TreeRepository.Instance
                .InsertRoot(subGuid, "combo", typeof(object))
                .Subscribe(a =>
                {
                    if(a.HasValue != false)
                    {
                        node.Data = new DatabasesModel { Name = "combo" };
                    }
                    node.Key = new GuidKey(subGuid);
                    observer.OnNext(node);
                });

            });
        }

        public static IObservable<Node> BuildFiltersRoot()
        {
            Guid guid = Guid.NewGuid();

            return Observable.Create<Node>(observer =>
            {

                var node = new Node("transformers") { IsExpanded = true, Orientation = Enums.Orientation.Vertical };

                return TreeRepository.Instance
                .InsertRoot(filterGuid, "transformers", typeof(object))
                .Subscribe(a =>
                {
                    if (a.HasValue)
                    {
                        node.Data = new TransformersModel { Name = "transformers" };
                    }
                    node.Key = new GuidKey(filterGuid);
                    observer.OnNext(node);
                });
            });
        }

        public static IObservable<Node> BuildHtmlRoot()
        {
            return Observable.Create<Node>(observer =>
            {
                var node = new Node("html") { };
                return TreeRepository.Instance
                .InsertRoot(htmlGuid, "html", typeof(object))
                .Subscribe(a =>
                {
                    if (a.HasValue)
                    {
                        node.Data = new StringModel { Name = "html" };
                    }
                    node.Key = new GuidKey(htmlGuid);
                    observer.OnNext(node);
                });
            });
        }

        public static IObservable<Node> BuildHtmlRenderRoot()
        {
            return Observable.Create<Node>(observer =>
            {
                var node = new Node("_html") { };
                return TreeRepository.Instance
                .InsertRoot(htmlRenderGuid, "_html", typeof(object))
                .Subscribe(a =>
                {
                    if(a.HasValue)
                    {
                        node.Data = new HtmlModel { Name = "_html" };
                    }
                    node.Key = new GuidKey(htmlRenderGuid);
                    observer.OnNext(node);
                });
            });
        }

        public static IObservable<Node> BuildContentRoot()
        {
            return Observable.Create<Node>(observer =>
            {
                var node = new Node("Groups");
                return TreeRepository.Instance
                .InsertRoot(contentGuid, "Groups", typeof(object))
                .Subscribe(a =>
                {
                    if(a.HasValue)
                    {
                        node.Data = build();
                    }
                    node.Key = new GuidKey(contentGuid);
                    observer.OnNext(node);
                });
            });

            static Model build()
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
                { Name = "Groups" };
            };
        }


        public static IObservable<Node> BreadcrumbRoot()
        {
            return Observable.Create<Node>(observer =>
            {
                return TreeRepository.Instance.InsertRoot(assemblyGuid, "too", typeof(object))
                .Subscribe(a =>
                {
                    var res = new Node(new NodePropertyRootModel() { Name = "too" }) { Key = new GuidKey(assemblyGuid), IsExpanded = true };
                    observer.OnNext(res);
                });
            });
        }

    }
}