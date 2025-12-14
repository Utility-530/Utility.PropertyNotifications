namespace Utility.Models.Templates
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Utility.Interfaces.Exs;
    using Utility.ServiceLocation;
    using Utility.WPF.Templates;

    public partial class VisualWriteTemplates : ResourceDictionary
    {
        public VisualWriteTemplates()
        {
            InitializeComponent();
        }

        public static VisualWriteTemplates Instance { get; } = new();
    //    public static VisualTemplates Instance =>
    //(VisualTemplates)Application.Current.Resources.MergedDictionaries
    //    .First(d => d is VisualTemplates);

        public DataTemplate Label => this["LabelTemplate"] as DataTemplate;
        public DataTemplate Title => this["TitleTemplate"] as DataTemplate;
        public DataTemplate Subtitle => this["SubtitleTemplate"] as DataTemplate;
        public DataTemplate SecondaryHeader => this["SecondaryHeaderTemplate"] as DataTemplate;
        public DataTemplate Text => this["TextTemplate"] as DataTemplate;
        public DataTemplate TableHeader => this["TableHeaderTemplate"] as DataTemplate;
        public DataTemplate TableCell => this["TableCellTemplate"] as DataTemplate;
        public DataTemplate CheckBox => this["CheckBoxTemplate"] as DataTemplate;
        public DataTemplate Image => this["ImageTemplate"] as DataTemplate;
    }

    public class VisualWriteTemplateSelector : CustomDataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IStyle { Style: { } style })
            {
                switch (style)
                {
                    case Enums.Visual.Label:
                        return VisualWriteTemplates.Instance.Label;
                    case Enums.Visual.Title:
                        return VisualWriteTemplates.Instance.Title;
                    case Enums.Visual.Subtitle:
                        return VisualWriteTemplates.Instance.Subtitle;
                    case Enums.Visual.SecondaryHeader:
                        return VisualWriteTemplates.Instance.SecondaryHeader;
                    case Enums.Visual.TextInput:
                        return VisualWriteTemplates.Instance.Text;
                    case Enums.Visual.Text:
                        return VisualWriteTemplates.Instance.Text;
                    case Enums.Visual.Image:
                        return VisualWriteTemplates.Instance.Image;
                    case Enums.Visual.TableHeader:
                        return VisualWriteTemplates.Instance.TableHeader;
                    case Enums.Visual.TableCell:
                        return VisualWriteTemplates.Instance.TableCell;
                    case Enums.Visual.CheckBox:
                        return VisualWriteTemplates.Instance.CheckBox;
                }
            }
            //throw new Exception("DFS £Fjgjg kvov33");
            return base.SelectTemplate(item, container);
        }

        public static VisualWriteTemplateSelector VisualInstance { get; } = new();
    }
}
