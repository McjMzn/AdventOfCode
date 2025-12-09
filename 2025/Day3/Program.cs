using AdventOfCode;

namespace Day3
{
    internal class Day3_2025 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            var result = 0L;
            foreach(var line in inputLines)
            {
                var joltages = line.ToCharArray().Select(x => int.Parse($"{x}")).ToList();
                var joltage = FindOptimalJoltage(joltages, 2);
                result += joltage;

                AdventOfCodeRunner.WriteLine(joltage);
            }

            return result;
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            var result = 0L;
            foreach (var line in inputLines)
            {
                var joltages = line.ToCharArray().Select(x => int.Parse($"{x}")).ToList();
                var joltage = FindOptimalJoltage(joltages, 12);
                result += joltage;

                AdventOfCodeRunner.WriteLine(joltage);
            }

            return result;
        }

        private long FindOptimalJoltage(List<int> joltages, int numberOfBatteries)
        {
            var selected = new List<int>();
            while (numberOfBatteries > 0)
            {
                var joltage = joltages.Take(joltages.Count - (numberOfBatteries - 1)).Max();
                var joltageIndex = joltages.IndexOf(joltage);
                selected.Add(joltage);
                joltages = joltages.Skip(joltageIndex + 1).ToList();
                numberOfBatteries--;
            }

            selected.Reverse();
            return (long)selected.Select((x, i) => x * Math.Pow(10, i)).Sum();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day3_2025>(357, 3121910778619, verbose: false);
        }
    }
}
