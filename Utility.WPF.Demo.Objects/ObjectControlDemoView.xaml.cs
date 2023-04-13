#nullable enable

using Bogus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Demo.Objects
{
    /// <summary>
    /// Interaction logic for ObjectControlDemoView.xaml
    /// </summary>
    public partial class ObjectControlDemoView : UserControl
    {
        private static readonly Randomizer randomizer = new Randomizer();

        public ObjectControlDemoView()
        {
            InitializeComponent();
        }

        public static Person Person
        {
            get
            {
                var person = new Person();
                return person;
            }
        }

        public static Empty Empty => new Empty();

        public static PropertyNamesWithDifferentLengths CustomClass
        {
            get
            {
                var person = new PropertyNamesWithDifferentLengths
                {
                    OldMacdonaldhadafarmEIEIO = "And on that farm he had some dogs. E-I-E-I-O",
                    Phrase =
                        " Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec adipiscing" +
                        "nulla quis libero egestas lobortis.Duis blandit imperdiet ornare.Nulla" +
                        "ac arcu ut purus placerat congue.Integer pretium fermentum gravida."
                };
                return person;
            }
        }

        public static PropertyNamesWithDifferentLengths[] CustomClassArray
        {
            get
            {
                return new[] { CustomClass, CustomClass };
            }
        }

        public static DateAndDuration DateAndDurationClass
        {
            get
            {
                return new DateAndDuration { Date = DateTime.Now, Duration = TimeSpan.FromDays(1), Name = randomizer.AlphaNumeric(10) };
            }
        }

        public static ClassWithInnerArray CustomArrayClass
        {
            get
            {
                var person = new ClassWithInnerArray
                {
                    Title = "Latin",
                    Phrases = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec adipiscing".Split(" "),
                    Objects = CustomClassArray
                };
                return person;
            }
        }

        public class PropertyNamesWithDifferentLengths
        {
            public string OldMacdonaldhadafarmEIEIO { get; set; }

            public string Phrase { get; set; }
        }

        public class ClassWithInnerArray
        {
            public string Title { get; set; }

            public string[] Phrases { get; set; }

            public PropertyNamesWithDifferentLengths[] Objects { get; set; }
        }

        public class DateAndDuration
        {
            public TimeSpan Duration { get; set; }

            public DateTime Date { get; set; }

            public string? Name { get; set; }
        }
    }

    public class NameDescendingComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            return x != null ? y?.CompareTo(x) ?? 1 : -1;
        }
    }

    public class CustomDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObjectControlDemoView.DateAndDuration dateAndDuration)
            {
                return dateAndDuration?.Name;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static CustomDescriptionConverter Instance { get; } = new CustomDescriptionConverter();
    }

    public class Empty
    {
    }
}