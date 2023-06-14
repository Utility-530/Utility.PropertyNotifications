using SevenZip;
using System;
using System.IO;
using System.IO.Compression;
using System.Reactive.Subjects;
using Utility.FileSystem.Transfer.Common;
using Utility.FileSystem.Transfer.Abstract;

namespace Utility.FileSystem.Transfer.Service
{
    public class ReactiveAsynCompress : ITransferer
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
                if (Directory.Exists(source) && Directory.GetParent(destination).Exists)
                {
                    SevenZipBase.SetLibraryPath(this.dll7z);
                    SevenZipCompressor sevenZipCompressor = new SevenZipCompressor(source);
                    DateTime start = new DateTime();
                    sevenZipCompressor.CompressionMode = SevenZip.CompressionMode.Create;
                    sevenZipCompressor.TempFolderPath = Path.GetTempPath();
                    sevenZipCompressor.ArchiveFormat = OutArchiveFormat.SevenZip;
                    sevenZipCompressor.Compressing += (a, b) => this.progress.OnNext(new Progress(start, (long)b.PercentDone, 100L));
                    sevenZipCompressor.BeginCompressDirectory(source, destination);
                }
                else
                    throw new Exception("Source does " + (Directory.Exists(source) ? string.Empty : "not") + " exist & destination's parent does " + (File.Exists(destination) ? string.Empty : "not") + " exist.");
            }
        }
    }
}
