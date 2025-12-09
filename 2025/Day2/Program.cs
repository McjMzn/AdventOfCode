using AdventOfCode;

namespace Day2
{
    internal record class Range
    {
        public long Start { get; set; }

        public long End { get; set; }

        public Range(string range)
        {
            var split = range.Split('-');
            Start = long.Parse(split[0]);
            End = long.Parse(split[1]);
        }

        public IEnumerable<long> EnumerateInvalidIdsForPart1()
        {
            for (var id = Start; id <= End; id++)
            {
                if (IsInvalidForPart1(id))
                {
                    yield return id;
                }
            }
        }

        public IEnumerable<long> EnumerateInvalidIdsForPart2()
        {
            for (var id = Start; id <= End; id++)
            {
                if (IsInvalidForPart2(id))
                {
                    yield return id;
                }
            }
        }

        private bool IsInvalidForPart1(long id)
        {
            var stringified = id.ToString();

            // Length needs to be even if it's to be a pattern repeated twice.
            if (stringified.Length % 2 == 1)
            {
                return false;
            }

            var firstHalf = stringified.Substring(0, stringified.Length / 2);
            var secondHalf = stringified.Substring(stringified.Length / 2);

            return firstHalf == secondHalf;
        }

        private bool IsInvalidForPart2(long id)
        {
            var stringified = id.ToString();
            var maxSegmentLength = stringified.Length / 2;

            for (var segmentLength = 1; segmentLength <= maxSegmentLength; segmentLength++)
            {
                if (stringified.Length % segmentLength != 0)
                {
                    continue;
                }

                var segmentsCount = stringified.Length / segmentLength;
                HashSet<string> segments = new();

                for (var i = 0; i < segmentsCount; i++)
                {
                    var segment = stringified.Substring(0 + i * segmentLength, segmentLength);
                    segments.Add(segment);
                }

                if (segments.Count == 1)
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal class Day2_2025 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            var result = 0L;
            var ranges = inputLines.First().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => new Range(x)).ToList();
            foreach (var range in ranges)
            {
                foreach(var invalidId in range.EnumerateInvalidIdsForPart1())
                {
                    result += invalidId;
                    AdventOfCodeRunner.WriteLine(invalidId);
                }
            }

            return result;
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            var result = 0L;
            var ranges = inputLines.First().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => new Range(x)).ToList();
            foreach (var range in ranges)
            {
                foreach (var invalidId in range.EnumerateInvalidIdsForPart2())
                {
                    result += invalidId;
                    AdventOfCodeRunner.WriteLine(invalidId);
                }
            }

            return result;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day2_2025>(1227775554, 4174379265, verbose: false);
        }
    }
}
