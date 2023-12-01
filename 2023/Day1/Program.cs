using AdventOfCode;
using System.Text.RegularExpressions;

public class Program
{
    static void Main(string[] args)
    {
        var digits = new Dictionary<string, int>
        {
            ["1"] = 1,
            ["2"] = 2,
            ["3"] = 3,
            ["4"] = 4,
            ["5"] = 5,
            ["6"] = 6,
            ["7"] = 7,
            ["8"] = 8,
            ["9"] = 9,

            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3,
            ["four"] = 4,
            ["five"] = 5,
            ["six"] = 6,
            ["seven"] = 7,
            ["eight"] = 8,
            ["nine"] = 9,
        };

        int sumPart1 = 0;
        int sumPart2 = 0;

        Input.Process(line =>
        {
            // Part 1
            var firstDigitChar = line.First(c => c > '0' && c <= '9');
            var lastDigitChar = line.Last(c => c > '0' && c <= '9');

            sumPart1 += 10 * int.Parse($"{firstDigitChar}") + int.Parse($"{lastDigitChar}");

            // Part 2
            var match = new List<(int Index, int Digit)>();

            for (var i = 0; i < line.Length; i++)
            {
                foreach(var digit in digits)
                {
                    if (line.Substring(i).StartsWith(digit.Key))
                    {
                        match.Add((i, digit.Value));
                    }
                }
            }

            var found = match.OrderBy(m => m.Index).Select(m => m.Digit).ToList();
            sumPart2 += 10 * found.First() + found.Last();

        });

        Console.WriteLine($"Part 1: {sumPart1}");
        Console.WriteLine($"Part 2: {sumPart2}");
    }
}