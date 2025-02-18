using Splat;
using System.Collections;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Nodes.Filters;
using Utility.Trees.Abstractions;
using Utility.ViewModels;

namespace Utility.Nodes.Demo.Filters
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        IReadOnlyTree[] control, selection, content, transformers, html, html_render, clones;
        private bool isRemovedShown;
        Lazy<INodeSource> source = new(() => Locator.Current.GetService<INodeSource>());

        public IReadOnlyTree[] Controls
        {
            get
            {
                if (control == null)
                    source.Value.Single(nameof(Factory.BuildControlRoot))
                        .Subscribe(a =>
                        {
                            control = [a]; base.RaisePropertyChanged(nameof(Controls));
                        });
                return control;
            }
        }

        public IReadOnlyTree[] Selection
        {
            get
            {
                if (selection == null)
                    source.Value.Single(nameof(Factory.BuildComboRoot))
                        .Subscribe(a => { selection = [a]; RaisePropertyChanged(nameof(Selection)); });
                return selection;
            }
        }

        public IReadOnlyTree[] Content
        {
            get
            {
                if (content == null)
                    source.Value.Single(nameof(Factory.BuildContentRoot))
                        .Subscribe(a => { content = [a]; RaisePropertyChanged(nameof(Content)); });
                return content;
            }
        }

        public IReadOnlyTree[] Clones
        {
            get
            {
                if (clones == null)
                    source.Value.Single(nameof(Factory.BuildContentRoot))
                        .Subscribe(a =>
                        {
                            clones = [a.Abstract()];
                            RaisePropertyChanged(nameof(Clones));
                        });

                return clones;
            }
        }

        public IReadOnlyTree[] Transformers
        {
            get
            {
                if (transformers == null)
                    source.Value.Single(nameof(Factory.BuildTransformersRoot))
                        .Subscribe(a => { transformers = [a]; RaisePropertyChanged(nameof(Transformers)); });
                return transformers;
            }
        }

        public IReadOnlyTree[] Html
        {
            get
            {
                if (html == null)
                    source.Value.Single(nameof(Factory.BuildHtmlRoot))
                        .Subscribe(a => { html = [a]; RaisePropertyChanged(nameof(Html)); });
                return html;
            }
        }


        public IReadOnlyTree[] Html_Render
        {
            get
            {
                if (html_render == null)
                    source.Value.Single(nameof(Factory.BuildHtmlRenderRoot))
                        .Subscribe(a => { html_render = [a]; RaisePropertyChanged(nameof(Html_Render)); });
                return html_render;
            }
        }

        public IEnumerable Dirty => NodeSource.Instance.DirtyNodes;


        public bool IsRemovedShown
        {
            get => isRemovedShown; set
            {
                isRemovedShown = value;
                RaisePropertyChanged();
            }
        }
    }
}


