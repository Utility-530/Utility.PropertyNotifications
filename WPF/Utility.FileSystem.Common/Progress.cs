// Decompiled with JetBrains decompiler
// Type: Utility.FileSystem.Common.Progress
// Assembly: Utility.FileSystem.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9FE8AE5A-52EB-4945-A17D-1FFEB450396B
// Assembly location: D:\Repos\UtilityWpfCore\Utility.FileSystem\Utility.FileSystem.Transfer.WPF.Demo\bin\Debug\net5.0-windows\Utility.FileSystem.Common.dll

using System;
using Utility.FileSystem.Transfer.Common;
using Utility.FileSystem.Transfer.Common.Infrastructure;

namespace Utility.FileSystem.Transfer.Common
{
    public readonly struct Progress
    {
        public Progress(DateTime startedTimestamp, long bytesTransferred, long total)
            : this((double)bytesTransferred / DateTime.Now.Subtract(startedTimestamp).TotalSeconds, bytesTransferred, total)
        {
        }

        public Progress(double bytesPerSecond, long bytesTransferred, long total)
        {
            this.BytesTransferred = bytesTransferred;
            this.BytesPerSecond = bytesPerSecond;
            this.Total = total;
            this.Fraction = (double)this.BytesTransferred / (double)total;
            this.Percentage = 100.0 * this.Fraction;
        }

        public long Total { get; }

        public long BytesTransferred { get; }

        public double BytesPerSecond { get; }

        public double Fraction { get; }

        public double Percentage { get; }

        public string AsPercentage() => this.Fraction.ToString("00 %");

        public string GetBytesTransferredFormatted(SuffixStyle suffixStyle, int decimalPlaces) => Helpers.ToSizeWithSuffix(this.BytesTransferred, suffixStyle, decimalPlaces);

        public string DataPerSecondFormatted(SuffixStyle suffixStyle, int decimalPlaces) => Helpers.ToSizeWithSuffix((long)this.BytesPerSecond, suffixStyle, decimalPlaces) + "/sec";

        public override string ToString() => string.Format("Total: {0}, BytesTransferred: {1}, Percentage: {2}", (object)this.Total, (object)this.BytesTransferred, (object)this.Percentage);
    }
}