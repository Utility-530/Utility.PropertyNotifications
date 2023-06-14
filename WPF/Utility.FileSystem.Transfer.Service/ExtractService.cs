using SevenZip;
using System;
using System.IO;
using System.Reactive.Subjects;
using Utility.FileSystem.Transfer.Common;
using Utility.FileSystem.Transfer.Abstract;

namespace Utility.FileSystem.Transfer.Service
{
    public class ExtractService : ITransferer
    {
        private readonly Subject<Progress> progress = new Subject<Progress>();
        private readonly string dll7z = AppDomain.CurrentDomain.BaseDirectory + "7z.dll";

        public IObservable<Progress> Transfer(params string[] args)
        {
            string source = args[0];
            string destination = args[1];
            Init();
            return (IObservable<Progress>)this.progress;

            void Init()
            {
                if (File.Exists(source) && Directory.GetParent(destination).Exists)
                {
                    Directory.CreateDirectory(destination);
                    SevenZipBase.SetLibraryPath(this.dll7z);
                    SevenZipExtractor sevenZipExtractor = new SevenZipExtractor(source);
                    int max = sevenZipExtractor.ArchiveFileData.Count;
                    DateTime start = new DateTime();
                    int current = 0;
                    sevenZipExtractor.FileExtractionStarted += (EventHandler<FileInfoEventArgs>)((a, b) =>
                    {
                        ++current;
                        this.progress.OnNext(new Progress(start, (long)current, (long)max));
                    });
                    sevenZipExtractor.BeginExtractArchive(destination);
                }
                else
                    throw new Exception("Source does " + (File.Exists(source) ? string.Empty : "not") + " exist & destination parent does " + (Directory.GetParent(destination).Exists ? string.Empty : "not") + " exist.");
            }
        }
    }
}
