using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Simulation.Infrastructure
{
    public class TwoWayEnumerator<T>
    {
        private IList<T> _buffer;
        private int _index;

        public TwoWayEnumerator(IList<T> list)
        {
            _buffer = list ?? throw new ArgumentNullException(nameof(list));
        }

        public T MovePrevious(int i = 1)
        {
            if (_index - i < 0)
            {
                throw new Exception("sdfdf");
            }

            _index -= i;
            return _buffer[_index];
        }

        public T MoveNext(int i = 1)
        {
            if (_index + i < _buffer.Count)
            {
                _index += i;
                return _buffer[_index];
            }

            throw new Exception("sdfdf");
        }

        public void Add(T value)
        {
            _buffer.Add(value);
        }

        public T Current
        {
            get
            {
                if (_index < 0 || _index >= _buffer.Count)
                    throw new InvalidOperationException();

                return _buffer[_index];
            }
        }

        public bool CanMovePrevious(int i = 1)
        {
            return _index - i >= 0;
        }

        public bool CanMoveNext(int i = 1)
        {
            return _index + i < _buffer.Count;
        }


        public void Reset()
        {
            _index = 0;
        }
    }
}
