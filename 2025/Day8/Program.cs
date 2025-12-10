using AdventOfCode;

namespace Day8
{
    internal class Day8_2025 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            var distances = LoadInput(inputLines);
            var conenctionsToMake = AdventOfCodeRunner.RunningDemo ? 10 : 1000;

            HashSet<((int X, int Y, int Z), (int X, int Y, int Z))> connected = new();
            var circuits = new List<HashSet<(int X, int Y, int Z)>>();

            for (var i = 0; i < conenctionsToMake; i++)
            {
                var closest = 
                    distances
                    .Where(kvp => !connected.Contains(kvp.Key))
                    .OrderBy(kvp => kvp.Value)
                    .First()
                    .Key;

                connected.Add(closest);

                var existingCircuits = circuits.Where(c => c.Contains(closest.Item1) || c.Contains(closest.Item2)).ToList();
                
                switch (existingCircuits.Count)
                {
                    case 0:
                        var circuit = new HashSet<(int X, int Y, int Z)>();
                        circuit.Add(closest.Item1);
                        circuit.Add(closest.Item2);
                        circuits.Add(circuit);

                        break;

                    case 1:
                        existingCircuits[0].Add(closest.Item1);
                        existingCircuits[0].Add(closest.Item2);
                        break;

                    case 2:
                        existingCircuits[1].ToList().ForEach(i => existingCircuits[0].Add(i));
                        existingCircuits[0].Add(closest.Item1);
                        existingCircuits[0].Add(closest.Item2);

                        circuits.Remove(existingCircuits[1]);

                        break;
                }
            }

            return circuits.Select(c => c.Count).OrderByDescending(x => x).Take(3).Aggregate(1, (aggregate, current) => aggregate * current);
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            var conenctionsToMake = AdventOfCodeRunner.RunningDemo ? 10 : 1000;
            
            var distances = LoadInput(inputLines);
            var queue = new Queue<((int X, int Y, int Z), (int X, int Y, int Z))>(distances.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key));

            HashSet<((int X, int Y, int Z), (int X, int Y, int Z))> connected = new();
            var circuits = new List<HashSet<(int X, int Y, int Z)>>();

            ((int X, int Y, int Z), (int X, int Y, int Z)) lastConnection;

            while (queue.Count > 0)
            {
                var closest = queue.Dequeue();
                
                connected.Add(closest);
                lastConnection = closest;

                var relatedCircuits = circuits.Where(c => c.Contains(closest.Item1) || c.Contains(closest.Item2)).ToList();
                switch (relatedCircuits.Count)
                {
                    case 0:
                        var circuit = new HashSet<(int X, int Y, int Z)>
                        {
                            closest.Item1,
                            closest.Item2
                        };

                        circuits.Add(circuit);

                        break;

                    case 1:
                        relatedCircuits[0].Add(closest.Item1);
                        relatedCircuits[0].Add(closest.Item2);
                        break;

                    case 2:
                        relatedCircuits[1].ToList().ForEach(i => relatedCircuits[0].Add(i));
                        relatedCircuits[0].Add(closest.Item1);
                        relatedCircuits[0].Add(closest.Item2);

                        circuits.Remove(relatedCircuits[1]);

                        break;
                }

                if (circuits.Count == 1 & circuits[0].Count == inputLines.Count())
                {
                    return lastConnection.Item1.X * lastConnection.Item2.X;
                }
            }

            return -1;
        }

        private Dictionary<((int X, int Y, int Z), (int X, int Y, int Z)), double> LoadInput(IEnumerable<string> inputLines)
        {
            var coordinates = new List<(int X, int Y, int Z)>();

            foreach (var line in inputLines)
            {
                var split = line.Split(',');
                var x = int.Parse(split[0]);
                var y = int.Parse(split[1]);
                var z = int.Parse(split[2]);

                coordinates.Add((x, y, z));
            }

            Dictionary<((int X, int Y, int Z), (int X, int Y, int Z)), double> distances = new();

            foreach (var a in coordinates)
            {
                foreach (var b in coordinates)
                {
                    if (a == b)
                    {
                        continue;
                    }

                    if (distances.ContainsKey((b, a)))
                    {
                        continue;
                    }

                    var key = (a, b);
                    var distance = Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2) + Math.Pow(b.Z - a.Z, 2));

                    distances.Add(key, distance);
                }
            }

            return distances;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day8_2025>(40, 25272, verbose: false);
        }
    }
}
