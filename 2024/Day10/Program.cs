using AdventOfCode;
using AdventOfCode.Grids;

namespace Day10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day10_2024>(36);
        }
    }

    internal record class Node
    {
        public int Height { get; set; }

        public override string ToString()
        {
            return $"{Height}";
        }
    }

    internal class Day10_2024 : IDailyChallenge<int>
    {
        private int _foundTrails;

        public int Part1(IEnumerable<string> inputLines)
        {
            var (map, startingPoints) = ProcessInput(inputLines);

            _foundTrails = 0;
            var score = 0;
            foreach (var startingPoint in startingPoints)
            {
                var reachablePeaks = new HashSet<(int Y, int X)>();
                FindTrailheads(map, startingPoint, new(), reachablePeaks, ref _foundTrails);
                score += reachablePeaks.Count;
            }

            return score;
        }

        public int Part2(IEnumerable<string> inputLines)
        {
            return _foundTrails;
        }

        private void FindTrailheads(ListGrid<Node> map, (int Y, int X) currentLocation, HashSet<(int Y, int X)> currentPath, HashSet<(int Y, int X)> reachablePeaks, ref int foundTrails)
        {
            var currentHeight = map.Get(currentLocation.Y, currentLocation.X).Height;
            if (currentHeight == 9)
            {
                foundTrails++;
                reachablePeaks.Add(currentLocation);
                return;
            }

            currentPath.Add(currentLocation);

            (int Y, int X)[] neighbours = [
                (currentLocation.Y - 1, currentLocation.X),
                (currentLocation.Y + 1, currentLocation.X),
                (currentLocation.Y, currentLocation.X - 1),
                (currentLocation.Y, currentLocation.X + 1),
            ];

            var legalSteps =
                neighbours
                .Where(n => n.Y >= 0 && n.Y < map.Height && n.X >= 0 && n.X < map.Width)
                .Where(n => !currentPath.Contains(n))
                .Where(n => map.Get(n.Y, n.X).Height == currentHeight + 1)
                .ToList();

            foreach (var legalStep in legalSteps)
            {
                FindTrailheads(map, legalStep, new HashSet<(int Y, int X)>(currentPath), reachablePeaks, ref foundTrails);
            }
        }

        private (ListGrid<Node> Map, HashSet<(int Y, int X)> StartingPoints) ProcessInput(IEnumerable<string> inputLines)
        {
            var grid = new ListGrid<Node>
            {
                Height = inputLines.Count(),
                Width = inputLines.First().Length
            };

            var startingPoints = new HashSet<(int Y, int X)>();

            foreach (var line in inputLines)
            {
                foreach(var c in line)
                {
                    var coordinates = grid.Add(new Node { Height = int.Parse(c == '.' ? "-1" : c.ToString()) });
                    if (c == '0')
                    {
                        startingPoints.Add(coordinates);
                    }
                }
            }

            return (grid, startingPoints);
        }
    }
}
