using System.Globalization;
using Utility.Conversions;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using _Key = Utility.Models.Key;

namespace Utility.Repos
{
    public class InMemoryRepository : IRepository
    {
        public record Table
        {
            public Guid Guid { get; set; }

            public Guid? Parent { get; set; }

            public string Name { get; set; }

            public Type Type { get; set; }

            public List<Property> Propertys { get; set; }
        }

        public record Type
        {
            public string? Assembly { get; set; }
            public string? Namespace { get; set; }
            public string Name { get; set; }

            public System.Type ToSystemType()
            {
                string assemblyQualifiedName = System.Reflection.Assembly.CreateQualifiedName(Assembly, $"{Namespace}.{Name}");
                var _type = System.Type.GetType(assemblyQualifiedName);
                return _type;
            }
        }

        public record Property
        {
            public DateTime Added { get; set; }
            public DateTime? Removed { get; set; }
            public object Value { get; set; }
        }

        public List<Table> Tables { get; set; } = new();
        public List<Type> Types { get; set; } = new();


        private Task initialisationTask;

        public InMemoryRepository()
        {
            Initialise();
        }



        public IEquatable Key => new Key<InMemoryRepository>(Guids.InMemory);


        private void Initialise()
        {
            initialisationTask = Task.WhenAll(

            );
        }

        public async Task Update(IEquatable key, object value)
        {
            if (key is not IGetGuid { Guid: var guid } _key)
            {
                throw new Exception("reg 43cs ");
            }

            await initialisationTask;

            var tables = Tables.Where(v => v.Guid.Equals(guid)).ToList();

            if (tables.Count == 1)
            {
                var single = tables.Single();

                try
                {

                    if (value is Key { Guid: Guid valueGuid, Name: var name } && valueGuid == guid)
                    {
                        if (single.Name != name)
                            single = (single with { Name = name });
                        return;
                    }
                    if (value == null)
                    {
                        tables.Remove(single);
                        return;
                    }


                    if (single.Propertys == null)
                        single.Propertys = new();
                    if (single.Propertys.LastOrDefault()?.Value != value)
                    {
                        single.Propertys.Add(new Property { Added = DateTime.Now, Value = value });
                    }
                }
                catch (Exception ex)
                {
                    throw;
                    //undo
                }
            }
            else if (tables.Count == 0)
            {
                throw new Exception("22v ere 4323");
            }
            else
            {
                throw new Exception("676 ere 4323");
            }
        }


        public async Task<IEquatable[]> FindKeys(IEquatable key)
        {
            if (key is not IGetGuid { Guid: var parent } _key)
            {
                throw new Exception("reg 43cs ");
            }

            await initialisationTask;

            if (key is _Key { Name: null, Type: null })
            {
                var tables = Tables.Where(a => a.Parent == parent);
                List<IEquatable> childKeys = new();
                foreach (var table in tables)
                {
                    var clrType = TypeHelper.ToType(table.Type.Assembly, table.Type.Namespace, table.Type.Name);
                    var childKey = new _Key(table.Guid, table.Name, clrType);
                    childKeys.Add(childKey);
                }
                return childKeys.ToArray();
            }

            if (key is _Key { Name: var name, Type: System.Type type })
            {
                if (name == null)
                {
                    var types = Types.Where(t => t.Assembly == type.Assembly.FullName && t.Namespace == type.Namespace && t.Name == type.Name);
                    var singleType = types.SingleOrDefault();
                    if (singleType == default)
                        return Array.Empty<IEquatable>();
                    var tables = Tables.Where(a => a.Parent == parent && a.Type == singleType);
                    List<IEquatable> childKeys = new();
                    foreach (var table in tables)
                    {
                        var clrType = TypeHelper.ToType(singleType.Assembly, singleType.Namespace, singleType.Name);
                        var childKey = new _Key(table.Guid, table.Name, clrType);
                        childKeys.Add(childKey);
                    }
                    return childKeys.ToArray();
                }
                else
                {
                    var tables = Tables.Where(a => a.Parent == parent && a.Name == name).ToList();
                    if (tables.Count == 0)
                    {
                        var guid = Guid.NewGuid();

                        try
                        { 
                            var newType = TryGetType();
                            if (newType == null)
                                throw new Exception("d s3322 88");
                            Tables.Add(new Table { Guid = guid, Name = name, Parent = parent, Type = newType });
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        return new[] { new _Key(guid, name, type) };
                    }
                    if (tables.Count == 1)
                    {
                        var table = tables.Single();
                        return new[] { new _Key(table.Guid, name, type) };
                    }
                    else
                    {
                        throw new Exception("3e909re 4323");
                    }
                }
            }
            throw new Exception(";d;d 3e9d 3209re 4323");

            Type TryGetType()
            {
                var tables = Tables.Where(a => a.Parent == parent && a.Name == name).ToList();
                if (tables.Count != 0)
                    return null;
                var types = Types.Where(t => t.Assembly == type.Assembly.FullName && t.Namespace == type.Namespace && t.Name == type.Name).ToList();
        
                if (types.Count == 0)
                {               
                    var newType = new Type { Assembly = type.Assembly.FullName, Namespace = type.Namespace, Name = type.Name };
                    Types.Add(newType);
                    return newType;
                }
                else if (types.Count == 1)
                {
                    return types.Single();
                }
                else
                    throw new Exception("f 434 4");
            }
        }

        public async Task<object?> FindValue(IEquatable key)
        {
            if (key is not IGetGuid { Guid: var guid } _key)
            {
                throw new Exception("reg 43cs ");
            }

            await initialisationTask;
            var tables = Tables.Where(a => a.Guid.Equals(guid)).ToList();
            if (tables.Count == 0)
            {
                throw new Exception($"!43 ere 4323 {key}");
            }
            else if (tables.Count == 1)
            {
                var table = tables.Single();
                if (table.Propertys == null || table.Propertys?.Count == 0)
                {
                    return null;
                }
                var properties = table.Propertys;
                {
                    List<object> list = new();
                    foreach (var property in properties)
                    {
                        if (ConversionHelper.TryChangeType(property.Value, table.Type.ToSystemType(), CultureInfo.CurrentCulture, out var value))
                            list.Add(value);
                        else
                            throw new Exception("332 b64ere 4323");
                    }
                    return list.LastOrDefault() ?? null;
                }
            }
            else
            {
                throw new Exception("ere 4323");
            }
        }
    }
}