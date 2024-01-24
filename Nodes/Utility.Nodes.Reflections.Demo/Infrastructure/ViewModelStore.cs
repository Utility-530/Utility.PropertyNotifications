using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Utility.Nodes.Demo;
using Utility.ViewModels;

namespace Utility.Nodes.Reflections.Demo.Infrastructure
{
    public class ViewModelStore
    {
        MiniStore miniStore = new MiniStore("viewmodels.sqlite");

        Queue<ViewModel> viewModels = new();
        ObjectsComparer.Comparer<ViewModel> comparer = new();

        private ViewModelStore()
        {
            CustomDataTemplateSelector.Instance
               .OfType<string>()
               .Subscribe(a =>
               {

                   while(viewModels.Any())
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
        public ViewModel Get(Guid guid)
        {
            if(miniStore.Get<ViewModel>(guid.ToString()) is ViewModel viewModel)
            {
                viewModels.Enqueue(viewModel);
                return viewModel;
            }
        
            viewModel = new ViewModel() { Guid = guid  };
            viewModels.Enqueue(viewModel);
            return viewModel;
        }


        public static ViewModelStore Instance { get; } = new ViewModelStore();
    }
}
