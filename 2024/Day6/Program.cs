using AdventOfCode;
using AdventOfCode.Grids;

namespace Day6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day6_2024>(41, 6, verbose: false);
        }
    }

    internal class Day6_2024 : IDailyChallenge
    {
        private List<(int Index, AllowedDirections Direction)> _history = new();

        public int Part1(IEnumerable<string> inputLines)
        {
            var map = LoadInput(inputLines);

            var playerIsInArea = true;
            while (playerIsInArea)
            {
                AdventOfCodeRunner.WriteLine(map.ToString());
                var guard = map.Nodes.Single(n => n.HasGuard);
                var (y, x) = map.GetIndices(guard);
                guard.IsVisited = true;

                var index = (map.Nodes as List<MapNode>).IndexOf(guard);
                if (_history.All(x => x.Index != index))
                {
                    _history.Add((map.Nodes.ToList().IndexOf(guard), guard.PlayerDirection));
                }

                var nextNode = map.GetNeighbours(y, x, guard.PlayerDirection).Single();

                // No available node to move forward - guard left the area.
                if (nextNode is null)
                {
                    playerIsInArea = false;
                    continue;
                }

                // Rotate right if facing an obstacle.
                if (nextNode.IsObstacle)
                {
                    guard.PlayerDirection = guard.PlayerDirection switch
                    {
                        AllowedDirections.Up => AllowedDirections.Right,
                        AllowedDirections.Right => AllowedDirections.Down,
                        AllowedDirections.Down => AllowedDirections.Left,
                        AllowedDirections.Left => AllowedDirections.Up,
                    };

                    continue;
                }

                guard.HasGuard = false;
                nextNode.HasGuard = true;
                nextNode.PlayerDirection = guard.PlayerDirection;
            }

            return map.Nodes.Count(n => n.IsVisited);
        }

        public int Part2(IEnumerable<string> inputLines)
        {
            var progress = 0;
            var nonObstaclesCount = string.Join(string.Empty, inputLines).Count(c => c == '.');
            var looping = 0;

            Parallel.ForEach(Enumerable.Range(1, _history.Count - 1), new ParallelOptions { MaxDegreeOfParallelism = 24 }, i =>
            {
                var map = LoadInput(inputLines);
                map.NodesList[_history[i].Index].IsObstacle = true;
                var takenTurns = new HashSet<(int Index, AllowedDirections Direction)>();

                var playerIsInArea = true;

                while (playerIsInArea)
                {
                    var guard = map.Nodes.Single(n => n.HasGuard);
                    var guardIndex = map.NodesList.IndexOf(guard);
                    var (y, x) = map.GetIndices(guard);
                    guard.IsVisited = true;
                    var nextNode = map.GetNeighbours(y, x, guard.PlayerDirection).Single();

                    // No available node to move forward - guard left the area.
                    if (nextNode is null)
                    {
                        playerIsInArea = false;
                        continue;
                    }

                    // Rotate right if facing an obstacle.
                    if (nextNode.IsObstacle)
                    {
                        if (takenTurns.Contains((guardIndex, guard.PlayerDirection)))
                        {
                            looping++;
                            break;
                        }

                        takenTurns.Add((guardIndex, guard.PlayerDirection));

                        guard.PlayerDirection = guard.PlayerDirection switch
                        {
                            AllowedDirections.Up => AllowedDirections.Right,
                            AllowedDirections.Right => AllowedDirections.Down,
                            AllowedDirections.Down => AllowedDirections.Left,
                            AllowedDirections.Left => AllowedDirections.Up,
                        };

                        continue;
                    }

                    guard.HasGuard = false;
                    nextNode.HasGuard = true;
                    nextNode.PlayerDirection = guard.PlayerDirection;
                }

                progress++;
                if (progress % 10 == 0)
                {
                    Console.WriteLine($"{progress}/{_history.Count} ({progress / (double)_history.Count})");
                }
            });

            return looping;
        }

        private ListGrid<MapNode> LoadInput(IEnumerable<string> inputLines)
        {
            var grid = new ListGrid<MapNode>();
            grid.Width = inputLines.First().Length;
            grid.Height = inputLines.Count();

            foreach(var line in inputLines)
            {
                foreach(var character in line)
                {
                    grid.Add(new MapNode(character));
                }
            }

            return grid;
        }
    }

    internal class MapNode
    {
        public MapNode(char c)
        {
            switch(c)
            {
                case '#':
                    IsObstacle = true;
                    break;

                case '^':
                    HasGuard = true;
                    PlayerDirection = AllowedDirections.Up;
                    break;

                case '.':
                    break;
            }
        }

        public bool IsVisited { get; set; }

        public bool IsObstacle { get; set; }

        public bool HasGuard { get; set; }

        public AllowedDirections PlayerDirection { get; set; }

        public override string ToString()
        {
            return
                HasGuard ? "G" :
                IsObstacle ? "#" :
                IsVisited ? "X" :
                ".";
        }
    }


}
