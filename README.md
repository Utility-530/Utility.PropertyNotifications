# WPF ComboBoxes Roslyn

*A demonstration project for*

- Integration of Roslyn's `Microsoft.CodeAnalysis` APIs into the WPF ComboBox control
- WPF data binding and MVVM patterns for UI updates


![me](https://github.com/dtaylor-530/WPF.ComboBoxes.Roslyn/blob/main/Assets/FilterTypesWithDebugging.gif)

![me](https://github.com/dtaylor-530/WPF.ComboBoxes.Roslyn/blob/main/Assets/FilterMethodsWithParamSelection.gif)

### Project Structure

- App.xaml: the styles for the controls used
- App.xaml.cs: the factory for constructing the demo project
- Common 
   - Helper: general extension methods
   - Compiler: methods for generating the CSharpCompilation class that is used to derived methods and types
- Filtering
   - FilterBehavior: a behavior class containing many attached properties for interfacing between the combobox's parts so their actions synchronise
- Infrastructure
   - Classes related to displaying the specific kind of data from CSharpCompilation in the ComboBox
- Roslyn
   - Classes related to the algorithm that efficiently filters the data from the CSharpCompilation object
    
### Building the Project

1. Clone the repository:
```bash
git clone https://github.com/dtaylor-530/WPF.ComboBoxes.Roslyn.git
```

2. Open the solution file `WPF.ComboBoxes.Roslyn.slnx` in Visual Studio

3. Restore NuGet packages and build the solution

## Specifications
- .NET 10

## Architecture
The project structure includes:

## License
MIT 

## Acknowledgements
- [Roslyn](https://github.com/dotnet/roslyn)
- [Basic.Reference.Assemblies](https://github.com/jaredpar/basic-reference-assemblies)


## Related Resources
- [Code Completion with Roslyn](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/syntax-analysis)
