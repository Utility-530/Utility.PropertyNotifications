using System;
using System.IO;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Markup;
using SevenZip;
using Utility.FileSystem.Transfer.Abstract;
using Utility.FileSystem.Transfer.Common;
using Utility.FileSystem.Transfer.Service;

namespace Utility.FileSystem.Transfer.Demo
{
    public partial class MainWindow : Window
    {
        private readonly string dll7z = AppDomain.CurrentDomain.BaseDirectory + "7z.dll";
        private readonly Subject<bool> subject = new Subject<bool>();
        private const string source = "../../../Data/Source/";
        private const string destination = "../../../Data/Destination/";
   

        public MainWindow()
        {

            this.InitializeComponent();



         this.FileProgressView1.Source = "../../../Data/Source/huge_dummy_file";
            this.FileProgressView1.SourceType = PathType.File;
            this.FileProgressView1.DestinationType = PathType.File;
            this.FileProgressView1.Destination = "../../../Data/Destination/";
            this.FileProgressView1.Transferer = (ITransferer) new CopyService();
            this.FileProgressView3.Source = "../../../Data/Source/huge_dummy_file.7z";
            this.FileProgressView3.SourceType = PathType.File;
            this.FileProgressView3.Destination = "../../../Data/Destination/Destination";
            this.FileProgressView3.Transferer = (ITransferer) new ExtractService();
            this.ProgressView1.Transferer = (ITransferer) new DummyService();
            this.FileProgressView4.Transferer = (ITransferer) new ReactiveAsynCompress();
            this.FileProgressView4.DestinationType = PathType.File;
            this.FileProgressView4.Source = "../../../Data/Source/";
            this.FileProgressView4.Destination = "../../../Data/Destination/";
            this.FileProgressView5.Transferer = (ITransferer) new DeleteService();
            this.FileProgressView5.PathType = PathType.File;
            this.FileProgressView5.Path = "../../../Data/Source/";
            Directory.CreateDirectory("../../../Data/Source/");
            Directory.CreateDirectory("../../../Data/Destination/");
            this.CreateDummyFile();
            this.CreateDummyZipFile();
        }

        private void CreateDummyZipFile()
        {
            if (File.Exists("../../../Data/Source/huge_dummy_file.7z"))
                return;
            SevenZipBase.SetLibraryPath(this.dll7z);
            new SevenZipCompressor()
            {
                CompressionMode = CompressionMode.Create,
                TempFolderPath = Path.GetTempPath(),
                ArchiveFormat = OutArchiveFormat.SevenZip
            }.CompressDirectory("../../../Data/Source/", "../../../Data/Source/huge_dummy_file.7z");
        }

        private void CreateDummyFile()
        {
            if (File.Exists("../../../Data/Source/huge_dummy_file"))
                return;
            Directory.CreateDirectory("../../../Data/Source/");
            FileStream fileStream = new FileStream("../../../Data/Source/huge_dummy_file", FileMode.CreateNew);
            fileStream.Seek(524288000L, SeekOrigin.Begin);
            fileStream.WriteByte((byte) 0);
            fileStream.Close();
        }

        private void ButtonBase2_OnClick(object sender, RoutedEventArgs e) => this.subject.OnNext(true);

        private void Show_Default_OnClick(object sender, RoutedEventArgs e) =>
            this.MultiStage.Content = (object) new MultiStageProgressView();
    }
}