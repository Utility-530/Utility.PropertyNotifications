// Decompiled with JetBrains decompiler
// Type: Utility.FileSystem.Transfer.WPF.Demo.MultiStageProgressView
// Assembly: Utility.FileSystem.Transfer.WPF.Demo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A9D04148-7007-4226-945F-7C416DD71EED
// Assembly location: D:\Repos\UtilityWpfCore\Utility.FileSystem\Utility.FileSystem.Transfer.WPF.Demo\bin\Debug\net5.0-windows\Utility.FileSystem.Transfer.WPF.Demo.dll

using System.IO;
using System.Windows.Controls;

namespace Utility.FileSystem.Transfer.Demo
{
    public partial class MultiStageProgressView : UserControl
    {
        public MultiStageProgressView()
        {
            this.InitializeComponent();
            Directory.CreateDirectory("Destination");
        }
    }
}
