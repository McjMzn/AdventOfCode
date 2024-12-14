using AdventOfCode;
using System.Text.RegularExpressions;

namespace Day13
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day13_2024>(480);
        }
    }




    internal class Day13_2024 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            return CountTokens(inputLines, 0);
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            return CountTokens(inputLines, 10000000000000);
        }

        private long CountTokens(IEnumerable<string> inputLines, long targetModifier)
        {
            var lines = inputLines.ToList();

            long count = 0;

            for (var i = 0; i < lines.Count; i++)
            {
                var a = Regex.Matches(lines[i], @"\d+").Cast<Match>().Select(x => long.Parse(x.Value)).ToList();
                var b = Regex.Matches(lines[i + 1], @"\d+").Cast<Match>().Select(x => long.Parse(x.Value)).ToList();
                var t = Regex.Matches(lines[i + 2], @"\d+").Cast<Match>().Select(x => long.Parse(x.Value) + targetModifier).ToList();
                i += 3;

                // a*ax + b*bx = tx
                // a*ay + b*by = ty
                // a = (tx - b*bx) / ax
                // b = (ty*ay - tx*ay) / (by*ax - bx*ay)

                var B = (t[1] * a[0] - t[0] * a[1]) / (b[1] * a[0] - b[0] * a[1]);
                var A = (t[0] - B * b[0]) / a[0];

                if (A >= 0 && B >= 0 && A % 1 == 0 && B % 1 == 0 && (A * a[0] + B * b[0] == t[0]) && (A * a[1] + B * b[1] == t[1]))
                {
                    count += (3 * A + B);
                }
            }

            return count;
        }
    }
}
