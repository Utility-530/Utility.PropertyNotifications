using Utility.Enums.Attributes;
using Utility.Enums.VisualElements;

namespace Utility.Enums
{
    public enum VisualLayout
    { 
        [VisualElement(
            "Structured grid of rows and columns for data display.",
            UIElementCategory.DataPresentation,
            UISubCategory.Collection,
            HTML.table,
            ReactNative.SectionList,
            XAML.DataGrid,
            Flutter.GridView,
            Qt.QTableView,
            Swing.JTable,
            JavaFX.TableView,
            GTK.GtkTreeView,
            Markup.Tabular)]
        Table = 26,


            [VisualElement(
            "Generic container panel",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.div,
            ReactNative.View,
            XAML.ItemsControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Panel = 1,

        [VisualElement(
            "A panel with a header providing context for its content.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.section,
            ReactNative.View,
            XAML.HeaderedContentControl,
            Flutter.Container,
            Qt.QFrame,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        HeaderedPanel = 2,

        [VisualElement(
            "A general-purpose container used for layout grouping.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.div,
            ReactNative.View,
            XAML.ContentControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Content = 7,

        [VisualElement(
            "A horizontal layout grouping elements across a single row.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.tr,
            ReactNative.View,
            XAML.UniformGrid,
            Flutter.Row,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        TableRow = 9,

        [VisualElement(
            "A vertical layout grouping elements down a column.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.div,
            ReactNative.View,
            XAML.ItemsControl,
            Flutter.Column,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Column = 10,
    }

    public enum Visual
    {
        [VisualElement(
            "Side panel usually used for navigation or secondary content.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.aside,
            ReactNative.View,
            XAML.ContentControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Sidebar = 11,

        [VisualElement(
            "The main horizontal navigation bar of an interface.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.nav,
            ReactNative.View,
            XAML.ToolBar,
            Flutter.Container,
            Qt.QFrame,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Navbar = 12,

        [VisualElement(
            "A bar containing commands or tools.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.div,
            ReactNative.View,
            XAML.ToolBar,
            Flutter.Container,
            Qt.QFrame,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Toolbar = 13,

        [VisualElement(
            "Navigation component allowing tab-based page switching.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.div,
            ReactNative.View,
            XAML.TabControl,
            Flutter.TabBarView,
            Qt.QTabWidget,
            Swing.JTabbedPane,
            JavaFX.TabPane,
            GTK.GtkNotebook,
            Markup.Section)]
        TabBar = 14,

        [VisualElement(
            "The container holding the content for each tab.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.div,
            ReactNative.View,
            XAML.ContentPresenter,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        TabPane = 15,

        [VisualElement(
            "Navigation indicator showing a hierarchical path.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.nav,
            ReactNative.View,
            XAML.WrapPanel,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Breadcrumbs = 16,

        [VisualElement(
            "A divider that allows resizing of adjacent areas.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.div,
            ReactNative.View,
            XAML.GridSplitter,
            Flutter.Container,
            Qt.QFrame,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Splitter = 17,

        [VisualElement(
            "Empty spacing element used for layout adjustments.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.div,
            ReactNative.View,
            XAML.Border,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Spacer = 18,

        [VisualElement(
            "A straight horizontal or vertical divider line.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.hr,
            ReactNative.View,
            XAML.Border,
            Flutter.Container,
            Qt.QFrame,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Divider = 19,

        [VisualElement(
            "Overlay area covering the UI for focus or modal support.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.div,
            ReactNative.Modal,
            XAML.ContentControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Overlay = 20,

        [VisualElement(
            "Dedicated drawing surface for rendering graphics.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.canvas,
            ReactNative.View,
            XAML.Canvas,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Figure)]
        DrawingSurface = 22,


        [VisualElement(
            "Defines column labels in a table.",
            UIElementCategory.DataPresentation,
            UISubCategory.Collection,
            HTML.th,
            ReactNative.Text,
            XAML.TextBlock,
            Flutter.Text,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.Text,
            GTK.GtkLabel,
            Markup.Tabular)]
        TableHeader = 27,

   
        // -------------------------------------------------------
        // TABLE CELL (merged from both enums)
        // -------------------------------------------------------
        [VisualElement(
            "A single data cell within a table row.",
            UIElementCategory.DataPresentation,
            UISubCategory.Collection,
            HTML.td,
            ReactNative.Text,
            XAML.TextBlock,
            Flutter.Text,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.Text,
            GTK.GtkLabel,
            Markup.Tabular)]
        TableCell = 29,

        [VisualElement(
            "Final row of a table, often containing totals or summaries.",
            UIElementCategory.DataPresentation,
            UISubCategory.Collection,
            HTML.tr,
            ReactNative.View,
            XAML.ItemsControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Tabular)]
        TableFooter = 30,


        [VisualElement(
            "A hierarchical tree of expandable/collapsible nodes.",
            UIElementCategory.DataPresentation,
            UISubCategory.Collection,
            HTML.ul, // hierarchical list
            ReactNative.SectionList,
            XAML.TreeView,
            Flutter.ListView,
            Qt.QTreeView,
            Swing.JList, // closest available
            JavaFX.TreeView,
            GTK.GtkTreeView,
            Markup.Itemize)]
        TreeView = 32,

        [VisualElement(
            "A list component displaying data items vertically.",
            UIElementCategory.DataPresentation,
            UISubCategory.Collection,
            HTML.ul,
            ReactNative.FlatList,
            XAML.ListView,
            Flutter.ListView,
            Qt.QListView,
            Swing.JList,
            JavaFX.ListView,
            GTK.GtkListBox,
            Markup.Itemize)]
        ListView = 33,

        [VisualElement(
            "A chronological display of events or activities.",
            UIElementCategory.DataPresentation,
            UISubCategory.Collection,
            HTML.ul,
            ReactNative.FlatList,
            XAML.ItemsControl,
            Flutter.ListView,
            Qt.QListView,
            Swing.JList,
            JavaFX.ListView,
            GTK.GtkListBox,
            Markup.Itemize)]
        Timeline = 36,

    
        [VisualElement(
            "Data visualization component for charts and graphs.",
            UIElementCategory.DataPresentation,
            UISubCategory.Collection,
            HTML.canvas,
            ReactNative.View,
            XAML.Canvas,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Figure)]
        Chart = 37,

        [VisualElement(
            "Mathematical or node-based graph display.",
            UIElementCategory.DataPresentation,
            UISubCategory.Collection,
            HTML.canvas,
            ReactNative.View,
            XAML.Canvas,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Figure)]
        Graph = 38,

        [VisualElement(
            "Field allowing freeform text entry.",
            UIElementCategory.Input,
            UISubCategory.Text,
            HTML.inputtext,
            ReactNative.TextInput,
            XAML.TextBox,
            Flutter.TextField,
            Qt.QLineEdit,
            Swing.JTextField,
            JavaFX.TextField,
            GTK.GtkEntry,
            Markup.Paragraph)]
        TextInput = 51,

        [VisualElement(
            "Password-masked text field.",
            UIElementCategory.Input,
            UISubCategory.Text,
            HTML.inputpassword,
            ReactNative.TextInput,
            XAML.PasswordBox,
            Flutter.TextField,
            Qt.QLineEdit,
            Swing.JPasswordField,
            JavaFX.PasswordField,
            GTK.GtkEntry,
            Markup.Paragraph)]
        PasswordInput = 52,

        [VisualElement(
            "Numeric input field.",
            UIElementCategory.Input,
            UISubCategory.Numeric,
            HTML.inputtext,
            ReactNative.TextInput,
            XAML.TextBox,
            Flutter.TextField,
            Qt.QLineEdit,
            Swing.JTextField,
            JavaFX.TextField,
            GTK.GtkEntry,
            Markup.Paragraph)]
        NumberInput = 53,


        //[VisualElement(
        //    "Field for entering multiline text.",
        //    UIElementCategory.Input,
        //    UISubCategory.Text,
        //    HTML.textarea,
        //    ReactNative.TextInput,
        //    XAML.TextBox,
        //    Flutter.TextField,
        //    Qt.QTextEdit,
        //    Swing.JTextArea,
        //    JavaFX.TextArea,
        //    GTK.GtkTextView,
        //    Markup.Paragraph)]
        //TextArea = 54,

        
        [VisualElement(
            "Checkbox allowing multiple selections.",
            UIElementCategory.Input,
            UISubCategory.Selection,
            HTML.checkbox,
            ReactNative.Switch,
            XAML.CheckBox,
            Flutter.Checkbox,
            Qt.QCheckBox,
            Swing.JCheckBox,
            JavaFX.CheckBox,
            GTK.GtkCheckButton,
            Markup.Itemize)]
        CheckBox = 55,

    
        [VisualElement(
            "Radio button allowing a single selection in a group.",
            UIElementCategory.Input,
            UISubCategory.Selection,
            HTML.radio,
            ReactNative.RadioButton,
            XAML.RadioButton,
            Flutter.Radio,
            Qt.QRadioButton,
            Swing.JRadioButton,
            JavaFX.RadioButton,
            GTK.GtkRadioButton,
            Markup.Itemize)]
        RadioButton = 56,
      
        [VisualElement(
            "A dropdown allowing selection from a list of options.",
            UIElementCategory.Input,
            UISubCategory.Selection,
            HTML.select,
            ReactNative.Picker,
            XAML.ComboBox,
            Flutter.DropdownButton,
            Qt.QComboBox,
            Swing.JComboBox,
            JavaFX.ComboBox,
            GTK.GtkComboBox,
            Markup.Enumerate)]
        Dropdown = 57,

        [VisualElement(
            "Slider control selecting a numeric range value.",
            UIElementCategory.Input,
            UISubCategory.Numeric,
            HTML.inputtext,
            ReactNative.Slider,
            XAML.Slider,
            Flutter.Slider,
            Qt.QSlider,
            Swing.JSlider,
            JavaFX.Slider,
            GTK.GtkSlider,
            Markup.Paragraph)]
        Slider = 58,

        [VisualElement(
            "Toggle switch input representing on/off.",
            UIElementCategory.Input,
            UISubCategory.Selection,
            HTML.checkbox,
            ReactNative.Switch,
            XAML.CheckBox,
            Flutter.Checkbox,
            Qt.QCheckBox,
            Swing.JCheckBox,
            JavaFX.CheckBox,
            GTK.GtkCheckButton,
            Markup.Itemize)]
        ToggleSwitch = 59,
       
        [VisualElement(
            "Calendar-based date selector.",
            UIElementCategory.Input,
            UISubCategory.Numeric,
            HTML.inputtext,
            ReactNative.TextInput,
            XAML.TextBox,
            Flutter.TextField,
            Qt.QLineEdit,
            Swing.JTextField,
            JavaFX.TextField,
            GTK.GtkEntry,
            Markup.Paragraph)]
        DatePicker = 60,

       
        [VisualElement(
            "Time selection control.",
            UIElementCategory.Input,
            UISubCategory.Numeric,
            HTML.inputtext,
            ReactNative.TextInput,
            XAML.TextBox,
            Flutter.TextField,
            Qt.QLineEdit,
            Swing.JTextField,
            JavaFX.TextField,
            GTK.GtkEntry,
            Markup.Paragraph)]
        TimePicker = 61,
       
        [VisualElement(
            "File selection input.",
            UIElementCategory.Input,
            UISubCategory.Action,
            HTML.inputtext,
            ReactNative.TextInput,
            XAML.TextBox,
            Flutter.TextField,
            Qt.QLineEdit,
            Swing.JTextField,
            JavaFX.TextField,
            GTK.GtkEntry,
            Markup.Paragraph)]
        FilePicker = 62,

        [VisualElement(
            "Color selection input.",
            UIElementCategory.Input,
            UISubCategory.Action,
            HTML.inputtext,
            ReactNative.TextInput,
            XAML.TextBox,
            Flutter.TextField,
            Qt.QLineEdit,
            Swing.JTextField,
            JavaFX.TextField,
            GTK.GtkEntry,
            Markup.Paragraph)]
        ColorPicker = 63,

        [VisualElement(
            "Tag input allowing multiple small labeled items.",
            UIElementCategory.Input,
            UISubCategory.Action,
            HTML.inputtext,
            ReactNative.TextInput,
            XAML.TextBox,
            Flutter.TextField,
            Qt.QLineEdit,
            Swing.JTextField,
            JavaFX.TextField,
            GTK.GtkEntry,
            Markup.Paragraph)]
        TagInput = 64,

        [VisualElement(
            "Star rating input.",
            UIElementCategory.Input,
            UISubCategory.Numeric,
            HTML.inputtext,
            ReactNative.Slider,
            XAML.Slider,
            Flutter.Slider,
            Qt.QSlider,
            Swing.JSlider,
            JavaFX.Slider,
            GTK.GtkSlider,
            Markup.Paragraph)]
        Rating = 65,

        [VisualElement(
            "Rich text editor for formatted content.",
            UIElementCategory.Input,
            UISubCategory.Text,
            HTML.textarea,
            ReactNative.TextInput,
            XAML.TextBox,
            Flutter.TextField,
            Qt.QTextEdit,
            Swing.JTextArea,
            JavaFX.TextArea,
            GTK.GtkTextView,
            Markup.Paragraph)]
        RichTextEditor = 66,
 
        [VisualElement(
            "Command button executed on user click.",
            UIElementCategory.Input,
            UISubCategory.Action,
            HTML.button,
            ReactNative.Button,
            XAML.Button,
            Flutter.ElevatedButton,
            Qt.QPushButton,
            Swing.JButton,
            JavaFX.Button,
            GTK.GtkButton,
            Markup.Paragraph)]
        Button = 67,

        [VisualElement(
            "Icon-only button.",
            UIElementCategory.Input,
            UISubCategory.Action,
            HTML.button,
            ReactNative.Button,
            XAML.Button,
            Flutter.ElevatedButton,
            Qt.QPushButton,
            Swing.JButton,
            JavaFX.Button,
            GTK.GtkButton,
            Markup.Paragraph)]
        IconButton = 68,

        [VisualElement(
            "Form containing grouped input elements.",
            UIElementCategory.Input,
            UISubCategory.Container,
            HTML.div,
            ReactNative.View,
            XAML.StackPanel,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Form = 69,
        
        [VisualElement(
            "Search input with built-in query semantics.",
            UIElementCategory.Input,
            UISubCategory.Text,
            HTML.inputtext,
            ReactNative.TextInput,
            XAML.TextBox,
            Flutter.TextField,
            Qt.QLineEdit,
            Swing.JTextField,
            JavaFX.TextField,
            GTK.GtkEntry,
            Markup.Paragraph)]
        SearchBox = 70,


        // =============================
        // OUTPUT ELEMENTS
        // Category: Output / Informational
        // =============================


        [VisualElement(
            "A primary-header/title area.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.h1,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Title = 71,

        [VisualElement(
            "A secoundary header ",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.h2,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        SecondaryHeader = 72,

        [VisualElement(
            "A tertiary header",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.h3,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        TertiaryHeader = 73,
        [VisualElement(
            "A primary header/title area.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.h4,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        QuaternaryHeader = 74,

        [VisualElement(
            "A primary header/title area.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.h5,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        QuinaryHeader = 75,

        [VisualElement(
            "A primary header/title area.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.h6,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        SextenaryHeader = 76,

        [VisualElement(
            "A primary header/title area.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.h6,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Subtitle = 76,

        [VisualElement(
            "A footer area usually containing metadata or actions.",
            UIElementCategory.Structural,
            UISubCategory.Container,
            HTML.footer,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Footer = 80,

        // -------------------------------------------------------
        // TEXT BLOCK / LABEL
        // -------------------------------------------------------
        [VisualElement(
            "Simple text block used to display static information.",
            UIElementCategory.Output,
            UISubCategory.Text,
            HTML.p,
            ReactNative.Text,
            XAML.TextBlock,
            Flutter.Text,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.Text,
            GTK.GtkLabel,
            Markup.Paragraph)]
        Text = 91,

        [VisualElement(
            "Inline label for identifying fields.",
            UIElementCategory.Output,
            UISubCategory.Text,
            HTML.label,
            ReactNative.Text,
            XAML.TextBlock,
            Flutter.Text,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.Text,
            GTK.GtkLabel,
            Markup.Paragraph)]
        Label = 94,

        // -------------------------------------------------------
        // IMAGE OUTPUT
        // -------------------------------------------------------
        [VisualElement(
            "Image output element.",
            UIElementCategory.Output,
            UISubCategory.Media,
            HTML.img,
            ReactNative.Image,
            XAML.Image,
            Flutter.Image,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.ImageView,
            GTK.GtkImage,
            Markup.Figure)]
        Image = 95,

        // -------------------------------------------------------
        // ICON
        // -------------------------------------------------------
        [VisualElement(
            "Icon-only visual symbol.",
            UIElementCategory.Output,
            UISubCategory.Media,
            HTML.img,
            ReactNative.Image,
            XAML.Image,
            Flutter.Image,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.ImageView,
            GTK.GtkImage,
            Markup.Figure)]
        Icon = 96,

        // -------------------------------------------------------
        // BADGE
        // -------------------------------------------------------
        [VisualElement(
            "Badge showing status or numeric count.",
            UIElementCategory.Output,
            UISubCategory.Status,
            HTML.span,
            ReactNative.Text,
            XAML.TextBlock,
            Flutter.Text,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.Text,
            GTK.GtkLabel,
            Markup.Paragraph)]
        Badge = 97,

        // -------------------------------------------------------
        // TAG
        // -------------------------------------------------------
        [VisualElement(
            "Non-interactive tag or chip element.",
            UIElementCategory.Output,
            UISubCategory.Text,
            HTML.span,
            ReactNative.Text,
            XAML.TextBlock,
            Flutter.Text,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.Text,
            GTK.GtkLabel,
            Markup.Paragraph)]
        Tag = 98,

        // -------------------------------------------------------
        // MESSAGE BAR / ALERT / CALLOUT / BANNER
        // -------------------------------------------------------
        [VisualElement(
            "Static message bar for status, warnings, etc.",
            UIElementCategory.Output,
            UISubCategory.Status,
            HTML.div,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Text,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.Text,
            GTK.GtkLabel,
            Markup.Paragraph)]
        MessageBar = 99,

        [VisualElement(
            "Visual alert box.",
            UIElementCategory.Output,
            UISubCategory.Status,
            HTML.div,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Text,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.Text,
            GTK.GtkLabel,
            Markup.Paragraph)]
        Alert = 100,

        [VisualElement(
            "Inline highlight callout.",
            UIElementCategory.Output,
            UISubCategory.Status,
            HTML.div,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Text,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.Text,
            GTK.GtkLabel,
            Markup.Paragraph)]
        Callout = 101,

        [VisualElement(
            "Banner area for messages or branding.",
            UIElementCategory.Output,
            UISubCategory.Status,
            HTML.div,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Text,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.Text,
            GTK.GtkLabel,
            Markup.Paragraph)]
        Banner = 102,

        [VisualElement(
            "Status light showing online/offline state.",
            UIElementCategory.Output,
            UISubCategory.Status,
            HTML.div,
            ReactNative.View,
            XAML.TextBlock,
            Flutter.Text,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.Text,
            GTK.GtkLabel,
            Markup.Paragraph)]
        StatusIndicator = 103,


        // =============================
        // MEDIA ELEMENTS
        // Category: Media
        // =============================

        [VisualElement(
            "Audio playback component.",
            UIElementCategory.Media,
            UISubCategory.Media,
            HTML.audio,
            ReactNative.Audio,
            XAML.MediaElement,
            Flutter.AudioPlayer,
            Qt.QMediaPlayer,
            Swing.JPanel,
            JavaFX.MediaView,
            GTK.GtkMedia,
            Markup.Figure)]
        AudioPlayer = 141,

        [VisualElement(
            "Video playback component.",
            UIElementCategory.Media,
            UISubCategory.Media,
            HTML.video,
            ReactNative.Video,
            XAML.MediaElement,
            Flutter.VideoPlayer,
            Qt.QMediaPlayer,
            Swing.JPanel,
            JavaFX.MediaView,
            GTK.GtkMedia,
            Markup.Figure)]
        VideoPlayer = 142,


        [VisualElement(
            "Collapsible/expandable content panel.",
            UIElementCategory.Interactive,
            UISubCategory.Container,
            HTML.details,
            ReactNative.View,
            XAML.Expander,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Accordion = 121,

        [VisualElement(
            "Dialog requiring user focus.",
            UIElementCategory.Interactive,
            UISubCategory.Container,
            HTML.div,
            ReactNative.Modal,
            XAML.ContentControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Modal = 122,

        [VisualElement(
            "Offscreen sliding panel.",
            UIElementCategory.Interactive,
            UISubCategory.Container,
            HTML.div,
            ReactNative.View,
            XAML.ContentControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Drawer = 123,

        [VisualElement(
            "Popup that appears relative to another element.",
            UIElementCategory.Interactive,
            UISubCategory.Container,
            HTML.div,
            ReactNative.View,
            XAML.ContentControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Popover = 124,

        [VisualElement(
            "Context menu triggered by right-click.",
            UIElementCategory.Interactive,
            UISubCategory.Action,
            HTML.ul,
            ReactNative.View,
            XAML.ContextMenu,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPopupMenu,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Itemize)]
        ContextMenu = 125,

        [VisualElement(
            "Menu and submenu hierarchy.",
            UIElementCategory.Interactive,
            UISubCategory.Action,
            HTML.ul,
            ReactNative.View,
            XAML.Menu,
            Flutter.Container,
            Qt.QWidget,
            Swing.JMenuBar,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Itemize)]
        Menu = 126,

        [VisualElement(
            "Ribbon-style grouped command tabs.",
            UIElementCategory.Interactive,
            UISubCategory.Action,
            HTML.div,
            ReactNative.View,
            XAML.TabControl,
            Flutter.TabBarView,
            Qt.QTabWidget,
            Swing.JTabbedPane,
            JavaFX.TabPane,
            GTK.GtkNotebook,
            Markup.Section)]
        Ribbon = 127,

        [VisualElement(
            "Clickable item in a menu.",
            UIElementCategory.Interactive,
            UISubCategory.Action,
            HTML.li,
            ReactNative.Button,
            XAML.MenuItem,
            Flutter.ElevatedButton,
            Qt.QPushButton,
            Swing.JMenuItem,
            JavaFX.Button,
            GTK.GtkButton,
            Markup.Paragraph)]
        MenuItem = 128,


        // =============================
        // ADVANCED / NICHE ELEMENTS
        // Category: Advanced
        // =============================

        [VisualElement(
            "Custom code execution region or scripting area.",
            UIElementCategory.Advanced,
            UISubCategory.Text,
            HTML.div,
            ReactNative.View,
            XAML.ContentControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        CodeEditor = 156,

        [VisualElement(
            "Canvas for freehand drawing.",
            UIElementCategory.Advanced,
            UISubCategory.Container,
            HTML.canvas,
            ReactNative.View,
            XAML.Canvas,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        SketchPad = 157,

        [VisualElement(
            "3D model viewer component.",
            UIElementCategory.Advanced,
            UISubCategory.Media,
            HTML.div,
            ReactNative.View,
            XAML.ContentControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        ModelViewer = 158,

        [VisualElement(
            "Augmented reality viewport.",
            UIElementCategory.Advanced,
            UISubCategory.Media,
            HTML.div,
            ReactNative.View,
            XAML.ContentControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        ARViewport = 159,

        [VisualElement(
            "Virtual reality viewport.",
            UIElementCategory.Advanced,
            UISubCategory.Media,
            HTML.div,
            ReactNative.View,
            XAML.ContentControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        VRViewport = 160,

        [VisualElement(
            "Terminal / console interface.",
            UIElementCategory.Advanced,
            UISubCategory.Text,
            HTML.div,
            ReactNative.View,
            XAML.ContentControl,
            Flutter.Container,
            Qt.QWidget,
            Swing.JPanel,
            JavaFX.Pane,
            GTK.GtkBox,
            Markup.Section)]
        Terminal = 161,

        [VisualElement(
            "QR code display or generator.",
            UIElementCategory.Advanced,
            UISubCategory.Media,
            HTML.img,
            ReactNative.Image,
            XAML.Image,
            Flutter.Image,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.ImageView,
            GTK.GtkImage,
            Markup.Figure)]
        QRCode = 162,

        [VisualElement(
            "Barcode display or generator.",
            UIElementCategory.Advanced,
            UISubCategory.Media,
            HTML.img,
            ReactNative.Image,
            XAML.Image,
            Flutter.Image,
            Qt.QLabel,
            Swing.JLabel,
            JavaFX.ImageView,
            GTK.GtkImage,
            Markup.Figure)]
        Barcode = 163
    }
}


