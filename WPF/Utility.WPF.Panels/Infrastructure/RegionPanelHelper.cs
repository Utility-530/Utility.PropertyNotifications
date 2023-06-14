using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Utility.WPF.Demo.Panels
{
    internal class RegionPanelHelper
    {
        public static IEnumerable<KeyValuePair<Region, IOrderedEnumerable<List<Coordinate>>>>
           SelectPotentialCoordinatesForRegions(ElementsBag bag, IEnumerable<Coordinate> existingCoordinates, int x, int y)
        {
            var combos = GetCoordinateCombinations(x, y).ToArray();
            //var test = combos.Select(a => a.Min).Select(x, y).ToArray();
            var remainingCoordinates = combos.Select(a => a.Min).Except(existingCoordinates).ToList();

            if (bag.right.Count > 0)
            {
                yield return GetCoordinateBoxesByRegion(Region.Right, x, y, remainingCoordinates);
            }
            if (bag.left.Count > 0)
            {
                yield return GetCoordinateBoxesByRegion(Region.Left, x, y, remainingCoordinates);
            }
            if (bag.top.Count > 0)
            {
                yield return GetCoordinateBoxesByRegion(Region.Top, x, y, remainingCoordinates);
            }
            if (bag.bottom.Count > 0)
            {
                yield return GetCoordinateBoxesByRegion(Region.Bottom, x, y, remainingCoordinates);
            }
            if (bag.topleft.Count > 0)
            {
                yield return GetCoordinateBoxesByRegion(Region.TopLeft, x, y, remainingCoordinates);
            }
            if (bag.topright.Count > 0)
            {
                yield return GetCoordinateBoxesByRegion(Region.TopRight, x, y, remainingCoordinates);
            }
            if (bag.bottomleft.Count > 0)
            {
                yield return GetCoordinateBoxesByRegion(Region.BottomLeft, x, y, remainingCoordinates);
            }
            if (bag.bottomright.Count > 0)
            {
                yield return GetCoordinateBoxesByRegion(Region.BottomRight, x, y, remainingCoordinates);
            }
            if (bag.middle.Count > 0)
            {
                yield return GetCoordinateBoxesByRegion(Region.Middle, x, y, remainingCoordinates);
            }

            static KeyValuePair<Region, IOrderedEnumerable<List<Coordinate>>> GetCoordinateBoxesByRegion(Region region, int x, int y, List<Coordinate> remainingCoordinates)
            {
                var coord = GetCoordinate(region, x - 1, y - 1);
                var sidesRemaining = remainingCoordinates.Select(a => (side: a.IsSide(coord), touching: a.IsTouching(coord), coord: a)).ToArray();

                if (remainingCoordinates.Count == 0 || !sidesRemaining.Any(a => a.side))
                {
                    var ae = new[] { new List<Coordinate> { coord } }.OrderBy(a => a);
                    return KeyValuePair.Create(region, ae);
                }
                if (sidesRemaining.Count(a => a.side) == 1)
                {
                    var ae = new[] {
                        new List<Coordinate> { coord, sidesRemaining.Single(ac => ac.side).coord },
                        new List<Coordinate> { coord }
                    }.OrderByDescending(a => a.Count);
                    return KeyValuePair.Create(region, ae);
                }
                if (sidesRemaining.Count(a => a.side) > 1)
                {
                    if (NewMethod1(region, coord, sidesRemaining) is { } aa)
                        return aa;

                    if (NewMethod2(region, coord, sidesRemaining) is { } ae)
                        return ae;
                }

                return AlternateMethod();

                static KeyValuePair<Region, IOrderedEnumerable<List<Coordinate>>>? NewMethod1(Region region, Coordinate coord, (bool side, bool touching, Coordinate coord)[] sidesRemaining)
                {
                    var ae = sidesRemaining.Where(a => a.side).GroupBy(a => a.coord.Y).ToArray();
                    if (ae.Length == 1)
                    {
                        var es = ae.Single().Select(a => a.coord.X).Concat(new[] { coord.X }).OrderBy(a => a).ToArray();
                        if ((es.Last() - es.First()) == es.Length - 1)
                        {
                            var aed = new[] {
                                    ae.Single().Select(c => c.coord).Concat(new[] { coord }).ToList() ,
                                    new List<Coordinate>{ae.Single().First().coord, coord },
                                    new List<Coordinate>{ae.Single().Last().coord, coord },
                                    new List<Coordinate>{coord}
                                }.OrderByDescending(a => a.Count);
                            return KeyValuePair.Create(region, aed);
                        }
                    }
                    return null;
                }

                static KeyValuePair<Region, IOrderedEnumerable<List<Coordinate>>>? NewMethod2(Region region, Coordinate coord, (bool side, bool touching, Coordinate coord)[] sidesRemaining)
                {
                    var ae = sidesRemaining.Where(a => a.side).GroupBy(a => a.coord.X).ToArray();
                    if (ae.Length == 1)
                    {
                        var es = ae.Single().Select(a => a.coord.Y).Concat(new[] { coord.Y }).OrderBy(a => a).ToArray();
                        if ((es.Last() - es.First()) == es.Length - 1)
                        {
                            var aed = new[] {
                                    ae.Single().Select(c => c.coord).Concat(new[] { coord }).ToList() ,
                                    new List<Coordinate>{ae.Single().First().coord, coord },
                                    new List<Coordinate>{ae.Single().Last().coord, coord },
                                    new List<Coordinate>{coord}
                                }.OrderByDescending(a => a.Count);
                            return KeyValuePair.Create(region, aed);
                        }
                    }
                    return null;
                }

                KeyValuePair<Region, IOrderedEnumerable<List<Coordinate>>> AlternateMethod()
                {
                    var remainingCoordinatesList = remainingCoordinates.Concat(new[] { coord }).ToList();

                    var remainingCoordinateBoxes = RegionPanelHelper.SelectCoordinates(remainingCoordinatesList).ToList()
                        .OrderByDescending(a => a.Count);

                    var boxes = SelectcoordinateBoxes(GetLimitFunc(region, x, y), x, y).OrderByDescending(a => a.Count);

                    return KeyValuePair.Create(region, boxes);

                    static Func<MinMax<Coordinate>, bool> GetLimitFunc(Region region, int x, int y)
                    {
                        return region switch
                        {
                            Region.TopLeft => new Func<MinMax<Coordinate>, bool>(a => a.Min.X <= 0 && a.Max.X >= 0 && a.Min.Y <= 0 && a.Max.Y >= 0),
                            Region.Top => a => a.Min.Y <= 0 && a.Max.Y >= 0,
                            Region.TopRight => (a => a.Min.X <= x - 1 && a.Max.X >= x - 1 && a.Min.Y <= 0 && a.Max.Y >= 0),

                            Region.Left => a => a.Min.X <= 0 && a.Max.X >= 0,
                            Region.Middle => (a => a.Min.X <= (x - 1) / 2 && a.Max.X >= (x - 1) / 2 && a.Min.Y <= (y - 1) / 2 && a.Max.Y >= (y - 1) / 2),
                            Region.Right => a => a.Min.X <= x - 1 && a.Max.X >= x - 1,

                            Region.BottomLeft => a => a.Min.X <= x - 1 && a.Max.X >= x - 1 && a.Min.Y <= y - 1 && a.Max.Y >= y - 1,
                            Region.Bottom => a => a.Min.Y <= y - 1 && a.Max.Y >= y - 1,
                            Region.BottomRight => a => a.Min.X <= x - 1 && a.Max.X >= x - 1 && a.Min.Y <= y - 1 && a.Max.Y >= y - 1,
                        };
                    }

                    static List<List<Coordinate>> SelectcoordinateBoxes(Func<MinMax<Coordinate>, bool> func, int x, int y)
                    {
                        var initialCombos = GetCoordinateCombinations(x, y);
                        var combos = initialCombos.Where(func).ToList();

                        var vls = combos
                            .Select(a =>
                            {
                                try
                                {
                                    var da =
                                    from x in Enumerable.Range(a.Min.X, 1 + a.Max.X - a.Min.X)
                                    join y in Enumerable.Range(a.Min.Y, 1 + a.Max.Y - a.Min.Y)
                                    on true equals true
                                    select new Coordinate(x, y);
                                    return da.ToList();
                                }
                                catch (Exception ex)
                                {
                                    throw;
                                }
                            })
                            .Where(a => a.Count > 0)
                            .ToList();

                        return vls;
                    }
                }
            }
        }

        private static List<MinMax<Coordinate>> GetCoordinateCombinations(int x, int y)
        {
            var cs = CombinationHelper.SelectSetCombinations(new[] { Enumerable.Range(0, x).ToList(), Enumerable.Range(0, y).ToList() })
                .Select(a => a.ToArray())
                .Select(a => new Coordinate(a[0], a[1]))
                .OrderBy(a => a.Y).ThenBy(a => a.X).ToList()
                .ToList();

            var cbs = CombinationHelper.SelectCombinations(cs);

            var minMaxes = cbs
                // Remove items where
                .Where(a => !(a.Item1.CompareTo(a.Item2) == 0 && !a.Item1.Equals(a.Item2)))
                .Select(a => new[] { a.Item1, a.Item2 }.OrderBy(a => a).ToArray()).Select(a => new MinMax<Coordinate>(a[0], a[1])).ToList();

            return minMaxes;
        }

        private static IEnumerable<List<Coordinate>> SelectCoordinates(List<Coordinate> coordinates)
        {
            var initial = SelectCoordinateBoxes(coordinates).ToList();
            var sede = initial.ToList();

            return sede.Select(a =>
            {
                var ad = (from x in Enumerable.Range(Math.Min(a.Item1.X, a.Item2.X), 1 + Math.Abs(a.Item1.X - a.Item2.X))
                          join y in Enumerable.Range(Math.Min(a.Item1.Y, a.Item2.Y), 1 + Math.Abs(a.Item1.Y - a.Item2.Y))
                          on true equals true
                          orderby x
                          orderby y
                          select new Coordinate(x, y)
                        ).ToList();
                return ad;
            });

            static IEnumerable<(Coordinate, Coordinate)> SelectCoordinateBoxes(List<Coordinate> coordinates)
            {
                var ordered = coordinates.OrderBy(a => a.X).OrderBy(a => a.Y).ToList();
                int i = 0;
                foreach (var coord in ordered)
                {
                    int j = 0, y = 0;
                    Coordinate? last = default;
                    int? maxX = default;
                    while (i + j < ordered.Count)
                    {
                        var next = ordered[(i + j)];
                        if (last.HasValue && last.Value.Y == next.Y && (next.X - 1) != last.Value.X ||
                            (maxX.HasValue && last.Value.Y < next.Y && (next.X > maxX.Value || next.X < coord.X)))
                        {
                            break;
                        }

                        if (last.HasValue == false || (last.HasValue && next.X >= coord.X && next.Y >= coord.Y))
                        {
                            maxX = next.X;
                            last = ordered[(i + j)];
                        }
                        j++;
                    }

                    if (last.HasValue)
                        yield return (coord, last.Value);

                    i++;
                }
            }
        }

        public static Coordinate GetCoordinate(Region region, int x, int y)
        {
            return region switch
            {
                Region.TopLeft => new Coordinate(0, 0),
                Region.Top => new Coordinate((int)(x / 2), 0),
                Region.TopRight => new Coordinate(x, 0),

                Region.Left => new Coordinate(0, (int)(y / 2)),
                Region.Middle => new Coordinate((int)(x / 2), (int)(y / 2)),
                Region.Right => new Coordinate(x, (int)(y / 2)),

                Region.BottomLeft => new Coordinate(0, y),
                Region.Bottom => new Coordinate((int)(x / 2), y),
                Region.BottomRight => new Coordinate(x, y),
            };
        }

        public static IEnumerable<(Rect, FrameworkElement)> SelectInnerElementRects(Region region, ICollection<FrameworkElement> elements, Rect rect, bool useDesiredSize)
        {
            var max = Math.Max(rect.Width, rect.Height);
            var isWiderThanTall = max == rect.Width;
            var min = isWiderThanTall ? rect.Height : rect.Width;

            var division = elements.Count > 1 ? (isWiderThanTall ?
                Math.Max(0, max - elements.Last().DesiredSize.Width) :
                Math.Max(0, max - elements.Last().DesiredSize.Height)) /
                (elements.Count - 1) : max;

            if (useDesiredSize == false)
            {
                if (elements.Sum(a => a.DesiredSize.Width) < rect.Width)
                    division = max / elements.Count;

                if (elements.Sum(a => a.DesiredSize.Height) < rect.Height)
                    division = max / elements.Count;
            }
            else
            {
                var maxWidth = elements.Max(a => a.DesiredSize.Width);
                var maxHeight = elements.Max(a => a.DesiredSize.Height);
                division = isWiderThanTall ?
                    maxWidth > 0 ? maxWidth : elements.Max(a => a.ActualHeight) :
                    maxHeight > 0 ? maxHeight : elements.Max(a => a.ActualWidth);
            }

            var point = rect.Location;
            var size = isWiderThanTall ? new Size(division, min) : new Size(min, division);
            foreach (var elem in elements)
            {
                if (elem.DesiredSize.Width == 0 || elem.DesiredSize.Height == 0 || useDesiredSize == false)
                {
                    yield return (new Rect(point, size), elem);
                    point = isWiderThanTall ? new Point(point.X + division, point.Y) : new Point(point.X, point.Y + division);
                }
                else
                {
                    Rect childRect = new Rect(point, new Size(
                        Math.Max(elem.DesiredSize.Width, isWiderThanTall ? division : rect.Width),
                        Math.Max(elem.DesiredSize.Height, isWiderThanTall ? rect.Height : division)));

                    GetChildRect(region, elem, rect, ref point, ref childRect, isWiderThanTall);

                    point = new Point(point.X + (isWiderThanTall ? Math.Min(childRect.Width, division) : 0), point.Y + (isWiderThanTall ? 0 : Math.Min(childRect.Height, division)));

                    yield return (childRect, elem);
                }
            }

            static void GetChildRect(Region region, UIElement elem, Rect rect, ref Point lastPoint, ref Rect childRect, bool isWiderThanTall)
            {
                if (elem.DesiredSize.Width > rect.Size.Width)
                {
                    lastPoint = new Point(rect.X + rect.Size.Width / 2 - elem.DesiredSize.Width / 2, lastPoint.Y);
                    childRect = new Rect(lastPoint, childRect.Size);
                }
                if (elem.DesiredSize.Height > rect.Size.Height)
                {
                    lastPoint = new Point(lastPoint.X, rect.Y + rect.Size.Height / 2 - elem.DesiredSize.Height / 2);
                    childRect = new Rect(lastPoint, childRect.Size);
                }
                if ((lastPoint.X + elem.DesiredSize.Width) > (rect.X + rect.Size.Width))
                {
                    lastPoint = region switch
                    {
                        Region.Right => new Point(rect.X - childRect.Width, isWiderThanTall ? rect.Y : lastPoint.Y),
                        Region.BottomRight => new Point(rect.Width + rect.X - childRect.Width, isWiderThanTall ? rect.Y : lastPoint.Y),
                        Region.TopRight => new Point(rect.Width + rect.X - childRect.Width, isWiderThanTall ? rect.Y : lastPoint.Y),
                        _ => new Point(rect.X, isWiderThanTall ? rect.Y : lastPoint.Y),
                    };
                    childRect = new Rect(lastPoint, childRect.Size);
                }
                if ((lastPoint.Y + elem.DesiredSize.Height) > (rect.Y + rect.Size.Height))
                {
                    lastPoint = region switch
                    {
                        Region.BottomRight => new Point(isWiderThanTall ? lastPoint.X : rect.X, rect.Height + rect.Y - childRect.Height),
                        Region.BottomLeft => new Point(isWiderThanTall ? lastPoint.X : rect.X, rect.Height + rect.Y - childRect.Height),
                        Region.Bottom => new Point(isWiderThanTall ? lastPoint.X : rect.X, rect.Height + rect.Y - childRect.Height),
                        _ => new Point(isWiderThanTall ? lastPoint.X : rect.X, rect.Y),
                    };
                    childRect = new Rect(lastPoint, childRect.Size);
                }
            }
        }

        public static Rect ConvertToRect(Region region, List<Coordinate> innerCoordsSet, Size finalSize, int x, int y, double widthRatio = 1, double heightRatio = 1)
        {
            var ((startX, diffX), (startY, diffY)) =

                region switch
                {
                    Region.Middle => (Middle_X(), Middle_Y()),
                    Region.Top => (Middle_X(), Edge_Y()),
                    Region.Bottom => (Middle_X(), Edge_Y()),
                    Region.Left => (Edge_X(), Middle_Y()),
                    Region.Right => (Edge_X(), Middle_Y()),
                    _ => (Edge_X(), Edge_Y()),
                };

            (double, double) Middle_X()
            {
                return x > 2 ? MiddleX(innerCoordsSet, finalSize, x, widthRatio) : EdgeX(innerCoordsSet, finalSize, x, widthRatio);
            }
            (double, double) Middle_Y()
            {
                return x > 2 ? MiddleY(innerCoordsSet, finalSize, y, heightRatio) : EdgeY(innerCoordsSet, finalSize, y, heightRatio);
            }
            (double, double) Edge_X()
            {
                return EdgeX(innerCoordsSet, finalSize, x, widthRatio);
            }
            (double, double) Edge_Y()
            {
                return EdgeY(innerCoordsSet, finalSize, y, heightRatio);
            }

            return (new Rect(new Point(startX, startY), new Size(diffX, diffY)));

            static (double start, double end) EdgeY(List<Coordinate> innerCoordsSet, Size finalSize, int y, double heightRatio = 1)
            {
                var startY = (innerCoordsSet.Min(a => a.Y) * finalSize.Height / y);
                var diffY = UnderFactor(heightRatio, y) * (((innerCoordsSet.Max(a => a.Y) + 1) * finalSize.Height / y) - startY);
                var es = (finalSize.Height - diffY) / startY;
                var of = OverFactor(heightRatio, y);
                startY *= OverFactor(heightRatio, y);

                return (startY, diffY);
            }

            static (double start, double end) MiddleY(List<Coordinate> innerCoordsSet, Size finalSize, int y, double heightRatio = 1)
            {
                var startY = (innerCoordsSet.Min(a => a.Y) * finalSize.Height / y);
                startY *= UnderFactor(heightRatio, y);
                var diffY = OverFactor(heightRatio, y) * (((innerCoordsSet.Max(a => a.Y) + 1) * finalSize.Height / y)) - startY;

                return (startY, diffY);
            }

            static (double start, double end) EdgeX(List<Coordinate> innerCoordsSet, Size finalSize, int x, double widthRatio = 1)
            {
                var startX = (innerCoordsSet.Min(a => a.X) * finalSize.Width / x);
                var diffX = UnderFactor(widthRatio, x) * (((innerCoordsSet.Max(a => a.X) + 1) * finalSize.Width / x) - startX);
                startX *= OverFactor(widthRatio, x);

                return (startX, diffX);
            }

            static (double start, double end) MiddleX(List<Coordinate> innerCoordsSet, Size finalSize, int x, double widthRatio = 1)
            {
                var startX = (innerCoordsSet.Min(a => a.X) * finalSize.Width / x);
                startX *= UnderFactor(widthRatio, x);
                var diffX = OverFactor(widthRatio, x) * (((innerCoordsSet.Max(a => a.X) + 1) * finalSize.Width / x)) - startX;
                return (startX, diffX);
            }

            static double UnderFactor(double ratio, int size) => 1d / ratio;

            static double OverFactor(double ratio, int size)
            {
                return size switch
                {
                    3 => 1 + (1 - 1 / ratio) / 2,
                    2 => 1 + (1 - 1 / ratio),
                    1 => 1,
                };
            }
        }
    }
}