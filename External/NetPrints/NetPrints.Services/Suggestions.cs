using NetPrints.Core;
using NetPrints.Graph;
using NetPrints.Interfaces;
using NetPrints.Reflection;
using NetPrintsEditor.Dialogs;
using NetPrintsEditor.Reflection;
using Splat;
using System.Collections.Generic;

namespace NetPrints.WPF.Demo
{



    public class Suggestions : ISuggestions
    {
        public IEnumerable<ITypesProvider> Get(/*INodeGraph Graph, IDataPin SuggestionPin,*/
            NodeDataPinType type,
            GraphType? graphType = default,
            ITypeSpecifier? visibleFrom = default,
            IEnumerable<ITypeSpecifier>? baseTypes = default,
            ITypeSpecifier? byType = default)
        //ITypeSpecifier inferredType,
        //ITypeSpecifier pinTypeSpec /*double mouseX, double mouseY*/)
        {
            //SuggestionViewModel.Graph = Graph;
            //SuggestionViewModel.PositionX = mouseX;
            //SuggestionViewModel.PositionY = mouseY;
            //SuggestionViewModel.SuggestionPin = SuggestionPin;
            //SuggestionViewModel.HideContextMenu = () => OnHideContextMenu?.Invoke(this, EventArgs.Empty);

            // Show all relevant methods for the type of the pin
            //List<SearchableComboBoxItem> suggestions = new List<SearchableComboBoxItem>();

            //List<SearchableComboBoxItem> AddSuggestionsWithCategory(string category, IEnumerable<object> newSuggestions)
            //{
            //    suggestions.AddRange(newSuggestions.Select(suggestion => new SearchableComboBoxItem(category, suggestion)));
            //    return suggestions;
            //}

            //if (SuggestionPin != null)
            //{
            //if (SuggestionPin is NodeOutputDataPin odp)
            if (type == NodeDataPinType.OutputData)
            {
                //if (typeSpecifier is TypeSpecifier pinTypeSpec)
                //{
                // Add make delegate
                yield return new NetPrints.Reflection.Basic("NetPrints", new DelegateSpecifier(byType, visibleFrom));

                // Add variables and methods of the pin type
                yield return Helper.CreateVariableQuery()
                         .WithType(byType)
                         .WithVisibleFrom(visibleFrom)
                         .WithStatic(false)
                         .AndName("Pin Variables");

                yield return Helper.CreateMethodQuery()
                        .WithVisibleFrom(visibleFrom)
                        .WithStatic(false)
                        .WithType(byType)
                        .AndName("Pin Methods");

                // Add methods of the base types that can accept the pin type as argument
                //foreach (var baseType in Graph.Class.AllBaseTypes)
                foreach (var baseType in baseTypes)
                {
                    yield return Helper.CreateMethodQuery()
                        .WithVisibleFrom(visibleFrom)
                        .WithStatic(false)
                        .WithArgumentType(byType)
                        .WithType(baseType)
                        .AndName("This Methods");
                }

                // Add static functions taking the type of the pin
                //AddSuggestionsWithCategory("Static Methods", reflectionProvider.Value.GetMethods(

                yield return Helper.CreateMethodQuery()
                    .WithArgumentType(byType)
                    .WithVisibleFrom(visibleFrom)
                    .WithStatic(true)
                    .AndName("Static Methods");
                //}
            }
            //else if (SuggestionPin is NodeInputDataPin idp)
            else if (type == NodeDataPinType.InputData)
            {
                //if (idp.PinType is TypeSpecifier pinTypeSpec)
                //{
                // Variables of base classes that inherit from needed type
                //foreach (var baseType in Graph.Class.AllBaseTypes)
                foreach (var baseType in baseTypes)
                {
                    //AddSuggestionsWithCategory("This Variables", reflectionProvider.Value.GetVariables(
                    //    new ReflectionProviderVariableQuery()
                    yield return Helper.CreateVariableQuery()
                            .WithType(baseType)
                            .WithVisibleFrom(visibleFrom)
                            .WithVariableType(byType, true)
                            .AndName("This Variables");
                }

                // Add static functions returning the type of the pin
                //AddSuggestionsWithCategory("Static Methods", reflectionProvider.Value.GetMethods(
                //    new ReflectionProviderMethodQuery()
                yield return Helper.CreateMethodQuery()
                        .WithStatic(true)
                        .WithVisibleFrom(visibleFrom)
                        .WithReturnType(byType)
                        .AndName("Static Methods");
                // }
            }
            //else if (SuggestionPin is NodeOutputExecPin oxp)
            else if (type == NodeDataPinType.OutputExec || type == NodeDataPinType.InputExec)
            {
                //ToDo: replace
                //GraphUtil.DisconnectOutputExecPin(oxp);
                if (graphType.HasValue)
                    yield return ReflectionHelper2.ByGraphType("NetPrints", graphType.Value);

                //foreach (var baseType in Graph.Class.AllBaseTypes)
                foreach (var baseType in baseTypes)
                {
                    yield return

                    //AddSuggestionsWithCategory("This Methods", reflectionProvider.Value.GetMethods(
                    //    new ReflectionProviderMethodQuery()
                    Helper.CreateMethodQuery()
                            .WithType(baseType)
                            .WithStatic(false)
                            .WithVisibleFrom(visibleFrom)
                            .AndName("This Methods");
                }

                //AddSuggestionsWithCategory("Static Methods", reflectionProvider.Value.GetMethods(
                //    new ReflectionProviderMethodQuery()
                yield return Helper.CreateMethodQuery()
                        .WithStatic(true)
                        .WithVisibleFrom(visibleFrom)
                        .AndName("Static Methods");
            }
            //else if (SuggestionPin is NodeInputExecPin ixp)

            //{
            //    foreach (var baseType in graphClassBaseTypes)
            //    {
            //        yield return ReflectionMethodHelper.Create()

            //        //AddSuggestionsWithCategory("This Methods", reflectionProvider.Value.GetMethods(
            //        //    new ReflectionProviderMethodQuery()
            //                .WithType(baseType)
            //                .WithStatic(false)
            //                .WithVisibleFrom(graphClassType)
            //                .AndName("This Methods");
            //    }

            //    //AddSuggestionsWithCategory("Static Methods", reflectionProvider.Value.GetMethods(
            //    //    new ReflectionProviderMethodQuery()
            //    yield return ReflectionMethodHelper.Create()
            //            .WithStatic(true)
            //            .WithVisibleFrom(graphClassType)
            //            .AndName("Static Methods");
            //}
            //else if (SuggestionPin is NodeInputTypePin itp)
            else if (type == NodeDataPinType.InputType)
            {
                // TODO: Consider static types
                //AddSuggestionsWithCategory("Types", reflectionProvider.Value.GetNonStaticTypes());
                yield return new NonStaticTypes("Types");
            }
            //else if (SuggestionPin is NodeOutputTypePin otp)
            else if (type == NodeDataPinType.OutputType)

            {
                //if (Graph is IExecutionGraph && otp.InferredType is TypeSpecifier typeSpecifier)
                if (graphType == GraphType.Execution)
                {
                    //AddSuggestionsWithCategory("Pin Static Methods", reflectionProvider.Value
                    //    .GetMethods(new ReflectionProviderMethodQuery()

                    yield return Helper.CreateMethodQuery()
                           .WithType(byType)
                           .WithStatic(true)
                           .WithVisibleFrom(visibleFrom)
                           .AndName("Pin Static Methods");
                }

                // Types with type parameters
                yield return new NonStaticGenericTypes("Generic Types");

                //if (Graph is IExecutionGraph)
                if (graphType == GraphType.Execution)
                {
                    // Public static methods that have type parameters
                    //AddSuggestionsWithCategory("Generic Static Methods", reflectionProvider.Value
                    //    .GetMethods(new ReflectionProviderMethodQuery()
                    yield return Helper.CreateMethodQuery()
                        .WithStatic(true)
                            .WithHasGenericArguments(true)
                            .WithVisibleFrom(visibleFrom)
                            .AndName("Generic Static Methods");
                }
            }

            //}
            else if (type == NodeDataPinType.None)
            {
                //AddSuggestionsWithCategory("NetPrints", BuiltInNodes.Instance[graphType]);
                if (graphType.HasValue)
                    yield return ReflectionHelper2.ByGraphType("NetPrints", graphType.Value);

                //if (Graph is IExecutionGraph)
                if (graphType == GraphType.Execution)
                {
                    // Get properties and methods of base class.
                    foreach (var baseType in baseTypes)
                    {
                        //AddSuggestionsWithCategory("This Variables", reflectionProvider.Value.GetVariables(
                        //new ReflectionProviderVariableQuery()
                        yield return Helper.CreateVariableQuery()
                            .WithVisibleFrom(visibleFrom)
                            .WithType(baseType)
                            .WithStatic(false)
                            .AndName("This Variables");

                        //AddSuggestionsWithCategory("This Methods", reflectionProvider.Value.GetMethods(
                        //new ReflectionProviderMethodQuery()
                        yield return Helper.CreateMethodQuery()
                            .WithType(baseType)
                            .WithVisibleFrom(visibleFrom)
                            .WithStatic(false)
                            .AndName("This Methods");
                    }

                    //AddSuggestionsWithCategory("Static Methods", reflectionProvider.Value.GetMethods(
                    //    new ReflectionProviderMethodQuery()
                    yield return Helper.CreateMethodQuery()
                        .WithStatic(true)
                            .WithVisibleFrom(visibleFrom)
                            .AndName("Static Methods");

                    //AddSuggestionsWithCategory("Static Variables", reflectionProvider.Value.GetVariables(
                    //new ReflectionProviderVariableQuery()
                    yield return Helper.CreateVariableQuery()
                        .WithStatic(true)
                            .WithVisibleFrom(visibleFrom)
                            .AndName("Static Variables");
                }
                //else if (Graph is IClassGraph)
                else if (graphType == GraphType.Class)
                {
                    //AddSuggestionsWithCategory("Types", reflectionProvider.Value.GetNonStaticTypes());
                    yield return Helper.NonStaticTypes("Types");
                }
            }


            //return suggestions.Distinct();
        }


    }

    public static class ReflectionHelper2
    {
        public static NetPrints.Reflection.Basic ByGraphType(string name, GraphType graphType)
        {
            var builtInNodes = Locator.Current.GetService<IBuiltInNodes>();
            return new NetPrints.Reflection.Basic(name, builtInNodes[graphType]);
        }


    }

    //public class ByGraphType(string Name, GraphType graphType) : Basic(Name, BuiltInNodes.Instance[graphType])
    //{
    //}

}