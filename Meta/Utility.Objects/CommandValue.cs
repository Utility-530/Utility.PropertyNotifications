using System.Reflection;
using System.Windows.Input;
using Utility.Commands;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Objects
{
    public class CommandValue : IValue<ICommand>
    {
        private ObservableCommand command;

        public CommandValue(MethodInfo methodInfo, object instance, Dictionary<int, object?> dictionary)
        {
            MethodInfo = methodInfo;
            Instance = instance;
            Dictionary = dictionary;
            Dictionary = dictionary;
            command = new ObservableCommand(a =>
            {
                methodInfo.Invoke(instance, dictionary.OrderBy(a => a.Key).Select(a => a.Value).ToArray());
            });
        }

        public ICommand Value => command;

        public MethodInfo MethodInfo { get; }
        public object Instance { get; }
        public Dictionary<int, object?> Dictionary { get; }

        object IValue.Value => Value;
    }

}
