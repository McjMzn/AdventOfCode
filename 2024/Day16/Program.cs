using AdventOfCode;
using AdventOfCode.Grids;

namespace Day16
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day16_2024>(11048);
        }
    }

    internal class Node
    {
        public Coordinates Coordinates { get; set; }
        
        public char Symbol { get; set; }
        public bool IsWall { get; set; }
        public bool IsStart { get; set; }
        public bool IsEnd { get; set; }

        public bool IsInBestPath { get; set; }

        public (int Cost, Vector2d Vector) Arrival { get; set; }

        public List<(int Cost, Vector2d Vector)> Arrivals { get; set; } = new();

        public override string ToString()
        {
            if (IsWall) return "#";
            if (IsStart) return "S";
            if (IsEnd) return "E";

            if (Arrival.Vector == (-1, 0)) return "^";
            if (Arrival.Vector == (1, 0)) return "v";
            if (Arrival.Vector == (0, 1)) return ">";
            if (Arrival.Vector == (0, -1)) return "<";
            
            return ".";
        }
    }


    internal class Day16_2024 : IDailyChallenge<int>
    {
        Dictionary<Coordinates, Node> _map;

        public int Part1(IEnumerable<string> inputLines)
        {
            _map = ProcessInput(inputLines);

            var start = _map.Single(kvp => kvp.Value.IsStart).Value;
            start.Arrivals.Add((0, Vector2d.Right));
            var queue = new List<Coordinates>
            {
                start.Coordinates
            };

            while (queue.Count > 0)
            {
                var nodeCoordinates = queue.OrderBy(x => _map[x].Arrivals.Select(a => a.Cost).Min()).First();
                queue.Remove(nodeCoordinates);

                var node = _map[nodeCoordinates];

                var neighbours = new List<Coordinates>
                {
                    node.Coordinates.Translated(Vector2d.Up),
                    node.Coordinates.Translated(Vector2d.Down),
                    node.Coordinates.Translated(Vector2d.Left),
                    node.Coordinates.Translated(Vector2d.Right),
                }
                .Where(c => _map.ContainsKey(c) && !_map[c].IsWall)
                .ToList();

                foreach (var neighbour in neighbours)
                {
                    Vector2d vector = (neighbour.Y - node.Coordinates.Y, neighbour.X - node.Coordinates.X);
                    
                    foreach(var arrival in node.Arrivals)
                    {
                        var stepCost =
                            arrival.Vector == vector ? 1 :
                            arrival.Vector.X == -vector.X && arrival.Vector.X == -vector.X ? 2001 :
                            1001;

                        var currentCost = arrival.Cost;


                        var hasSameVectorArrival = _map[neighbour].Arrivals.Any(a => a.Vector == vector);
                        if (!hasSameVectorArrival)
                        {
                            _map[neighbour].Arrivals.Add((currentCost + stepCost, vector));
                            queue.Add(neighbour);
                            continue;
                        }

                        var sameVectorArrival = _map[neighbour].Arrivals.FirstOrDefault(a => a.Vector == vector);
                        var hasMoreExpensiveSameVectorArrival = hasSameVectorArrival && currentCost + stepCost < sameVectorArrival.Cost;
                        if (hasMoreExpensiveSameVectorArrival)
                        {
                            _map[neighbour].Arrivals.Remove(sameVectorArrival);
                            _map[neighbour].Arrivals.Add((currentCost + stepCost, vector));
                            queue.Add(neighbour);
                            continue;
                        }
                    }
                }
            }
                       
            return _map.Single(kvp => kvp.Value.IsEnd).Value.Arrivals.OrderBy(a => a.Cost).First().Cost;
        }

        public int Part1_(IEnumerable<string> inputLines)
        {
            var map = ProcessInput(inputLines);
            var queue = new PriorityQueue<Coordinates, int>();

            queue.Enqueue(map.Single(kvp => kvp.Value.IsStart).Value.Coordinates, 0);

            Predicate<Coordinates> isToBeVisited = coordinates => map.ContainsKey(coordinates) && !map[coordinates].IsWall;

            Action<Coordinates, Vector2d, int> enqueueIfApplicable = (coordinates, vector, cost) =>
            {
                if (!isToBeVisited(coordinates))
                {
                    return;
                }

                var node = map[coordinates];
                if (cost < node.Arrival.Cost)
                {
                    node.Arrival = (cost, vector);
                    queue.Enqueue(coordinates, cost);
                }
                else if ((vector == node.Arrival.Vector.TurnedRight()) || vector == node.Arrival.Vector.TurnedLeft() && cost < node.Arrival.Cost + 1000)
                {
                    node.Arrival = (cost, vector);
                    queue.Enqueue(coordinates, cost);
                }

            };

            while (queue.Count > 0)
            {
                var currentLocation = queue.Dequeue();
                Print(map, currentLocation);

                var currentNode = map[currentLocation];
                if (currentNode.IsEnd)
                {
                    break;
                }

                var currentCost = map[currentLocation].Arrival.Cost;
                var currentVector = map[currentLocation].Arrival.Vector;
                
                var turnedLeftVector = currentVector.TurnedLeft();
                var turnedRightVector = currentVector.TurnedRight();

                var inlineCoordinates = currentLocation.Translated(currentVector);
                var toLeftCoordinates = currentLocation.Translated(turnedLeftVector);
                var toRightCoordinates = currentLocation.Translated(turnedRightVector);

                enqueueIfApplicable(inlineCoordinates, currentVector, currentCost + 1);
                enqueueIfApplicable(toLeftCoordinates, turnedLeftVector, currentCost + 1001);
                enqueueIfApplicable(toRightCoordinates, turnedRightVector, currentCost + 1001);
            }


            var endNode = map.Single(kvp => kvp.Value.IsEnd).Value;

            var part2Queue = new Queue<Coordinates>();
            part2Queue.Enqueue(endNode.Coordinates);
            var counter = 0;
            while (part2Queue.Count > 0)
            {
                counter++;
                var coordinates = part2Queue.Dequeue();
                map[coordinates].IsInBestPath = true;
                Print2(map, map.Single(kvp => kvp.Value.IsStart).Value.Coordinates);
                var nextNodes =
                new List<Coordinates>
                {
                    coordinates.Translated(Vector2d.Up),
                    coordinates.Translated(Vector2d.Down),
                    coordinates.Translated(Vector2d.Left),
                    coordinates.Translated(Vector2d.Right)
                }
                .Where(c => map[c].Arrival.Cost < map[coordinates].Arrival.Cost)
                .ToList();

                nextNodes.ForEach(c => part2Queue.Enqueue(c));
            }

            Print2(map, map.Single(kvp => kvp.Value.IsStart).Value.Coordinates);


            return endNode.Arrival.Cost;
        }

        public int Part2(IEnumerable<string> inputLines)
        {
            var end = _map.Single(kvp => kvp.Value.IsEnd).Value;
            var queue = new Queue<(Coordinates Coordinates, int target)>();
            queue.Enqueue((end.Coordinates, end.Arrivals.MinBy(a => a.Cost).Cost));
            
            while (queue.Count > 0)
            {
                var (coordinates, target) = queue.Dequeue();
                _map[coordinates].IsInBestPath = true;
                if (_map[coordinates].IsStart)
                {
                    break;
                }

                var previousNodes =
                new List<Coordinates>
                {
                    coordinates.Translated(Vector2d.Up),
                    coordinates.Translated(Vector2d.Down),
                    coordinates.Translated(Vector2d.Left),
                    coordinates.Translated(Vector2d.Right)
                }
                .Where(c => _map.ContainsKey(c) && !_map[c].IsWall)
                .ToList();

                var toQueue = 
                _map[coordinates]
                    .Arrivals
                    .Where(a => a.Cost == target || target - a.Cost == 1 || target - a.Cost == 1001)
                    .Select(a => ((coordinates.Y - a.Vector.Y, coordinates.X - a.Vector.X), a.Cost))
                    .ToList();

                toQueue.ForEach(x => queue.Enqueue(x));
            }

            return _map.Count(kvp => kvp.Value.IsInBestPath);
        }

        private Dictionary<Coordinates, Node> ProcessInput(IEnumerable<string> inputLines)
        {
            var lines = inputLines.ToList();
            
            var map = new Dictionary<Coordinates, Node>();

            for (var y = 0; y < lines.Count; y++)
            {
                for (var x = 0; x < lines[y].Length; x++)
                {
                    var symbol = lines[y][x];
                    map[(y, x)] = new Node
                    {
                        Symbol = symbol,
                        Coordinates = (y, x),
                        IsStart = symbol == 'S',
                        IsEnd = symbol == 'E',
                        IsWall = symbol == '#',
                        Arrival = symbol == 'S' ? (0, Vector2d.Right) : (int.MaxValue, (-999, -999)),
                        IsInBestPath = false
                    };
                }
            }

            return map;
        }

        private void Print(Dictionary<Coordinates, Node> map, Coordinates current)
        {
            if (!AdventOfCodeRunner.RunningDemo)
            {
                return;
            }

            if (current is not null)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{current}: {map[current].Arrival.Cost}");
            }

            Console.ResetColor();
            map.GroupBy(kvp => kvp.Key.Y).OrderBy(g => g.Key).ToList().ForEach(group =>
            {
                group.OrderBy(g => g.Key.X).ToList().ForEach(g =>
                {
                    if (g.Value.Coordinates == current)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    
                    Console.Write(g.Value);
                    Console.ResetColor();
                });
                Console.WriteLine();
            });

            Console.WriteLine();
        }

        private void Print2(Dictionary<Coordinates, Node> map, Coordinates current)
        {
            if (!AdventOfCodeRunner.RunningDemo)
            {
                return;
            }

            if (current is not null)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{current}: {map[current].Arrival.Cost}");
            }

            Console.ResetColor();
            map.GroupBy(kvp => kvp.Key.Y).OrderBy(g => g.Key).ToList().ForEach(group =>
            {
                group.OrderBy(g => g.Key.X).ToList().ForEach(g =>
                {
                    Console.ForegroundColor = 
                        g.Value.IsInBestPath ? ConsoleColor.Green :
                        g.Value.IsWall ? ConsoleColor.DarkGray :
                        ConsoleColor.Gray;

                    if (g.Value.IsWall || g.Value.IsStart || g.Value.IsEnd)
                    {
                        Console.Write($"{new string(g.Value.Symbol, 6)} ");
                    }                    
                    else
                    {
                        Console.Write($"{g.Value.Arrivals.OrderBy(a => a.Cost).First().Cost.ToString("000000")} ");
                    }

                    Console.ResetColor();
                });
                Console.WriteLine();
            });

            Console.WriteLine();
        }
    }
}
