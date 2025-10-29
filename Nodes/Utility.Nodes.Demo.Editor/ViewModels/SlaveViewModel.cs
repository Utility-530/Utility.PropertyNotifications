//using System.Reactive.Disposables;
//using Utility.Interfaces.Exs.Diagrams;
//using Utility.Models;
//using Utility.Models.Trees;
//using Utility.Nodes.Ex;
//using Utility.Nodes.Meta;
//using Utility.PropertyNotifications;
//using Utility.Repos;
//using Utility.Trees.Abstractions;

//namespace Utility.Nodes.Demo.Editor
//{
//    public class SlaveViewModel : ViewModel
//    {
//        private readonly DataFileModel dataFileModel;

//        //IReadOnlyTree[] control, selection, content, transformers, html, html_render, clones, dirty;
//        private IReadOnlyTree[] content, clones;

//        private NodeEngine nodeSource;

//        public SlaveViewModel(DataFileModel dataFileModel)
//        {
//            var repo = new TreeRepository(dataFileModel.FilePath);
//            nodeSource = new NodeEngine(repo);
//            this.dataFileModel = dataFileModel;
//        }

//        public IReadOnlyTree[] Content
//        {
//            get
//            {
//                if (content == null)
//                    nodeSource.Create(dataFileModel.Alias, dataFileModel.Guid, s => new CollectionModel() { Name = s }).Subscribe(a =>
//                    {
//                        content = [a];
//                    });
//                return content;
//            }
//        }

//        public IReadOnlyTree[] Clones
//        {
//            get
//            {
//                if (clones == null)
//                    this.WithChangesTo(a => a.Content)
//                        .Subscribe(a =>
//                        {
//                            clones = [(a.Single() as INodeViewModel).Abstract(out var disposables)]; RaisePropertyChanged(nameof(Clones));
//                        }).DisposeWith(disposables); ;

//                return clones;
//            }
//        }

//        //public IReadOnlyTree[] Html
//        //{
//        //    get
//        //    {
//        //        if (html == null)
//        //            Subscribe(nameof(NodeMethodFactory.BuildHtmlRoot), a => html = [a]);
//        //        return html;
//        //    }
//        //}
//        //public IReadOnlyTree[] Dirty
//        //{
//        //    get
//        //    {
//        //        if (dirty == null)
//        //            Subscribe(nameof(NodeMethodFactory.BuildDirty), a => dirty = [a]);
//        //        return dirty;
//        //    }
//        //}
//    }
//}