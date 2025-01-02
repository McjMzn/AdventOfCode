using AdventOfCode;
using AdventOfCode.Grids;
using AdventOfCode.Pathfinding;

namespace Day18
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day18_2024>(22);
        }
    }

    internal class Node : IReachable
    {
        public long Distance { get; set; } = int.MaxValue;

        public bool IsVisited { get; set; }

        public bool IsCorrupted { get; set; }

        public override string ToString()
        {
            return IsCorrupted ? "#" : ".";
        }
    }

    internal class Day18_2024 : IDailyChallenge<string>
    {
        public string Part1(IEnumerable<string> inputLines)
        {
            var mapHeight = AdventOfCodeRunner.RunningDemo ? 7 : 71;
            var mapWidth = AdventOfCodeRunner.RunningDemo ? 7 : 71;
            var bytesToSimulate = AdventOfCodeRunner.RunningDemo ? 12 : 1024;

            var map = ProcessInput(inputLines, mapHeight, mapWidth, bytesToSimulate);
            map[(0, 0)].Distance = 0;

            Dijkstra.SolveReachable(map, node => !node.IsCorrupted);

            return map[(mapHeight - 1, mapWidth - 1)].Distance.ToString();
        }

        public string Part2(IEnumerable<string> inputLines)
        {
            var mapHeight = AdventOfCodeRunner.RunningDemo ? 7 : 71;
            var mapWidth = AdventOfCodeRunner.RunningDemo ? 7 : 71;
            var bytesToSimulate = AdventOfCodeRunner.RunningDemo ? 12 : 1024;

            var lines = inputLines.ToList();
            var stepNumber = bytesToSimulate;
            var found = false;

            while (!found)
            {
                var map = ProcessInput(lines, mapHeight, mapWidth, stepNumber + 1);
                map[(0,0)].Distance = 0;

                Dijkstra.SolveReachable(map, n => !n.IsCorrupted);

                if (map[(mapHeight - 1, mapWidth - 1)].Distance == int.MaxValue)
                {
                    found = true;
                }
                else
                {
                    stepNumber++;
                }
            }

            return lines[stepNumber];
        }

        private Dictionary<Coordinates, Node> ProcessInput(IEnumerable<string> inputLines, int height, int width, int bytesToSimulate)
        {
            var map = new Dictionary<Coordinates, Node>();

            for(var y  = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    map.Add((y, x), new Node());
                }
            }

            foreach (var line in inputLines.Take(bytesToSimulate))
            {
                var split = line.Split(',');
                var coordinates = (int.Parse(split[1]), int.Parse(split[0]));

                map[coordinates].IsCorrupted = true;
            }

            return map;
        }
    }
}
