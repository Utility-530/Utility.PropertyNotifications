using System.Windows;

namespace Utility.WPF.Controls
{
    public class PathDataTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public DataTemplate DirectoryDataTemplate { get; set; }
        public DataTemplate FileDataTemplate { get; set; }
        public DataTemplate DefaultDataTemplate { get; set; }
        public DataTemplate ContentPresenterTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return null; ;

            var type = item.GetType();

            DataTemplate myDataTemplate = ((System.Windows.Controls.ContentPresenter)container).ContentTemplate;

            try
            {
                var dataTemplateKey = new DataTemplateKey(type);
                var dataTemplate = (container as FrameworkElement).FindResource(dataTemplateKey);
                if (dataTemplate != null)
                    return ContentPresenterTemplate;
            }
            catch
            {
            }

            //if (item is FileViewModel)
            //    return FileDataTemplate;
            //else if (item is DirectoryViewModel)
            //    return DirectoryDataTemplate;
            //else
            return DefaultDataTemplate;
        }
    }

    //public class ObjectDataTemplateSelector : System.Windows.Controls.DataTemplateSelector
    //{
    //    public DataTemplate DefaultDataTemplate { get; set; }
    //    public DataTemplate EnumerableDataTemplate { get; set; }
    //    public DataTemplate ContentPresenterTemplate { get; set; }
    //    public DataTemplate IConvertibleTemplate { get; set; }

    //    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    //    {
    //        if(item==null)
    //            return DefaultDataTemplate;

    //        var type = item.GetType();

    //        if (type.Equals(typeof(ViewModel.SHDObject<object>)))
    //            type = ((ViewModel.SHDObject<object>)item).Object.GetType();
    //        else
    //            throw new Exception("type must be " + nameof(ViewModel.SHDObject<object>));

    //        DataTemplate myDataTemplate = ((System.Windows.Controls.ContentPresenter)container).ContentTemplate;

    //        try
    //        {
    //            var dataTemplateKey = new DataTemplateKey(type);
    //            var dataTemplate = (container as FrameworkElement).FindResource(dataTemplateKey);
    //            if (dataTemplate != null)
    //                return ContentPresenterTemplate;
    //        }
    //        catch
    //        {
    //        }

    //        var interfaces = type.GetInterfaces();
    //        if ((interfaces.Contains(typeof(IConvertible))))
    //            return IConvertibleTemplate;
    //        else if (interfaces.Contains(typeof(IEnumerable)))
    //            return EnumerableDataTemplate;
    //        else
    //            return DefaultDataTemplate;

    //    }
    //}
}