using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility.Observables;

namespace Utility.Nodes
{
    public class DirectoryNode : Node, IObserver
    {
        private readonly Lazy<DirectoryInfo> lazyContent;
        private readonly string path;
        private bool flag;

        public DirectoryNode(string path) : this()
        {
            lazyContent = new Lazy<DirectoryInfo>(() => new(path));
            this.path = path;
        }

        public DirectoryNode(DirectoryInfo info) : this()
        {
            lazyContent = new Lazy<DirectoryInfo>(() => info);
            path = info.Name;
        }

        Subject subject = new();

        private DirectoryNode()
        {
            subject.Subscribe(this);

        }

        public override DirectoryInfo Content => lazyContent.Value;

        public override async Task<bool> HasMoreChildren()
        {
            return await Task.FromResult(flag == false);
        }

        public override string ToString()
        {
            return path.EndsWith("\\") ? path.Replace("/", "\\").Remove(path.Length - 1).Split("\\").Last() : path.Replace("/", "\\").Split("\\").Last();
        }

        public override Node ToNode(object value)
        {
            if (value is string str)
                return new DirectoryNode(str) { Parent = this };
            else if (value is DirectoryInfo info)
                return new DirectoryNode(info) { Parent = this };
            throw new Exception("r 3 33");
        }



        bool propertyflag;
        bool childrenflag;

        protected override async Task<bool> RefreshChildrenAsync()
        {
            await RefreshBranchesAsync();
            return await RefreshLeavesAsync();
        }

        protected override Task<bool> RefreshBranchesAsync()
        {
            if (childrenflag == false)
                childrenflag = true;
            else
                return Task.FromResult(false);
            Task.Run(() =>
            {
                try
                {
                    foreach (var directoryInfo in Directory.EnumerateDirectories(Content.FullName).Select(item => new DirectoryInfo(item)))
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

        protected override Task<bool> RefreshLeavesAsync()
        {
            if (propertyflag == false)
                propertyflag = true;
            else
                return Task.FromResult(false);

            Task.Run(() =>
            {
                try
                {
                    foreach (var fileInfo in Directory.EnumerateFiles(Content.FullName).Select(item => new FileInfo(item)))
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

        public override Task<object> GetChildren()
        {
            throw new NotImplementedException();
        }

        public void OnNext(object value)
        {

            if (value is DirectoryInfo directoryInfo)
                _branches.Add(directoryInfo);
            else if (value is FileInfo fileInfo)
                _leaves.Add(fileInfo);

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
    }
}
