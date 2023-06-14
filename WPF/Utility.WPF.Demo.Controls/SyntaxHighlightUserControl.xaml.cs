using System.Linq;
using System.Windows.Controls;

namespace Utility.WPF.Demo.Controls
{
    /// <summary>
    /// Interaction logic for HighlightUserControl.xaml
    /// </summary>
    public partial class HighlightUserControl : UserControl
    {
        public HighlightUserControl()
        {
            InitializeComponent();

            this.JsonSyntaxHighlightTextBox.Json = @"{
'title': 'Person',
    'type': 'object',
    'properties': {
        'firstName': {
            'type': 'string'
        },
        'lastName': {
            'type': 'string'
        },
        'age': {
            'description': 'Age in years',
            'type': 'integer',
            'minimum': 0
        }
    },
    'required': ['firstName', 'lastName']
}".Replace('\'', '"');
            TextBox.Text = @"
 <!DOCTYPE html>
<html>
<body>

<h1>My First Heading</h1>
<p>My first paragraph.</p>

</body>
</html> ";
            this.Languages.ItemsSource = ColorCode.Languages.All.ToArray();
        }
    }
}