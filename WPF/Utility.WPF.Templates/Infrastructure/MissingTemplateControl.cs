using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Templates.Infrastructure
{
    public class MissingTemplateControl : ContentControl
    {
        static MissingTemplateControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MissingTemplateControl), new FrameworkPropertyMetadata(typeof(MissingTemplateControl)));
        }
    }
}