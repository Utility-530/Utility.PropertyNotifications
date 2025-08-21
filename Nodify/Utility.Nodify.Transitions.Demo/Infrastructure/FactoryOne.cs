using Utility.Interfaces.Exs;
using Utility.Models;
using Utility.Models.Diagrams;
using Utility.Nodify.Generator.Services;
using Utility.ServiceLocation;
using Utility.Interfaces.NonGeneric;
using Utility.Extensions;

namespace Utility.Nodify.Transitions.Demo.NewFolder
{
    internal class FactoryOne
    {
        const string react_to_3 = nameof(react_to_3);
        const string react_to_4 = nameof(react_to_4);
        const string root = nameof(root);

        public static void build(IModelResolver modelResolver)
        {
            modelResolver.Register(react_to_3, () => new ValueModel<Type>() { Name = react_to_3, Parent = modelResolver[root] });
            modelResolver.Register(react_to_4, () => new ValueModel<string>() { Name = react_to_4, Parent = modelResolver[root] });
            modelResolver.Register(root, () => new Model(() => [modelResolver[react_to_3], modelResolver[react_to_4]]) { Name = root });
        }

        public static void connect(IServiceResolver serviceResolver, IModelResolver modelResolver)
        {
            serviceResolver.Connect<PredicateReturnParam, PredicateParam>();
            serviceResolver.Connect<ListInstanceReturnParam, ListInParam>();
            serviceResolver.Connect<ListInstanceReturnParam, ListParam>();
            serviceResolver.Observe<InstanceTypeParam>((IValueModel)modelResolver[react_to_3]);
            serviceResolver.Observe<FilterParam>((IValueModel)modelResolver[react_to_4]);
        }

        public static void initialise(IModelResolver modelResolver)
        {
            Globals.Resolver.Resolve<IPlaybackEngine>().OnNext(
                new PlaybackAction(() => modelResolver[react_to_4],
                () => (modelResolver[react_to_4] as Interfaces.Generic.ISet<string>).Set("something"),
                () => (modelResolver[react_to_4] as Interfaces.Generic.ISet<string>).Set(null),
                b => (modelResolver[react_to_4] as ISetIsActive).IsActive = b)
                { Name = react_to_4 });

            Globals.Resolver.Resolve<IPlaybackEngine>().OnNext(
                new PlaybackAction(() => modelResolver[react_to_3],
                () => (modelResolver[react_to_3] as Interfaces.Generic.ISet<Type>).Set(typeof(List<object>)),
                () => (modelResolver[react_to_3] as Interfaces.Generic.ISet<Type>).Set(null),
                b => (modelResolver[react_to_3] as ISetIsActive).IsActive = b)
                { Name = react_to_3 });
        }
    }
}
