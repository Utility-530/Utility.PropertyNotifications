using Autofac;
using System;
using System.Collections.Generic;
using Utility.Infrastructure.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.PropertyTrees.Infrastructure;
using Utility.PropertyTrees.WPF.Demo;

namespace Utility.Infrastructure
{
    public class Resolver : IResolver
    {
        private readonly IContainer container;

        public Resolver(IContainer container)
        {
            this.container = container;
        }

        public ICollection<IObserver> Observers(IEquatable equatable)
        {
            if (equatable is not Key key)
            {
                throw new Exception("£ dfgdf");
            }
            else if (key.Name == nameof(AutoObject))
            {
                return new IObserver[] { container.Resolve<IHistory>() };
            }
            else if (key.Name == nameof(History))
            {
                return new IObserver[] { container.Resolve<HistoryController>(), container.Resolve<IPropertyStore>() };
            }
            else if (key.Name == nameof(HistoryController))
            {
                return new IObserver[] { container.Resolve<Playback>() };
            }
            else if (key.Name == nameof(Playback))
            {
                return new IObserver[] { container.Resolve<History>() };
            }
            throw new Exception("nnnnndsfs s");
        }
    }
}
