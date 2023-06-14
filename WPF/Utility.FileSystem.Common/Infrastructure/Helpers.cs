using System;
using System.IO;

namespace Utility.FileSystem.Transfer.Common.Infrastructure
{
    public static class Helpers
    {
        private static readonly string[] SizeWindowsSuffixes = new string[9]
        {
      "bytes",
      "KB",
      "MB",
      "GB",
      "TB",
      "PB",
      "EB",
      "ZB",
      "YB"
        };
        private static readonly string[] SizeBinarySuffixes = new string[9]
        {
      "bytes",
      "KiB",
      "MiB",
      "GiB",
      "TiB",
      "PiB",
      "EiB",
      "ZiB",
      "YiB"
        };
        private static readonly string[] SizeMetricSuffixes = new string[9]
        {
      "bytes",
      "kB",
      "MB",
      "GB",
      "TB",
      "PB",
      "EB",
      "ZB",
      "YB"
        };

        internal static string ToSizeWithSuffix(long value, SuffixStyle style, int decimalPlaces = 1)
        {
            int num = 1024;
            if (style == SuffixStyle.Metric)
                num = 1000;
            if (decimalPlaces < 0)
                throw new ArgumentOutOfRangeException(nameof(decimalPlaces));
            if (value == 0L)
                return string.Format("{0:n" + decimalPlaces.ToString() + "} bytes", (object)0);
            int index = (int)Math.Log((double)value, (double)num);
            Decimal d = (Decimal)value / (Decimal)(1L << index * 10);
            if (style == SuffixStyle.Metric)
                d = (Decimal)value / (Decimal)Math.Pow((double)num, (double)index);
            if (Math.Round(d, decimalPlaces) >= 1000M)
            {
                ++index;
                d /= (Decimal)num;
            }
            return string.Format("{0:n2} {1}", (object)d, (object)Helpers.GetSuffixAtIndex(style, index));
        }

        private static string GetSuffixAtIndex(SuffixStyle style, int index)
        {
            switch (style)
            {
                case SuffixStyle.Windows:
                    return Helpers.SizeWindowsSuffixes[index];
                case SuffixStyle.Binary:
                    return Helpers.SizeBinarySuffixes[index];
                case SuffixStyle.Metric:
                    return Helpers.SizeMetricSuffixes[index];
                default:
                    return string.Empty;
            }
        }

        public static Helpers.DirectorySizeInfo Size(this DirectoryInfo d)
        {
            Helpers.DirectorySizeInfo directorySizeInfo = new Helpers.DirectorySizeInfo();
            try
            {
                FileInfo[] files = d.GetFiles();
                foreach (FileInfo fileInfo in files)
                    directorySizeInfo.Size += fileInfo.Length;
                directorySizeInfo.FileCount += (long)files.Length;
                DirectoryInfo[] directories = d.GetDirectories();
                directorySizeInfo.DirectoryCount += (long)directories.Length;
                foreach (DirectoryInfo d1 in directories)
                    directorySizeInfo += d1.Size();
            }
            catch (Exception ex)
            {
            }
            return directorySizeInfo;
        }

        public sealed class DirectorySizeInfo
        {
            public long DirectoryCount;
            public long FileCount;
            public long Size;

            public static Helpers.DirectorySizeInfo operator +(
              Helpers.DirectorySizeInfo s1,
              Helpers.DirectorySizeInfo s2)
            {
                return new Helpers.DirectorySizeInfo()
                {
                    DirectoryCount = s1.DirectoryCount + s2.DirectoryCount,
                    FileCount = s1.FileCount + s2.FileCount,
                    Size = s1.Size + s2.Size
                };
            }
        }
    }
}
