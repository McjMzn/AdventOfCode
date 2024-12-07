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

                AdventOfCodeRunner.Verbose = false;

                checked
                {
                    if (Test(expectedResult, values[0], values, 1, string.Empty))
                    {
                        Console.WriteLine($"{expectedResult} ~ [{string.Join(", ", values)}]");
                        result += expectedResult;
                    }
                }
            }

            Console.WriteLine();
            return result;
        }

        private bool Test(long expectedResult, long currentResult, List<long> numbers, int index, string usedOperators)
        {
            return
                Test(expectedResult, currentResult, numbers, index, '+', usedOperators) ||
                Test(expectedResult, currentResult, numbers, index, '*', usedOperators);
        }

        private bool Test(long expectedResult, long currentResult, List<long> numebrs, int index, char mathOperation, string usedOperators)
        {
            var newResult = mathOperation switch
            {
                '+' => currentResult + numebrs[index],
                '*' => currentResult * numebrs[index],
                '|' => long.Parse($"{currentResult}{numebrs[index]}"),
            };


            AdventOfCodeRunner.WriteLine(new string(' ', 2 * index), $"{currentResult} {mathOperation} {numebrs[index]} = {newResult}");
            if (index + 1 < numebrs.Count)
            {
                return Test(expectedResult, newResult, numebrs, index + 1, usedOperators + mathOperation);
            }


            var success = newResult == expectedResult;
            if (success)
            {
                Console.Write($"{usedOperators} ~ ");
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
            return 0;
        }
    }
}
