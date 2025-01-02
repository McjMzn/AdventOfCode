using AdventOfCode;
using AdventOfCode.Grids;
using AdventOfCode.Pathfinding;
using System.Collections.Concurrent;

namespace Day20
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day20_2024>(0, runDemo: true);
        }
    }

    internal class Node : IReachable
    {
        public long Distance { get; set; } = int.MaxValue;
        public bool IsVisited { get; set; } = false;

        public bool IsStart { get; set; }
        public bool IsEnd { get; set; }
        public bool IsWall { get; set; }
    }

    internal class Day20_2024 : IDailyChallenge<int>
    {
        public int Part1(IEnumerable<string> inputLines)
        {
            return Solve(inputLines, 2);
        }

        public int Part2(IEnumerable<string> inputLines)
        {
            return Solve(inputLines, 20);
        }

        private int Solve(IEnumerable<string> inputLines, int numberOfAllowedCheats)
        {
            // Setup
            var (map, height, width) = ProcessInput(inputLines.ToList());
            var end = map.Single(x => x.Value.IsEnd).Value;
            var start = map.Single(x => x.Value.IsStart).Value;

            // Solve original
            Dijkstra.Solve(map, n => !n.IsWall);
            var defaultDistance = end.Distance;

            // Backtrack the path
            var defaultPath = new List<Node>();
            var current = end;
            while (current != start)
            {
                defaultPath.Add(current);
                current = map.Values.Where(n => n.Distance < current.Distance).OrderByDescending(n => n.Distance).First();
            }

            defaultPath.Add(current);

            defaultPath.Reverse();

            var defaultPathCoordinates = defaultPath.Select(node => map.First(kvp => kvp.Value == node).Key).ToList();


            var cheatResults = new Dictionary<long, int>();

            var dictionaryLock = new object();

            // Cheat
            Parallel.For(0, defaultPath.Count, new ParallelOptions { MaxDegreeOfParallelism = 22 }, i =>
            {
                var step = defaultPathCoordinates[i];
                var reachable = new Dictionary<Coordinates, int>
                {
                    [step] = 0
                };

                for (var s = 1; s <= numberOfAllowedCheats; s++)
                {
                    foreach (var cheatStep in reachable.Keys.ToList())
                    {
                        reachable.TryAdd(cheatStep.Translated(Vector2d.Up), s);
                        reachable.TryAdd(cheatStep.Translated(Vector2d.Down), s);
                        reachable.TryAdd(cheatStep.Translated(Vector2d.Left), s);
                        reachable.TryAdd(cheatStep.Translated(Vector2d.Right), s);
                    }
                }

                foreach (var cheatStep in reachable)
                {
                    var cheatStepCoordinates = cheatStep.Key;
                    var cheatStepCost = cheatStep.Value;

                    if (!map.ContainsKey(cheatStepCoordinates) || i == defaultPath.Count - 1)
                    {
                        continue;
                    }

                    if (map[cheatStepCoordinates].Distance == int.MaxValue)
                    {
                        continue;
                    }

                    var save = map[cheatStepCoordinates].Distance - defaultPath[i + 1].Distance + 1 - cheatStepCost;
                    if (save <= 0)
                    {
                        continue;
                    }

                    lock(dictionaryLock)
                    {
                        if (!cheatResults.ContainsKey(save))
                        {
                            cheatResults.Add(save, 1);
                        }
                        else
                        {
                            cheatResults[save]++;
                        }
                    }
                }
            });

            return cheatResults.Where(kvp => kvp.Key >= 100).Select(kvp => kvp.Value).Sum();
        }

        private (Dictionary<Coordinates, Node> Map, int Height, int Width) ProcessInput(List<string> inputLines)
        {
            var height = inputLines.Count;
            var width = inputLines.First().Length;
            var map = new Dictionary<Coordinates, Node>();

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var symbol = inputLines[y][x];
                    var node = new Node
                    {
                        IsWall = symbol == '#',
                        IsStart = symbol == 'S',
                        IsEnd = symbol == 'E'
                    };

                    if (node.IsStart)
                    {
                        node.Distance = 0;
                    }

                    map.Add((y, x), node);
                }
            }

            return (map, height, width);
        }
    }
}
