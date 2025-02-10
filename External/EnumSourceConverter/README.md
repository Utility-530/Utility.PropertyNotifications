# EnumSourceConverter
Directly bind items controls to enum properties

#Nuget:
```
Install-Package Tonic.UI.EnumSource
```

#Usage
Add enum type properties to your view model:
```C#
public enum Animal
{
    Dog,
    Cat,
    [Description("Danger noodle")]
    Snake
}
public class ViewModel
{
    public Animal? PetType { get; set; }
}
```

Bind items controls to that property:
```xml
...
xmlns:ui="http://toniccomputing.com/patterns/ui"
...
<ComboBox ItemsSource="{ui:EnumSource PetType}" SelectedItem="{ui:EnumBinding PetType}" />
```
