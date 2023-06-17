using Utility.Nodify.Core;
using System;
using System.Collections.Generic;
using ICommand = System.Windows.Input.ICommand;
using DryIoc;
using Utility.Nodify.Operations;
using System.Collections.ObjectModel;
using Utility.Models;
using Message = Utility.Nodify.Operations.Message;

namespace Utility.Nodify.Demo
{
    public class MessagesViewModel : BaseViewModel
    {
        private readonly IContainer container;

        public MessagesViewModel(IContainer container)
        {
            this.container = container;
        }


        public ICommand Next => container.Resolve<ICommand>(Keys.NextCommand);

        public ICommand Previous => container.Resolve<ICommand>(Keys.PreviousCommand);

        public ICollection<Message> Current => container.Resolve<RangeObservableCollection<Message>>(Keys.Current);

        public ICollection<Message> Past => container.Resolve<RangeObservableCollection<Message>>(Keys.Past);

        public ICollection<Message> Future => container.Resolve<RangeObservableCollection<Message>>(Keys.Future);

        public ICollection<Exception> Exceptions => container.Resolve<RangeObservableCollection<Exception>>(Keys.Exceptions);
     
    }
}
