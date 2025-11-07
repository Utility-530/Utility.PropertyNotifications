using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Demo.Panels
{
    internal class EdgeLegacyPanelHelper
    {
        public static dsf MeasureTwo(IEnumerable<UIElement> children, Size availableSize)
        {
            return Measure(children, availableSize,
                 (child, widthSizing) => child.DesiredSize.Width == 0 || widthSizing == Sizing.FromParent,
               (child, heightSizing) => child.DesiredSize.Height == 0 || heightSizing == Sizing.FromParent);
        }

        public static dsf Measure(IEnumerable<UIElement> children, Size availableSize, Func<UIElement, Sizing, bool> includeWidth, Func<UIElement, Sizing, bool> includeHeight)
        {
            List<UIElement> leftWidth = new List<UIElement>(), centerWidth = new List<UIElement>(), rightWidth = new List<UIElement>();
            List<UIElement> topHeight = new List<UIElement>(), middleHeight = new List<UIElement>(), bottomHeight = new List<UIElement>();

            List<UIElement> left2Width = new List<UIElement>(), center2Width = new List<UIElement>(), right2Width = new List<UIElement>();
            List<UIElement> top2Height = new List<UIElement>(), middle2Height = new List<UIElement>(), bottom2Height = new List<UIElement>();

            int leftCount = 0, centerCount = 0, rightCount = 0;
            int topCount = 0, middleCount = 0, bottomCount = 0;

            int center2Count = 0, middle2Count = 0;

            double bottomleftWidth = 0, bottomrightWidth = 0;
            double topleftWidth = 0, toprightWidth = 0;

            double bottomleftHeight = 0, bottomrightHeight = 0;
            double topleftHeight = 0, toprightHeight = 0;

            int bottomleftWidthCount = 0, bottomrightWidthCount = 0;
            int topleftWidthCount = 0, toprightWidthCount = 0;

            int bottomleftHeightCount = 0, bottomrightHeightCount = 0;
            int topleftHeightCount = 0, toprightHeightCount = 0;

            foreach (UIElement child in children)
            {
                var region = EdgeLegacyPanel.GetCircleRegion(child);
                var heightSizing = EdgeLegacyPanel.GetHeightSizing(child);
                var widthSizing = EdgeLegacyPanel.GetWidthSizing(child);

                //var aSize = asizes[region] = asizes.GetValueOrDefault(region, new A());

                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                int widthCount = includeWidth(child, widthSizing) ? 1 : 0;
                double desiredWidth = includeWidth(child, widthSizing) ? 0 : child.DesiredSize.Width;
                int heightCount = includeHeight(child, widthSizing) ? 1 : 0;
                double desiredHeight = includeHeight(child, heightSizing) ? 0 : child.DesiredSize.Height;

                switch (region)
                {
                    case CircleRegion.TopLeft:
                        topleftWidth += desiredWidth;
                        topleftWidthCount += widthCount;
                        topleftHeightCount += heightCount;
                        topleftHeight += desiredHeight;
                        break;

                    case CircleRegion.BottomLeft:
                        bottomleftWidth += desiredWidth;
                        bottomleftWidthCount += widthCount;
                        bottomleftHeightCount += heightCount;
                        bottomleftHeight += desiredHeight;
                        break;

                    case CircleRegion.BottomRight:
                        bottomrightWidth += desiredWidth;
                        bottomrightWidthCount += widthCount;
                        bottomrightHeightCount += heightCount;
                        bottomrightHeight += desiredHeight;
                        break;

                    case CircleRegion.TopRight:
                        toprightWidth += desiredWidth;
                        toprightWidthCount += widthCount;
                        toprightHeightCount += heightCount;
                        toprightHeight += desiredHeight;
                        break;

                    case CircleRegion.Left:
                        if (desiredWidth > 0) leftWidth.Add(child);
                        leftCount += widthCount;
                        if (desiredHeight > 0) middle2Height.Add(child);
                        middle2Count = 1;
                        break;

                    case CircleRegion.Right:
                        if (desiredWidth > 0) rightWidth.Add(child);
                        rightCount += widthCount;
                        if (desiredHeight > 0) middle2Height.Add(child);
                        middle2Count = 1;
                        break;

                    case CircleRegion.Top:
                        center2Count = 1;
                        if (desiredWidth > 0) center2Width.Add(child);
                        topCount += heightCount;
                        if (desiredHeight > 0) topHeight.Add(child);
                        break;

                    case CircleRegion.Bottom:
                        center2Count = 1;
                        if (desiredWidth > 0) center2Width.Add(child);
                        bottomCount += heightCount;
                        if (desiredHeight > 0) bottomHeight.Add(child);
                        break;

                    case CircleRegion.Middle:
                        if (desiredWidth > 0) centerWidth.Add(child);
                        centerCount += widthCount;
                        if (desiredHeight > 0) middleHeight.Add(child);
                        middleCount += heightCount;
                        break;
                }
                ;
            }

            bottomCount = Math.Max(bottomCount, Math.Max(SizerUtility.GetCount(bottomleftHeightCount), SizerUtility.GetCount(bottomrightHeightCount)));
            topCount = Math.Max(topCount, Math.Max(SizerUtility.GetCount(topleftHeightCount), SizerUtility.GetCount(toprightHeightCount)));
            middleCount = Math.Max(middle2Count, middleCount);

            leftCount = Math.Max(leftCount, Math.Max(SizerUtility.GetCount(bottomleftWidthCount), SizerUtility.GetCount(bottomrightWidthCount)));
            rightCount = Math.Max(rightCount, Math.Max(SizerUtility.GetCount(bottomleftHeightCount), SizerUtility.GetCount(bottomrightHeightCount)));
            centerCount = Math.Max(center2Count, centerCount);

            bottomHeight = Enumerable.Range(0, bottomCount).Select(a => (UIElement)new Control { Height = availableSize.Height / (bottomCount + topCount + middleCount) }).ToList();
            topHeight = Enumerable.Range(0, topCount).Select(a => (UIElement)new Control { Height = availableSize.Height / (bottomCount + topCount + middleCount) }).ToList();
            middleHeight = Enumerable.Range(0, middleCount).Select(a => (UIElement)new Control { Height = availableSize.Height / (bottomCount + topCount + middleCount) }).ToList();

            leftWidth = Enumerable.Range(0, leftCount).Select(a => (UIElement)new Control { Height = availableSize.Width / (leftCount + rightCount + centerCount) }).ToList();
            rightWidth = Enumerable.Range(0, rightCount).Select(a => (UIElement)new Control { Height = availableSize.Width / (leftCount + rightCount + centerCount) }).ToList();
            centerWidth = Enumerable.Range(0, centerCount).Select(a => (UIElement)new Control { Height = availableSize.Width / (leftCount + rightCount + centerCount) }).ToList();

            var (w, h) = Helper.GetLowestRatio((int)availableSize.Width, (int)availableSize.Height);

            var newMethod1 = NewMethod(availableSize.Height, GetHeight, new[] { bottomHeight, middleHeight, topHeight }).ToArray();

            var wrappedHeight = newMethod1
                                    .SelectMany(a => a.Select((c, i) => (c, i)))
                                    .GroupBy(a => a.i)
                                    .Select(a => a.Max(c => c.c.Sum(GetHeight))).ToOptionalArray();

            var grouped = newMethod1
                           .SelectMany(a => a.Select((c, i) => (c, i)))
                           .GroupBy(a => a.i).ToArray();

            var wrappedWidth = grouped
                           .Select(a => a.Where(a => a.c.Any()).Sum(c => c.c.Max(GetWidth))).ToArray();

            var offsetCount = grouped.Select(a => a.Count(c => c.c.Any())).ToOptionalArray();

            var newMethod2 = NewMethod(availableSize.Width, GetWidth, new[] { leftWidth, centerWidth, rightWidth }).ToArray();

            var wrappedWidth2 = newMethod2
                                    .SelectMany(a => a.Select((c, i) => (c, i)))
                                    .GroupBy(a => a.i)
                                    .Select(a => a.Max(c => c.c.Sum(GetWidth))).ToOptionalArray();

            var grouped2 = newMethod2
                           .SelectMany(a => a.Select((c, i) => (c, i)))
                           .GroupBy(a => a.i).ToArray();

            var wrappedHeight2 = grouped2
                           .Select(a => a.Where(a => a.c.Any()).Sum(c => c.c.Max(GetHeight))).ToArray();

            var offsetCount2 = grouped2.Select(a => a.Count(c => c.c.Any())).ToOptionalArray();

            var newMethod3 = NewMethod(availableSize.Height, GetHeight, new[] {
                      bottom2Height.OrderByDescending(GetHeight).Take(1).ToArray() ,
                    middle2Height.OrderByDescending(GetHeight).Take(1).ToArray() ,
                    top2Height.OrderByDescending(GetHeight).Take(1).ToArray()  }).ToArray();

            var wrappedHeight3 = newMethod3
                                    .SelectMany(a => a.Select((c, i) => (c, i)))
                                    .GroupBy(a => a.i)
                                    .Select(a => a.Max(c => c.c.Sum(GetHeight))).ToOptionalArray();

            var grouped3 = newMethod3
                           .SelectMany(a => a.Select((c, i) => (c, i)))
                           .GroupBy(a => a.i).ToArray();

            var wrappedWidth3 = grouped3
                           .Select(a => a.Where(a => a.c.Any()).Sum(c => c.c.Max(GetWidth))).ToArray();

            var offsetCount3 = grouped3.Select(a => a.Count(c => c.c.Any())).ToOptionalArray();

            var newMethod4 = NewMethod(availableSize.Width, GetWidth, new[] {
                    left2Width.OrderByDescending(GetWidth).Take(1).ToArray(),
                    center2Width.OrderByDescending(GetWidth).Take(1).ToArray(),
                    right2Width.OrderByDescending(GetWidth).Take(1).ToArray()
                }).ToArray();

            var wrappedWidth4 = newMethod4
                                    .SelectMany(a => a.Select((c, i) => (c, i)))
                                    .GroupBy(a => a.i)
                                    .Select(a => a.Max(c => c.c.Sum(GetWidth))).ToOptionalArray();

            var grouped4 = newMethod4
                           .SelectMany(a => a.Select((c, i) => (c, i)))
                           .GroupBy(a => a.i).ToArray();

            var wrappedHeight4 = grouped4
                           .Select(a => a.Where(a => a.c.Any()).Sum(c => c.c.Max(GetHeight))).ToArray();

            var offsetCount4 = grouped4.Select(a => a.Count(c => c.c.Any())).ToOptionalArray();

            var se = new dsf
            {
                actualLeftOffset = Common.Max(offsetCount4[0], offsetCount2[0]).GetValueOrDefault(0),
                actualCenterOffset = Common.Max(offsetCount4[1], offsetCount2[1]).GetValueOrDefault(0),
                actualRightOffset = Common.Max(offsetCount4[2], offsetCount2[2]).GetValueOrDefault(0),

                actualBottomOffset = Common.Max(offsetCount3[0], offsetCount[0]).GetValueOrDefault(0),
                actualMiddleOffset = Common.Max(offsetCount3[1], offsetCount[1]).GetValueOrDefault(0),
                actualTopOffset = Common.Max(offsetCount3[2], offsetCount[2]).GetValueOrDefault(0),

                actualLeftWidth = Common.Max(wrappedWidth4[0], wrappedWidth2[0]).GetValueOrDefault(0),
                actualCenterWidth = Common.Max(wrappedWidth4[1], wrappedWidth2[1]).GetValueOrDefault(0),
                actualRightWidth = Common.Max(wrappedWidth4[2], wrappedWidth2[2]).GetValueOrDefault(0),

                actualBottomHeight = Common.Max(wrappedHeight3[0], wrappedHeight[0]).GetValueOrDefault(0),
                actualMiddleHeight = Common.Max(wrappedHeight3[1], wrappedHeight[1]).GetValueOrDefault(0),
                actualTopHeight = Common.Max(wrappedHeight3[2], wrappedHeight[2]).GetValueOrDefault(0),
            };
            se.RemainingSize = new Size(
             availableSize.Width - se.actualLeftWidth - se.actualCenterWidth - se.actualRightWidth,
             availableSize.Height - se.actualBottomHeight - se.actualMiddleHeight - se.actualTopHeight);

            return se;
            //if (middleCount > 0)
            //{
            //var ax = remainingHeight * HeightRatio;
            //remainingHeight -= ax;
            //middleRequiredHeight = ax / middleCount;
            //}
            //else
            //    middleRequiredHeight= 0;

            //if (centerCount > 0)
            //{
            //var ax2 = remainingWidth * WidthRatio;
            //remainingWidth -= ax2;
            //centerRequiredWidth = ax2 / centerCount;
            //}
            //else
            //    centerRequiredWidth = 0;

            double individualWidth = 0;//= remainingWidth / ((leftCount > 0 ? 1 : 0) + (rightCount > 0 ? 1 : 0) + (center2Count > 0 ? 1 : 0));
            double individualHeight = 0;//= remainingHeight / ((topCount > 0 ? 1 : 0) + (bottomCount > 0 ? 1 : 0) + (middle2Count > 0 ? 1 : 0));

            //if (leftCount > 0)
            //{
            //    remainingWidth -= individualWidth;
            //    leftRequiredWidth = individualWidth / leftCount;
            //    leftRegionWidth = individualWidth;
            //}
            //if (rightCount > 0)
            //{
            //    remainingWidth -= individualWidth;
            //    rightRequiredWidth = individualWidth / rightCount;
            //    rightRegionWidth = individualWidth;
            //}
            //if (center2Count > 0)
            //{
            //    remainingWidth -= individualWidth;
            //    centerRequiredWidth = individualWidth / center2Count;
            //    centerRegionWidth = individualWidth;
            //}
            //if (topCount > 0)
            //{
            //    remainingHeight -= individualHeight;
            //    topRequiredHeight = individualHeight / topCount;
            //    topRegionHeight = individualHeight;
            //}
            //if (bottomCount > 0)
            //{
            //    remainingHeight -= individualHeight;
            //    bottomRequiredHeight = individualHeight / bottomCount;
            //    bottomRegionHeight = individualHeight;
            //}
            //if (middle2Count > 0)
            //{
            //    remainingHeight -= individualHeight;
            //    middleRequiredHeight = individualHeight / middle2Count;
            //    middleRegionHeight = individualHeight;
            //}

            //double finalWidth;
            //if (individualWidth != double.PositiveInfinity)
            //    finalWidth = availableSize.Width;
            //else
            //    finalWidth = availableSize.Width - se.RemainingSize.Width;

            //double finalHeight;
            //if (individualHeight != double.PositiveInfinity)
            //    finalHeight = availableSize.Height;
            //else
            //    finalHeight = availableSize.Height - se.RemainingSize.Height;

            //return new Size(finalWidth, finalHeight);
        }

        public static Size GetDesiredSize(UIElement element, Size childConstraint)
        {
            // Measure child.
            element.Measure(childConstraint);
            return element.DesiredSize;
        }

        public class dsf
        {
            public int actualLeftOffset { get; set; }
            public int actualCenterOffset { get; set; }
            public int actualRightOffset { get; set; }

            public int actualBottomOffset { get; set; }
            public int actualMiddleOffset { get; set; }
            public int actualTopOffset { get; set; }

            public double actualLeftWidth { get; set; }
            public double actualCenterWidth { get; set; }
            public double actualRightWidth { get; set; }

            public double actualBottomHeight { get; set; }
            public double actualMiddleHeight { get; set; }
            public double actualTopHeight { get; set; }

            public Size RemainingSize { get; set; }
        }

        public static double GetWidth(UIElement element) => element.DesiredSize.Width;

        public static double GetHeight(UIElement element) => element.DesiredSize.Height;

        public static IEnumerable<IList<IList<T>>> NewMethod<T>(double size, Func<T, double> func, IList<IList<T>> values)
        {
            var orderedwidths = values.Select((a, i) => (a, i)).OrderByDescending(a => a.a.Sum(func)).Where(a => a.a.Count > 0).ToArray();
            var enumr = orderedwidths.GetEnumerator();

            List<List<T>> nextLevelWidths = new List<List<T>>();

            while (orderedwidths.Any(a => a.a.Count > 1) && size - values.Sum(a => a.Sum(func)) < 0)
            {
                if (!enumr.MoveNext())
                {
                    enumr.Reset();
                    enumr.MoveNext();
                }

                (IList<T> list, int i) = ((IList<T>, int))enumr.Current;

                if (list.Count <= 1)
                    continue;

                var last = list[list.Count - 1];
                list.Remove(last);
                while (nextLevelWidths.Count <= i)
                    nextLevelWidths.Add(new List<T>());

                nextLevelWidths[i].Add(last);
            }

            if (values.Any(a => a.Any()))
                yield return values.ToList();

            if (nextLevelWidths.Any())
            {
                nextLevelWidths.ForEach(a => a.Reverse());

                foreach (var m in NewMethod(size, func, nextLevelWidths.ToArray()))
                    yield return m;
            }
        }

        public static Size Measure2(IEnumerable<UIElement> children, Size availableSize, Func<UIElement, Sizing, bool> includeWidth, Func<UIElement, Sizing, bool> includeHeight)
        {
            List<UIElement> leftWidth = new List<UIElement>(), centerWidth = new List<UIElement>(), rightWidth = new List<UIElement>();
            List<UIElement> topHeight = new List<UIElement>(), middleHeight = new List<UIElement>(), bottomHeight = new List<UIElement>();

            List<UIElement> left2Width = new List<UIElement>(), center2Width = new List<UIElement>(), right2Width = new List<UIElement>();
            List<UIElement> top2Height = new List<UIElement>(), middle2Height = new List<UIElement>(), bottom2Height = new List<UIElement>();

            double leftCount = 0, centerCount = 0, rightCount = 0;
            double topCount = 0, middleCount = 0, bottomCount = 0;

            double center2Count = 0, middle2Count = 0;

            int bottomleftWidthCount = 0, bottomrightWidthCount = 0;
            int topleftWidthCount = 0, toprightWidthCount = 0;

            int bottomleftHeightCount = 0, bottomrightHeightCount = 0;
            int topleftHeightCount = 0, toprightHeightCount = 0;

            foreach (UIElement child in children)
            {
                var region = EdgeLegacyPanel.GetCircleRegion(child);
                var heightSizing = EdgeLegacyPanel.GetHeightSizing(child);
                var widthSizing = EdgeLegacyPanel.GetWidthSizing(child);

                //var aSize = asizes[region] = asizes.GetValueOrDefault(region, new A());

                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                int widthCount = includeWidth(child, widthSizing) ? 1 : 0;
                int heightCount = includeHeight(child, widthSizing) ? 1 : 0;

                switch (region)
                {
                    case CircleRegion.TopLeft:

                        topleftWidthCount += widthCount;
                        topleftHeightCount += heightCount;

                        break;

                    case CircleRegion.BottomLeft:

                        bottomleftWidthCount += widthCount;
                        bottomleftHeightCount += heightCount;

                        break;

                    case CircleRegion.BottomRight:

                        bottomrightWidthCount += widthCount;
                        bottomrightHeightCount += heightCount;

                        break;

                    case CircleRegion.TopRight:

                        toprightWidthCount += widthCount;
                        toprightHeightCount += heightCount;

                        break;

                    case CircleRegion.Left:

                        leftCount += widthCount;

                        middle2Count = Math.Max(1, heightCount);
                        break;

                    case CircleRegion.Right:

                        rightCount += widthCount;

                        middle2Count = Math.Max(1, heightCount);
                        break;

                    case CircleRegion.Top:
                        center2Count = Math.Max(1, widthCount);

                        topCount += heightCount;
                        break;

                    case CircleRegion.Bottom:
                        center2Count = Math.Max(1, widthCount);
                        bottomCount += heightCount;
                        break;

                    case CircleRegion.Middle:
                        centerCount += widthCount;
                        middleCount += heightCount;
                        break;
                }
                ;
            }

            bottomCount = Math.Max(bottomCount, Math.Max(SizerUtility.GetCount(bottomleftHeightCount), SizerUtility.GetCount(bottomrightHeightCount)));
            topCount = Math.Max(topCount, Math.Max(SizerUtility.GetCount(topleftHeightCount), SizerUtility.GetCount(toprightHeightCount)));
            leftCount = Math.Max(leftCount, Math.Max(SizerUtility.GetCount(bottomleftWidthCount), SizerUtility.GetCount(bottomrightWidthCount)));
            rightCount = Math.Max(rightCount, Math.Max(SizerUtility.GetCount(bottomleftHeightCount), SizerUtility.GetCount(bottomrightHeightCount)));

            //var remainingWidth = availableSize.Width - leftWidth.Sum() - centerWidth.Sum() - rightWidth.Sum();
            //var remainingHeight = availableSize.Height - topHeight.Sum() - middleHeight.Sum() - bottomHeight.Sum();

            var newMethod1 = NewMethod(availableSize.Height, GetHeight, new[] { bottomHeight, middleHeight, topHeight }).ToArray();

            var wrappedHeight = newMethod1
                                    .SelectMany(a => a.Select((c, i) => (c, i)))
                                    .GroupBy(a => a.i)
                                    .Select(a => a.Max(c => c.c.Sum(GetHeight))).ToOptionalArray();

            var grouped = newMethod1
                           .SelectMany(a => a.Select((c, i) => (c, i)))
                           .GroupBy(a => a.i).ToArray();

            var wrappedWidth = grouped
                           .Select(a => a.Where(a => a.c.Any()).Sum(c => c.c.Max(GetWidth))).ToArray();

            var offsetCount = grouped.Select(a => a.Count(c => c.c.Any())).ToOptionalArray();

            var newMethod2 = NewMethod(availableSize.Width, GetWidth, new[] { leftWidth, centerWidth, rightWidth }).ToArray();

            var wrappedWidth2 = newMethod2
                                    .SelectMany(a => a.Select((c, i) => (c, i)))
                                    .GroupBy(a => a.i)
                                    .Select(a => a.Max(c => c.c.Sum(GetWidth))).ToOptionalArray();

            var grouped2 = newMethod2
                           .SelectMany(a => a.Select((c, i) => (c, i)))
                           .GroupBy(a => a.i).ToArray();

            var wrappedHeight2 = grouped2
                           .Select(a => a.Where(a => a.c.Any()).Sum(c => c.c.Max(GetHeight))).ToArray();

            var offsetCount2 = grouped2.Select(a => a.Count(c => c.c.Any())).ToOptionalArray();

            var newMethod3 = NewMethod(availableSize.Height, GetHeight, new[] {
                      bottom2Height.OrderByDescending(GetHeight).Take(1).ToArray() ,
                    middle2Height.OrderByDescending(GetHeight).Take(1).ToArray() ,
                    top2Height.OrderByDescending(GetHeight).Take(1).ToArray()  }).ToArray();

            var wrappedHeight3 = newMethod3
                                    .SelectMany(a => a.Select((c, i) => (c, i)))
                                    .GroupBy(a => a.i)
                                    .Select(a => a.Max(c => c.c.Sum(GetHeight))).ToOptionalArray();

            var grouped3 = newMethod3
                           .SelectMany(a => a.Select((c, i) => (c, i)))
                           .GroupBy(a => a.i).ToArray();

            var wrappedWidth3 = grouped3
                           .Select(a => a.Where(a => a.c.Any()).Sum(c => c.c.Max(GetWidth))).ToArray();

            var offsetCount3 = grouped3.Select(a => a.Count(c => c.c.Any())).ToOptionalArray();

            var newMethod4 = NewMethod(availableSize.Width, GetWidth, new[] {
                    left2Width.OrderByDescending(GetWidth).Take(1).ToArray(),
                    center2Width.OrderByDescending(GetWidth).Take(1).ToArray(),
                    right2Width.OrderByDescending(GetWidth).Take(1).ToArray()
                }).ToArray();

            var wrappedWidth4 = newMethod4
                                    .SelectMany(a => a.Select((c, i) => (c, i)))
                                    .GroupBy(a => a.i)
                                    .Select(a => a.Max(c => c.c.Sum(GetWidth))).ToOptionalArray();

            var grouped4 = newMethod4
                           .SelectMany(a => a.Select((c, i) => (c, i)))
                           .GroupBy(a => a.i).ToArray();

            var wrappedHeight4 = grouped4
                           .Select(a => a.Where(a => a.c.Any()).Sum(c => c.c.Max(GetHeight))).ToArray();

            var offsetCount4 = grouped4.Select(a => a.Count(c => c.c.Any())).ToOptionalArray();

            //actualLeftOffset = Common.Max(offsetCount4[0], offsetCount2[0]).GetValueOrDefault(0);
            //actualCenterOffset = Common.Max(offsetCount4[1], offsetCount2[1]).GetValueOrDefault(0);
            //actualRightOffset = Common.Max(offsetCount4[2], offsetCount2[2]).GetValueOrDefault(0);

            //actualBottomOffset = Common.Max(offsetCount3[0], offsetCount[0]).GetValueOrDefault(0);
            //actualMiddleOffset = Common.Max(offsetCount3[1], offsetCount[1]).GetValueOrDefault(0);
            //actualTopOffset = Common.Max(offsetCount3[2], offsetCount[2]).GetValueOrDefault(0);

            //actualLeftWidth = Common.Max(wrappedWidth4[0], wrappedWidth2[0]).GetValueOrDefault(0);
            //actualCenterWidth = Common.Max(wrappedWidth4[1], wrappedWidth2[1]).GetValueOrDefault(0);
            //actualRightWidth = Common.Max(wrappedWidth4[2], wrappedWidth2[2]).GetValueOrDefault(0);

            //actualBottomHeight = Common.Max(wrappedHeight3[0], wrappedHeight[0]).GetValueOrDefault(0);
            //actualMiddleHeight = Common.Max(wrappedHeight3[1], wrappedHeight[1]).GetValueOrDefault(0);
            //actualTopHeight = Common.Max(wrappedHeight3[2], wrappedHeight[2]).GetValueOrDefault(0);

            var remainingWidth = 0;//= availableSize.Width - actualLeftWidth - actualCenterWidth - actualRightWidth;
            var remainingHeight = 0;//= availableSize.Height - actualBottomHeight - actualMiddleHeight - actualTopHeight;

            //if (middleCount > 0)
            //{
            //var ax = remainingHeight * HeightRatio;
            //remainingHeight -= ax;
            //middleRequiredHeight = ax / middleCount;
            //}
            //else
            //    middleRequiredHeight= 0;

            //if (centerCount > 0)
            //{
            //var ax2 = remainingWidth * WidthRatio;
            //remainingWidth -= ax2;
            //centerRequiredWidth = ax2 / centerCount;
            //}
            //else
            //    centerRequiredWidth = 0;

            double individualWidth = 0;//= remainingWidth / ((leftCount > 0 ? 1 : 0) + (rightCount > 0 ? 1 : 0) + (center2Count > 0 ? 1 : 0));
            double individualHeight = 0;//= remainingHeight / ((topCount > 0 ? 1 : 0) + (bottomCount > 0 ? 1 : 0) + (middle2Count > 0 ? 1 : 0));

            //if (leftCount > 0)
            //{
            //    remainingWidth -= individualWidth;
            //    leftRequiredWidth = individualWidth / leftCount;
            //    leftRegionWidth = individualWidth;
            //}
            //if (rightCount > 0)
            //{
            //    remainingWidth -= individualWidth;
            //    rightRequiredWidth = individualWidth / rightCount;
            //    rightRegionWidth = individualWidth;
            //}
            //if (center2Count > 0)
            //{
            //    remainingWidth -= individualWidth;
            //    centerRequiredWidth = individualWidth / center2Count;
            //    centerRegionWidth = individualWidth;
            //}
            //if (topCount > 0)
            //{
            //    remainingHeight -= individualHeight;
            //    topRequiredHeight = individualHeight / topCount;
            //    topRegionHeight = individualHeight;
            //}
            //if (bottomCount > 0)
            //{
            //    remainingHeight -= individualHeight;
            //    bottomRequiredHeight = individualHeight / bottomCount;
            //    bottomRegionHeight = individualHeight;
            //}
            //if (middle2Count > 0)
            //{
            //    remainingHeight -= individualHeight;
            //    middleRequiredHeight = individualHeight / middle2Count;
            //    middleRegionHeight = individualHeight;
            //}

            double finalWidth;
            if (individualWidth != double.PositiveInfinity)
                finalWidth = availableSize.Width;
            else
                finalWidth = availableSize.Width - remainingWidth;

            double finalHeight;
            if (individualHeight != double.PositiveInfinity)
                finalHeight = availableSize.Height;
            else
                finalHeight = availableSize.Height - remainingHeight;

            return new Size(finalWidth, finalHeight);
        }
    }
}

//List<UIElement> leftWidth = new List<UIElement>(), centerWidth = new List<UIElement>(), rightWidth = new List<UIElement>();
//List<UIElement> topHeight = new List<UIElement>(), middleHeight = new List<UIElement>(), bottomHeight = new List<UIElement>();

//List<UIElement> left2Width = new List<UIElement>(), center2Width = new List<UIElement>(), right2Width = new List<UIElement>();
//List<UIElement> top2Height = new List<UIElement>(), middle2Height = new List<UIElement>(), bottom2Height = new List<UIElement>();

//double leftCount = 0, centerCount = 0, rightCount = 0;
//double topCount = 0, middleCount = 0, bottomCount = 0;

//double center2Count = 0, middle2Count = 0;

//double bottomleftWidth = 0, bottomrightWidth = 0;
//double topleftWidth = 0, toprightWidth = 0;

//double bottomleftHeight = 0, bottomrightHeight = 0;
//double topleftHeight = 0, toprightHeight = 0;

//int bottomleftWidthCount = 0, bottomrightWidthCount = 0;
//int topleftWidthCount = 0, toprightWidthCount = 0;

//int bottomleftHeightCount = 0, bottomrightHeightCount = 0;
//int topleftHeightCount = 0, toprightHeightCount = 0;

//foreach (UIElement child in Children)
//{
//    var region = GetCircleRegion(child);
//    var heightSizing = GetHeightSizing(child);
//    var widthSizing = GetWidthSizing(child);

//    //var aSize = asizes[region] = asizes.GetValueOrDefault(region, new A());

//    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

//    int widthCount = child.DesiredSize.Width == 0 || widthSizing == Sizing.FromParent ? 1 : 0;
//    double desiredWidth = child.DesiredSize.Width == 0 || widthSizing == Sizing.FromParent ? 0 : child.DesiredSize.Width;
//    int heightCount = child.DesiredSize.Height == 0 || heightSizing == Sizing.FromParent ? 1 : 0;
//    double desiredHeight = child.DesiredSize.Height == 0 || heightSizing == Sizing.FromParent ? 0 : child.DesiredSize.Height;

//    switch (region)
//    {
//        case CircleRegion.TopLeft:
//            topleftWidth += desiredWidth;
//            topleftWidthCount += widthCount;
//            topleftHeightCount += heightCount;
//            topleftHeight += desiredHeight;
//            break;
//        case CircleRegion.BottomLeft:
//            bottomleftWidth += desiredWidth;
//            bottomleftWidthCount += widthCount;
//            bottomleftHeightCount += heightCount;
//            bottomleftHeight += desiredHeight;
//            break;
//        case CircleRegion.BottomRight:
//            bottomrightWidth += desiredWidth;
//            bottomrightWidthCount += widthCount;
//            bottomrightHeightCount += heightCount;
//            bottomrightHeight += desiredHeight;
//            break;
//        case CircleRegion.TopRight:
//            toprightWidth += desiredWidth;
//            toprightWidthCount += widthCount;
//            toprightHeightCount += heightCount;
//            toprightHeight += desiredHeight;
//            break;

//        case CircleRegion.Left:
//            if (desiredWidth > 0) leftWidth.Add(child);
//            leftCount += widthCount;
//            if (desiredHeight > 0) middle2Height.Add(child);
//            middle2Count = Math.Max(1, heightCount);
//            break;

//        case CircleRegion.Right:
//            if (desiredWidth > 0) rightWidth.Add(child);
//            rightCount += widthCount;
//            if (desiredHeight > 0) middle2Height.Add(child);
//            middle2Count = Math.Max(1, heightCount);
//            break;

//        case CircleRegion.Top:
//            center2Count = Math.Max(1, widthCount);
//            if (desiredWidth > 0) center2Width.Add(child);
//            topCount += heightCount;
//            if (desiredHeight > 0) topHeight.Add(child);
//            break;

//        case CircleRegion.Bottom:
//            center2Count = Math.Max(1, widthCount);
//            if (desiredWidth > 0) center2Width.Add(child);
//            bottomCount += heightCount;
//            if (desiredHeight > 0) bottomHeight.Add(child);
//            break;

//        case CircleRegion.Middle:
//            if (desiredWidth > 0) centerWidth.Add(child);
//            centerCount += widthCount;
//            if (desiredHeight > 0) middleHeight.Add(child);
//            middleCount += heightCount;
//            break;

//    };
//}

//bottomCount = Math.Max(bottomCount, Math.Max(Sizer.GetCount(bottomleftHeightCount), Sizer.GetCount(bottomrightHeightCount)));
//topCount = Math.Max(topCount, Math.Max(Sizer.GetCount(topleftHeightCount), Sizer.GetCount(toprightHeightCount)));
//leftCount = Math.Max(leftCount, Math.Max(Sizer.GetCount(bottomleftWidthCount), Sizer.GetCount(bottomrightWidthCount)));
//rightCount = Math.Max(rightCount, Math.Max(Sizer.GetCount(bottomleftHeightCount), Sizer.GetCount(bottomrightHeightCount)));

////var remainingWidth = availableSize.Width - leftWidth.Sum() - centerWidth.Sum() - rightWidth.Sum();
////var remainingHeight = availableSize.Height - topHeight.Sum() - middleHeight.Sum() - bottomHeight.Sum();

//var newMethod1 = NewMethod(availableSize.Height, GetHeight, new[] { bottomHeight, middleHeight, topHeight }).ToArray();

//var wrappedHeight = newMethod1
//                        .SelectMany(a => a.Select((c, i) => (c, i)))
//                        .GroupBy(a => a.i)
//                        .Select(a => a.Max(c => c.c.Sum(GetHeight))).ToOptionalArray();

//var grouped = newMethod1
//               .SelectMany(a => a.Select((c, i) => (c, i)))
//               .GroupBy(a => a.i).ToArray();

//var wrappedWidth = grouped
//               .Select(a => a.Where(a => a.c.Any()).Sum(c => c.c.Max(GetWidth))).ToArray();

//var offsetCount = grouped.Select(a => a.Count(c => c.c.Any())).ToOptionalArray();

//var newMethod2 = NewMethod(availableSize.Width, GetWidth, new[] { leftWidth, centerWidth, rightWidth }).ToArray();

//var wrappedWidth2 = newMethod2
//                        .SelectMany(a => a.Select((c, i) => (c, i)))
//                        .GroupBy(a => a.i)
//                        .Select(a => a.Max(c => c.c.Sum(GetWidth))).ToOptionalArray();

//var grouped2 = newMethod2
//               .SelectMany(a => a.Select((c, i) => (c, i)))
//               .GroupBy(a => a.i).ToArray();

//var wrappedHeight2 = grouped2
//               .Select(a => a.Where(a => a.c.Any()).Sum(c => c.c.Max(GetHeight))).ToArray();

//var offsetCount2 = grouped2.Select(a => a.Count(c => c.c.Any())).ToOptionalArray();

//var newMethod3 = NewMethod(availableSize.Height, GetHeight, new[] {
//                      bottom2Height.OrderByDescending(GetHeight).Take(1).ToArray() ,
//                    middle2Height.OrderByDescending(GetHeight).Take(1).ToArray() ,
//                    top2Height.OrderByDescending(GetHeight).Take(1).ToArray()  }).ToArray();

//var wrappedHeight3 = newMethod3
//                        .SelectMany(a => a.Select((c, i) => (c, i)))
//                        .GroupBy(a => a.i)
//                        .Select(a => a.Max(c => c.c.Sum(GetHeight))).ToOptionalArray();

//var grouped3 = newMethod3
//               .SelectMany(a => a.Select((c, i) => (c, i)))
//               .GroupBy(a => a.i).ToArray();

//var wrappedWidth3 = grouped3
//               .Select(a => a.Where(a => a.c.Any()).Sum(c => c.c.Max(GetWidth))).ToArray();

//var offsetCount3 = grouped3.Select(a => a.Count(c => c.c.Any())).ToOptionalArray();

//var newMethod4 = NewMethod(availableSize.Width, GetWidth, new[] {
//                    left2Width.OrderByDescending(GetWidth).Take(1).ToArray(),
//                    center2Width.OrderByDescending(GetWidth).Take(1).ToArray(),
//                    right2Width.OrderByDescending(GetWidth).Take(1).ToArray()
//                }).ToArray();

//var wrappedWidth4 = newMethod4
//                        .SelectMany(a => a.Select((c, i) => (c, i)))
//                        .GroupBy(a => a.i)
//                        .Select(a => a.Max(c => c.c.Sum(GetWidth))).ToOptionalArray();

//var grouped4 = newMethod4
//               .SelectMany(a => a.Select((c, i) => (c, i)))
//               .GroupBy(a => a.i).ToArray();

//var wrappedHeight4 = grouped4
//               .Select(a => a.Where(a => a.c.Any()).Sum(c => c.c.Max(GetHeight))).ToArray();

//var offsetCount4 = grouped4.Select(a => a.Count(c => c.c.Any())).ToOptionalArray();

////actualLeftOffset = Common.Max(offsetCount4[0], offsetCount2[0]).GetValueOrDefault(0);
////actualCenterOffset = Common.Max(offsetCount4[1], offsetCount2[1]).GetValueOrDefault(0);
////actualRightOffset = Common.Max(offsetCount4[2], offsetCount2[2]).GetValueOrDefault(0);

////actualBottomOffset = Common.Max(offsetCount3[0], offsetCount[0]).GetValueOrDefault(0);
////actualMiddleOffset = Common.Max(offsetCount3[1], offsetCount[1]).GetValueOrDefault(0);
////actualTopOffset = Common.Max(offsetCount3[2], offsetCount[2]).GetValueOrDefault(0);

////actualLeftWidth = Common.Max(wrappedWidth4[0], wrappedWidth2[0]).GetValueOrDefault(0);
////actualCenterWidth = Common.Max(wrappedWidth4[1], wrappedWidth2[1]).GetValueOrDefault(0);
////actualRightWidth = Common.Max(wrappedWidth4[2], wrappedWidth2[2]).GetValueOrDefault(0);

////actualBottomHeight = Common.Max(wrappedHeight3[0], wrappedHeight[0]).GetValueOrDefault(0);
////actualMiddleHeight = Common.Max(wrappedHeight3[1], wrappedHeight[1]).GetValueOrDefault(0);
////actualTopHeight = Common.Max(wrappedHeight3[2], wrappedHeight[2]).GetValueOrDefault(0);

//var remainingWidth = 0;//= availableSize.Width - actualLeftWidth - actualCenterWidth - actualRightWidth;
//var remainingHeight = 0;//= availableSize.Height - actualBottomHeight - actualMiddleHeight - actualTopHeight;

////if (middleCount > 0)
////{
////var ax = remainingHeight * HeightRatio;
////remainingHeight -= ax;
////middleRequiredHeight = ax / middleCount;
////}
////else
////    middleRequiredHeight= 0;

////if (centerCount > 0)
////{
////var ax2 = remainingWidth * WidthRatio;
////remainingWidth -= ax2;
////centerRequiredWidth = ax2 / centerCount;
////}
////else
////    centerRequiredWidth = 0;

//double individualWidth = 0;//= remainingWidth / ((leftCount > 0 ? 1 : 0) + (rightCount > 0 ? 1 : 0) + (center2Count > 0 ? 1 : 0));
//double individualHeight = 0;//= remainingHeight / ((topCount > 0 ? 1 : 0) + (bottomCount > 0 ? 1 : 0) + (middle2Count > 0 ? 1 : 0));

//if (leftCount > 0)
//{
//    remainingWidth -= individualWidth;
//    leftRequiredWidth = individualWidth / leftCount;
//    leftRegionWidth = individualWidth;
//}
//if (rightCount > 0)
//{
//    remainingWidth -= individualWidth;
//    rightRequiredWidth = individualWidth / rightCount;
//    rightRegionWidth = individualWidth;
//}
//if (center2Count > 0)
//{
//    remainingWidth -= individualWidth;
//    centerRequiredWidth = individualWidth / center2Count;
//    centerRegionWidth = individualWidth;
//}
//if (topCount > 0)
//{
//    remainingHeight -= individualHeight;
//    topRequiredHeight = individualHeight / topCount;
//    topRegionHeight = individualHeight;
//}
//if (bottomCount > 0)
//{
//    remainingHeight -= individualHeight;
//    bottomRequiredHeight = individualHeight / bottomCount;
//    bottomRegionHeight = individualHeight;
//}
//if (middle2Count > 0)
//{
//    remainingHeight -= individualHeight;
//    middleRequiredHeight = individualHeight / middle2Count;
//    middleRegionHeight = individualHeight;
//}

//double finalWidth;
//if (individualWidth != double.PositiveInfinity)
//    finalWidth = availableSize.Width;
//else
//    finalWidth = availableSize.Width - remainingWidth;

//double finalHeight;
//if (individualHeight != double.PositiveInfinity)
//    finalHeight = availableSize.Height;
//else
//    finalHeight = availableSize.Height - remainingHeight;

//return new Size(finalWidth, finalHeight);

//static Size GetDesiredSize(UIElement element, Size childConstraint)
//{
//    // Measure child.
//    element.Measure(childConstraint);
//    return element.DesiredSize;
//}

//            }
//            return availableSize;
//        }

//        double GetWidth(UIElement element) => element.DesiredSize.Width;
//double GetHeight(UIElement element) => element.DesiredSize.Height;

//private static IEnumerable<IList<IList<T>>> NewMethod<T>(double size, Func<T, double> func, IList<IList<T>> values)
//{
//    var orderedwidths = values.Select((a, i) => (a, i)).OrderByDescending(a => a.a.Sum(func)).Where(a => a.a.Count > 0).ToArray();
//    var enumr = orderedwidths.GetEnumerator();

//    List<List<T>> nextLevelWidths = new List<List<T>>();

//    while (orderedwidths.Any(a => a.a.Count > 1) && size - values.Sum(a => a.Sum(func)) < 0)
//    {
//        if (!enumr.MoveNext())
//        {
//            enumr.Reset();
//            enumr.MoveNext();
//        }

//        (IList<T> list, int i) = ((IList<T>, int))enumr.Current;

//        if (list.Count <= 1)
//            continue;

//        var last = list[list.Count - 1];
//        list.Remove(last);
//        while (nextLevelWidths.Count <= i)
//            nextLevelWidths.Add(new List<T>());

//        nextLevelWidths[i].Add(last);
//    }

//    if (values.Any(a => a.Any()))
//        yield return values.ToList();

//    if (nextLevelWidths.Any())
//    {
//        nextLevelWidths.ForEach(a => a.Reverse());

//        foreach (var m in NewMethod(size, func, nextLevelWidths.ToArray()))
//            yield return m;
//    }

//}