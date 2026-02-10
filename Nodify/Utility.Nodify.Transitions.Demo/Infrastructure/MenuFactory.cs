using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;
using Utility.Nodify.Base.Abstractions;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;

namespace Utility.Nodify.Transitions.Demo.Infrastructure
{
    public class MenuFactory : IMenuFactory
    {
        Subject<(PointF, object)> subject = new();

        public MenuFactory()
        {

        }

        string name(object obj)
        {
            switch (obj)
            {
                case Ref2 ref2:
                    return ref2.Key;
                case Ref3 ref3:
                    return ref3.MethodInfo.Name;
                case Ref4 ref4:
                    return ref4.Type.Name;
            }
            throw new NotImplementedException();
        }

        PointF location = new();
        public INodeViewModel? CreateMenu()
        {
            var menu = new MenuViewModel();
            Globals.Resolver.Resolve<Subject<Type>>().Subscribe(a =>
            {
                menu.Type = a;
            });
            var node = new NodeViewModel() { Data = menu, IsVisible = false };
            node.WhenReceivedFrom(a => a.Location)
                .Subscribe(a =>
                {
                    if (a != default(PointF))
                    {
                        location = a;
                        node.RaisePropertyChanged(nameof(INodeViewModel.Location));
                    }
                });
            node.WhenReceivedFrom(a => a.Current)
                .Subscribe(a =>
                {
                    if (a is IGetData data)
                    {
                        subject.OnNext((location, data));
                        node.RaisePropertyChanged(nameof(INodeViewModel.Location));
                    }
                });

            return node;
        }

        public IDisposable Subscribe(IObserver<(PointF, object)> observer)
        {
            return subject.Subscribe(observer);
        }

        public class MenuViewModel : NotifyPropertyClass
        {
            private Type type;

            public Type Type
            {
                get => type;
                set
                {
                    type = value;
                    RaisePropertyChanged();
                }
            }

        }
    }
}
