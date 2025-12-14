namespace Utility.Models.Templates
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Utility.Interfaces.Exs;
    using Utility.Interfaces.NonGeneric;
    using Utility.ServiceLocation;
    using Utility.WPF.Templates;

    public partial class VisualReadOnlyTemplates : ResourceDictionary
    {
        public VisualReadOnlyTemplates()
        {
            InitializeComponent();
        }

        public static VisualReadOnlyTemplates Instance { get; } = new();
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

    public class VisualReadOnlyTemplateSelector : CustomDataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            bool isReadOnly = false;
            if (item is IStyle { Style: { } style })
            {
                if (item is IIsReadOnly { IsReadOnly: { } _isReadOnly })
                    isReadOnly = _isReadOnly;

                switch (style)
                {
                    case Enums.Visual.Label:
                        return VisualReadOnlyTemplates.Instance.Label;
                    case Enums.Visual.Title:
                        return VisualReadOnlyTemplates.Instance.Title;
                    case Enums.Visual.Subtitle:
                        return VisualReadOnlyTemplates.Instance.Subtitle;
                    case Enums.Visual.SecondaryHeader:
                        return VisualReadOnlyTemplates.Instance.SecondaryHeader;
                    case Enums.Visual.TextInput:
                        return VisualReadOnlyTemplates.Instance.Text;
                    case Enums.Visual.Text:
                        return VisualReadOnlyTemplates.Instance.Text;
                    case Enums.Visual.Image:
                        return VisualReadOnlyTemplates.Instance.Image;
                    case Enums.Visual.TableHeader:
                        return VisualReadOnlyTemplates.Instance.TableHeader;
                    case Enums.Visual.TableCell:
                        return VisualReadOnlyTemplates.Instance.TableCell;
                    case Enums.Visual.CheckBox:
                        return VisualReadOnlyTemplates.Instance.CheckBox;
                }
            }
            //throw new Exception("DFS £Fjgjg kvov33");
            return base.SelectTemplate(item, container);
        }
    }
}
