using System;
using System.Windows;
using Utility.Persists;
using Utility.WPF.Demo.Date.Infrastructure.Repository;

namespace Utility.WPF.Demo.Date
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            FreeSqlFactory.InitialiseSQLite();
        }

        public static DateTime CurrentDate { get; set; }

        protected override void OnExit(ExitEventArgs e)
        {
            SaveNote();
            base.OnExit(e);
        }

        private void SaveNote()
        {
            if (CurrentDate != default)
                _ = NotesRepository.Instance.Save(CurrentDate).Result;
        }
    }
}
