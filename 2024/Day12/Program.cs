using AdventOfCode;

namespace Day12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day12_2024>(1930);
        }
    }

    internal class Day12_2024 : IDailyChallenge<int>
    {
        private int _part2;

        public int Part1(IEnumerable<string> inputLines)
        {
            var inputList = inputLines.ToList();
            
            var map = new Dictionary<(int Y, int X), string>();
            for (var y = 0; y < inputList.Count; y++)
            {
                for (var x = 0; x < inputList[y].Length; x++)
                {
                    map[(y, x)] = inputList[y][x].ToString();
                }
            }

            var visited = new HashSet<(int Y, int X)>();
            var regionsFound = 0;
            var costPart1 = 0;
            _part2 = 0;

            foreach (var coordinates  in map.Keys)
            {
                if (visited.Contains(coordinates))
                {
                    continue;
                }

                var (region, fence) = FindRegion(coordinates, map);
                var sides = FindNumberOfSides(map[coordinates], fence, map);

                costPart1 += region.Count * fence.Count;
                _part2 += region.Count * sides;

                regionsFound++;
                region.ToList().ForEach(x => visited.Add(x));
            }

            return costPart1;
        }

        public int Part2(IEnumerable<string> inputLines)
        {
            return _part2;
        }

        private int FindNumberOfSides(string regionSymbol, HashSet<((int Y, int X) From, (int Y, int X) To)> fence, Dictionary<(int Y, int X), string> map)
        {
            var sortedByX = fence.Select(f => (Y: (f.From.Y + f.To.Y) / 2.0, X: (f.From.X + f.To.X) / 2.0)).ToList().OrderBy(f => f.X).ThenBy(f => f.Y).ToList();
            var sortedByY = fence.Select(f => (Y: (f.From.Y + f.To.Y) / 2.0, X: (f.From.X + f.To.X) / 2.0)).ToList().OrderBy(f => f.Y).ThenBy(f => f.X).ToList();

            var verticalLines = new List<List<(double Y, double X)>>();
            var horizontalLines = new List<List<(double Y, double X)>>();

            Func<(double Y, double X), (double Y, double X), bool> areOnTheSameSideForVertical = (previous, current) =>
            {
                var previousXFloor = ((int)previous.Y, (int)Math.Floor(previous.X));
                var currentXFloor = ((int)current.Y, (int)Math.Floor(current.X));

                var previousXCeiling = ((int)previous.Y, (int)Math.Ceiling(previous.X));
                var currentXCeiling = ((int)current.Y, (int)Math.Ceiling(current.X));

                map.TryGetValue(previousXFloor, out var previousFloor);
                map.TryGetValue(currentXFloor, out var currentFloor);
                map.TryGetValue(previousXCeiling, out var previousCeiling);
                map.TryGetValue(currentXCeiling, out var currentCeiling);

                return (previousFloor == regionSymbol && currentFloor == regionSymbol) || (previousCeiling == regionSymbol && currentCeiling == regionSymbol);
            };

            Func<(double Y, double X), (double Y, double X), bool> areOnTheSameSideForHorizontal = (previous, current) =>
            {
                var previousXFloor = ((int)Math.Floor(previous.Y), (int)previous.X);
                var currentXFloor = ((int)Math.Floor(current.Y), (int)current.X);

                var previousXCeiling = ((int)Math.Ceiling(previous.Y), (int)previous.X);
                var currentXCeiling = ((int)Math.Ceiling(current.Y), (int)current.X);

                map.TryGetValue(previousXFloor, out var previousFloor);
                map.TryGetValue(currentXFloor, out var currentFloor);
                map.TryGetValue(previousXCeiling, out var previousCeiling);
                map.TryGetValue(currentXCeiling, out var currentCeiling);

                return (previousFloor == regionSymbol && currentFloor == regionSymbol) || (previousCeiling == regionSymbol && currentCeiling == regionSymbol);
            };

            var previous = (Y: -999.0, X: -999.0);
            foreach (var item in sortedByX)
            {
                if (item.X % 1 == 0)
                {
                    continue;
                }

                if (
                    item.X == previous.X && // item must be in line
                    Math.Abs(item.Y - previous.Y) == 1 && // and next to the previous one 
                    areOnTheSameSideForVertical(previous, item) // and on the same side
                )
                {
                    verticalLines.Last().Add(item);
                }
                else
                {
                    verticalLines.Add(new List<(double Y, double X)>());
                    verticalLines.Last().Add(item);
                }

                previous = item;
            }

            previous = (Y: -999.0, X: -999.0);
            foreach (var item in sortedByY)
            {
                if (item.Y % 1 == 0)
                {
                    continue;
                }

                if (
                    item.Y == previous.Y &&
                    Math.Abs(item.X - previous.X) == 1 &&
                    areOnTheSameSideForHorizontal(previous, item)
                )
                {
                    horizontalLines.Last().Add(item);
                }
                else
                {
                    horizontalLines.Add(new List<(double Y, double X)>());
                    horizontalLines.Last().Add(item);
                }

                previous = item;
            }

            return horizontalLines.Count + verticalLines.Count;
        }

        private (HashSet<(int Y, int X)> Region, HashSet<((int Y, int X) From, (int Y, int X) To)> Fence) FindRegion((int Y, int X) root, Dictionary<(int Y, int X), string> map)
        {
            var region = new HashSet<(int Y, int X)>();
            var fence = new HashSet<((int Y, int X) From, (int Y, int X) To)>();

            Func<(int Y, int X), (int Y, int X)[]> getNeighbours = coordinates => [
                (coordinates.Y - 1, coordinates.X),
                (coordinates.Y, coordinates.X + 1),
                (coordinates.Y + 1, coordinates.X),
                (coordinates.Y, coordinates.X - 1),
            ];

            var queue = new Queue<((int Y, int X) From, (int Y, int X) To)>();
            
            region.Add(root);
            getNeighbours(root).ToList().ForEach(x => queue.Enqueue((root, x)));
            
            while (queue.Count > 0)
            {
                var (from, current) = queue.Dequeue();
                if (map.ContainsKey(current) && map[current] == map[root])
                {
                    region.Add(current);
                    var neighbours = getNeighbours(current);
                    neighbours
                        .ToList()
                        .ForEach(n =>
                        {
                            if (queue.Contains((from, current)))
                            {
                                return;
                            }

                            if (region.Contains(n))
                            {
                                return;
                            }

                            if (fence.Contains((current, n)))
                            {
                                return;
                            }

                            queue.Enqueue((current, n));
                        });
                }
                else
                {
                    fence.Add((from, current));
                }
            }

            return (region, fence);
        }
    }
}
