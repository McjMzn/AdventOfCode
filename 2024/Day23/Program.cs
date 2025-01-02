using AdventOfCode;
using System.ComponentModel.DataAnnotations;

namespace Day23
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day23_2024>(7);
        }
    }

    internal class Day23_2024 : IDailyChallenge<object>
    {
        public object Part1(IEnumerable<string> inputLines)
        {
            var connectionsDictionary = ProcessInput(inputLines);

            var networks = new HashSet<string>();
            foreach (var computer in connectionsDictionary.Keys.Where(c => c.StartsWith('t')))
            {
                foreach (var a in connectionsDictionary.Keys)
                {
                    foreach (var b in connectionsDictionary.Keys)
                    {
                        if (connectionsDictionary[computer].Contains(a) && connectionsDictionary[computer].Contains(b) && connectionsDictionary[a].Contains(b))
                        {
                            networks.Add(string.Join(" ", new List<string> { computer, a, b }.Order()));
                        }
                    }
                }
            }
            
            return networks.Count();
        }

        public object Part2(IEnumerable<string> inputLines)
        {
            var connectionsDictionary = ProcessInput(inputLines);

            var networks = BronKerbosch([], connectionsDictionary.Keys.ToList(), [], connectionsDictionary).ToList();

            return networks.OrderByDescending(r => r.Length).First();
        }

        private IEnumerable<string> BronKerbosch(List<string> r, List<string> p, List<string> x, Dictionary<string, List<string>> connections)
        {
            if (p.Count() + x.Count() == 0)
            {
                yield return string.Join(",", r.Order());
                yield break;
            }

            foreach (var computer in p.ToList())
            {
                var connected = connections[computer];

                var found = BronKerbosch(r.Concat([computer]).ToList(), p.Intersect(connected).ToList(), x.Intersect(connected).ToList(), connections);
                foreach (var f in found)
                {
                    yield return f;
                }
                

                p.Remove(computer);
                x.Add(computer);
            }
        }

        private Dictionary<string, List<string>> ProcessInput(IEnumerable<string> inputLines)
        {
            var connections = new List<(string ComputerA, string ComputerB)>();

            foreach (var line in inputLines)
            {
                var split = line.Split('-');
                connections.Add((split[0], split[1]));
            }

            var connectionsDictionary = new Dictionary<string, List<string>>();

            foreach (var connection in connections)
            {
                connectionsDictionary.TryAdd(connection.ComputerA, new());
                connectionsDictionary.TryAdd(connection.ComputerB, new());

                connectionsDictionary[connection.ComputerA].Add(connection.ComputerB);
                connectionsDictionary[connection.ComputerB].Add(connection.ComputerA);
            }

            return connectionsDictionary;
        }
    }
}
