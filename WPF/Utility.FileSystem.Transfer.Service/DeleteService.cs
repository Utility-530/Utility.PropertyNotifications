using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Utility.FileSystem.Transfer.Common;
using Utility.FileSystem.Helper;
using Utility.FileSystem.Transfer.Abstract;

namespace Utility.FileSystem.Transfer.Service
{
    public class DeleteService : ITransferer
    {
        private readonly Subject<Progress> progress = new Subject<Progress>();

        public IObservable<Progress> Transfer(params string[] args)
        {
            string source = args[0];
            Init();
            return (IObservable<Progress>)this.progress;

            static IEnumerable<Progress> DeleteProgress(IList<string> fileList)
            {
                DateTime dateTime = DateTime.Now;
                for (int i = 0; i < fileList.Count; ++i)
                {
                    try
                    {
                        bool? nullable = fileList[i].IsPathFile();
                        bool flag = false;
                        (nullable.GetValueOrDefault() == flag & nullable.HasValue ? (FileSystemInfo)new FileInfo(fileList[i]) : (FileSystemInfo)new DirectoryInfo(fileList[i])).Delete();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine((object)ex);
                    }
                    yield return new Progress(dateTime, (long)(i + 1), (long)fileList.Count);
                }
            }

            async void Init() => await Task.Run((Action)(() =>
            {
                bool? nullable1 = source.IsPathFile();
                bool flag1 = true;
                if (nullable1.GetValueOrDefault() == flag1 & nullable1.HasValue)
                {
                    foreach (Progress progress in DeleteProgress((IList<string>)((IEnumerable<string>)Directory.GetFiles(source, "*", SearchOption.AllDirectories)).Concat<string>((IEnumerable<string>)Directory.GetDirectories(source)).Concat<string>((IEnumerable<string>)new string[1]
                    {
            source
                    }).ToArray<string>()))
                        this.progress.OnNext(progress);
                }
                else
                {
                    bool? nullable2 = source.IsPathFile();
                    bool flag2 = false;
                    if (!(nullable2.GetValueOrDefault() == flag2 & nullable2.HasValue))
                        throw new IOException("Path does not exist on disk.");
                    DateTime now = DateTime.Now;
                    new FileInfo(source).Delete();
                    this.progress.OnNext(new Progress(now, 1L, 1L));
                }
            }));
        }
    }
}
