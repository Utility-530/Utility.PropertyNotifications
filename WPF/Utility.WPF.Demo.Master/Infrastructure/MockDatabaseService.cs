using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Utility.Interfaces.NonGeneric.Data;

namespace Utility.WPF.Demo.Master.Infrastructure
{
    public class MockDatabaseService : IRepository
    {
        //public bool Delete(object item)
        //{
        //    System.Windows.MessageBox.Show("Delete");
        //    return true;
        //}

        //public bool Insert(object item)
        //{
        //    System.Windows.MessageBox.Show("Insert");
        //    return true;
        //}
        private int count = 3;

        private List<string> list = new[] { "One", "Two", "Three" }.ToList();

        public object Add(object item)
        {
            var next = (++count).ToWords();
            list.Add(next);
            System.Windows.MessageBox.Show(next);
            return next;
        }

        public object AddBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable AddMany(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable AddManyBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public object Find(object item)
        {
            throw new NotImplementedException();
        }

        public object FindBy(IQuery query)
        {
            return 1;
        }

        public IEnumerable FindMany(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable FindManyBy(IQuery query)
        {
            return list;
        }

        public object Remove(object item)
        {
            return list.Contains(item.ToString()) == false &&
                   list.Remove(item.ToString());
        }

        public object RemoveBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable RemoveMany(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable RemoveManyBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public object Update(object item)
        {
            throw new NotImplementedException();
        }

        public object UpdateBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable UpdateMany(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable UpdateManyBy(IQuery query)
        {
            throw new NotImplementedException();
        }
    }
}