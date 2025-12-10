using AdventOfCode;

namespace Day9
{
    internal class Day9_2025 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            var coordinates = new List<(long Y, long X)>();
            foreach (var line in inputLines)
            {
                var split = line.Split(',');
                var x = long.Parse(split[0]);
                var y = long.Parse(split[1]);

                coordinates.Add((y, x));
            }

            var areas = new List<long>();
            foreach (var a in coordinates)
            {
                foreach(var b in coordinates)
                {
                    if (a == b)
                    {
                        continue;
                    }

                    var width = Math.Abs(a.X - b.X) + 1;
                    var height = Math.Abs(a.Y - b.Y) + 1;

                    var area = width * height;
                    
                    areas.Add(area);
                }
            }

            return areas.Max();
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            return 0L;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day9_2025>(50, -1, verbose: false);
        }
    }
}
