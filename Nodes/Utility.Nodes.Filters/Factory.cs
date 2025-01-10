using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

        static readonly Guid assGuid = Guid.Parse("10126895-6855-45ab-97af-21ed90c02fe8");
        static readonly Guid relGuid = Guid.Parse("5c90fcd6-2324-4f88-bffb-73b8f9fbcf6b");
        static readonly Guid rootModelGuid = Guid.Parse("76ae564a-fda0-419c-9b88-dee3ac7430c1");
        static readonly Guid assemblyGuid = Guid.Parse("ae00fed1-c58d-4e09-ac24-99cad4fbbc56");



        static readonly Guid subGuid = Guid.Parse("cd79fd35-46c4-4da6-a53f-2a0fb307781a");
        static readonly Guid filterGuid = Guid.Parse("76ae564a-fda0-419c-9b88-dee3ac7430c1");
        static readonly Guid contentGuid = Guid.Parse("004cf888-a762-4149-a3b9-7a0911cdf1a9");
        static readonly Guid htmlGuid = Guid.Parse("d44f1125-66ac-4b2e-8330-b2bdfb4797cb");
        static readonly Guid htmlRenderGuid = Guid.Parse("48e723e9-5e67-4381-940d-e1f240d31ea6");
        static readonly Guid controlsGuid = Guid.Parse("f4565c31-c35b-4cbf-911a-26a841d3bc04");

        //static readonly Guid assemblyGuid = Guid.Parse("ae00fed1-c58d-4e09-ac24-99cad4fbbc56");

        static readonly Guid transformerGuid = Guid.Parse("2afe3e32-eb9e-4d2c-b862-5fe8ac95559f");

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
                    TreeRepository.Instance.InsertRoot(guid, tableName, typeof(object));
                    observer.OnNext(new Node("root", new Model { Name = "root" }) { Guid = guid });
                    return Disposable.Empty;
                });
        }
        public static IObservable<Node> BuildControlRoot()
        {
            Guid guid = Guid.NewGuid();

            return Observable.Create<Node>(observer =>
            {
                var subControlRoot = new Node("controls", new ControlsModel { Name = "controls" });

                return TreeRepository.Instance
                .InsertRoot(controlsGuid, subControlRoot.Name, typeof(object))
                .Subscribe(a =>
                {
                    subControlRoot.Guid = a.Guid;
                    observer.OnNext(subControlRoot);
                });
            });
        }

        public static IObservable<Node> BuildComboRoot()
        {
            Guid guid = Guid.NewGuid();

            return Observable.Create<Node>(observer =>
            {
                var subControlRoot = new Node("combo", new DatabasesModel { Name = "combo" });
                return TreeRepository.Instance
                .InsertRoot(subGuid, subControlRoot.Name, typeof(object))
                .Subscribe(a =>
                {
                    subControlRoot.Guid = a.Guid;
                    observer.OnNext(subControlRoot);
                });

            });
        }


        public class ControlsModel : Model
        {
            public override IEnumerable<Node> CreateChildren()
            {
                //yield return new Node("load", new CommandModel { Name = Load });
                yield return new Node("save", new CommandModel { Name = Save });
                yield return new Node("save_filters", new CommandModel { Name = Save_Filters });
                //yield return new Node("clear", new CommandModel { Name = Clear });
                //yield return new Node("_new", new CommandModel { Name = New });
                //yield return new Node("expand", new CommandModel { Name = Expand });
                //yield return new Node("collapse", new CommandModel { Name = Collapse });
                //yield return new Node("refresh", new CommandModel { Name = Refresh });
                //yield return new Node("search", new SearchModel { Name = Search });
                //yield return new Node("next", new CommandModel { Name = Next });
                //yield return new Node("next", new ExceptionsModel { Name = "exceptions" });
            }
            public override void SetNode(Node node)
            {
                node.IsExpanded = true;
                node.Orientation = Enums.Orientation.Horizontal;
                base.SetNode(node);
            }
        }

        public static IObservable<Node> BuildFiltersRoot()
        {
            Guid guid = Guid.NewGuid();

            return Observable.Create<Node>(observer =>
            {
                //return NodeSource.Instance.Single(nameof(BuildRoot)).Subscribe(root =>
                //{
                var subRoot = new Node("transformers", new TransformersModel { Name = "transformers" }) { IsExpanded = true, Orientation = Enums.Orientation.Vertical };
                //root.Add(subRoot);

                return TreeRepository.Instance
                .InsertRoot(filterGuid, subRoot.Name, typeof(object))
                .Subscribe(a =>
                {
                    subRoot.Guid = a.Guid;
                    //subRoot.Load();
                    observer.OnNext(subRoot);
                });
                //});
            });
        }

        public static IObservable<Node> BuildHtmlRoot()
        {
            return Observable.Create<Node>(observer =>
            {
                var node = new Node("html", new StringModel { Name = "html" }) { };
                return TreeRepository.Instance
                .InsertRoot(htmlGuid, node.Name, typeof(object))
                .Subscribe(a =>
                {
                    node.Guid = a.Guid;
                    observer.OnNext(node);
                });
            });
        }

        public static IObservable<Node> BuildHtmlRenderRoot()
        {
            return Observable.Create<Node>(observer =>
            {
                var node = new Node("_html", new HtmlModel { Name = "_html" }) { };
                return TreeRepository.Instance
                .InsertRoot(htmlRenderGuid, node.Name, typeof(object))
                .Subscribe(a =>
                {
                    node.Guid = a.Guid;
                    observer.OnNext(node);
                });
            });
        }

        public static IObservable<Node> BuildContentRoot()
        {
            return Observable.Create<Node>(observer =>
            {
                var _node = build();
                return TreeRepository.Instance
       .InsertRoot(contentGuid, _node.Name, typeof(object))
       .Subscribe(a =>
       {
           _node.Guid = a.Guid;
           //subRoot.Load();
           observer.OnNext(_node);
       });
                //});
            });

            static Node build()
            {
                return new Node("Groups", new Model(() =>
                {
                    return [ new Node("Group 1", new Model(() =>
                    {
                        return new List<string> { "test 1", "test 2", "test 3" }.Select(a => new Node(a, new Model { Name = a }));
                    })
                    { Name = "Group 1" })
                    { IsExpanded =true },

                    new Node("Group 2", new Model(() =>
                    {
                        return [
                            new("test 1", new Model { Name = "test 1" }),
                            new("five", new IndexModel { Value = 5 }) {  },
                            new("test 5", new Model(()=> [new ("lower", new Model { Name = "lower" })]) { Name = "test 5" }),
                            new Node("nom", new Model { Name = "nom" })
                       ];
                    }){ Name = "Group 2"}),
                    new Node("Group 3", new Model(() =>
                    {
                        return [
                            (new Node("test 1", new Model { Name = "test 1" }) { }),
                            (new Node("test 7", new Model { Name = "test 7" }) {  }),
                            (new Node("test 8", new Model { Name = "test 8" }) {  }),
                            (new Node("eight", new IndexModel { Value = 8 }) {  }),
                       ];
                    }){ Name = "Group 3"})];
                })
                { Name = "Groups" })
                { IsExpanded = true }
               ;
            }
        }




        public static IObservable<Node> Assembly()
        {
            return Observable.Create<Node>(observer =>
            {
                return TreeRepository.Instance.InsertRoot(assemblyGuid, "too", typeof(object))
                .Subscribe(a =>
                {
                    var assembly = typeof(IndexModel).Assembly;
                    var res = new ResolvableNode(assemblyGuid, assembly) { };
                    observer.OnNext(res);
                });
            });
        }

    }
}