using System.Drawing;

namespace Nodify.Playground
{
    public struct NodesGeneratorSettings
    {
        private static readonly Random _rand = new Random();

        public NodesGeneratorSettings(uint count)
        {
            GridSnap = 15;
            MinNodesCount = MaxNodesCount = count;
            MinInputCount = MinOutputCount = 0;
            MaxInputCount = MaxOutputCount = 7;

            ConnectorNameGenerator = (s, i) => $"{new string('C', (int)i % 5)} {i}";
            NodeNameGenerator = (s, i) => $"Node {i}";
            NodeLocationGenerator = (s, i) =>
            {
                static double EaseOut(double percent, double increment, double start, double end, double total)
                    => -end * (increment /= total) * (increment - 2) + start;

                var xDistanceBetweenNodes = _rand.Next(150, 350);
                var yDistanceBetweenNodes = _rand.Next(200, 350);
                var randSignX = _rand.Next(0, 100) > 50 ? 1 : -1;
                var randSignY = _rand.Next(0, 100) > 50 ? 1 : -1;
                var gridOffsetX = i * xDistanceBetweenNodes;
                var gridOffsetY = i * yDistanceBetweenNodes;

                var x = gridOffsetX * Math.Sin(xDistanceBetweenNodes * randSignX / (i + 1));
                var y = gridOffsetY * Math.Sin(yDistanceBetweenNodes * randSignY / (i + 1));
                var easeX = x * EaseOut(i / count, i, 1, 0.01, count);
                var easeY = y * EaseOut(i / count, i, 1, 0.01, count);

                x = s.Snap((int)easeX);
                y = s.Snap((int)easeY);

                return new PointF((float)x, (float)y);
            };
        }

        public uint GridSnap;
        public uint MinNodesCount;
        public uint MaxNodesCount;
        public uint MinInputCount;
        public uint MaxInputCount;
        public uint MinOutputCount;
        public uint MaxOutputCount;

        public Func<NodesGeneratorSettings, uint, string?> ConnectorNameGenerator;
        public Func<NodesGeneratorSettings, uint, string?> NodeNameGenerator;
        public Func<NodesGeneratorSettings, uint, PointF> NodeLocationGenerator;

        public int Snap(int x)
            => x / (int)GridSnap * (int)GridSnap;
    }
}