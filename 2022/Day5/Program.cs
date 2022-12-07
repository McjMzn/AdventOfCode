using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.TwentyTwentyTwo.Day5;


internal class Program
{
    static void Main(string[] args)
    {
        // Part 1
        var part1Result = OperateCrane(false);
        Console.Write("Part 1: ");
        part1Result.Values.ToList().ForEach(stack => Console.Write(stack.Peek()));
        Console.WriteLine();

        // Part 2
        var part2Result = OperateCrane(true);
        Console.Write("Part 2: ");
        part2Result.Values.ToList().ForEach(stack => Console.Write(stack.Peek()));
        Console.WriteLine();
    }

    private static Dictionary<int, Stack<char>> OperateCrane(bool useCrateMover9001)
    {
        const int numberOfStacks = 9;
        var stacksLoaded = false;
        var stacks = new Dictionary<int, Stack<char>>();
        for (var i = 1; i <= numberOfStacks; i++)
        {
            stacks.Add(i, new Stack<char>());
        }

        Input.Process(line =>
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                stacks.Keys.ToList().ForEach(key => stacks[key] = new Stack<char>(stacks[key]));
                stacksLoaded = true;
                return;
            }

            if (stacksLoaded)
            {
                var match = Regex.Match(line, @"move (?<count>\d+) from (?<source>\d+) to (?<destination>\d+)");
                var count = int.Parse(match.Groups["count"].Value);
                var source = stacks[int.Parse(match.Groups["source"].Value)];
                var destination = stacks[int.Parse(match.Groups["destination"].Value)];


                var lifted = new Stack<char>();

                if (useCrateMover9001)
                {
                    for (var i = 0; i < count; i++)
                    {
                        lifted.Push(source.Pop());
                    }

                    for (var i = 0; i < count; i++)
                    {
                        destination.Push(lifted.Pop());
                    }
                }
                else
                {
                    for (var i = 0; i < count; i++)
                    {
                        destination.Push(source.Pop());
                    }
                }
            }

            if (!stacksLoaded)
            {
                for (var i = 1; i <= numberOfStacks; i++)
                {
                    var index = 1 + (i - 1) * 4;
                    if (line[index] >= 'A' && line[index] <= 'Z')
                    {
                        stacks[i].Push(line[index]);
                    }
                }
            }
        });

        return stacks;
    }
}
