using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Utility.Enums;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Generic;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees.Abstractions;

namespace Utility.Models.Trees
{
    public class TransformerModel : Model
    {
        public const string inputs = nameof(inputs);
        public const string output = nameof(output);
        public const string converter = nameof(converter);

        private InputsModel _inputs;
        private ThroughPutModel _output;
        private ConverterModel _converter;
        private IDisposable disposable;

        //[JsonIgnore]
        //public InputsModel Inputs
        //{
        //    get => _inputs;
        //    set
        //    {
        //        if (_inputs != value)
        //        {
        //            var _previous = _inputs;
        //            _inputs = value;
        //            this.RaisePropertyChanged(_previous, value);
        //        }
        //    }
        //}

        //[JsonIgnore]
        //public ThroughPutModel Outputs
        //{
        //    get => _output;
        //    set
        //    {
        //        if (_output != value)
        //        {
        //            var _previous = _output;
        //            _output = value;
        //            this.RaisePropertyChanged(_previous, value);
        //        }
        //    }
        //}

        //[JsonIgnore]
        //public ConverterModel Converter
        //{
        //    get => _converter;
        //    set
        //    {
        //        var previous = _converter;
        //        _converter = value;
        //        disposable?.Dispose();
        //        disposable = value.WithChangesTo(a => a.Method)
        //            .Where(a => a != null)
        //            .Subscribe(method =>
        //            {
        //                this.WithChangesTo(a => a.Outputs)
        //                .Where(a => a != null)
        //                .Subscribe(a =>
        //                {
        //                    a.Parameter = value.Method.ReturnParameter;
        //                });

        //                this.WithChangesTo(a => a.Inputs)
        //                .Where(i => i != null)
        //                .Subscribe(async i =>
        //                {
        //                    i.Parameters = value.Method.GetParameters();
        //                });
        //            });

        //        RaisePropertyChanged(previous, value);
        //    }
        //}

        //public override IEnumerable<IReadOnlyTree> Items()
        //{
        //    yield return _converter ??= new ConverterModel { Name = converter };
        //    yield return _inputs ??= new InputsModel { Name = inputs };
        //    yield return _output ??= new ThroughPutModel { Name = output };
        //}

        //public override void Addition(IReadOnlyTree a)
        //{
        //    switch (a.ToString())
        //    {
        //        case inputs: Inputs = a as InputsModel; break;
        //        case output: Outputs = a as ThroughPutModel; break;
        //        case converter: Converter = a as ConverterModel; break;
        //        default: throw new ArgumentOutOfRangeException("ds 33` 33kfl.. ");
        //    }
        //    base.Addition(a);
        //}

        public TransformerModel()
        {
            this.WithChangesTo(a => (a as IGetParent<IReadOnlyTree>).Parent).Subscribe(a =>
            {
                this.LocalIndex = a.Children.Count();
                this.Arrangement = Arrangement.Uniform;
                this.Columns.Add(new Structs.Dimension());
                this.Columns.Add(new Structs.Dimension());
                this.Columns.Add(new Structs.Dimension());
                this.Rows.Add(new Structs.Dimension());
                this.IsExpanded = true;
                //base.SetNode(node);
            });
        }
    }
}