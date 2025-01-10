using ColorCode.Compilation.Languages;
using Utility.Nodes.Filters;
using Utility.Trees.Abstractions;
using Utility.ViewModels;

namespace Utility.Nodes.Demo.Filters
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        IReadOnlyTree[] control, selection, content, filters, html,html_render;

        public IReadOnlyTree[] Controls
        {
            get
            {
                if (control == null)
                    NodeSource.Instance.Single(nameof(Factory.BuildControlRoot))
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
                    NodeSource.Instance.Single(nameof(Factory.BuildComboRoot))
                        .Subscribe(a => { selection = [a]; RaisePropertyChanged(nameof(Selection)); });
                return selection;
            }
        }

        public IReadOnlyTree[] Content
        {
            get
            {
                if (content == null)
                    NodeSource.Instance.Single(nameof(Factory.BuildContentRoot))
                        .Subscribe(a => { content = [a]; RaisePropertyChanged(nameof(Content)); });
                return content;
            }
        }

        public IReadOnlyTree[] Filters
        {
            get
            {
                if (filters == null)
                    NodeSource.Instance.Single(nameof(Factory.BuildFiltersRoot))
                        .Subscribe(a => { filters = [a]; RaisePropertyChanged(nameof(Filters)); });
                return filters;
            }
        }   
        
        public IReadOnlyTree[] Html
        {
            get
            {
                if (html == null)
                    NodeSource.Instance.Single(nameof(Factory.BuildHtmlRoot))
                        .Subscribe(a => { html = [a]; RaisePropertyChanged(nameof(Html)); });
                return html;
            }
        }


        public IReadOnlyTree[] Html_Render
        {
            get
            {
                if (html_render == null)
                    NodeSource.Instance.Single(nameof(Factory.BuildHtmlRenderRoot))
                        .Subscribe(a => { html_render = [a]; RaisePropertyChanged(nameof(Html_Render)); });
                return html_render;
            }
        }
    }
}


