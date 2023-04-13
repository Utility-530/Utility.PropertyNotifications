using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Utility.WPF.Panels.Infrastructure
{
    public class HorizontalTableLayoutStrategy : ILayoutStrategy
    {
        private int rowCount;
        private double[] rowHeights;
        private readonly List<double> columnWidths = new List<double>();
        private int _elementCount;

        public Size ResultSize
        {
            get { return rowHeights != null && columnWidths.Any() ? new Size(columnWidths.Sum(), rowHeights.Sum()) : new Size(0, 0); }
        }

        public void Calculate(Size availableSize, Size[] measures)
        {
            BaseCalculation(availableSize, measures);
            AdjustEmptySpace(availableSize);

            void BaseCalculation(Size availableSize, Size[] measures)
            {
                _elementCount = measures.Length;
                rowCount = GetRowCount(availableSize, measures);
                if (rowHeights == null || rowHeights.Length < rowCount)
                    rowHeights = new double[rowCount];
                var calculating = true;
                while (calculating)
                {
                    calculating = false;
                    ResetSizes();
                    int col;
                    for (col = 0; col * rowCount < measures.Length; col++)
                    {
                        var columnWidth = 0.0;
                        int row;
                        for (row = 0; row < rowCount; row++)
                        {
                            int i = col * rowCount + row;
                            if (i >= measures.Length) break;
                            rowHeights[row] = Math.Max(rowHeights[row], measures[i].Height);
                            columnWidth = Math.Max(columnWidth, measures[i].Width);
                        }

                        if (rowCount > 1 && rowHeights.Sum() > availableSize.Height)
                        {
                            rowCount--;
                            calculating = true;
                            break;
                        }
                        columnWidths.Add(columnWidth);
                    }
                }
            }

            void AdjustEmptySpace(Size availableSize)
            {
                var height = rowHeights.Sum();
                if (!double.IsNaN(availableSize.Height) && 
                    !double.IsInfinity(availableSize.Height) && availableSize.Height > height)
                {
                    var dif = (availableSize.Height - height) / rowCount;

                    for (var i = 0; i < rowCount; i++)
                    {
                        rowHeights[i] += dif;
                    }
                }
            }

            void ResetSizes()
            {
                columnWidths.Clear();
                for (var j = 0; j < rowHeights.Length; j++)
                {
                    rowHeights[j] = 0;
                }
            }

            static int GetRowCount(Size availableSize, Size[] measures)
            {
                double height = 0;
                for (int rowCnt = 0; rowCnt < measures.Length; rowCnt++)
                {
                    var nHeight = height + measures[rowCnt].Height;
                    if (nHeight > availableSize.Height)
                        return Math.Max(1, rowCnt);
                    height = nHeight;
                }
                return measures.Length;
            }
        }

        public Rect GetPosition(int index)
        {
            var columnIndex = index / rowCount;
            var rowIndex = index % rowCount;
            var x = 0d;
            for (int i = 0; i < columnIndex; i++)
            {
                x += columnWidths[i];
            }
            var y = 0d;
            for (int i = 0; i < rowIndex; i++)
            {
                y += rowHeights[i];
            }
            return new Rect(new Point(x, y), new Size(columnWidths[columnIndex], rowHeights[rowIndex]));
        }

        public int GetIndex(Point position)
        {
            var _row = 0;
            var y = 0d;
            while (y < position.Y && rowCount > _row)
            {
                y += rowHeights[_row];
                _row++;
            }
            _row--;
            var col = 0;
            var x = 0d;
            while (x < position.X && columnWidths.Count > col)
            {
                x += columnWidths[col];
                col++;
            }
            col--;
            if (col < 0) col = 0;
            if (_row < 0) _row = 0;
            if (_row >= rowCount) _row = rowCount - 1;
            var result = col * rowCount + _row;
            if (result > _elementCount) result = _elementCount - 1;
            return result;
        }
    }
}