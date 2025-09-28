
using System.Threading.Tasks;
using Utility.Collections;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Interfaces.Reactive.NonGeneric;
using Utility.Keys;
using Utility.Observables;
using Utility.Observables.NonGeneric;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public class DirectoryNode : NodeViewModel<FileSystemInfo>, IObserver
    {
        private readonly Lazy<FileSystemInfo> lazyContent;
        private readonly string path;
        private readonly Collection _leaves = new();
        private readonly Collection _branches = new();
        private bool propertyflag;
        private bool childrenflag;
        private bool flag;
        private Subject subject = new();

        public DirectoryNode(DirectoryInfo info) : this((FileSystemInfo)info)
        {
        }

        public DirectoryNode(FileInfo info) : this((FileSystemInfo)info)
        {
        }



        private DirectoryNode(FileSystemInfo info) : base(false)
        {
            lazyContent = new Lazy<FileSystemInfo>(() => info);
            RaisePropertyChanged(nameof(Data));
            path = info.Name;
            subject.Subscribe(this);
        }

        public override string Key => lazyContent.Value.FullName;


        public override FileSystemInfo Data
        {
            get
            {
                if (lazyContent == null)
                {
                    return null;
                }
                return lazyContent.Value;
            }
        }

        public override async Task<bool> HasMoreChildren()
        {
            return await Task.FromResult(flag == false);
        }

        public override Task<ITree> ToTree(object value)
        {
            if (value is DirectoryInfo info)
                return Task.FromResult<ITree>(new DirectoryNode(info) { Parent = this });
            else if (value is FileInfo _info)
                return Task.FromResult<ITree>(new DirectoryNode(_info) { Parent = this });
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
            //Task.Run(() =>
            //{
            try
            {
                foreach (var directoryInfo in Directory.EnumerateDirectories(Data.FullName).Select(item => new DirectoryInfo(item)))
                {
                    subject.OnNext(directoryInfo);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
            }
            //});
            return Task.FromResult(true);
        }

        protected Task<bool> RefreshLeavesAsync()
        {
            if (propertyflag == false)
                propertyflag = true;
            else
                return Task.FromResult(false);

            //Task.Run(() =>
            //{
            try
            {
                foreach (var fileInfo in Directory.EnumerateFiles(Data.FullName).Select(item => new FileInfo(item)))
                {
                    subject.OnNext(fileInfo);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
            }
            //});
            return Task.FromResult(true);
        }


        public async void OnNext(object value)
        {
            if (value is DirectoryInfo directoryInfo)
                _branches.Add(directoryInfo);
            else if (value is FileInfo fileInfo)
                _leaves.Add(fileInfo);

            m_items.Add(await ToTree(value));
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(IEquatable? other)
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
    }
}