using AdventOfCode;

namespace Day2
{
    internal class Day2_2024 : IDailyChallenge<int>
    {
        static bool IsSafe(List<int> levels)
        {
            var increasing = true;
            var decreasing = true;

            var thresholdStart = 1;
            var thresholdEnd = 3;

            for (var i = 0; i < levels.Count - 1; i++)
            {
                var diff = Math.Abs(levels[i] - levels[i + 1]);
                if (diff < thresholdStart || diff > thresholdEnd)
                {
                    return false;
                }

                increasing = increasing && (levels[i + 1] > levels[i]);
                decreasing = decreasing && (levels[i + 1] < levels[i]);

                if (increasing || decreasing)
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        public int Part1(IEnumerable<string> inputLines)
        {
            var safeReports = 0;
            foreach (var line in inputLines)
            {
                var levels = line.Split(' ').Select(l => int.Parse(l)).ToList();
                if (IsSafe(levels))
                {
                    safeReports++;
                }
            }

            return safeReports;
        }

        public int Part2(IEnumerable<string> inputLines)
        {
            var safeReports = 0;
            foreach (var line in inputLines)
            {
                var levels = line.Split(' ').Select(l => int.Parse(l)).ToList();
                for (var i = 0; i < levels.Count; i++)
                {
                    var alteredLevels = levels.Where((level, index) => index != i).ToList();
                    if (IsSafe(alteredLevels))
                    {
                        safeReports++;
                        break;
                    }
                }
            }

            return safeReports;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day2_2024>(2, 4);
        }
    }
}
