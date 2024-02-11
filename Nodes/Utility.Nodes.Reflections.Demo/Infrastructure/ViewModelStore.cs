using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Utility.Nodes.Demo;
using Utility.ViewModels;

namespace Utility.Nodes.Reflections.Demo.Infrastructure
{
    public class ViewModelStore
    {
        readonly MiniStore miniStore = new MiniStore("../../../Data/viewmodels.sqlite");
        readonly Queue<ViewModel> viewModels = new();
        readonly ObjectsComparer.Comparer<ViewModel> comparer = new();

        private ViewModelStore()
        {
            Directory.CreateDirectory("../../../Data");
            CustomDataTemplateSelector.Instance
                .OfType<string>()
                .Where(a => a.Equals("save", StringComparison.InvariantCultureIgnoreCase))
                .Subscribe(a =>
                {
                    while (viewModels.Any())
                    {
                        var dequeue = viewModels.Dequeue();
                        var original = miniStore.Get<ViewModel>(dequeue.Guid.ToString());
                        if (comparer.Compare(dequeue, original) == false)
                        {
                            miniStore.Put(dequeue.Guid.ToString(), string.Empty, dequeue);
                        }
                    }
                });  
        }

        public void Save(ViewModel viewModel)
        {
            viewModels.Enqueue(viewModel);
        }

        public ViewModel Get(Guid guid)
        {
            if (miniStore.Get<ViewModel>(guid.ToString()) is ViewModel viewModel)
            {          
                return viewModel;
            }

            viewModel = new ViewModel() { Guid = guid };
            viewModels.Enqueue(viewModel);
            return viewModel;
        }




        public static ViewModelStore Instance { get; } = new ViewModelStore();
    }
}
