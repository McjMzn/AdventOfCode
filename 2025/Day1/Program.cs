using AdventOfCode;

namespace Day1
{
    internal class Day1_2025 : IDailyChallenge<int>
    {
        public int Part1(IEnumerable<string> inputLines)
        {
            var result = 0;

            var position = 50;
            var dialSize = 100;

            foreach (var line in inputLines)
            {
                var multiplier = line.StartsWith("L") ? -1 : 1;
                var modifier = int.Parse(line.Substring(1));

                position = (position + multiplier * modifier) % dialSize;
                if (position == 0)
                {
                    result++;
                }
            }

            return result;
        }

        public int Part2(IEnumerable<string> inputLines)
        {
            var result = 0;

            var position = 50;
            var dialSize = 100;

            foreach (var line in inputLines)
            {
                var multiplier = line.StartsWith("L") ? -1 : 1;
                var modifier = int.Parse(line.Substring(1));
                var startedAtZero = position == 0;


                var fullSpins = modifier / dialSize;
                result += fullSpins;
                modifier = modifier % dialSize;

                position = (position + multiplier * modifier);
                if (!startedAtZero && (position >= dialSize || position <= 0))
                {
                    result++;
                }

                // Canonical modulus: -10 mod 20 = 10
                position = position % dialSize;
                if (position < 0)
                {
                    position = position + dialSize;
                }
            }

            return result;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day1_2025>(3, 6);
        }
    }
}
