using System;
using System.Collections.Generic;
using System.ComponentModel;
using Utility.Interfaces.NonGeneric;
using Utility.Observables;
using Utility.Trees.Abstractions;

namespace Utility.Models.Trees
{
    public class SetModel : ResolvableModel, /*ISet,*/ IValue, System.IObservable<ValueChanged>
    {
        private object value;
        private List<System.IObserver<ValueChanged>> list = new();
        private List<ValueChanged> values = [];

        public override void AddDescendant(IReadOnlyTree node, int level)
        {
            if (node == null)
                throw new NullReferenceException();
            switch (node)
            {
                case Model { Value: var value } Model:
                    {
                        Model.PropertyChanged -= Model_PropertyChanged;
                        Model.PropertyChanged += Model_PropertyChanged;
                        Value = value;
                        break;
                    }
            }
        }

        public override void ReplaceDescendant(IReadOnlyTree @new, IReadOnlyTree old, int level)
        {
            switch (@new)
            {
                case Model { Value: var value } Model:
                    {
                        Model.PropertyChanged += Model_PropertyChanged;
                        Value = value;
                        break;
                    }
            }

            switch (old)
            {
                case Model { Value: var value } Model:
                    {
                        Model.PropertyChanged -= Model_PropertyChanged;
                        Value = value;
                        break;
                    }
            }
        }

        public override void SubtractDescendant(IReadOnlyTree @new, int level)
        {
            switch (@new)
            {
                case Model { Value: var value } Model:
                    {
                        Model.PropertyChanged -= Model_PropertyChanged;
                        Value = null;
                        break;
                    }
            }
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var prop = sender.GetType().GetProperty(e.PropertyName);
            Value = prop.GetValue(sender);
            values.Add(new ValueChanged(prop, Value));

            foreach (var s in list)
            {
                s.OnNext(new ValueChanged(prop, Value));
            }
        }

        public IDisposable Subscribe(System.IObserver<ValueChanged> observer)
        {
            foreach (var value in values)
            {
                observer.OnNext(value);
            }
            list.Add(observer);
            return new ActionDisposable(() => list.Remove(observer));
        }
    }
}