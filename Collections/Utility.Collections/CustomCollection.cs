using System.Collections.ObjectModel;

namespace Utility.Collections
{
    /// <summary>
    /// for inserting items with arbitary indices
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomCollection<T> : ObservableCollection<T>
    {
        private Dictionary<int, int> indexMapping = [];

        public CustomCollection()
        {
        }

        // Method to insert an item at a specific index
        public void InsertSpecial(int index, T item)
        {
            try
            {
                // Ensure index is not negative
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");

                if (Count == 0)
                {
                    indexMapping[index] = 0;
                    Add(item);
                }
                else if (indexMapping.ContainsKey(index))
                {
                    var _index = indexMapping[index];
                    this[_index] = item;
                }
                else
                {
                    //throw new Exception("ds ssdsdsd");

                    var (small, large) = findClosestSmallerAndLarger(indexMapping.Select(a => a.Key).OrderBy(a => a).ToArray(), index);

                    if (small != -1)
                    {
                        indexMapping[index] = indexMapping[small] + 1;
                        foreach (var map in indexMapping)
                        {
                            if (map.Key > index)
                            {
                                indexMapping[map.Key]++;
                            }
                        }
                        Insert(indexMapping[index], item);
                    }
                    else
                    {
                        Add(item);
                        indexMapping[index] = Count;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            // If the index is beyond the current size, we insert at the end
            //if (index >= this.Count)
            //{
            //    this.Add(item);
            //    indexMapping[index] = this.Count - 1;
            //}
            //else
            //{
            //    // If the index is within the current bounds of the collection, adjust according to the mapping
            //    int actualIndex = getActualIndex(index);

            //    try
            //    {
            //        this.Insert(actualIndex, item);
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //    // Adjust the mapping for future insertions
            //    updateIndexMappingAfterInsertion(actualIndex);
            //}
        }

        // Method to replace an item at a specific logical index
        public void ReplaceSpecial(int index, T item)
        {
            // Ensure index is not negative
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");

            // Find the actual index using the index mapping
            int actualIndex = getActualIndex(index);

            // Check if the actual index is within bounds of the collection
            if (actualIndex >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of bounds.");
            }

            // Replace the item at the actual index
            this[actualIndex] = item;
        }

        public void RemoveAtSpecial(int index)
        {
            // Ensure index is not negative
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");

            // Find the actual index using the index mapping
            int actualIndex = getActualIndex(index);

            // Check if the actual index is within bounds of the collection

            // Replace the item at the actual index
            RemoveAt(actualIndex);
        }

        // Method to get the actual index for a given logical index
        private int getActualIndex(int index)
        {
            if (indexMapping.ContainsKey(index))
            {
                return indexMapping[index];
            }
            else
            {
                throw new Exception("VDS  sdsddd");
            }
        }

        private static (int, int) findClosestSmallerAndLarger(int[] numbers, int target)
        {
            // Sort the numbers to easily find the closest smaller and larger numbers
            var sortedNumbers = numbers.OrderBy(num => num).ToArray();

            int closestSmaller = -1;
            int closestLarger = -1;

            foreach (int num in sortedNumbers)
            {
                if (num < target)
                {
                    closestSmaller = num; // Keep updating closest smaller number
                }
                else if (num > target && closestLarger == -1)
                {
                    closestLarger = num; // First number greater than the target
                }
            }

            return (closestSmaller, closestLarger);
        }

        // Method to update the index mapping after an insertion
        private void updateIndexMappingAfterInsertion(int startIndex)
        {
            foreach (var key in new List<int>(indexMapping.Keys))
            {
                if (indexMapping[key] >= startIndex)
                {
                    indexMapping[key]++;
                }
            }
        }
    }
}