using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Utility.WPF.Attached
{
    public class FileEx
    {

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached(
                "Source",
                typeof(string),
                typeof(FileEx),
                new PropertyMetadata(null, OnSourceChanged));

        public static readonly DependencyProperty FileContentProperty =
            DependencyProperty.RegisterAttached(
                "FileContent",
                typeof(string),
                typeof(FileEx),
                new PropertyMetadata(null));

        public static void SetSource(DependencyObject element, string value)
        {
            element.SetValue(SourceProperty, value);
        }

        public static string GetSource(DependencyObject element)
        {
            return (string)element.GetValue(SourceProperty);
        }

        public static void SetFileContent(DependencyObject element, string value)
        {
            element.SetValue(FileContentProperty, value);
        }

        public static string GetFileContent(DependencyObject element)
        {
            return (string)element.GetValue(FileContentProperty);
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;

            string relativePath = e.NewValue as string;
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                SetFileContent(d, null);
                return;
            }

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.GetFullPath(Path.Combine(baseDir, relativePath));

                if (File.Exists(fullPath))
                {
                    string json = File.ReadAllText(fullPath);
                    SetFileContent(d, json);
                }
                else
                {
                    SetFileContent(d, $"File not found: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                SetFileContent(d, $"Error loading JSON: {ex.Message}");
            }
        }
    }
}

