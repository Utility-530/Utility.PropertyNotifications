using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.Interfaces.Trees;
using Utility.Trees.Abstractions;
using Utility.WPF.Panels;
using Index = Utility.Structs.Index;

namespace Utility.WPF.Demo.Panels
{
    // Example data models
    public class FileSystemItem(string key, string name, string type, long size = 0) : ITreeIndex
    {
        public string Key { get; set; } = key;
        public string Name { get; set; } = name;
        public string Type { get; set; } = type;
        public long Size { get; set; } = size;
        public IIndex Index => (Index)Key;
    }

    public class OrganizationItem : ITreeIndex
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public int EmployeeCount { get; set; }
        public IIndex Index => (Index)Key;

        public OrganizationItem(string key, string title, string department, int employeeCount = 0)
        {
            Key = key;
            Title = title;
            Department = department;
            EmployeeCount = employeeCount;
        }
    }

    public class TaskItem : ITreeIndex
    {
        public string Key { get; set; }
        public string TaskName { get; set; }
        public string Status { get; set; }
        public string Assignee { get; set; }

        public IIndex Index => (Index)Key;

        public TaskItem(string key, string taskName, string status, string assignee)
        {
            Key = key;
            TaskName = taskName;
            Status = status;
            Assignee = assignee;
        }
    }

    /// <summary>
    /// Interaction logic for TreePanelView.xaml
    /// </summary>
    public partial class TreePanelView : UserControl
    {
        private TreePanel currentTreePanel;
        private ComboBox exampleSelector;

        public TreePanelView()
        {
            InitializeComponent();
            SetupUI();
            LoadFileSystemExample();
        }

        private void SetupUI()
        {
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Top panel with example selector
            var topPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10),
                Background = Brushes.LightGray,
                Height = 40
            };

            var label = new Label { Content = "Example:", VerticalAlignment = VerticalAlignment.Center };
            exampleSelector = new ComboBox
            {
                Width = 200,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 0, 0)
            };

            exampleSelector.Items.Add("File System");
            exampleSelector.Items.Add("Organization Chart");
            exampleSelector.Items.Add("Project Tasks");
            exampleSelector.SelectedIndex = 0;
            exampleSelector.SelectionChanged += OnExampleChanged;

            topPanel.Children.Add(label);
            topPanel.Children.Add(exampleSelector);

            // Scroll viewer for tree panel
            var scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(10)
            };

            currentTreePanel = new TreePanel
            {
                IndentSize = 30,
                ItemSpacing = 2,
                Background = Brushes.White,
                Margin = new Thickness(5)
            };

            scrollViewer.Content = currentTreePanel;

            Grid.SetRow(topPanel, 0);
            Grid.SetRow(scrollViewer, 1);
            mainGrid.Children.Add(topPanel);
            mainGrid.Children.Add(scrollViewer);

            Content = mainGrid;
        }

        private void OnExampleChanged(object sender, SelectionChangedEventArgs e)
        {
            currentTreePanel.Children.Clear();

            switch (exampleSelector.SelectedIndex)
            {
                case 0:
                    LoadFileSystemExample();
                    break;

                case 1:
                    LoadOrganizationExample();
                    break;

                case 2:
                    LoadProjectTasksExample();
                    break;
            }
        }

        private void LoadFileSystemExample()
        {
            var items = new[]
            {
                new FileSystemItem("1", "Documents", "Folder"),
                new FileSystemItem("1.1", "Reports", "Folder"),
                new FileSystemItem("1.1.1", "Q1_Report.pdf", "PDF", 2048000),
                new FileSystemItem("1.1.2", "Q2_Report.pdf", "PDF", 1856000),
                new FileSystemItem("1.1.3", "Annual_Summary.docx", "Word", 1024000),
                new FileSystemItem("1.2", "Presentations", "Folder"),
                new FileSystemItem("1.2.1", "Sales_Pitch.pptx", "PowerPoint", 5120000),
                new FileSystemItem("1.2.2", "Training_Slides.pptx", "PowerPoint", 3072000),
                new FileSystemItem("1.3", "Templates", "Folder"),
                new FileSystemItem("1.3.1", "Letter_Template.dotx", "Word Template", 512000),
                new FileSystemItem("2", "Projects", "Folder"),
                new FileSystemItem("2.1", "WebApp", "Folder"),
                new FileSystemItem("2.1.1", "src", "Folder"),
                new FileSystemItem("2.1.1.1", "index.html", "HTML", 4096),
                new FileSystemItem("2.1.1.2", "styles.css", "CSS", 2048),
                new FileSystemItem("2.1.1.3", "app.js", "JavaScript", 8192),
                new FileSystemItem("2.1.2", "assets", "Folder"),
                new FileSystemItem("2.1.2.1", "logo.png", "Image", 32768),
                new FileSystemItem("2.1.2.2", "background.jpg", "Image", 204800),
                new FileSystemItem("2.2", "MobileApp", "Folder"),
                new FileSystemItem("2.2.1", "MainActivity.java", "Java", 16384),
                new FileSystemItem("2.2.2", "activity_main.xml", "XML", 4096),
                new FileSystemItem("3", "Downloads", "Folder"),
                new FileSystemItem("3.1", "installer.exe", "Executable", 52428800),
                new FileSystemItem("3.2", "data.zip", "Archive", 10485760)
            };

            foreach (var item in items)
            {
                var element = CreateFileSystemElement(item);
                currentTreePanel.Children.Add(element);
            }
        }

        private void LoadOrganizationExample()
        {
            var items = new[]
            {
                new OrganizationItem("1", "CEO", "Executive", 1),
                new OrganizationItem("1.1", "VP Engineering", "Engineering", 1),
                new OrganizationItem("1.1.1", "Frontend Team Lead", "Engineering", 5),
                new OrganizationItem("1.1.2", "Backend Team Lead", "Engineering", 6),
                new OrganizationItem("1.1.3", "DevOps Manager", "Engineering", 3),
                new OrganizationItem("1.2", "VP Sales", "Sales", 1),
                new OrganizationItem("1.2.1", "Regional Manager North", "Sales", 4),
                new OrganizationItem("1.2.2", "Regional Manager South", "Sales", 3),
                new OrganizationItem("1.2.3", "Inside Sales Manager", "Sales", 6),
                new OrganizationItem("1.3", "VP Marketing", "Marketing", 1),
                new OrganizationItem("1.3.1", "Digital Marketing Manager", "Marketing", 4),
                new OrganizationItem("1.3.2", "Content Marketing Manager", "Marketing", 3),
                new OrganizationItem("1.3.3", "Brand Manager", "Marketing", 2),
                new OrganizationItem("1.4", "CFO", "Finance", 1),
                new OrganizationItem("1.4.1", "Accounting Manager", "Finance", 3),
                new OrganizationItem("1.4.2", "Financial Analyst", "Finance", 2),
                new OrganizationItem("1.5", "HR Director", "Human Resources", 1),
                new OrganizationItem("1.5.1", "Recruiter", "Human Resources", 2),
                new OrganizationItem("1.5.2", "Benefits Coordinator", "Human Resources", 1)
            };

            foreach (var item in items)
            {
                var element = CreateOrganizationElement(item);
                currentTreePanel.Children.Add(element);
            }
        }

        private void LoadProjectTasksExample()
        {
            var items = new[]
            {
                new TaskItem("1", "Website Redesign Project", "In Progress", "Project Manager"),
                new TaskItem("1.1", "Design Phase", "Completed", "Design Team"),
                new TaskItem("1.1.1", "User Research", "Completed", "UX Designer"),
                new TaskItem("1.1.2", "Wireframes", "Completed", "UX Designer"),
                new TaskItem("1.1.3", "Visual Design", "Completed", "UI Designer"),
                new TaskItem("1.2", "Development Phase", "In Progress", "Dev Team"),
                new TaskItem("1.2.1", "Frontend Development", "In Progress", "Frontend Dev"),
                new TaskItem("1.2.1.1", "Header Component", "Completed", "Frontend Dev"),
                new TaskItem("1.2.1.2", "Navigation Menu", "In Progress", "Frontend Dev"),
                new TaskItem("1.2.1.3", "Footer Component", "Pending", "Frontend Dev"),
                new TaskItem("1.2.2", "Backend Development", "In Progress", "Backend Dev"),
                new TaskItem("1.2.2.1", "API Endpoints", "In Progress", "Backend Dev"),
                new TaskItem("1.2.2.2", "Database Schema", "Completed", "Backend Dev"),
                new TaskItem("1.2.3", "Integration", "Pending", "Full Stack Dev"),
                new TaskItem("1.3", "Testing Phase", "Pending", "QA Team"),
                new TaskItem("1.3.1", "Unit Testing", "Pending", "Developers"),
                new TaskItem("1.3.2", "Integration Testing", "Pending", "QA Engineer"),
                new TaskItem("1.3.3", "User Acceptance Testing", "Pending", "Product Owner"),
                new TaskItem("2", "Mobile App Development", "Planning", "Mobile Team"),
                new TaskItem("2.1", "Requirements Analysis", "In Progress", "Business Analyst"),
                new TaskItem("2.2", "Technical Architecture", "Pending", "Tech Lead"),
                new TaskItem("2.3", "Prototype Development", "Pending", "Mobile Dev")
            };

            foreach (var item in items)
            {
                var element = CreateTaskElement(item);
                currentTreePanel.Children.Add(element);
            }
        }

        private UIElement CreateFileSystemElement(FileSystemItem item)
        {
            var border = new Border
            {
                Background = GetFileSystemBackground(item.Type),
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(3),
                Margin = new Thickness(1),
                Padding = new Thickness(6, 3, 6, 3),
                DataContext = item
            };

            var panel = new StackPanel { Orientation = Orientation.Horizontal };

            var icon = new TextBlock
            {
                Text = GetFileSystemIcon(item.Type),
                FontSize = 14,
                Margin = new Thickness(0, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            var nameText = new TextBlock
            {
                Text = item.Name,
                FontWeight = item.Type == "Folder" ? FontWeights.Bold : FontWeights.Normal,
                VerticalAlignment = VerticalAlignment.Center,
                MinWidth = 120
            };

            var sizeText = new TextBlock
            {
                Text = item.Type != "Folder" ? FormatFileSize(item.Size) : "",
                FontSize = 10,
                Foreground = Brushes.Gray,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };

            panel.Children.Add(icon);
            panel.Children.Add(nameText);
            panel.Children.Add(sizeText);
            border.Child = panel;

            return border;
        }

        private UIElement CreateOrganizationElement(OrganizationItem item)
        {
            var border = new Border
            {
                Background = GetOrganizationBackground(item.Department),
                BorderBrush = Brushes.Navy,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(1),
                Padding = new Thickness(8, 4, 8, 4),
                DataContext = item
            };

            var panel = new StackPanel { Orientation = Orientation.Horizontal };

            var titleText = new TextBlock
            {
                Text = item.Title,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
                MinWidth = 180
            };

            var countText = new TextBlock
            {
                Text = item.EmployeeCount > 0 ? $"({item.EmployeeCount} people)" : "",
                FontSize = 10,
                Foreground = Brushes.DarkBlue,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };

            panel.Children.Add(titleText);
            panel.Children.Add(countText);
            border.Child = panel;

            return border;
        }

        private UIElement CreateTaskElement(TaskItem item)
        {
            var border = new Border
            {
                Background = GetTaskBackground(item.Status),
                BorderBrush = Brushes.DarkGreen,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(3),
                Margin = new Thickness(1),
                Padding = new Thickness(6, 3, 6, 3),
                DataContext = item
            };

            var panel = new StackPanel { Orientation = Orientation.Horizontal };

            var statusIndicator = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = GetStatusColor(item.Status),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 6, 0)
            };

            var taskText = new TextBlock
            {
                Text = item.TaskName,
                VerticalAlignment = VerticalAlignment.Center,
                MinWidth = 200
            };

            var assigneeText = new TextBlock
            {
                Text = $"[{item.Assignee}]",
                FontSize = 10,
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.DarkGreen,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };

            panel.Children.Add(statusIndicator);
            panel.Children.Add(taskText);
            panel.Children.Add(assigneeText);
            border.Child = panel;

            return border;
        }

        private Brush GetFileSystemBackground(string type)
        {
            return type switch
            {
                "Folder" => Brushes.LightBlue,
                "PDF" => Brushes.LightCoral,
                "Word" => Brushes.LightSteelBlue,
                "PowerPoint" => Brushes.Orange,
                "HTML" => Brushes.LightGreen,
                "CSS" => Brushes.LightPink,
                "JavaScript" => Brushes.LightYellow,
                _ => Brushes.White
            };
        }

        private string GetFileSystemIcon(string type)
        {
            return type switch
            {
                "Folder" => "📁",
                "PDF" => "📄",
                "Word" => "📝",
                "PowerPoint" => "📊",
                "HTML" => "🌐",
                "CSS" => "🎨",
                "JavaScript" => "⚡",
                "Image" => "🖼️",
                "Executable" => "⚙️",
                "Archive" => "📦",
                _ => "📄"
            };
        }

        private Brush GetOrganizationBackground(string department)
        {
            return department switch
            {
                "Executive" => Brushes.Gold,
                "Engineering" => Brushes.LightCyan,
                "Sales" => Brushes.LightGreen,
                "Marketing" => Brushes.LightPink,
                "Finance" => Brushes.LightYellow,
                "Human Resources" => Brushes.Lavender,
                _ => Brushes.White
            };
        }

        private Brush GetTaskBackground(string status)
        {
            return status switch
            {
                "Completed" => Brushes.LightGreen,
                "In Progress" => Brushes.LightYellow,
                "Pending" => Brushes.LightGray,
                "Planning" => Brushes.LightBlue,
                _ => Brushes.White
            };
        }

        private Brush GetStatusColor(string status)
        {
            return status switch
            {
                "Completed" => Brushes.Green,
                "In Progress" => Brushes.Orange,
                "Pending" => Brushes.Gray,
                "Planning" => Brushes.Blue,
                _ => Brushes.Black
            };
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}