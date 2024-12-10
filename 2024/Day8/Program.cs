using AdventOfCode;

namespace Day8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day8_2024>(14, 34);
        }
    }

    internal record class Node
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Node Rounded()
        {
            var threshold = 0.001;
            
            var roundedX = Math.Round(X);
            var roundedY = Math.Round(Y);

            return new Node
            {
                X = Math.Abs(roundedX - X) < threshold ? roundedX : X,
                Y = Math.Abs(roundedY - Y) < threshold ? roundedY : Y
            };
        }
    }

    internal class Day8_2024 : IDailyChallenge<int>
    {
        public int Part1(IEnumerable<string> inputLines)
        {
            var antennas = ProcessInputLines(inputLines);
            var antinodes = new List<Node>();

            foreach(var type in antennas.Keys)
            {
                var sameAntennas = antennas[type];
                foreach (var a1 in sameAntennas)
                {
                    foreach(var a2 in sameAntennas)
                    {
                        if (a1 == a2)
                        {
                            continue;
                        }

                        var firstAntinodesPair = FindAntinodes(a1.X, a1.Y, a2.X, a2.Y);
                        var secondAntinodesPair = FindAntinodes(a2.X, a2.Y, a1.X, a1.Y);

                        antinodes.Add(firstAntinodesPair.Antinode1.Rounded());
                        antinodes.Add(firstAntinodesPair.Antinode2.Rounded());
                        antinodes.Add(secondAntinodesPair.Antinode1.Rounded());
                        antinodes.Add(secondAntinodesPair.Antinode1.Rounded());
                    }
                }
            }

            var validAntinodes =
                antinodes.Where(antinode =>
                    antinode.X % 1 == 0 &&
                    antinode.Y % 1 == 0 &&
                    antinode.X >= 0 && antinode.X < inputLines.First().Length &&
                    antinode.Y >= 0 && antinode.Y < inputLines.Count()
                )
                .ToList();

            return new HashSet<Node>(validAntinodes).Count();
        }

        public int Part2(IEnumerable<string> inputLines)
        {
            var antennas = ProcessInputLines(inputLines);
            var antinodes = new List<Node>();

            foreach (var type in antennas.Keys)
            {
                var sameAntennas = antennas[type];
                foreach (var a1 in sameAntennas)
                {
                    foreach (var a2 in sameAntennas)
                    {
                        if (a1 == a2)
                        {
                            continue;
                        }

                        var (a, b) = FindLinearFunction(a1.X, a1.Y, a2.X, a2.Y);

                        for (var y = 0; y < inputLines.Count(); y++)
                        {
                            for (var x = 0; x < inputLines.First().Length; x++)
                            {
                                if (MatchesLinearFunction(x, y, a, b))
                                {
                                    antinodes.Add(new Node() { X = x, Y = y });
                                }
                            }
                        }
                    }
                }
            }

            return new HashSet<Node>(antinodes).Count;
        }

        private Dictionary<char, List<Node>> ProcessInputLines(IEnumerable<string> inputLines)
        {
            var inputList = inputLines.ToList();

            var width = inputLines.First().Length;
            var height = inputLines.Count();

            var antennas = new Dictionary<char, List<Node>>();

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var symbol = inputList[y][x];
                    if (symbol == '.')
                    {
                        continue;
                    }

                    if (!antennas.ContainsKey(symbol))
                    {
                        antennas[symbol] = new List<Node>();
                    }

                    antennas[symbol].Add(new Node { X = x, Y = y });
                }
            }

            return antennas;
        }

        private bool MatchesLinearFunction(int x, int y, double a, double b)
        {
            var result = a * x + b;
         
            return Math.Abs(y - result) < 0.001;
        }

        private (double A, double B) FindLinearFunction(double x1, double y1, double x2, double y2)
        {
            var a = (double)(y2 - y1) / (x2 - x1);
            var b = y1 - a * x1;

            return (a, b);
        }

        private (double Solution1, double Solution2) SolveQuadraticEquation(double a, double b, double c)
        {
            var delta = Math.Pow(b, 2) - (4 * a * c);

            var solution1 = (-b + Math.Sqrt(delta)) / (2 * a);
            var solution2 = (-b - Math.Sqrt(delta)) / (2 * a);

            return (solution1, solution2);
        }

        private (Node Antinode1, Node Antinode2) FindAntinodes(double x1, double y1, double x2, double y2)
        {
            var (a, b) = FindLinearFunction(x1, y1, x2, y2);

            var qA = (3 * a * a) + 3;
            var qB = (6 * a * b) + (2 * x1) + (2 * y1 * a) - (8 * x2) - (8 * y2 * a);
            var qC = (3 * b * b) + (2 * y1 * b) - (8 * y2 * b) - (x1 * x1) - (y1 * y1) + (4 * x2 * x2) + (4 * y2 * y2);

            var (antinodeX1, antinodeX2) = SolveQuadraticEquation(qA, qB, qC);

            var antinode1 = new Node { X = antinodeX1, Y = a * antinodeX1 + b };
            var antinode2 = new Node { X = antinodeX2, Y = a * antinodeX2 + b };

            return (antinode1, antinode2);
        }
    }
}
