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
                    Subscribe(nameof(NodeMethodFactory.BuildContentRoot), a => content = [a]);
                return content;
            }
        }

        public IReadOnlyTree[] Clones
        {
            get
            {
                if (clones == null)
                    source.Value[nameof(NodeMethodFactory.BuildContentRoot)]
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
                    Subscribe(nameof(NodeMethodFactory.BuildHtmlRoot), a => html = [a]);
                return html;
            }
        }
        public IReadOnlyTree[] Dirty
        {
            get
            {
                if (dirty == null)
                    Subscribe(nameof(NodeMethodFactory.BuildDirty), a => dirty = [a]);
                return dirty;
            }
        }
    }
}


