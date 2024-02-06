using System;
using System.Collections;

using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Utility.Helpers;
using Utility.ProjectStructure;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Solutions
{
    public class AssemblyNode : Node
    {
        Lazy<IList> lazy = new();
        object data;


        public AssemblyNode()
        {
            lazy = new Lazy<IList>(() =>
            {
                var x = Assembly.GetEntryAssembly();
                var files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*" + "Utility" + "*.dll").Where(a => a.Equals(x.Location) == false).ToArray();
                return files;
            });
            data = AppDomain.CurrentDomain.BaseDirectory;
        }

        public AssemblyNode(string assemblyFileName)
        {
            if (assemblyFileName.Contains("dll") == false)
                throw new Exception("fsd3lp[pf");
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyFileName));
            lazy = new Lazy<IList>(() =>
            {
                return assembly.SelectResourceDictionaries().ToList();
            });
            data = assembly.FullName;
        }


        public AssemblyNode(Assembly assembly)
        {
            lazy = new Lazy<IList>(() =>
            {
                return assembly.SelectResourceDictionaries().ToList();
            });
            data = assembly.FullName;
        }

        public AssemblyNode(ResourceDictionaryKeyValue resourceDictionaryKeyValue)
        {
            lazy = new Lazy<IList>(() =>
            {
                IList values = null;
                values = resourceDictionaryKeyValue.ResourceDictionary.Cast<DictionaryEntry>().Select(a => new DictionaryEntryKeyValue(a)).ToList();
                return values;
            });
            data = resourceDictionaryKeyValue.Key;
        }

        public AssemblyNode(DictionaryEntryKeyValue entryKeyValue)
        {

            lazy = new Lazy<IList>(Array.Empty<object>);
            data = entryKeyValue.Value;
        }

        public override object Data => data;

        //public override Task<object?> GetChildren()
        //{
        //    //return Task.Run(() => (object)lazy.Value);
        //    return Task.FromResult((object)lazy.Value);
        //}
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

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            return Task.FromResult(NewMethod(value));

        }

        private static IReadOnlyTree NewMethod(object value)
        {
            if (value is Assembly assembly)
            {
                return new AssemblyNode(assembly);
            }
            if (value is string sAssembly)
            {
                return new AssemblyNode(sAssembly);
            }
            else if (value is ResourceDictionaryKeyValue resourceDictionaryKeyValue)
            {
                return new AssemblyNode(resourceDictionaryKeyValue);
            }
            else if (value is DictionaryEntryKeyValue entryKeyValue)
            {
                return new AssemblyNode(entryKeyValue);
            }
            else
                throw new Exception("df 343325");
        }


    }
}
