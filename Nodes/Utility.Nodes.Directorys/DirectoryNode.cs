
using Utility.Collections;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Models;
using Utility.Observables;
using Utility.Observables.NonGeneric;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public class DirectoryNode : Node, IObserver
    {
        private readonly Lazy<FileSystemInfo> lazyContent;
        private readonly string path;
        private readonly Collection _leaves = new();
        private readonly Collection _branches = new();
        private bool propertyflag;
        private bool childrenflag;
        private bool flag;

        //public DirectoryNode(string path) : this()
        //{
        //    lazyContent = new Lazy<FileSystemInfo>(() => new(path));
        //    this.path = path;
        //}

        public DirectoryNode(DirectoryInfo info) : this()
        {
            lazyContent = new Lazy<FileSystemInfo>(() => info);
            path = info.Name;
        }
            
        public DirectoryNode(FileInfo info) : this()
        {
            lazyContent = new Lazy<FileSystemInfo>(() => info);
            path = info.Name;
        }

        private Subject subject = new();

        private DirectoryNode()
        {
            subject.Subscribe(this);
        }

        public override string Key => lazyContent.Value.FullName;


        public override FileSystemInfo Data => lazyContent.Value;

        public override async Task<bool> HasMoreChildren()
        {
            return await Task.FromResult(flag == false);
        }

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            //if (value is string str)
            //    return Task.FromResult<IReadOnlyTree>(new DirectoryNode(new str) { Parent = this });
            if (value is DirectoryInfo info)
                return Task.FromResult<IReadOnlyTree>(new DirectoryNode(info) { Parent = this });
            else if (value is FileInfo _info)
                return Task.FromResult<IReadOnlyTree>(new DirectoryNode(_info) { Parent = this });
            throw new Exception("r 3 33");
        }

        public virtual IObservable Folders
        {
            get
            {
                _ = RefreshBranchesAsync();
                return _branches;
            }
        }


        public virtual IObservable Files
        {
            get
            {
                _ = RefreshLeavesAsync();
                return _leaves;
            }
        }

        public override async Task<bool> RefreshChildrenAsync()
        {
            await RefreshBranchesAsync();
            return await RefreshLeavesAsync();
        }

        protected Task<bool> RefreshBranchesAsync()
        {
            if (childrenflag == false)
                childrenflag = true;
            else
                return Task.FromResult(false);
            Task.Run(() =>
            {
                try
                {
                    foreach (var directoryInfo in Directory.EnumerateDirectories(Data.FullName).Select(item => new DirectoryInfo(item)))
                    {
                        subject.OnNext(directoryInfo);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    return;
                }
                catch (Exception ex)
                {
                }
            });
            return Task.FromResult(true);
        }

        protected Task<bool> RefreshLeavesAsync()
        {
            if (propertyflag == false)
                propertyflag = true;
            else
                return Task.FromResult(false);

            Task.Run(() =>
            {
                try
                {
                    foreach (var fileInfo in Directory.EnumerateFiles(Data.FullName).Select(item => new FileInfo(item)))
                    {
                        subject.OnNext(fileInfo);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    return;
                }
                catch (Exception ex)
                {
                }
            });
            return Task.FromResult(true);
        }

  
        public async void OnNext(object value)
        {
            if (value is DirectoryInfo directoryInfo)
                _branches.Add(directoryInfo);
            else if (value is FileInfo fileInfo)
                _leaves.Add(fileInfo);

            m_items.Add(await ToNode(value));
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
            return (other as DirectoryNode)?.path == path;
        }

        public void OnProgress(int complete, int total)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return path.EndsWith("\\") ? path.Replace("/", "\\").Remove(path.Length - 1).Split("\\").Last() : path.Replace("/", "\\").Split("\\").Last();
        }

        public override System.IObservable<object?> GetChildren()
        {
            throw new NotImplementedException();
        }
    }
}