using System;
using System.Linq;
using System.Windows;

namespace Utility.WPF.Panels.Infrastructure
{
    public static class MeasureHelper
    {
        public static (Size, T)[] GetRowsColumns2<T>(Size availableSize, (double?, double?, T)[] weights, int widthCount, int heightCount)
        {
            var widthWeights = weights.Select(a => a.Item1);
            var heightWeights = weights.Select(a => a.Item2);

            var relativeWidthCount = widthWeights.Count(a => !a.HasValue);
            var relativeHeightCount = heightWeights.Count(a => !a.HasValue);

            var widthAbsoluteCount = widthWeights.Where(a => a.HasValue);

            var absoluteWidth = widthWeights.Where(a => a.HasValue).Sum(a => a.Value);
            var absoluteHeight = heightWeights.Where(a => a.HasValue).Sum(a => a.Value);

            var remainingWidthDivision = (availableSize.Width - absoluteWidth) / widthCount;
            var remainingHeightDivision = (availableSize.Height - absoluteHeight) / heightCount;

            var widthTotalSize = absoluteWidth + (double.IsInfinity(remainingWidthDivision) ? 0 : availableSize.Width - absoluteWidth);
            var heightTotalSize = absoluteHeight + (double.IsInfinity(remainingHeightDivision) ? 0 : availableSize.Height - absoluteHeight);

            return
            widthWeights.Select(a => a.HasValue ? a.Value : remainingWidthDivision).Zip(
            heightWeights.Select(a => a.HasValue ? a.Value : remainingHeightDivision), (a, b) => (a, b))
            .Zip(weights, (a, b) => (new Size(a.a, a.b), b.Item3)).ToArray();
        }

        public static (int rows, int columns) GetRowsColumns(Size availableSize, int count)
        {
            int division = 1;
            while (GetLowestRatio((int)(availableSize.Width / division), (int)(availableSize.Height / division)) is
                 (int r, int c) &&
                 r > 1 && c > 1)
            {
                division *= 2;
            }

            var (a, b) = GetLowestRatio((int)(availableSize.Width / division), (int)(availableSize.Height / division));

            (a, b) = Expand(count, a, b);

            return (a, b);

            static (int row, int column) Expand(int count, int sizeA, int sizeB)
            {
                IncreaseArraySize(count, ref sizeA, ref sizeB);
                var (max, _) = GetMaxMin(sizeA, sizeB);
                var (row2, column2) = GetRowColumn(count, max, max != sizeA);
                return (row2, column2);

                static void IncreaseArraySize(int count, ref int sizeA, ref int sizeB)
                {
                    while (sizeA * sizeB < count)
                    {
                        sizeB += 1;
                        sizeA += 1;
                    }
                }

                static (int row, int column) GetRowColumn(int count, int max, bool b)
                {
                    int column = 1, row = 0, maxRow = 0;

                    for (int i = 0; i < count; i++)
                    {
                        if (++row == max)
                        {
                            maxRow = Math.Max(maxRow, row);
                            row = 0;

                            if (i != count - 1)
                                column++;
                        }
                    }

                    return b ? (Math.Max(maxRow, row), column) : (column, Math.Max(maxRow, row));
                }
            }

            static (int one, int two) GetLowestRatio(int number1, int number2)
            {
                var min = Math.Min(number1, number2);
                var number1IsMin = min == number1;
                var max = number1IsMin ? number2 : number1;
                int tempMin = min;
                while (tempMin > 1 && tempMin <= min)
                {
                    var overMax = 1d * max / tempMin % 1;
                    var overMin = 1d * min / tempMin % 1;
                    if (overMax < double.Epsilon && overMin < double.Epsilon)
                    {
                        min /= tempMin;
                        max /= tempMin;
                    }
                    else
                        tempMin--;
                }
                return number1IsMin ? (min, max) : (max, min);
            }

            static (int max, int min) GetMaxMin(int number1, int number2)
            {
                var max = Math.Max(number1, number2);
                var min = max == number1 ? number2 : number1;
                return (max, min);
            }
        }
    }
}