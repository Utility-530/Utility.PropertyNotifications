using System.Collections;
using System.Reflection;
using Utility.ProjectStructure;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public class AssemblyNode : Node
    {
        Lazy<IList> lazy = new();
        object data;

        public AssemblyNode(Assembly assembly) {
            lazy = new Lazy<IList>(() =>
            {
                return assembly.SelectResourceDictionaries().ToList();
            });
            data = assembly.FullName;
        }
        
        public AssemblyNode(ResourceDictionaryKeyValue resourceDictionaryKeyValue) {

            lazy = new Lazy<IList>(() =>
            {
                var values = resourceDictionaryKeyValue.ResourceDictionary.Cast<DictionaryEntry>().Select(a=> new DictionaryEntryKeyValue(a)).ToList();
                return values;
            });
            data = resourceDictionaryKeyValue.Key;
        }
                
        public AssemblyNode(DictionaryEntryKeyValue entryKeyValue) {

            lazy = new Lazy<IList>(Array.Empty<object>);

            data = entryKeyValue.Key;
        }

        public override object Data => data;

        public override Task<object?> GetChildren()
        {
            return Task.Run(()=> (object)lazy.Value);
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(true);
        }

        public override IReadOnlyTree ToNode(object value)
        {
            if (value is Assembly assembly)
            {
                return new AssemblyNode(assembly);
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
