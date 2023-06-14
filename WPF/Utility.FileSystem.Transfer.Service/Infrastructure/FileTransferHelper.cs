using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Utility.FileSystem.Transfer.Common;
using Utility.FileSystem.Transfer.Common.Infrastructure;
using Utility.FileSystem.Helper;

namespace Utility.FileSystem.Transfer.Service.Infrastructure
{
    public static class FileTransferHelper
    {
        public static bool MoveWithProgress(
          string source,
          string destination,
          Action<Progress> progress)
        {
            DateTime startTimestamp = DateTime.Now;
            NativeMethods.CopyProgressRoutine lpProgressRoutine = (size, transferred, streamSize, bytesTransferred, number, reason, file, destinationFile, data) =>
            {
                Progress progress1 = new Progress(startTimestamp, bytesTransferred, size);
                try
                {
                    progress(progress1);
                    return NativeMethods.CopyProgressResult.PROGRESS_CONTINUE;
                }
                catch (Exception ex)
                {
                    return NativeMethods.CopyProgressResult.PROGRESS_STOP;
                }
            };
            return NativeMethods.MoveFileWithProgress(source, destination, lpProgressRoutine, IntPtr.Zero, NativeMethods.MoveFileFlags.MOVE_FILE_REPLACE_EXISTSING | NativeMethods.MoveFileFlags.MOVE_FILE_COPY_ALLOWED | NativeMethods.MoveFileFlags.MOVE_FILE_WRITE_THROUGH);
        }

        public static Task<bool> MoveWithProgressAsync(
          string source,
          string destination,
          Action<Progress> progress)
        {
            return Task.Run(() =>
            {
                string destination1 = destination;
                bool? nullable = source.IsPathFile();
                bool flag = false;
                if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
                    destination1 = FileSystemHelper.CorrectFileDestinationPath(source, destination);
                return Utility.FileSystem.Transfer.Service.Infrastructure.FileTransferHelper.MoveWithProgress(source, destination1, progress);
            });
        }

        public static Task<OperationResult> CopyWithProgressAsync(
          string source,
          string destination,
          Action<Progress> progress,
          bool continueOnFailure,
          bool copyContentOfDirectory = false)
        {
            return FileTransferHelper.CopyWithProgressAsync(source, destination, progress, continueOnFailure, CancellationToken.None, copyContentOfDirectory);
        }

        public static Task<OperationResult> CopyWithProgressAsync(
          string source,
          string destination,
          Action<Progress> progress,
          bool continueOnFailure,
          CancellationToken cancellationToken,
          bool copyContentOfDirectory = false)
        {
            return Task.Run<OperationResult>((Func<OperationResult>)(() =>
            {
                try
                {
                    return FileTransferHelper.CopyWithProgress(source, destination, progress, continueOnFailure, cancellationToken, copyContentOfDirectory);
                }
                catch
                {
                    return OperationResult.Failed;
                }
            }), cancellationToken);
        }

        public static OperationResult CopyWithProgress(
          string source,
          string destination,
          Action<Progress> progress,
          bool continueOnFailure,
          bool copyContentOfDirectory = false)
        {
            return CopyWithProgress(source, destination, progress, continueOnFailure, CancellationToken.None, copyContentOfDirectory);
        }

        public static OperationResult CopyWithProgress(
          string source,
          string destination,
          Action<Progress> progress,
          bool continueOnFailure,
          CancellationToken cancellationToken,
          bool copyContentOfDirectory = false)
        {
            bool? nullable1 = source.IsPathFile();
            bool? nullable2 = nullable1 ?? throw new ArgumentException("Source parameter has to be file or directory! " + source);
            bool flag = true;
            if (nullable2.GetValueOrDefault() == flag & nullable2.HasValue)
                return FileTransferHelper.CopyDirectoryWithProgress(source, destination, progress, continueOnFailure, cancellationToken, copyContentOfDirectory);
            if (cancellationToken.IsCancellationRequested)
                return OperationResult.Cancelled;
            string newFile = FileSystemHelper.CorrectFileDestinationPath(source, destination);
            return CopyFileWithProgress(source, newFile, progress, cancellationToken);
        }

        private static OperationResult CopyDirectoryWithProgress(
          string sourceDirectory,
          string destinationDirectory,
          Action<Progress> progress,
          bool continueOnFailure,
          CancellationToken cancellationToken,
          bool copyContentOfDirectory)
        {
            DirectoryInfo d = new DirectoryInfo(sourceDirectory.TrimEnd('\\'));
            int length = d.FullName.Length;
            Helpers.DirectorySizeInfo rootSourceSize = d.Size();
            long totalTransferred = 0;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(destinationDirectory.TrimEnd('\\'));
                if (!copyContentOfDirectory)
                    directoryInfo = Directory.CreateDirectory(Path.Combine(destinationDirectory, d.Name));
                DateTime fileCopyStartTimestamp = DateTime.Now;
                bool flag = false;
                foreach (DirectoryInfo enumerateDirectory in d.EnumerateDirectories("*", SearchOption.AllDirectories))
                {
                    flag = true;
                    if (cancellationToken.IsCancellationRequested)
                        return OperationResult.Cancelled;
                    string path2 = enumerateDirectory.FullName.Substring(length + 1);
                    Directory.CreateDirectory(Path.Combine(directoryInfo.FullName, path2));
                }
                foreach (FileInfo enumerateFile in d.EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    flag = true;
                    if (cancellationToken.IsCancellationRequested)
                        return OperationResult.Cancelled;
                    string path2 = enumerateFile.FullName.Substring(length + 1);
                    OperationResult operationResult = FileTransferHelper.CopyFileWithProgress(enumerateFile.FullName, Path.Combine(directoryInfo.FullName, path2), (Action<Progress>)(partialProgress => progress(new Progress(fileCopyStartTimestamp, totalTransferred + partialProgress.BytesTransferred, rootSourceSize.Size))), cancellationToken);
                    if (operationResult == OperationResult.Failed && !continueOnFailure)
                        return OperationResult.Failed;
                    if (operationResult == OperationResult.Cancelled)
                        return OperationResult.Cancelled;
                    totalTransferred += enumerateFile.Length;
                }
                if (!flag)
                    progress(new Progress(fileCopyStartTimestamp, 1L, 1L));
            }
            catch (Exception ex)
            {
                return OperationResult.Failed;
            }
            return OperationResult.Success;
        }

        private static OperationResult CopyFileWithProgress(
          string sourceFile,
          string newFile,
          Action<Progress> progress,
          CancellationToken cancellationToken)
        {
            int pbCancel = 0;
            DateTime startTimestamp = DateTime.Now;
            NativeMethods.CopyProgressRoutine lpProgressRoutine = (NativeMethods.CopyProgressRoutine)((size, transferred, streamSize, bytesTransferred, number, reason, file, destinationFile, data) =>
            {
                Progress progress1 = new Progress(startTimestamp, bytesTransferred, size);
                try
                {
                    lock (progress)
                    {
                        progress(progress1);
                        return NativeMethods.CopyProgressResult.PROGRESS_CONTINUE;
                    }
                }
                catch (Exception ex)
                {
                    return NativeMethods.CopyProgressResult.PROGRESS_STOP;
                }
            });
            if (cancellationToken.IsCancellationRequested)
                return OperationResult.Cancelled;
            cancellationToken.Register((Action)(() => pbCancel = 1));
            bool flag = NativeMethods.CopyFileEx(sourceFile, newFile, lpProgressRoutine, IntPtr.Zero, ref pbCancel, NativeMethods.CopyFileFlags.COPY_FILE_RESTARTABLE);
            return cancellationToken.IsCancellationRequested ? OperationResult.Cancelled : (flag ? OperationResult.Success : OperationResult.Failed);
        }
    }
}
