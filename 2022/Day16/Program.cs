using AdventOfCode;
using AdventOfCode.Extensions;
using System.Text.RegularExpressions;

namespace Day16
{
    internal class Valve
    {
        public string Name { get; set; }
        public int FlowRate { get; set; }
        public List<Valve> Tunnels { get; set; }
        public Dictionary<Valve, int> TimeCosts { get; set; }

        public bool IsOpen { get; set; }

        public override string ToString()
        {
            return $"{this.Name}";
            return $"[{(this.IsOpen ? " " : "X")}] {this.Name} ({this.FlowRate}) => [ {string.Join(", ", this.Tunnels.Select(v => v.Name))} ]";
        }

        public Dictionary<Valve, int> GetDistances()
        {
            var distances = new Dictionary<Valve, int>() { [this] = 0 };
            this.GetDistances(this, new(), distances, 0);

            return distances;
        }

        private void GetDistances(Valve valve, List<Valve> visited, Dictionary<Valve, int> distances, int distance)
        {
            visited.Add(valve);
            foreach(var v in valve.Tunnels)
            {
                if (!distances.ContainsKey(v))
                {
                    distances.Add(v, distance + 1);
                }

                
                if (distances[v] > distance + 1)
                {
                    distances[v] = distance + 1;
                    visited.Remove(v);
                }

                if (!visited.Contains(v))
                {
                    GetDistances(v, visited, distances, distances[v]);
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var valves = new Dictionary<string, Valve>();
            var tunnels = new Dictionary<string, List<string>>();

            Input.Process(line =>
            {
                var match = Regex.Match(line, @"Valve (?<valve>..) has flow rate=(?<flow>\d+); tunnels? leads? to valves? (?<valves>.*)");
                var valve = new Valve
                {
                    Name = match.Groups["valve"].Value,
                    FlowRate = int.Parse(match.Groups["flow"].Value)
                };

                valves[valve.Name] = valve;
                tunnels[valve.Name] = match.Groups["valves"].Value.Split(',').Select(v => v.Trim()).ToList();
            });

            foreach(var kvp in tunnels)
            {
                var valve = valves[kvp.Key];
                valve.Tunnels = tunnels[valve.Name].Select(t => valves[t]).ToList();
            }

            foreach(var valve in valves.Values)
            {
                valve.TimeCosts = valve.GetDistances().ToDictionary(x => x.Key, x => x.Value + 1);
            }

            // Part 1
            var valvesOfInterest = valves.Values.Where(v => v.FlowRate > 0).ToList();
            var maxRelease = Search(valvesOfInterest, new List<Valve>(), valves["AA"], 30);

            Console.WriteLine($"Part 1: {maxRelease}");

            // Part 2
            var part2 = 0;
            var variants = Math.Pow(2, valvesOfInterest.Count);
            Parallel.For(0, (int)variants, (i, state) =>
            {
                var masked = valvesOfInterest.GetByBitMask(i);
                var rest = valvesOfInterest.Except(masked).ToList();

                var me = Search(masked, new List<Valve>(), valves["AA"], 26);
                var elephant = Search(rest, new List<Valve>(), valves["AA"], 26);
                if (me + elephant > part2)
                {
                    part2 = me + elephant;
                }
            });

            Console.WriteLine($"Part 2: {part2}");
        }

        private static int Search(List<Valve> valves, List<Valve> openedValves, Valve currentValve, int minutesRemaining)
        {
            int maxRelease = 0;
            foreach(var valve in valves.Except(openedValves))
            {
                var timeCost = currentValve.TimeCosts[valve];
                if (minutesRemaining - timeCost < 0)
                {
                    continue;
                }

                var pressureRelease = (minutesRemaining - timeCost) * valve.FlowRate;
                var recursiveRelease = Search(valves, openedValves.Concat(new[] { valve }).ToList(), valve, minutesRemaining - timeCost);
                if (pressureRelease + recursiveRelease > maxRelease)
                {
                    maxRelease = pressureRelease + recursiveRelease;
                }
            }

            return maxRelease;
        }
    }
}