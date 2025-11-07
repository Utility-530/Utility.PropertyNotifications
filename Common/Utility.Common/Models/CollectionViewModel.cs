namespace Utility.Common.Models;

//public class CollectionViewModel<T> : ViewModel
//{
//    private readonly ObservableCollection<T> notes = new();
//    private T? selectedNote;
//    private int selectedIndex;

//    public T? SelectedNote
//    {
//        get => selectedNote;
//        set => this.RaiseAndSetIfChanged(ref selectedNote, value);
//    }

//    public int SelectedIndex
//    {
//        get => selectedIndex;
//        set => this.RaiseAndSetIfChanged(ref selectedIndex, value);
//    }

//    public ICollection Notes => notes;

//    public void Replace(IList<T> notes, T selectedNote)
//    {
//        //notes.SelectChanges()
//        //    .Subscribe(a =>
//        //    {
//        //        if (a.Action == NotifyCollectionChangedAction.Add)
//        //            this.notes.AddRange(a.NewItems.OfType<NoteViewModel>().ToArray());
//        //    });

//        for (int i = this.notes.Count - 1; i >= 0; i--)
//        {
//            //await Task.Delay(50);
//            this.notes.RemoveAt(i);
//        }

//        for (int i = 0; i < notes.Count; i++)
//        {
//            this.notes.Add(notes[i]);
//        }
//        //for (int i = notes.Count - 1; i >= 0; i--)
//        //{
//        //    //await Task.Delay(50);
//        //    this.notes.Add(notes[i]);
//        //}

//        this.selectedNote = default;
//        //await Task.Delay(500);
//        this.selectedNote = selectedNote;

//        this.WhenAnyValue(a => a.SelectedNote)
//            .Subscribe(a => SelectedIndex = notes.IndexOf(a));

//        this.WhenAnyValue(a => a.SelectedIndex)
//            .Subscribe(a =>
//            {
//                if (a < 0)
//                {
//                    if (notes.Count > 0)
//                        SelectedIndex = 0;
//                    return;
//                }

//                if (notes.Count == 1)
//                {
//                    if (SelectedNote.Equals(default) == false)
//                        return;
//                    SelectedNote = notes[0];
//                    SelectedIndex = 0;
//                    return;
//                }

//                if (notes.Count > a)
//                {
//                    SelectedNote = notes[a];
//                    SelectedIndex = a;
//                }

//                //else {
//                //   SelectedNote = new[] { selectedNote }
//                //}
//            });
//    }

//}