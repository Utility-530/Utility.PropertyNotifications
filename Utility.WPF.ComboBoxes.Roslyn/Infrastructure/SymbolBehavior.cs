using System;
using System.Collections;
using System.Collections.Concurrent;
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
using Microsoft.VisualBasic;
using Microsoft.Xaml.Behaviors;
using Utility.PatternMatchings;
using Utility.Roslyn;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utility.WPF.ComboBoxes.Roslyn
{
    public enum Kind
    {
        Type,
        Method,
        Parameters,
        Interfaces,
        Properties,
    }
    class Paraphernalia
    {
        public AsyncEngine asyncEngine;
        public TelemetryTracker telemetry;
        public MruTracker mru;
    }

    public class SymbolBehavior : Behavior<ComboBox>
    {
        Dictionary<Kind, Paraphernalia> engines = new();
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(nameof(Kind), typeof(Kind), typeof(SymbolBehavior), new PropertyMetadata(default));
        public static readonly DependencyProperty CompilationProperty =
            DependencyProperty.Register(nameof(Compilation), typeof(CSharpCompilation), typeof(SymbolBehavior), new PropertyMetadata(changed));
        public static readonly DependencyProperty SecondaryKindProperty =
            DependencyProperty.Register(nameof(SecondaryKind), typeof(Kind?), typeof(SymbolBehavior), new PropertyMetadata());
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(Type), typeof(SymbolBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty AccessibilityProperty =
            DependencyProperty.Register(nameof(Microsoft.CodeAnalysis.Accessibility), typeof(Microsoft.CodeAnalysis.Accessibility), typeof(SymbolBehavior), new PropertyMetadata(Microsoft.CodeAnalysis.Accessibility.Public));

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

        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public Microsoft.CodeAnalysis.Accessibility Accessibility
        {
            get { return (Microsoft.CodeAnalysis.Accessibility)GetValue(AccessibilityProperty); }
            set { SetValue(AccessibilityProperty, value); }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == TypeProperty || e.Property == AccessibilityProperty)
            {
                updateCollection();
            }

            base.OnPropertyChanged(e);
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
                    updateCollection();
                }
            };

            AssociatedObject.DropDownClosed += (s, e) =>
            {
                if (FilterBehavior.GetSelectedItem((ComboBox)AssociatedObject) is Result { Symbol.Item: ISymbol symbol } result)
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

        void updateCollection()
        {
            setCollection(filter(), engines.TryGetValue(Kind, out var x)? x: engines[Kind]= new());

            IEnumerable<ISymbol> filter()
            {
                IEnumerable<ISymbol> collection = null;

                if (Kind == Kind.Type)
                {
                    collection = Compilation.GetValidTypes();
                }
                else if (Kind == Kind.Method)
                {
                    collection = Compilation.GetValidTypes()
                            .SelectMany(t => t.GetMethods()
                            .Where(a => a.IsPublic()));
                    collection = collection.Where(a =>
                    a is IMethodSymbol method &&
                    (Type != null ? method.Parameters.Any(p => Type.IsEquivalent(p.Type)) : true &&
                    method.DeclaredAccessibility >= Accessibility)).ToArray();
                }
                return collection;
            }
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

        async Task setCollection(IEnumerable objects, Paraphernalia paraphernalia)
        {
            List<Input> symbols = new List<Input>();
            var count = objects.Count();
            paraphernalia.telemetry = new TelemetryTracker();
            paraphernalia.mru = new MruTracker();
            var session = new Session(symbols.ToArray(), paraphernalia.mru, paraphernalia.telemetry, x => SymbolKindWeights.Get((IntelliSenseSymbolKind)x));
            paraphernalia.asyncEngine = new AsyncEngine(session, new AsyncRankingController());
            IProgress<double> progress = new Progress<double>(p =>
            {
                FilterBehavior.SetProgress(AssociatedObject, p);    
            });

            await Task.Run(() =>
            {
                foreach (ISymbol item in objects)
                {
                    var text = item switch
                    {
                        IMethodSymbol => item.Name,
                        INamedTypeSymbol => item?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                        _ => throw new Exception("VDSdd")
                    };
                    var token = new PatternToken(text);
                    var kind = IntelliSenseSymbolKind.Type;
                    var fullname = text;
                    symbols.Add(new Input(item, token, (int)kind, fullname));
                    var increment = Math.Max(1, (int)(count / 100.0));
                    if (symbols.Count % increment == 0 || symbols.Count == count)
                    {
                        progress.Report(100.0 * symbols.Count / count);
                    }
                }
            });
            session = new Session(symbols.ToArray(), paraphernalia.mru, paraphernalia.telemetry, x => SymbolKindWeights.Get((IntelliSenseSymbolKind)x));
            paraphernalia.asyncEngine = new AsyncEngine(session, new AsyncRankingController());
        }
    }
}
