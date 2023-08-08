using DryIoc.ImTools;
using Fasterflect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Meta;
using Utility.Models;
using Utility.Nodes;
using Utility.Observables.NonGeneric;
using Utility.WPF.Helpers;
using Utility.WPF.Meta;

namespace Utility.WPF.Nodes
{
    public class ProjectRootNode : Node, IObserver
    {
        private Subject subject = new();
        bool flag;
        public ProjectRootNode()
        {
            subject.Subscribe(this);
        }

        public override IEquatable Key => new StringKey("root");

        public override string Content => "root";

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }

        public override Node ToNode(object value)
        {
            if (value is Assembly assembly)
                return new ProjectNode(assembly) { Parent = this };
            //else if (value is Assembly info)
            //    return new ProjectNode(info) { Parent = this };
            throw new Exception("r 3 33");
        }

        public override Task<object?> GetChildren()
        {
            flag = true;
            return Task.FromResult((object)AssemblySingleton.Instance.Assemblies);
        }

        public void OnNext(object value)
        {
            _children.Add(ToNode(value));
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IEquatable? other)
        {
            return (other as ProjectRootNode)?.Key == Key;
        }

        public void OnProgress(int complete, int total)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "root";
        }
    }

    public class ProjectNode : Node, IObserver
    {
        private readonly Lazy<Assembly> lazyContent;
        private readonly string path;
        private bool flag;
        private Subject subject = new();

        public ProjectNode(string path) : this()
        {
            lazyContent = new Lazy<Assembly>(() => Assembly.LoadFrom(path));
            this.path = path;
        }

        public ProjectNode(Assembly info) : this()
        {
            lazyContent = new Lazy<Assembly>(() => info);
            path = info.FullName;
        }

        private ProjectNode()
        {
            subject.Subscribe(this);
        }

        public override IEquatable Key => new StringKey(lazyContent.Value.FullName);

        public override Assembly Content => lazyContent.Value;

        public override async Task<bool> HasMoreChildren()
        {
            return await Task.FromResult(flag == false);
        }

        public override Node ToNode(object value)
        {
            if (value is KeyValuePair<DictionaryEntry, ResourceDictionary> str)
                return new ResourceDictionaryNode(str.Key, str.Value) { Parent = this };
            //else if (value is Assembly info)
            //    return new ProjectNode(info) { Parent = this };
            throw new Exception("r 3 33");
        }

        public override async Task<object?> GetChildren()
        {
            var x = ResourceDictionaryKeyValue.ResourceViewTypes(await lazyContent)               
                .ToDictionary(a => a.Key, a => a.Value);
            flag = true;
            return x;
        }

        public void OnNext(object value)
        {
            _children.Add(ToNode(value));
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IEquatable? other)
        {
            return (other as ProjectNode)?.path == path;
        }

        public void OnProgress(int complete, int total)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return path.EndsWith("\\") ? path.Replace("/", "\\").Remove(path.Length - 1).Split("\\").Last() : path.Replace("/", "\\").Split("\\").Last();
        }

        //private static bool Predicate(string key)
        //{
        //    var rKey = key.Remove(".baml");

        //    foreach (var ignore in new[] { "view", "usercontrol", "app" })
        //    {
        //        if (rKey.EndsWith(ignore))
        //            return false;
        //    }
        //    return true;
        //}
    }

    public class ResourceDictionaryNode : Node, IObserver
    {
        //private readonly string path;
        private bool flag;
        private Subject subject = new();
        private readonly DictionaryEntry entry;
        private readonly ResourceDictionary dictionary;

        public ResourceDictionaryNode(DictionaryEntry entry, ResourceDictionary dictionary) : this()
        {
            this.entry = entry;
            this.dictionary = dictionary;
        }

        //public ResourceDictionaryNode(Assembly info) : this()
        //{
        //    lazyContent = new Lazy<Assembly>(() => info);
        //    path = info.FullName;
        //}

        private ResourceDictionaryNode()
        {
            subject.Subscribe(this);
        }

        public override IEquatable Key => new StringKey(entry.Key.ToString());

        public override ResourceDictionary Content => dictionary;

        public override async Task<bool> HasMoreChildren()
        {
            return await Task.FromResult(flag == false);
        }

        public override Node ToNode(object value)
        {
            if (value is DictionaryEntry str)
                return new DictionaryElementNode(Factorys.FrameworkElementFactory.GetFrameworkElement(str.Value)) { Parent = this };
            //else if (value is Assembly info)
            //    return new ProjectNode(info) { Parent = this };
            throw new Exception("r 3 33");
        }

        public override async Task<object?> GetChildren()
        {
            flag = true;
            return dictionary;

        }

        public void OnNext(object value)
        {
            _children.Add(ToNode(value));
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IEquatable? other)
        {
            return (other as ResourceDictionaryNode)?.Key == Key;
        }

        public void OnProgress(int complete, int total)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return entry.Key.ToString();
        }
    }


    public class DictionaryElementNode : Node
    {
        private FrameworkElement frameworkElement;
        bool flag;
        public DictionaryElementNode(FrameworkElement frameworkElement)
        {
            this.frameworkElement = frameworkElement;
        }

        public override IEquatable Key => new StringKey(frameworkElement.Name.ToString());

        public override FrameworkElement Content => frameworkElement;

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }

        public override Node ToNode(object value)
        {
            if (value is DictionaryEntry str)
                return new DictionaryElementNode(Factorys.FrameworkElementFactory.GetFrameworkElement(str.Value)) { Parent = this };
            //else if (value is Assembly info)
            //    return new ProjectNode(info) { Parent = this };
            throw new Exception("r 3 33");
        }

        public override async Task<object?> GetChildren()
        {
            flag = true;
            return Array.Empty<object>();
        }

        public void OnNext(object value)
        {
            _children.Add(ToNode(value));
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IEquatable? other)
        {
            return (other as ResourceDictionaryNode)?.Key == Key;
        }

        public void OnProgress(int complete, int total)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return frameworkElement.Name.ToString();
        }
    }


}