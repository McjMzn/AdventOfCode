using AdventOfCode;

namespace Day5
{
    internal class Day5_2025 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            var result = 0L;

            var (ranges, ids) = LoadInput(inputLines);
            foreach(var id in ids)
            {
                if (ranges.Any(r => r.Contains(id)))
                {
                    result++;
                }
            }

            return result;
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            var (ranges, ids) = LoadInput(inputLines);
            var mergedRanges = MergeRanges(ranges);

            return mergedRanges.Select(r => r.Length).Sum();
        }

        private (List<NumericalRange> Ranges, List<long> Ids) LoadInput(IEnumerable<string> inputLines)
        {
            var ranges = new List<NumericalRange>();
            var ids = new List<long>();

            foreach (var line in inputLines)
            {
                if (line.Contains('-'))
                {
                    var bounds = line.Split('-').Select(long.Parse).ToArray();
                    var range = new NumericalRange(bounds[0], bounds[1]);
                    ranges.Add(range);

                    continue;
                }

                if (long.TryParse(line, out var id))
                {
                    ids.Add(id);
                }
            }

            return (ranges, ids);
        }

        private List<NumericalRange> MergeRanges(List<NumericalRange> ranges)
        {
            var sorted = ranges.OrderBy(r => r.Start).ToList();
            var merged = new List<NumericalRange>();
            merged.Add(sorted[0]);

            foreach (var range in sorted.Skip(1))
            {
                var last = merged.Last();
                if (range.Start <= last.End)
                {
                    last.End = Math.Max(last.End, range.End);
                }
                else
                {
                    merged.Add(range);
                }
            }

            return merged;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day5_2025>(3, 14, verbose: false);
        }
    }
}
