namespace Utility.Models.Templates
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Utility.Interfaces.Exs;
    using Utility.ServiceLocation;
    using Utility.WPF.Templates;

    public partial class VisualTemplates : ResourceDictionary
    {
        public VisualTemplates()
        {
            InitializeComponent();
        }

        public static VisualTemplates Instance { get; } = new();
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
    }

    public class VisualTemplateSelector : CustomDataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IStyle { Style: { } style })
            {
                switch (style)
                {
                    case Enums.Visual.Label:
                        return VisualTemplates.Instance.Label;
                    case Enums.Visual.Title:
                        return VisualTemplates.Instance.Title;
                    case Enums.Visual.Subtitle:
                        return VisualTemplates.Instance.Subtitle;
                    case Enums.Visual.SecondaryHeader:
                        return VisualTemplates.Instance.SecondaryHeader;
                    case Enums.Visual.TextInput:
                        return VisualTemplates.Instance.Text;
                    case Enums.Visual.TableHeader:
                        return VisualTemplates.Instance.TableHeader;
                    case Enums.Visual.TableCell:
                        return VisualTemplates.Instance.TableCell;
                    case Enums.Visual.CheckBox:
                        return VisualTemplates.Instance.CheckBox;
                }
            }
            //throw new Exception("DFS £Fjgjg kvov33");
            return base.SelectTemplate(item, container);
        }


        public static VisualTemplateSelector VisualInstance { get; } = new();
    }
}
