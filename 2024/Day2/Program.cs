using AdventOfCode;

namespace Day2
{
    internal class Program
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

        static void Main(string[] args)
        {
            var safeReportsPart1 = 0;
            var safeReportsPart2 = 0;
            
            Input.LoadLines().ForEach(line =>
            {
                var levels = line.Split(' ').Select(l => int.Parse(l)).ToList();
                
                if (IsSafe(levels))
                {
                    safeReportsPart1++;
                }

                for (var i = 0; i < levels.Count; i++)
                {
                    var alteredLevels = levels.Where((level, index) => index != i).ToList();
                    if (IsSafe(alteredLevels))
                    {
                        safeReportsPart2++;
                        return;
                    }
                }

            });

            Output.Part1(safeReportsPart1);
            Output.Part2(safeReportsPart2);
        }
    }
}
