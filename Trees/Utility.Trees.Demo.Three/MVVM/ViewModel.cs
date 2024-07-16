//using Utility.Interfaces.NonGeneric;
//using Utility.PropertyNotifications;

//namespace Utility.Trees.Demo.MVVM
//{

//    public record ViewModel : NotifyProperty
//    {
//        private string dataTemplateKey;
//        private string styleKey;
//        private string itemsPanelTemplateKey;
//        private bool isSelected;

//        public string DataTemplateKey
//        {
//            get
//            {

//                this.RaisePropertyCalled(dataTemplateKey);
//                return dataTemplateKey;
                
//            }
//            set
//            {
//                if (value.Equals(dataTemplateKey))
//                    return;
//                dataTemplateKey = value;
//                this.RaisePropertyReceived(value);
//            }
//        }
        
//        public string ItemsPanelTemplateKey
//        {
//            get
//            {
//                this.RaisePropertyCalled(itemsPanelTemplateKey);
//                return itemsPanelTemplateKey;
//            }
//            set
//            {
//                if (value.Equals(itemsPanelTemplateKey))
//                    return;
//                itemsPanelTemplateKey = value;
//                this.RaisePropertyReceived(value);
//            }
//        }

//        public string StyleKey
//        {
//            get
//            {
//                this.RaisePropertyCalled(styleKey);
//                return styleKey;
//            }
//            set
//            {
//                if (value.Equals(styleKey))
//                    return;
//                styleKey = value;
//                this.RaisePropertyReceived(value);
//            }
//        }



//        public class StyleSelector : System.Windows.Controls.StyleSelector
//        {
//            public static StyleSelector Instance { get; } = new();
//        }
//    }
//}
