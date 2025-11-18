using System;
using System.Reactive.Linq;
using System.Windows.Forms;
using Utility.Reactives;

namespace Utility.WPF.Controls.FileSystem
{
    public class FolderBrowserCommand : BrowserCommand
    {
        public FolderBrowserCommand() : base(Select())
        {
        }

        private static Func<IObservable<string>> Select()
        {
            var obs =
                Observable.Create<string>(obs =>
                {
                    return Observable.Return(
                   OpenDialog(string.Empty, string.Empty))
                .Where(output => output.result ?? false)
                .ObserveOn(SynchronizationContextScheduler.Instance)
                .Select(output => output.path)
                .WhereIsNotNull()
                .Subscribe(obs);
                });
            return new Func<IObservable<string>>(() => obs);
        }

        //#region properties
        //public bool IsFolderPicker
        //{
        //    get;
        //    set;
        //}
        //#endregion properties

        protected static (bool? result, string path) OpenDialog(string filter, string extension)
        {
            var dialog = new FolderBrowserEx.FolderBrowserDialog();
            {
                DialogResult result = dialog.ShowDialog();

                return result == DialogResult.OK ? (true, dialog.SelectedFolder) : (false, null);
            }
        }
    }
}