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

        public static IObservable<Node> BuildContentRoot()
        {
            return Observable.Create<Node>(observer =>
            {
                //return NodeSource.Instance
                //.Single(nameof(BuildRoot))
                //.Subscribe(rootNode =>
                //{
                var _node = build();
                //rootNode.Add(_node);
                //observer.OnNext(_node);

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


        //public static IObservable<Node> BuildAndOr()
        //{

        //    Node andOr = new Node("andor", new AndOrModel { Value = Enums.AndOr.Or }) { Guid = subGuid };

        //    return Observable.Create<Node>(observer =>
        //    {

        //        return TreeRepository.Instance.InsertRoot(subGuid, andOr.Name, typeof(object))
        //            .Subscribe(a =>
        //            {
        //                //andOrNodes.Add(andOr);

        //                resolvableModels(andOr).Subscribe(node =>
        //                {
        //                    andOr.Add(node);
        //                });

        //                observer.OnNext(andOr);
        //            });

        //    });

        //    static IObservable<Node> resolvableModels(Node parent)
        //    {
        //        return Observable.Create<Node>(observer =>
        //        {


        //            var assemblyBranch = new Node("ass1", new AssemblyModel { Name = "Assembly", Assembly = typeof(Model).Assembly }) { Parent = parent };
        //            var assembly2Branch = new Node("ass2", new AssemblyModel { Name = "Assembly", Assembly = typeof(IndexModel).Assembly }) { Parent = parent };

        //            var modelBranch = new Node(nameof(Model), new TypeModel { Name = nameof(Model), Type = typeof(Model) }) { Parent = assemblyBranch };
        //            var model2Branch = new Node(nameof(IndexModel), new TypeModel { Name = nameof(IndexModel), Type = typeof(IndexModel) }) { Parent = assembly2Branch };
        //            //var model3Branch = new Node(nameof(Model), new TypeModel { Name = nameof(Model), Type = typeof(Model) }) { Parent = assembly2Branch };

        //            assemblyBranch.Add(modelBranch);
        //            assembly2Branch.Add(model2Branch);
        //            //assembly2Branch.Items.Add(model3Branch);

        //            var branch6 = new Node(nameof(Model.Name), new PropertyModel { Name = nameof(Model.Name), PropertyInfo = typeof(Model).GetProperty(nameof(Model.Name)) }) { Parent = modelBranch };
        //            var branch62 = new Node(nameof(Model.Name) + "2", new PropertyModel { Name = nameof(Model.Name), PropertyInfo = typeof(IndexModel).GetProperty(nameof(Model.Name)) }) { Parent = model2Branch };
        //            modelBranch.Add(branch6);
        //            model2Branch.Add(branch62);

        //            var branch7 = new Node("Value", new ValueModel { Name = "Value", Value = "test 8" }) { Parent = branch6 };
        //            //var branch72 = new Node { Data = new ValueModel { Name = "Value", Value = "Group 2" }, Parent = branch62 };


        //            branch6.Add(branch7);
        //            //branch62.Items.Add(branch72);

        //            var one = new Node("One", new ResolvableModel { Name = "One" }) { Root = assemblyBranch, Current = branch7 };
        //            var two = new Node("One", new ResolvableModel { Name = "Two" }) { Root = assembly2Branch, Current = branch62 };

        //            observer.OnNext(one);
        //            observer.OnNext(two);
        //            return Disposable.Empty;
        //        });
        //    }
        //}

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

        //public static IObservable<Node> Relation()
        //{
        //    return Observable.Create<Node>(observer =>
        //    {
        //        return TreeRepository.Instance.InsertRoot(rootModelGuid, "Root Relation", typeof(object))
        //        .Subscribe(a =>
        //        {

        //            var rootModelBranch = new Node("Root Relation", new Model { Name = "Root Relation" }) { Guid = rootModelGuid };

        //            var relationBranch = new Node(nameof(Enums.Relation.Self), new RelationModel { Relation = Enums.Relation.Self }) { Parent = rootModelBranch };

        //            var indexBranch = new Node("one", new IndexModel { Value = 1 }) { Parent = relationBranch };
        //            var model = new Node("sd", new RelationshipModel()) { Root = rootModelBranch, Current = indexBranch };

        //            rootModelBranch.Add(relationBranch);

        //            relationBranch.Add(indexBranch);

        //            observer.OnNext(model);
        //        });
        //    });
        //}

        //public static IObservable<Node> SetModel()
        //{

        //    return Observable.Create<Node>(observer =>
        //    {
        //        var assemblyBranch = new Node("Assembly", new AssemblyModel { Name = "Assembly", Assembly = typeof(Node).Assembly }) { Guid = assGuid };

        //        //var two = new Node("two", twoModel) { Guid = assGuid };


        //        return TreeRepository.Instance.InsertRoot(assemblyBranch.Guid, assemblyBranch.Name, typeof(object))
        //            .Subscribe(a =>
        //            {
        //                var modelBranch = new Node(nameof(Node), new TypeModel { Name = nameof(Node), Type = typeof(Node) }) { Parent = assemblyBranch };
        //                var branch6 = new Node(nameof(Node.IsHighlighted), new PropertyModel { Name = nameof(Node.IsHighlighted), PropertyInfo = typeof(Node).GetProperty(nameof(Node.IsHighlighted)) }) { Parent = modelBranch };
        //                var branch7 = new Node("IsHighlighted", new ValueModel { Name = "IsHighlighted", Value = true }) { Parent = branch6 };

        //                var twoModel = new Node("a", new SetModel { Name = "Two" })
        //                {
        //                    Current = branch7
        //                };
        //                twoModel.Add(assemblyBranch);

        //                assemblyBranch.Add(modelBranch);

        //                var branch62 = new Node(nameof(Node.Data), new PropertyModel { Name = nameof(Node.Data), PropertyInfo = typeof(Node).GetProperty(nameof(Node.Data)) }) { Parent = modelBranch };

        //                modelBranch.Add(branch6);

        //                branch62.Parent = modelBranch;
        //                modelBranch.Add(branch62);


        //                var branch72 = new Node("Value", new ValueModel { Name = "Value", Value = 3 }) { Parent = branch62 };

        //                branch6.Add(branch7);

        //                branch72.Parent = branch62;
        //                branch62.Add(branch72);

        //                observer.OnNext(twoModel);
        //            });
        //    });
        //}

        //public static IObservable<Node> BuildControls()
        //{
        //    return Observable.Create<Node>(observer =>
        //    {
        //        NodeSource.Instance.Single(nameof(BuildSave)).Subscribe(observer.OnNext);
        //        NodeSource.Instance.Single(nameof(BuildSaveFilters)).Subscribe(observer.OnNext);
        //        //NodeSource.Instance.Single(nameof(BuildLoad)).Subscribe(observer.OnNext);
        //        //NodeSource.Instance.Single(nameof(BuildClear)).Subscribe(observer.OnNext);
        //        //NodeSource.Instance.Single(nameof(BuildNew)).Subscribe(observer.OnNext);
        //        ////NodeSource.Instance.Single(nameof(BuildExpand)).Subscribe(observer.OnNext);
        //        ////NodeSource.Instance.Single(nameof(BuildCollapse)).Subscribe(observer.OnNext);
        //        //NodeSource.Instance.Single(nameof(BuildRefresh)).Subscribe(observer.OnNext);
        //        //NodeSource.Instance.Single(nameof(BuildSearch)).Subscribe(observer.OnNext);
        //        NodeSource.Instance.Single(nameof(BuildExceptions)).Subscribe(observer.OnNext);
        //        return Disposable.Empty;

        //    });
        //}

        //public static Node BuildLoad()
        //{
        //    return new Node("load", new CommandModel { Name = Load });
        //}

        //public static Node BuildSave()
        //{
        //    return new Node("save", new CommandModel { Name = Save });
        //}

        //public static Node BuildSaveFilters()
        //{
        //    return new Node("save_filters", new CommandModel { Name = Save_Filters });
        //}
        //public static Node BuildClear()
        //{
        //    return new Node("clear", new CommandModel { Name = Clear });
        //}

        //public static Node BuildNew()
        //{
        //    return new Node("_new", new CommandModel { Name = New });
        //}

        //public static Node BuildExpand()
        //{
        //    return new Node("expand", new CommandModel { Name = Expand });
        //}

        //public static Node BuildCollapse()
        //{
        //    return new Node("collapse", new CommandModel { Name = Collapse });
        //}

        //public static Node BuildRefresh()
        //{
        //    return new Node("refresh", new CommandModel { Name = Refresh });
        //}
        //public static Node BuildSearch()
        //{
        //    return new Node("search", new SearchModel { Name = Search });
        //}
        //public static Node BuildNext()
        //{
        //    return new Node("next", new CommandModel { Name = Next });
        //}

        //public static Node BuildExceptions()
        //{
        //    return new Node("next", new ExceptionsModel { Name = "exceptions" });
        //}


    }
}
//    public static IObservable<Node> BuildFilterTransformer()
//    {
//        Node filter_and_transformers = null;
//        return Observable.Create<Node>(observer =>
//        {

//            return NodeSource.Instance.Single(nameof(BuildFilterTransformersRoot)).Subscribe(node =>
//            {
//                filter_and_transformers = new Node("filter_and_transformers", new Model { Name = "filter_and_transformers" }) { Parent = node };
//                node.Add(filter_and_transformers);
//                BuildTrueFalse()
//                    .Subscribe
//                    (a =>
//                    {
//                        var transformers = new Node("transformers", new Model { Name = "transformers" }) { Parent = filter_and_transformers };
//                        filter_and_transformers.Add(transformers);

//                        BuildTransformer(transformers).Subscribe(x =>
//                        {
//                            transformers.Add(x);
//                            observer.OnNext(node);
//                        });

//                    });
//            });
//        });
//        IObservable<Node> BuildTrueFalse()
//        {
//            return Observable.Create<Node>(observer =>
//            {
//                Node trueFalse = new()
//                {
//                    Data = "TrueFalse",
//                    Parent = filter_and_transformers
//                };
//                filter_and_transformers.Add(trueFalse);

//                return NodeSource.Instance.Single(nameof(BuildAndOr)).Subscribe(a =>
//                {
//                    a.Parent = trueFalse;
//                    trueFalse.Add(a);
//                    observer.OnNext(trueFalse);
//                });

//            });
//        }

//        IObservable<Node> BuildTransformer(Node transformers)
//        {
//            return Observable.Create<Node>(observer =>
//            {
//                CompositeDisposable disposables = new();
//                Node transformer = new("transformer", new Model { Name = "transformer" })
//                {
//                    Parent = transformers
//                };

//                transformer.Add(new Node("True", new BooleanModel { Name = "True", Value = true }) { Parent = transformer });

//                SetModel().Subscribe(a =>
//                {
//                    a.Parent = transformer;
//                    transformer.Add(a);
//                }).DisposeWith(disposables);

//                NodeSource.Instance.Single(nameof(Relation)).Subscribe(a =>
//                {
//                    a.Parent = transformer;
//                    transformer.Add(a);
//                }).DisposeWith(disposables);


//                observer.OnNext(transformer);
//                return disposables;
//            });
//        }
//    }
//}

//public static class DisposerHelper
//{
//    public static IDisposable DisposeWith(this IDisposable disposable, ICollection<IDisposable> disposer)
//    {
//        disposer.Add(disposable);
//        return disposable;
//    }
//}
//}