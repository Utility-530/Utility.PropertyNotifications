using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]
[assembly: XmlnsPrefix("http://schemas.utility.com/trees", "tree")]
[assembly: XmlnsDefinition("http://schemas.utility.com/trees", "Utility.WPF.Controls.Trees")]
[assembly: XmlnsDefinition("http://schemas.utility.com/trees", "Utility.WPF.Controls.Trees.Infrastructure")]
//[assembly: XmlnsDefinition("http://schemas.utility.com/dragablz", "Dragablz.Controls")]
//[assembly: XmlnsDefinition("http://schemas.utility.com/dockablz", "Dragablz.Controls.Dockablz")]