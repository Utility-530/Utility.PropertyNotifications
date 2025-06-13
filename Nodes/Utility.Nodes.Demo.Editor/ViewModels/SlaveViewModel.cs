using System.Reactive.Disposables;
using Utility.Nodes.Ex;
using Utility.Nodes.Filters;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Editor
{
    public class SlaveViewModel : ViewModel
    {
        IReadOnlyTree[] control, selection, content, transformers, html, html_render, clones, dirty;

        public IReadOnlyTree[] Content
        {
            get
            {
                if (content == null)
                    source.Value.Single(nameof(NodeMethodFactory.BuildContentRoot))
                        .Subscribe(a => { content = [a]; RaisePropertyChanged(nameof(Content)); }).DisposeWith(disposables); ;
                return content;
            }
        }

        public IReadOnlyTree[] Clones
        {
            get
            {
                if (clones == null)
                    source.Value.Single(nameof(NodeMethodFactory.BuildContentRoot))
                        .Subscribe(a =>
                        {
                            clones = [a.Abstract(out var disposables)]; RaisePropertyChanged(nameof(Clones));
                        }).DisposeWith(disposables); ;

                return clones;
            }
        }
        public IReadOnlyTree[] Html
        {
            get
            {
                if (html == null)
                    source.Value.Single(nameof(NodeMethodFactory.BuildHtmlRoot))
                        .Subscribe(a => { html = [a]; RaisePropertyChanged(nameof(Html)); }).DisposeWith(disposables); ;
                return html;
            }
        }
        public IReadOnlyTree[] Dirty
        {
            get
            {
                if (dirty == null)
                    source.Value.Single(nameof(NodeMethodFactory.BuildDirty))
                        .Subscribe(a => { dirty = [a]; RaisePropertyChanged(nameof(Dirty)); }).DisposeWith(disposables); ;
                return dirty;
            }

            //public IReadOnlyTree[] Transformers
            //{
            //    get
            //    {
            //        if (transformers == null)
            //            source.Value.Single(nameof(Factory.BuildTransformersRoot))
            //                .Subscribe(a => { transformers = [a]; RaisePropertyChanged(nameof(Transformers)); });
            //        return transformers;
            //    }
            //}



            //public IReadOnlyTree[] Html_Render
            //{
            //    get
            //    {
            //        if (html_render == null)
            //            source.Value.Single(nameof(Factory.BuildHtmlRenderRoot))
            //                .Subscribe(a => { html_render = [a]; RaisePropertyChanged(nameof(Html_Render)); });
            //        return html_render;
            //    }
            //}
        }
    }
}


