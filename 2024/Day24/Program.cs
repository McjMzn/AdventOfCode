using AdventOfCode;
using System.Collections;
using System.Text.RegularExpressions;

namespace Day24
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day24_2024>(2024, runDemo: false);
        }
    }

    internal enum GateType
    {
        OR,
        AND,
        XOR
    }

    internal class Gate
    {
        public static Dictionary<string, Gate> Gates { get; set; }

        public int? Override { get; set; }
        public string Name { get; set; }
        public GateType Type { get; set; }
        public string Input1 { get; set; }
        public string Input2 { get; set; }

        public int Output => GetOutput();

        public override string ToString()
        {
            return Override is not null ? $"{Name} = {Override.ToString()}" : $"{Input1} {Type} {Input2} -> {Name}";
        }

        private int GetOutput()
        {
            if (Override is not null)
            {
                return Override.Value;
            }

            switch (Type)
            {
                case GateType.OR:
                    return Gates[Input1].Output == 1 || Gates[Input2].Output == 1 ? 1 : 0;

                case GateType.AND:
                    return Gates[Input1].Output == 1 && Gates[Input2].Output == 1 ? 1 : 0;

                case GateType.XOR:
                    return Gates[Input1].Output != Gates[Input2].Output ? 1 : 0;
            }

            throw new Exception("Illegal case.");
        }
    }

    internal class Day24_2024 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            ProcessInput(inputLines);

            return DecodeGroup('z');
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            ProcessInput(inputLines);

            var x = DecodeGroup('x');
            var y = DecodeGroup('y');

            x = x + 3;
            y = y + 3;

            var z = DecodeGroup('z');


            var diff = (x + y) ^ z;
            var bits = new BitArray(new int[] { (int)diff });

            var wrongBits = bits.Cast<bool>().Select((value, index) => (value, index)).Where(x => x.value).Select(x => x.index).ToList();

            var dict = new Dictionary<int, HashSet<Gate>>();
            foreach (var bit in wrongBits)
            {
                var gateName = $"z{bit:00}";
                var sources = GetSourceGates(gateName);
                dict[bit] = sources.ToHashSet();
            }

            var sourcesOfErrors = wrongBits.Select(index => $"z{index:00}").SelectMany(name => GetSourceGates(name)).Where(gate => gate.Override is null).ToHashSet();

            for (var i = 0; i < Gate.Gates.Keys.Count(k => k.StartsWith('z')); i++)
            {
                if (wrongBits.Contains(i))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                var name = $"z{i:00}";
                var gate = Gate.Gates[name];
                var input1 = Gate.Gates[gate.Input1];
                var input2 = Gate.Gates[gate.Input2];
                Console.WriteLine($"{name}: {Gate.Gates[name].Type,-3} ({input1.Output} + {input2.Output}) {Gate.Gates[name].Output}");
                Console.ResetColor();
            }

            return 0;
        }

        private IEnumerable<Gate> GetSourceGates(string gateName)
        {
            var gate = Gate.Gates[gateName];
            yield return gate;

            if (gate.Input1 is null && gate.Input2 is null)
            {
                yield break;
            }

            foreach (var source in GetSourceGates(gate.Input1))
            {
                yield return source;
            }

            foreach (var source in GetSourceGates(gate.Input2))
            {
                yield return source;
            }
        }

        public long DecodeGroup(char groupSymbol)
        {
            return
                Gate
                    .Gates
                    .Keys
                    .Where(gateName => Regex.IsMatch(gateName, $@"{groupSymbol}\d+"))
                    .Select(gateName =>
                        Gate.Gates[gateName].Output * (long)Math.Pow(2, int.Parse(Regex.Match(gateName, @"\d+").Value))
                    )
                    .Sum();
        }

        private void ProcessInput(IEnumerable<string> inputLines)
        {
            Gate.Gates = new();

            Match match;
            foreach (var line in inputLines)
            {
                match = Regex.Match(line, @"(?<gateName>.*): (?<value>\d+)");
                if (match.Success)
                {
                    var gate = new Gate { Name = match.Groups["gateName"].Value, Override = int.Parse(match.Groups["value"].Value) };
                    Gate.Gates.Add(match.Groups["gateName"].Value, gate);
                    continue;
                }

                match = Regex.Match(line, @"(?<gateA>.*) (?<gateType>.*) (?<gateB>.*) -> (?<gateName>.*)");
                if (match.Success)
                {
                    var gate = new Gate
                    {
                        Name = match.Groups["gateName"].Value,
                        Input1 = match.Groups["gateA"].Value,
                        Input2 = match.Groups["gateB"].Value,
                        Type = Enum.Parse<GateType>(match.Groups["gateType"].Value),
                    };

                    Gate.Gates.Add(match.Groups["gateName"].Value, gate);
                }
            }
        }
    }
}
