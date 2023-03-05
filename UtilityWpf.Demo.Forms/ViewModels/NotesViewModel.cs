using Endless;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using Utility.Models;

namespace UtilityWpf.Demo.Forms.ViewModels
{
    public class NotesViewModel : Utility.ViewModels.ViewModel
    {
        private ICommand? changeCommand;

        public NotesViewModel(string header, IReadOnlyCollection<string> enumerable) : this(header, enumerable.Select(a => new NoteViewModel(a)).ToArray())
        {
        }

        public NotesViewModel(string header, IReadOnlyCollection<NoteViewModel> collection) : base(header)
        {
            Children = new ObservableCollection<NoteViewModel>(collection);
            //Intialise();
        }

        public override ObservableCollection<NoteViewModel> Children { get; }

        public NotesViewModel(string header) : this(header, Array.Empty<NoteViewModel>())
        {
        }

        public System.Collections.IEnumerator NewItem => 0.Repeat().Select(_ => new NoteViewModel(string.Empty)).GetEnumerator();

        public ICommand ChangeCommand => changeCommand ??= ReactiveCommand.Create<object, Unit>(Change);

        public override Property Model { get; }

        //public override Model Model { get; }

        private Unit Change(object xx)
        {
            //if (xx is CollectionEventArgs { Item: string item })
            //{
            //    var ivm = new NoteViewModel(item);
            //    //Subscribe(ivm);
            //    Collection.Add(ivm);
            //}
            return Unit.Default;
        }
    }
}