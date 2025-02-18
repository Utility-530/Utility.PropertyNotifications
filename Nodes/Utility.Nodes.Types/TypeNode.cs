using System.Collections;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Types
{
    public class TypeNode : Node<object>
    {
        Lazy<IEnumerable> lazy = new();
        object data;

        public TypeNode()
        {
            lazy = new Lazy<IEnumerable>(() =>
            {
                var x = Assembly.GetEntryAssembly();
                var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*" + "Utility" + "*.dll").Where(a => a.Equals(x.Location) == false).ToArray();
                return files;
            });
            data = AppDomain.CurrentDomain.BaseDirectory;
        }

        public TypeNode(string assemblyFileName) : this(Convert(assemblyFileName))
        {
        }

        static Assembly Convert(string assemblyFileName)
        {
            if (assemblyFileName.Contains("dll") == false)
                throw new Exception("fsd3lp[pf");
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyFileName));
            return assembly;
        }

        public TypeNode(Assembly assembly)
        {
            lazy = new Lazy<IEnumerable>(() =>
            {
                return assembly.Namespaces();
            });
            data = assembly;
        }
        public TypeNode(NameSpace entryKeyValue)
        {

            lazy = new Lazy<IEnumerable>(() =>
            {
                return entryKeyValue.Types;
            });
            data = entryKeyValue;
        }
        public TypeNode(Type entryKeyValue)
        {

            lazy = new Lazy<IEnumerable>(Array.Empty<object>);
            data = entryKeyValue;
        }

        public override object Data => data;

        public override string ToString()
        {
            switch (data)
            {
                case Assembly assembly:
                    return assembly.FullName;
                case NameSpace nameSpace:
                    return nameSpace.Name;
                case Type type:
                    return type.FullName;
            }
            throw new Exception(" sd3332767");
        }

        public override IObservable<object?> GetChildren()
        {
            return Observable.Create<object?>(observer =>
            {
                foreach (var item in lazy.Value)
                    observer.OnNext(item);
                return Disposable.Empty;
            });
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(true);
        }

        public override Task<ITree> ToTree(object value)
        {
            return Task.FromResult(NewMethod(value));
        }

        private static ITree NewMethod(object value)
        {
            if (value is string assemblyName)
            {
                return new TypeNode(assemblyName);
            }
            if (value is Assembly assembly)
            {
                return new TypeNode(assembly);
            }
            if (value is NameSpace _namespace)
            {
                return new TypeNode(_namespace);
            }   
            if (value is Type type)
            {
                return new TypeNode(type);
            }
            else
                throw new Exception("df 343325");
        }
    }

    static class Helper
    {
        public static ISet<NameSpace> Namespaces(this Assembly assembly)
        {
            HashSet<NameSpace> namespacelist = new();
            foreach (Type type in assembly.GetTypes())
            {
                namespacelist.Add(new(type.Namespace, type.Assembly));
            }
            return namespacelist;
        }
    }

    public record NameSpace(string Name, Assembly Assembly)
    {
        public Type[] Types => Assembly.GetTypes().Where(a => Name?.Equals(a.Namespace) == true).ToArray();
    }
}
