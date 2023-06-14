using NetFabric.Hyperlinq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Helpers.Ex;
using Utility.WPF.Demo.Date.Infrastructure.Entity;
using Utility.WPF.Demo.Date.Infrastructure.ViewModels;

namespace Utility.WPF.Demo.Date.Infrastructure.Repository
{
    internal class NotesRepository
    {
        private readonly Dictionary<DateTime, ObservableCollection<NoteViewModel>> notes = new();

        public NoteViewModel FindMostRecent(DateTime date)
        {
            var key = new string(new[] { (char)random.Next(0, 100) });
            NoteViewModel noteViewModel = new(key) { Date = date };

            if (notes.ContainsKey(date.Date) == false)
            {
                notes[date] = new ObservableCollection<NoteViewModel> { noteViewModel };
            }

            _ = NoteEntity
                .Where(a => a.Date == date)
                .OrderByDescending(a => a.CreateTime)
                .FirstAsync()
                .ToObservable()
                .WhereNotNull()
                .Take(1)
                .Select(single =>
                {
                    return Task.Run(() =>
                    {
                        var map = AutoMapperSingleton.Instance.Map<NoteViewModel>(single);
                        return map;
                    });
                })
                .Switch()
                .Subscribe(map =>
                {
                    noteViewModel.Text = map.Text;
                    noteViewModel.CreateTime = map.CreateTime;
                    noteViewModel.Id = map.Id;
                    notes[date].InsertInOrderIfMissing(noteViewModel);
                });

            return noteViewModel;
        }

        private static Random random = new();

        public async Task<ObservableCollection<NoteViewModel>> FindAllAsync(DateTime date)
        {
            if (notes.ContainsKey(date))
            {
                //return await Observable.Return(notes[date]).ToTask();
            }
            else
                notes[date] = new ObservableCollection<NoteViewModel>();

            return await NoteEntity
                .Where(a => a.Date == date)
                .OrderByDescending(a => a.CreateTime)
                .ToListAsync()
                .ToObservable()
                .Select(list =>
                {
                    if (list == null || list.Count == 0)
                    {
                        var key = new string(new[] { (char)random.Next(0, 100) });
                        var noteViewModel = new NoteViewModel(key) { Date = date };
                        notes[date].InsertInOrderIfMissing(noteViewModel);
                    }
                    else
                    {
                        var map = AutoMapperSingleton.Instance.Map<NoteViewModel[]>(list);
                        notes[date].InsertInOrderIfMissing(map);
                    }

                    return notes[date];
                }).ToTask();
        }

        public async Task<NoteEntity?> Save(DateTime date)
        {
            var dayNotes = notes[date];
            var last = dayNotes.Last();
            // ignore if nothing has been changed
            if (last.Text == null)
                return null;

            var map = await Task.Run(() => AutoMapperSingleton.Instance.Map<NoteEntity>(dayNotes.Last()));

            if (await NoteEntity.FindAsync(map.Id) is { Text: var text })
                if (text == map.Text)
                {
                    // no need to insert if text hasn't changed
                    return null;
                }
                else
                {
                    // not possible to insert successfully if id matches a record in the database
                    map.Id = default;
                }
            var insert = await map.InsertAsync();
            var mapReverse = await Task.Run(() => AutoMapperSingleton.Instance.Map<NoteViewModel>(insert));
            dayNotes.Add(mapReverse);
            return insert;
        }

        public static NotesRepository Instance { get; } = new();
    }
}