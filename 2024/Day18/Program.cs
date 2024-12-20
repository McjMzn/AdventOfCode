using AdventOfCode;
using AdventOfCode.Grids;
using System.Xml.Linq;

namespace Day18
{
    internal interface IReachable
    {
        long Distance { get; set; }

        bool IsVisited { get; set; }
    }

    internal class DijkstraSolver
    {
        public static void Solve<T>(IDictionary<Coordinates, T> map)
            where T : IReachable
        {
            Solve(map, _ => true);
        }

        public static void Solve<T>(IDictionary<Coordinates, T> map, Predicate<T> isLegal)
            where T : IReachable
        {
            while (map.Any(kvp => !kvp.Value.IsVisited && isLegal(kvp.Value)))
            {
                var selected = map.Where(kvp => !kvp.Value.IsVisited).OrderBy(kvp => kvp.Value.Distance).FirstOrDefault();

                var node = selected.Value;
                var coordinates = selected.Key;
                node.IsVisited = true;

                List<Coordinates> neighbours = new Coordinates[] {
                    coordinates.Translated(Vector2d.Up),
                    coordinates.Translated(Vector2d.Down),
                    coordinates.Translated(Vector2d.Left),
                    coordinates.Translated(Vector2d.Right),
                 }
                .Where(c => map.ContainsKey(c) && isLegal(map[c]))
                .ToList();

                foreach (var neigbour in neighbours)
                {
                    if (node.Distance + 1 < map[neigbour].Distance)
                    {
                        map[neigbour].Distance = node.Distance + 1;
                    }
                }
            }
        }

        public static void SolveReachable<T>(IDictionary<Coordinates, T> map, Predicate<T> isLegal)
            where T : IReachable
        {
            var first = map.Where(kvp => kvp.Value.Distance == 0).Select(kvp => kvp.Key).First();

            var queue = new PriorityQueue<Coordinates, long>();
            queue.Enqueue(first, 0);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (map[current].IsVisited)
                {
                    continue;
                }

                map[current].IsVisited = true;

                List<Coordinates> neighbours = new Coordinates[] {
                    current.Translated(Vector2d.Up),
                    current.Translated(Vector2d.Down),
                    current.Translated(Vector2d.Left),
                    current.Translated(Vector2d.Right),
                 }
                .Where(c => map.ContainsKey(c) && isLegal(map[c]) && !map[c].IsVisited)
                .ToList();

                foreach (var neigbour in neighbours)
                {
                    if (map[current].Distance + 1 < map[neigbour].Distance)
                    {
                        map[neigbour].Distance = map[current].Distance + 1;
                    }

                    queue.Enqueue(neigbour, map[neigbour].Distance);
                }
            }
        }
    }

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

            DijkstraSolver.SolveReachable(map, node => !node.IsCorrupted);

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

                DijkstraSolver.SolveReachable(map, n => !n.IsCorrupted);

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
