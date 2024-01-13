using Utility.Infrastructure;
using Utility.Observables.NonGeneric;
using Utility.Observables.Generic;
using ReactiveUI;
using Utility.Helpers;
using Utility.PropertyTrees.Services;
using Utility.Helpers.Ex;
using Utility.PropertyTrees.Demo.ViewModels;
using System.Reactive.Linq;
using System.Reactive;
using System;
using System.Linq;
using Utility.Models;

namespace Utility.PropertyTrees.Demo.ViewModels;

internal class ModelController : BaseObject
{
    private RootModel model { get; set; }

    public override Key Key => new(Guids.ModelController, nameof(ModelController), typeof(ModelController));

    public ModelController()
    {
    }

    public void OnNext(StartEvent startEvent)
    {
        if (startEvent.Property.Data is RootModel rootModel)
        {
            Initialise(rootModel);
        }
    }

    private void Initialise(RootModel rootModel)
    {
        model = rootModel;

        model.ViewModels.WhenAnyValue(a => a.Key)
            .WhereNotNull()
        .Subscribe(a =>
        {
            var key = new Key(model.ViewModels.Guid, model.ViewModels.Name, Type.GetType(model.ViewModels.Type));
            foreach (var x in model.ViewModels.Collection)
                Observe<SetViewModelResponse, SetViewModelRequest>(new(key, x))
                .Subscribe(response =>
                {

                });
        });

        model.WhenAnyValue(a => a.LastRefresh)
            .WhereNotDefault()
            .Subscribe(a =>
            {
                Send(new RefreshRequest(a));
            });
    }


    public void OnNext(SelectionChange selectionChange)
    {
        if (selectionChange is { Node: PropertyBase { Key: Key Key } node })
        {
            if (node.SelfAndAncestors().Any(a => a.Name == "ViewModels"))
                return;

            model.ViewModels.Guid = Key.Guid;
            model.ViewModels.Type = Key.Type.AsString();
            model.ViewModels.Name = Key.Name;

            Observe<GetViewModelResponse, GetViewModelRequest>(new(Key))
                .Subscribe(response =>
                {
                    if (model.ViewModels.Collection.Any())
                        model.ViewModels.Collection.Clear();
                    foreach (ViewModel viewmodel in response.ViewModels)
                        model.ViewModels.Collection.Add(viewmodel);
                });
        }
    }
}