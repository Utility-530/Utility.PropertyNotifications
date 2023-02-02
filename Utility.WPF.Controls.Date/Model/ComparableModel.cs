using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Utility.Helpers.Ex;

namespace Utility.WPF.Controls.Date.Model;

/// <summary>
/// Replaces IComparable entities in <see cref="Collection"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class ComparableModel<T> where T : IComparable<T>
{
    private readonly List<T> removes = new();
    private readonly List<T> adds = new();
    private readonly ObservableCollection<T> collection = new();

    public async void Replace(ICollection<T> replacements)
    {
        removes.Clear();
        adds.Clear();
        removes.AddRange(collection.Except(replacements).OrderBy(a => a));
        adds.AddRange(replacements.Except(collection).OrderBy(a => a).ToList());

        while (removes.Count > 0)
        {
            collection.Remove(removes[0]);
            removes.RemoveAt(0);
            await Task.Delay(10);
        }

        while (adds.Count > 0)
        {
            collection.InsertInOrder(adds[0]);
            adds.RemoveAt(0);
            await Task.Delay(10);
        }
    }

    public ObservableCollection<T> Collection => collection;
}