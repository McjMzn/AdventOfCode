using AdventOfCode;
using System.Text.RegularExpressions;

namespace Day3
{
    internal class Day3_2024 : IDailyChallenge<int>
    {
        public int Part1(IEnumerable<string> inputLines)
        {
            var input = string.Join(Environment.NewLine, inputLines);

            return 
                Regex
                .Matches(input, @"mul\((?<arg1>\d+),(?<arg2>\d+)\)")
                .Cast<Match>()
                .Select(m => int.Parse(m.Groups["arg1"].Value) * int.Parse(m.Groups["arg2"].Value))
                .Sum();
        }

        public int Part2(IEnumerable<string> inputLines)
        {
            var input = string.Join(Environment.NewLine, inputLines);

            var result = 0;
            var mulEnabled = true;
            var matches = 
                Regex
                .Matches(input, @"(mul\((?<arg1>\d+),(?<arg2>\d+)\)|do\(\)|don't\(\))")
                .Cast<Match>()
                .ToList();

            foreach(var match in matches )
            {
                switch (match.Value)
                {
                    case "do()":
                        mulEnabled = true;
                        break;

                    case "don't()":
                        mulEnabled = false;
                        break;

                    default:
                        if (mulEnabled)
                        {
                            result += int.Parse(match.Groups["arg1"].Value) * int.Parse(match.Groups["arg2"].Value);
                        }

                        break;
                }
            }

            return result;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day3_2024>(161, 48);
        }
    }
}
