using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Microsoft.Win32;
using ReactiveUI;

namespace Utility.WPF.Controls.FileSystem
{
    public class FileBrowserCommand : BrowserCommand
    {
        private string filter;
        private string extension;
        private bool multiSelect;
        protected Subject<string> filterChanges = new();
        protected Subject<string> extensionChanges = new();
        protected Subject<bool> multiSelectChanges = new();

        public FileBrowserCommand() : base(Select(out var filterSubject, out var extensionSubject, out var multiSelect))
        {
            filterChanges.StartWith(Filter).Subscribe(filterSubject);
            extensionChanges.StartWith(Extension).Subscribe(extensionSubject);
            multiSelectChanges.StartWith(IsMultiSelect).Subscribe(multiSelect);
        }

        private static Func<IObservable<string>> Select(out ReplaySubject<string> filter, out ReplaySubject<string> extension, out ReplaySubject<bool> multiSelectChanges)
        {
            filter = new(1);
            extension = new(1);
            multiSelectChanges = new(1);

            var obs = filter.CombineLatest(extension, multiSelectChanges, (b, c, d) => OpenDialog(b, c, d))
                .Take(1)
            .Where(output => output.result ?? false)
            .ObserveOnDispatcher()
            .SelectMany(output => output.path.ToObservable())
            .WhereNotNull();
            return new Func<IObservable<string>>(() => obs);
        }

        public string Filter
        {
            get => filter; set { filter = value; filterChanges.OnNext(value); }
        }

        public string Extension
        {
            get => extension; set { extension = value; extensionChanges.OnNext(value); }
        }

        public bool IsMultiSelect
        {
            get => multiSelect; set { multiSelect = value; multiSelectChanges.OnNext(value); }
        }

        protected static (bool? result, string[] path) OpenDialog(string filter, string extension, bool isMultiSelect)
        {
            OpenFileDialog dlg = new();
            dlg.Multiselect = isMultiSelect;
            if (extension != null)
                dlg.DefaultExt = extension.StartsWith(".") ? extension : "." + extension;
            if (filter != null)
                dlg.Filter = filter;

            bool? result = dlg.ShowDialog();

            // if dialog closed dlg.FileName may become inaccessible; hence check for result equal to true
            return result == true ? (result, dlg.FileNames) : (result, null);
        }

        public static (string filter, string extension) GetImageFilterAndExtension()
        {
            StringBuilder filter = new();
            StringBuilder allfilter = new();

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;
            foreach (var (filenameExtension, codecName) in GetCaption(codecs))
            {
                filter.Append($"{sep}{codecName} ({filenameExtension})|{filenameExtension}");
                allfilter.Append(filenameExtension + ";");
                sep = "|";
            }

            filter.Append($"{sep}{"All Files"} ({"*.*"})|{"*.*"}");

            var combined = "Image Files|" + allfilter.ToString() + "|" + filter.ToString();
            return (combined, ".png"); // Default file extension

            static IEnumerable<(string fileNameExtension, string codecName)> GetCaption(ImageCodecInfo[] codecs)
            {
                return from codec in codecs
                       let codecName = codec.CodecName[8..].Replace("Codec", "Files").Trim()
                       select (codec.FilenameExtension, codecName);
            }
        }
    }
}