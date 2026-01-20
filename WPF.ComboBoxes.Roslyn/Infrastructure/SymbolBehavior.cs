using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Xaml.Behaviors;

namespace WPF.ComboBoxes.Roslyn
{
    public enum Kind
    {
        Type,
        Method,
        Parameters,
        Interfaces

    }
    class Paraphernalia
    {
        public AsyncIntelliSenseEngine asyncEngine;
        public TelemetryTracker telemetry;
        public MruTracker mru;
    }

    internal class SymbolBehavior : Behavior<ComboBox>
    {
        Dictionary<Kind, Paraphernalia> engines = new();

        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(nameof(Kind), typeof(Kind), typeof(SymbolBehavior), new PropertyMetadata(default));
        public static readonly DependencyProperty CompilationProperty =
            DependencyProperty.Register(nameof(Compilation), typeof(CSharpCompilation), typeof(SymbolBehavior), new PropertyMetadata(changed));
        public static readonly DependencyProperty SecondaryKindProperty =
            DependencyProperty.Register(nameof(SecondaryKind), typeof(Kind?), typeof(SymbolBehavior), new PropertyMetadata());

        public Kind Kind
        {
            get { return (Kind)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        public CSharpCompilation Compilation
        {
            get { return (CSharpCompilation)GetValue(CompilationProperty); }
            set { SetValue(CompilationProperty, value); }
        }

        public Kind? SecondaryKind
        {
            get { return (Kind?)GetValue(SecondaryKindProperty); }
            set { SetValue(SecondaryKindProperty, value); }
        }

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        protected override void OnAttached()
        {
            FilterBehavior.SetConverter(AssociatedObject, new SymbolStringConverter());

            AssociatedObject.DropDownOpened += (s, e) =>
            {
                if (AssociatedObject.ItemsSource is null)
                {
                    if (engines.ContainsKey(Kind))
                    {
                        return;
                    }
                    engines[Kind] = new();
                    IEnumerable collection = null;
                    if (Kind == Kind.Type)
                    {
                        collection = Compilation.GetValidTypes();
                    }
                    else if (Kind == Kind.Method)
                    {
                        collection = Compilation.GetValidTypes()
                                .Where(t => t.IsPublic())
                                .SelectMany(t => t.GetMethods());

                    }
                    setCollection(collection, engines[Kind]);
                }
            };

            AssociatedObject.DropDownClosed += (s, e) =>
            {
                if (FilterBehavior.GetSelectedItem((ComboBox)AssociatedObject) is IntelliSenseResult { Symbol.Item: ISymbol symbol } result)
                {
                    var paraphernalia = engines[Kind];
                    paraphernalia.telemetry.MarkUsed(symbol);
                    paraphernalia.mru.MarkUsed(symbol);

                    if (SecondaryKind is Kind kind)
                    {
                        if (kind == Kind.Parameters)
                        {
                            if (symbol is IMethodSymbol methodSymbol)
                            {
                                AssociatedObject.ItemsSource = methodSymbol.Parameters;
                                //AssociatedObject.ItemsSource = methodSymbol.ToDisplayParts();
                            }

                            AssociatedObject.IsDropDownOpen = true;

                        }
                        else if (kind == Kind.Interfaces)
                        {
                            if (symbol is ITypeSymbol typeSymbol)
                            {
                                AssociatedObject.ItemsSource = typeSymbol.AllInterfaces;
                            }
                            AssociatedObject.IsDropDownOpen = true;
                        }
                        else
                        {
                        }
                    }              
                }
            };

            AssociatedObject.SelectionChanged += (s, e) =>
            {
            };

            var dpd = DependencyPropertyDescriptor.FromProperty(FilterBehavior.SearchTextProperty, typeof(UIElement));
            dpd.AddValueChanged(AssociatedObject, OnAttachedPropertyChanged);


            base.OnAttached();


        }

        private async void OnAttachedPropertyChanged(object? sender, EventArgs e)
        {
            var results = await engines[Kind].asyncEngine.UpdateAsync(FilterBehavior.GetSearchText(AssociatedObject), fast =>
            {
                AssociatedObject.Dispatcher.Invoke(() =>
                {
                    var array = fast.Take(10).ToArray();
                    AssociatedObject.ItemsSource = array;

                });
            });
            AssociatedObject.ItemsSource = results.Take(10).ToArray();
        }

        async void setCollection(IEnumerable objects, Paraphernalia paraphernalia)
        {
            List<IndexedSymbol> symbols = new List<IndexedSymbol>();
            var count = objects.Count();
            IProgress<double> progress = new Progress<double>(p =>
            {
                paraphernalia.telemetry = new TelemetryTracker();
                paraphernalia.mru = new MruTracker();
                var session = new IntelliSenseSession(symbols.ToArray(), paraphernalia.mru, paraphernalia.telemetry);
                paraphernalia.asyncEngine = new AsyncIntelliSenseEngine(session, new AsyncRankingController());
                FilterBehavior.SetProgress(AssociatedObject, p);
            });

            await Task.Run(() =>
            {
                foreach (ISymbol item in objects)
                {
                    var text = item?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    var token = new PatternToken(text);
                    var kind = IntelliSenseSymbolKind.Type;
                    var fullname = text;
                    symbols.Add(new IndexedSymbol(item, token, kind, fullname));

                    if (symbols.Count % 100 == 0 || symbols.Count == count)
                    {
                        progress.Report(100.0 * symbols.Count / count);
                    }
                }
            });
        }
    }
}
