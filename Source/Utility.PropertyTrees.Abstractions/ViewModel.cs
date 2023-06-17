namespace Utility.PropertyTrees.Abstractions
{
    public class ViewModel //: IViewModel
    {
        public ViewModel()
        {
        }

        //public Guid Id { get; set; }

        public MetaData MetaData { get; set; }

        public Template Template { get; set; }

        public Panel Panel { get; set; }

        public CollectionPanel CollectionPanel { get; set; }
    }

    public class MetaData
    {
        public Guid ParentId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool Inherit { get; set; }

        public string Mode { get; set; }

    }

    public class Template
    {
        public string DataTemplateKey { get; set; }
    }

    public class CollectionPanel
    {
        public CollectionGrid Grid { get; set; }
    }

    public class Panel
    {
        public GridParams GridParams { get; set; }
        public string PanelType { get; set; } = "WrapPanel";
    }

    public class Dock
    {
        //public Dock Row { get; set; }
    }

    public class GridParams
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int RowSpan { get; set; }
        public int ColumnSpan { get; set; }
    }

    public class CollectionGrid
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
    }
}