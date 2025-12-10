using AdventOfCode;

namespace Day9
{
    internal class Day9_2025 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            var coordinates = LoadRedTiles(inputLines);
            var areas = new List<long>();
            foreach (var a in coordinates)
            {
                foreach(var b in coordinates)
                {
                    if (a == b)
                    {
                        continue;
                    }

                    var width = Math.Abs(a.X - b.X) + 1;
                    var height = Math.Abs(a.Y - b.Y) + 1;

                    var area = width * height;
                    
                    areas.Add(area);
                }
            }

            return areas.Max();
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            var redTiles = LoadRedTiles(inputLines);
            var greenTiles = LoadGreenTiles(redTiles);

            return 0L;
        }

        private List<(long Y, long X)> LoadRedTiles(IEnumerable<string> inputLines)
        {
            var coordinates = new List<(long Y, long X)>();
            foreach (var line in inputLines)
            {
                var split = line.Split(',');
                var x = long.Parse(split[0]);
                var y = long.Parse(split[1]);

                coordinates.Add((y, x));
            }

            return coordinates;
        }

        private List<(long Y, long X)> LoadGreenTiles(List<(long Y, long X)> redTiles)
        {
            List<(long Y, long X)> greenTiles = new();

            for (var i = 0; i < redTiles.Count; i++)
            {
                var from = redTiles[i];
                var to = redTiles[(i + 1) % redTiles.Count];

                var steps = Math.Max(Math.Abs(to.Y - from.Y), Math.Abs(to.X - from.X));
                var vector = (Y: Math.Sign(to.Y - from.Y), X: Math.Sign(to.X - from.X));

                for (var s = 1; s < steps; s++)
                {
                    var tile = (from.Y + s * vector.Y, from.X + s * vector.X);
                    greenTiles.Add(tile);
                }
            }

            return greenTiles;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day9_2025>(50, -1, verbose: false);
        }
    }
}
