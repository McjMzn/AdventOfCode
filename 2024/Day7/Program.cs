using AdventOfCode;
using System.Text.Json;

namespace Day7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day7_2024>(3749L, 11387L, verbose: false, runDemo: true);
        }
    }

    internal class Day7_2024 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            long result = 0;
            foreach (var line in inputLines)
            {
                var split = line.Split(':');
                var expectedResult = long.Parse(split[0]);

                var values = split[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(long.Parse).ToList();

                if (Test(expectedResult, values[0], values, 1, ['+', '*']))
                {
                    result += expectedResult;
                }
            }

            return result;
        }

        private bool Test(long expectedResult, long currentResult, List<long> numbers, int index, char[] operations)
        {
            foreach (var operation in operations)
            {
                if (Test(expectedResult, currentResult, numbers, index, operation, operations))
                {
                    return true;
                }
            }

            return false;
        }

        private bool Test(long expectedResult, long currentResult, List<long> numebrs, int index, char mathOperation, char[] operations)
        {
            var newResult = mathOperation switch
            {
                '+' => currentResult + numebrs[index],
                '*' => currentResult * numebrs[index],
                '|' => long.Parse($"{currentResult}{numebrs[index]}"),
            };

            if (newResult > expectedResult)
            {
                return false;
            }

            AdventOfCodeRunner.WriteLine(new string(' ', 2 * index), $"{currentResult} {mathOperation} {numebrs[index]} = {newResult}");
            if (index + 1 < numebrs.Count)
            {
                return Test(expectedResult, newResult, numebrs, index + 1, operations);
            }

            var success = newResult == expectedResult;
            if (success)
            {
                AdventOfCodeRunner.WriteLine(new string(' ', 2 * index), ConsoleColor.Green, success);
            }
            else
            {
                AdventOfCodeRunner.WriteLine(new string(' ', 2 * index), ConsoleColor.Red, success);
            }

            return success;
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            long result = 0;
            foreach (var line in inputLines)
            {
                var split = line.Split(':');
                var expectedResult = long.Parse(split[0]);

                var values = split[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(long.Parse).ToList();

                AdventOfCodeRunner.Verbose = false;

                checked
                {
                    if (Test(expectedResult, values[0], values, 1, ['+', '*', '|']))
                    {
                        result += expectedResult;
                    }
                }
            }

            return result;
        }
    }
}
